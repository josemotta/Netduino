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
    /// 74HC595 IC Chain
    /// </summary>
    /// <remarks><![CDATA[
    /// 74HC595 (N) DIP16 pin layout:
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
    /// 15 Q0 (parallel data output) -> Ic74HC595.GPO_PIN_D0
    ///  1 Q1 (parallel data output) -> Ic74HC595.GPO_PIN_D1
    ///  2 Q2 (parallel data output) -> Ic74HC595.GPO_PIN_D2
    ///  3 Q3 (parallel data output) -> Ic74HC595.GPO_PIN_D3
    ///  4 Q4 (parallel data output) -> Ic74HC595.GPO_PIN_D4
    ///  5 Q5 (parallel data output) -> Ic74HC595.GPO_PIN_D5
    ///  6 Q6 (parallel data output) -> Ic74HC595.GPO_PIN_D6
    ///  7 Q7 (parallel data output) -> Ic74HC595.GPO_PIN_D7
    ///
    ///  8 GND (ground) -> Gnd
    /// 13 OE (output enable, active low) -> Gnd
    /// 10 MR (master reset, active low) -> +3.3V
    /// 16 Vcc (positive supply voltage) -> +3.3V
    ///
    /// 11 SCLK -> SPI SCLK (on Netduino pin 13)
    /// 12 SS -> SPI SS (on Netduino any GPIO pin)
    /// 14 MOSI -> SPI MOSI (on Netduino pin 11 or another Ic74HC595 pin 9)
    ///
    ///  9 Q7’ (serial data output) -> an optional 74HC595 slave
    /// ]]></remarks>
    public class Ic74HC595Chain : IDisposable
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

        /// <summary>When using bitbang mode, this bool is true. <see cref="_SpiInterface"/> won't be used if this is true.</summary>
        private bool _BitBangMode = false;
        /// <summary>When using bitbang mode, this will contain a reference to the SPCK pin. See also <see cref="_BitBangMode"/></summary>
        private OutputPort _BBM_SPCK;
        /// <summary>When using bitbang mode, this will contain a reference to the CS pin. See also <see cref="_BitBangMode"/></summary>
        private OutputPort _BBM_CS;
        /// <summary>When using bitbang mode, this will contain a reference to the MOSI pin. See also <see cref="_BitBangMode"/></summary>
        private OutputPort _BBM_MOSI;

        /// <summary>
        /// Initialises a chain of one or multiple 74HC595 IC's over managed SPI
        /// </summary>
        /// <param name="SPI_Module">The SPI interface it's connected to</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        /// <param name="IcCount">The amount of IC's connected</param>
        public Ic74HC595Chain(SPI.SPI_module SPI_Module, Cpu.Pin LatchPin, uint IcCount = 1)
        {
            // Full SPI configuration
            this._SpiInterface = new MultiSPI(new SPI.Configuration(
                LatchPin,
                false,
                0,
                0,
                false,
                true,
                1000,
                SPI_Module
            ));
            // The amount of ICs
            this._IcCount = IcCount;
            this._LastState = new byte[IcCount];
        }

        /// <summary>
        /// Initialises a chain of one or multiple 74HC595 IC's over bitbanged SPI [WHEN POSSIBLE, USE MANAGED MODE!]
        /// </summary>
        /// <remarks>
        /// Use only when the managed SPI-pins can't be used. This method is way slower and locks the pins for any other purpose until disposed.
        /// </remarks>
        /// <param name="ClockPin">The clock pin connected to the IC</param>
        /// <param name="DataPin">The data pin connected to the IC</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        /// <param name="IcCount">The amount of IC's connected</param>
        public Ic74HC595Chain(Cpu.Pin ClockPin, Cpu.Pin DataPin, Cpu.Pin LatchPin, uint IcCount = 1)
        {
            // Enables bitbang mode
            this._BitBangMode = true;
            // Makes references to the SPI pins
            this._BBM_CS = new OutputPort(LatchPin, true);
            this._BBM_MOSI = new OutputPort(DataPin, false);
            this._BBM_SPCK = new OutputPort(ClockPin, false);
            // The amount of ICs
            this._IcCount = IcCount;
            this._LastState = new byte[IcCount];
        }

        /// <summary>
        /// Sets the value for a specific IC
        /// </summary>
        /// <param name="IcIndex">The IC index</param>
        /// <param name="Value">The new value</param>
        public void SetValue(uint IcIndex, byte Value)
        {
            this._LastState[this._IcCount - 1 - IcIndex] = Value;
            this._WriteSPI();
        }

        /// <summary>
        /// Writes all data to the SPI interface
        /// </summary>
        private void _WriteSPI()
        {
            // Normal mode
            if (!this._BitBangMode)
            {
                this._SpiInterface.Write(this._LastState);
                return;
            }

            // Bitbang mode; enables output to the 74HC595 chain
            this._BBM_CS.Write(false);
            // Loops through all bytes
            for (uint ByteNo = 0; ByteNo < this._IcCount; ++ByteNo)
            {
                byte Value = this._LastState[ByteNo];
                // Loops through all 8 bits
                for (byte BitNo = 0; BitNo < 8; ++BitNo)
                {
                    // Checks if the 8th bit is set
                    int BitVal = Value & 0x80;
                    // Shifts all bits in the value one place
                    Value = (byte)(Value << 1);
                    // If the 8th bit is true, we write true, elsewise false
                    this._BBM_MOSI.Write(BitVal == 0x80);
                    // Enable the clock for a short moment
                    this._BBM_SPCK.Write(true);
                    Thread.Sleep(1);
                    this._BBM_SPCK.Write(false);
                }
            }
            // Disables output to the 74HC595 chain
            this._BBM_CS.Write(true);
        }

        /// <summary>
        /// Frees the pins used for bitbang mode, when used bitbang mode. Otherwise disposing isn't really required.
        /// </summary>
        public void Dispose()
        {
            if (this._BitBangMode)
            {
                this._BBM_CS.Dispose();
                this._BBM_MOSI.Dispose();
                this._BBM_SPCK.Dispose();
            }
        }
    }
}
