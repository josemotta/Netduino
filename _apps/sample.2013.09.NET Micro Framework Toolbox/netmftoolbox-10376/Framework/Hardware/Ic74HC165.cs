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
    /// 74HC165 8-bit parallel-in/serial out shift register
    /// </summary>
    public class Ic74HC165
    {
        /// <summary>
        /// Reference to the Ic74HC165Chain object
        /// </summary>
        private Ic74HC165Chain _Chain;

        /// <summary>
        /// The index of this IC on the chain
        /// </summary>
        private uint _IcIndex;

        /// <summary>
        /// Contains boolean states of all pins
        /// </summary>
        private bool[] _PinState = new bool[8];

        /// <summary>
        /// When true, this.onPinChange works. When false, it doesn't. Default value: True.<br />
        /// Keep in mind, this.EventsEnabled is linked to Ic74HC165Chain.EventsEnabled, therefore linked to all IC's in the chain.
        /// </summary>
        public bool EventsEnabled
        {
            get { return this._Chain.EventsEnabled; }
            set { this._Chain.EventsEnabled = value; }
        }

        /// <summary>Triggered when a pin's value changed (requires this.EventsEnabled on true)</summary>
        public event NativeEventHandler onPinChange;

        /// <summary>
        /// A list of all General Purpose Input pins
        /// </summary>
        public enum Pins
        {
            /// <summary>1st GPI pin</summary>
            GPI_PIN_D0 = 0,
            /// <summary>2nd GPI pin</summary>
            GPI_PIN_D1 = 1,
            /// <summary>3rd GPI pin</summary>
            GPI_PIN_D2 = 2,
            /// <summary>4th GPI pin</summary>
            GPI_PIN_D3 = 3,
            /// <summary>5th GPI pin</summary>
            GPI_PIN_D4 = 4,
            /// <summary>6th GPI pin</summary>
            GPI_PIN_D5 = 5,
            /// <summary>7th GPI pin</summary>
            GPI_PIN_D6 = 6,
            /// <summary>8th GPI pin</summary>
            GPI_PIN_D7 = 7
        }

        /// <summary>
        /// Defines a specific 74HC165 IC in a chain
        /// </summary>
        /// <param name="Chain">Reference to the chain</param>
        /// <param name="IcIndex">The index of the IC (start counting at 0!)</param>
        public Ic74HC165(Ic74HC165Chain Chain, uint IcIndex)
        {
            // Stores the references
            this._Chain = Chain;
            this._IcIndex = IcIndex;
            // Enables the event
            this._Chain.onByteChange += new NativeEventHandler(this._Chain_onByteChange);
        }

        /// <summary>
        /// Defines a single 74HC165 IC
        /// </summary>
        /// <param name="SPI_Module">The SPI interface it's connected to</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        public Ic74HC165(SPI.SPI_module SPI_Module, Cpu.Pin LatchPin)
        {
            // Stores the references
            this._Chain = new Ic74HC165Chain(SPI_Module, LatchPin, 1);
            this._IcIndex = 0;
            // Enables the event
            this._Chain.onByteChange += new NativeEventHandler(this._Chain_onByteChange);
        }

        /// <summary>
        /// Gets the current state of a specific pin
        /// </summary>
        /// <param name="Pin">The pin</param>
        /// <returns>The current state of the pin</returns>
        public bool GetPinState(Ic74HC165.Pins Pin)
        {
            this._ByteRead();
            return this._PinState[(int)Pin];
        }

        /// <summary>
        /// Reads the IC's byte and convert it to pins
        /// </summary>
        private void _ByteRead()
        {
            byte IncomingByte = this._Chain.GetValue(this._IcIndex);
            bool bit;

            // Loops through all bits
            for (int PinCount = 0; PinCount < this._PinState.Length; ++PinCount)
            {
                bit = (IncomingByte & (1 << PinCount)) != 0;
                // Value is changed
                if (bit != this._PinState[PinCount])
                {
                    this._PinState[PinCount] = bit;
                    // Is there an event and are events enabled?
                    if (this.onPinChange != null && this.EventsEnabled)
                    {
                        this.onPinChange((uint)PinCount, (uint)(bit ? 1 : 0), new DateTime());
                    }
                }
            }
        }

        /// <summary>
        /// A byte on the IC chain is changed
        /// </summary>
        /// <param name="IcIndex">The IC index of the change</param>
        /// <param name="IncomingByte">The new byte</param>
        /// <param name="time">Date and time on which the event occured</param>
        private void _Chain_onByteChange(uint IcIndex, uint IncomingByte, DateTime time)
        {
            if (IcIndex != this._IcIndex) return;
            this._ByteRead();
        }
    }
}
