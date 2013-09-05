using System;
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
    /// 74HC595 8-Bit Ser-In/Ser Or Par-Out Shift Register with Out Latches (3-State)
    /// </summary>
    public class Ic74HC595
    {
        /// <summary>
        /// Reference to the Ic74HC595Chain object
        /// </summary>
        private Ic74HC595Chain _Chain;

        /// <summary>
        /// The index of this IC on the chain
        /// </summary>
        private uint _IcIndex;

        /// <summary>
        /// Contains boolean states of all pins
        /// </summary>
        private bool[] _PinState = new bool[8];

        /// <summary>
        /// A list of all General Purpose Output pins
        /// </summary>
        public enum Pins
        {
            /// <summary>1st GPO pin</summary>
            GPO_PIN_D0 = 0,
            /// <summary>2nd GPO pin</summary>
            GPO_PIN_D1 = 1,
            /// <summary>3rd GPO pin</summary>
            GPO_PIN_D2 = 2,
            /// <summary>4th GPO pin</summary>
            GPO_PIN_D3 = 3,
            /// <summary>5th GPO pin</summary>
            GPO_PIN_D4 = 4,
            /// <summary>6th GPO pin</summary>
            GPO_PIN_D5 = 5,
            /// <summary>7th GPO pin</summary>
            GPO_PIN_D6 = 6,
            /// <summary>8th GPO pin</summary>
            GPO_PIN_D7 = 7
        }

        /// <summary>
        /// Defines a specific 74HC595 IC in a chain
        /// </summary>
        /// <param name="Chain">Reference to the chain</param>
        /// <param name="IcIndex">The index of the IC (start counting at 0!)</param>
        public Ic74HC595(Ic74HC595Chain Chain, uint IcIndex)
        {
            // Stores the references
            this._Chain = Chain;
            this._IcIndex = IcIndex;
        }

        /// <summary>
        /// Defines a single 74HC595
        /// </summary>
        /// <param name="SPI_Module">The SPI interface it's connected to</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        public Ic74HC595(SPI.SPI_module SPI_Module, Cpu.Pin LatchPin)
        {
            // Stores the references
            this._Chain = new Ic74HC595Chain(SPI_Module, LatchPin, 1);
            this._IcIndex = 0;
        }

        /// <summary>
        /// Sets the state of a specific pin
        /// </summary>
        /// <param name="Pin">The pin</param>
        /// <param name="State">The new state of the pin</param>
        public void SetPinState(Ic74HC595.Pins Pin, bool State)
        {
            this._PinState[(int)Pin] = State;
            this._WritePins();
        }

        /// <summary>
        /// Gets the current state of a specific pin
        /// </summary>
        /// <param name="Pin">The pin</param>
        /// <returns>The current state of the pin</returns>
        public bool GetPinState(Ic74HC595.Pins Pin)
        {
            return this._PinState[(int)Pin];
        }

        /// <summary>
        /// Writes all data to the IC
        /// </summary>
        private void _WritePins()
        {
            byte WriteBuffer;
            WriteBuffer = (byte)(
                (this._PinState[0] ? 0x01 : 0) +
                (this._PinState[1] ? 0x02 : 0) +
                (this._PinState[2] ? 0x04 : 0) +
                (this._PinState[3] ? 0x08 : 0) +
                (this._PinState[4] ? 0x10 : 0) +
                (this._PinState[5] ? 0x20 : 0) +
                (this._PinState[6] ? 0x40 : 0) +
                (this._PinState[7] ? 0x80 : 0)
            );
            this._Chain.SetValue(this._IcIndex, WriteBuffer);
        }
    }
}
