using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

/*
 * Copyright 2011 Stefan Thoolen (http://netmftoolbox.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Toolbox.NETMF.Hardware
{
    /// <summary>
    /// 74HC165 IC Chain
    /// </summary>
    /// <remarks><![CDATA[
    /// 74HC165 (N) DIP16 pin layout:
    /// 
    ///   16 15 14 13 12 11 10 9
    ///   │  │  │  │  │  │  │  │
    /// █████████████████████████
    /// ▀████████████████████████
    ///   ███████████████████████
    /// ▄████████████████████████
    /// █████████████████████████
    ///   │  │  │  │  │  │  │  │
    ///   1  2  3  4  5  6  7  8
    /// 
    /// NOTE: The pins below aren't mentioned in pin sequence but grouped by connection
    ///
    /// 11 D0 (parallel data input) -> Ic74HC164.GPI_PIN_D0
    /// 12 D1 (parallel data input) -> Ic74HC164.GPI_PIN_D1
    /// 13 D2 (parallel data input) -> Ic74HC164.GPI_PIN_D2
    /// 14 D3 (parallel data input) -> Ic74HC164.GPI_PIN_D3
    ///  3 D4 (parallel data input) -> Ic74HC164.GPI_PIN_D4
    ///  4 D5 (parallel data input) -> Ic74HC164.GPI_PIN_D5
    ///  5 D6 (parallel data input) -> Ic74HC164.GPI_PIN_D6
    ///  6 D7 (parallel data input) -> Ic74HC164.GPI_PIN_D7
    ///
    ///  8 GND (ground) -> Gnd
    /// 15 CE -> Gnd
    /// 16 Vcc (positive supply voltage) -> +3.3V
    ///
    ///  2 SCLK -> SPI SCLK (on Netduino pin 13)
    ///  1 PL -> SPI CS (on Netduino any GPIO pin)
    ///  9 MISO -> SPI MISO (on Netduino pin 12 or another Ic74HC165 pin 10)
    ///
    /// 10 DS (serial data output) -> an optional 74HC165 slave
    /// ]]></remarks>
    public class Ic74HC165Chain
    {
        /// <summary>
        /// A reference to the SPI Interface
        /// </summary>
        private MultiSPI _SpiInterface;

        /// <summary>
        /// Contains the amount of connected IC's
        /// </summary>
        private uint _IcCount;

        /// <summary>
        /// Contains the last state of all IC's
        /// </summary>
        private byte[] _LastState;

        /// <summary>
        /// When true, this.onByteChange works. When false, it doesn't. Default value: True
        /// </summary>
        public bool EventsEnabled
        {
            get { return this._EventsEnabled; }
            set
            {
                if (value && !this._EventsEnabled)
                {
                    this._EventsEnabled = true;
                    // Starts the background thread
                    Thread InterruptThread = new Thread(this._InterruptStarter);
                    InterruptThread.Start();
                }
                else
                    this._EventsEnabled = value;
            }
        }
        private bool _EventsEnabled;

        /// <summary>
        /// Contains a reference to the interrupt thread
        /// </summary>
        private ThreadStart _InterruptStarter;

        /// <summary>Triggered when data changed (requires this.EventsEnabled on true)</summary>
        public event NativeEventHandler onByteChange;

        /// <summary>When using bitbang mode, this bool is true. <see cref="_SpiInterface"/> won't be used if this is true.</summary>
        private bool _BitBangMode = false;
        /// <summary>When using bitbang mode, this will contain a reference to the SPCK pin. See also <see cref="_BitBangMode"/></summary>
        private OutputPort _BBM_SPCK;
        /// <summary>When using bitbang mode, this will contain a reference to the CS pin. See also <see cref="_BitBangMode"/></summary>
        private OutputPort _BBM_CS;
        /// <summary>When using bitbang mode, this will contain a reference to the MISO pin. See also <see cref="_BitBangMode"/></summary>
        private InputPort _BBM_MISO;

        /// <summary>
        /// Initialises a chain of one or multiple 74HC165 IC's
        /// </summary>
        /// <param name="SPI_Module">The SPI interface it's connected to</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        /// <param name="IcCount">The amount of IC's connected</param>
        public Ic74HC165Chain(SPI.SPI_module SPI_Module, Cpu.Pin LatchPin, uint IcCount = 1)
        {
            // Full SPI configuration
            this._SpiInterface = new MultiSPI(new SPI.Configuration(
                LatchPin,
                true,
                0,
                0,
                true,
                false,
                1000,
                SPI_Module
            ));
            // The amount of ICs
            this._IcCount = IcCount;
            this._LastState = new byte[IcCount];

            // Creates a background thread for interrupt checking
            this._InterruptStarter = new ThreadStart(this._InterruptLoop);
        }

       /// <summary>
        /// Initialises a chain of one or multiple 74HC165 IC's over bitbanged SPI [WHEN POSSIBLE, USE MANAGED MODE!]
        /// </summary>
        /// <remarks>
        /// Use only when the managed SPI-pins can't be used. This method is way slower and locks the pins for any other purpose until disposed.
        /// </remarks>
        /// <param name="ClockPin">The clock pin connected to the IC</param>
        /// <param name="DataPin">The data pin connected to the IC</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        /// <param name="IcCount">The amount of IC's connected</param>
        public Ic74HC165Chain(Cpu.Pin ClockPin, Cpu.Pin DataPin, Cpu.Pin LatchPin, uint IcCount = 1)
        {
            // Enables bitbang mode
            this._BitBangMode = true;
            // Makes references to the SPI pins
            this._BBM_CS = new OutputPort(LatchPin, false);
            this._BBM_MISO = new InputPort(DataPin, false, Port.ResistorMode.Disabled);
            this._BBM_SPCK = new OutputPort(ClockPin, true);
            // The amount of ICs
            this._IcCount = IcCount;
            this._LastState = new byte[IcCount];

            // Creates a background thread for interrupt checking
            this._InterruptStarter = new ThreadStart(this._InterruptLoop);
        }

        /// <summary>
        /// Background thread checking for data changes
        /// </summary>
        private void _InterruptLoop()
        {
            while (this._EventsEnabled)
            {
                this._ReadSPI();
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Returns the data from a specific IC
        /// </summary>
        /// <param name="IcIndex">The IC index</param>
        /// <returns></returns>
        public byte GetValue(uint IcIndex)
        {
            // If the Events-loop isn't running we need to get the data manually
            if (!this.EventsEnabled)
                this._ReadSPI();
            // Returns the correct byte
            return this._LastState[IcIndex];
        }

        /// <summary>
        /// Reads all data from the SPI interface
        /// </summary>
        private void _ReadSPI()
        {
            // reads the data from all IC's
            byte[] ReadBuffer = new byte[this._IcCount];
            if (this._BitBangMode)
            {
                // Bitbang mode
                // Enables the device
                this._BBM_CS.Write(true);
                // Loops throug all IC's
                for (uint IcIndex = 0; IcIndex < ReadBuffer.Length; ++IcIndex)
                {
                    // Loops through all bits
                    byte ReadByte = 0;
                    byte Multiplier = 0x80;
                    for (byte BitNo = 0; BitNo < 8; ++BitNo)
                    {
                        // Enables the clock and wait for a ms. Then read out the bit and disables the clock again
                        this._BBM_SPCK.Write(true);
                        Thread.Sleep(1);
                        if (this._BBM_MISO.Read())
                            ReadByte += Multiplier;
                        Multiplier = (byte)(Multiplier / 2);
                        this._BBM_SPCK.Write(false);
                    }
                    ReadBuffer[IcIndex] = ReadByte;
                }
                // Disables the device
                this._BBM_CS.Write(false);
                this._BBM_SPCK.Write(true);
            }
            else
            {
                // Normal mode
                this._SpiInterface.Read(ReadBuffer);
            }

            // Loops through all the values
            for (uint IcIndex = 0; IcIndex < ReadBuffer.Length; ++IcIndex)
            {
                // We got a change!
                if (ReadBuffer[IcIndex] != this._LastState[IcIndex])
                {
                    // Updates the LastState array
                    this._LastState[IcIndex] = ReadBuffer[IcIndex];
                    // Triggers the event when an event exists and is enabled
                    if (this.onByteChange != null && this._EventsEnabled)
                        this.onByteChange(IcIndex, (uint)ReadBuffer[IcIndex], new DateTime());
                }
            }
        }
    }
}
