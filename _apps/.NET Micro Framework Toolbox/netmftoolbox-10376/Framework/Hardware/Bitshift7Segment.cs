using System;
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
    /// A 7-segment display over a bitshift IC
    /// </summary>
    public class Bitshift7Segment
    {
        /// <summary>
        /// Contains an array of bytes; to light the leds specific bytes need to be true. Can be changed with <see cref="ChangeSignal"/> and <see cref="ChangeSignals"/>
        /// </summary>
        /// <remarks><![CDATA[
        /// At default, these bytes are used:
        ///    LowerLeft = 0
        ///    Bottom = 1
        ///    LowerRight = 2
        ///    UpperRight = 4
        ///    Top = 5
        ///    UpperLeft = 6
        ///    Middle = 7
        ///    (Dot = 3)
        /// ]]></remarks>
        private byte[] _Values = new byte[] {

                  //                              (76543210)
            0x77, // 0 brights up: 0 1 2 4 5 6    (01110111)
            0x14, // 1 brights up: 2 4            (00010100)
            0xb3, // 2 brights up: 0 1 4 5 7      (10110011)
            0xb6, // 3 brights up: 1 2 4 5 7      (10110110)
            0xd4, // 4 brights up: 2 4 6 7        (11010100)
            0xe6, // 5 brights up: 1 2 5 6 7      (11100110)
            0xe7, // 6 brights up: 0 1 2 5 6 7    (11100111)
            0x34, // 7 brights up: 2 4 5          (00110100)
            0xf7, // 8 brights up: 0 1 2 4 5 6 7  (11110111)
            0xf6, // 9 brights up: 1 2 4 5 6 7    (11110110)
            0x00, // all go down: 0 1 2 4 5 6 7   (00000000)

            // Space to store 256 digits in total
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };

        /// <summary>Reference to the bit which contains the dot (4th bit by default)</summary>
        private byte _DotBit = 8;

        /// <summary>Reference to the IC Chain</summary>
        private Ic74HC595Chain _Chain;
        /// <summary>Index of the IC connected to the 7-segment</summary>
        private uint _IcIndex;

        /// <summary>Contains the current displayed nummeric value</summary>
        private byte _Digit;
        /// <summary>Contains the current state of the dot</summary>
        private bool _Dot;
        // True when it's common anode, false if it's common cathode
        private bool _CommonAnode;

        /// <summary>
        /// Changes the byte that needs to be sent to display a character (useful to customize characters)
        /// </summary>
        /// <param name="Index">The character to change (0 to 9 are actually 0 to 9 and 10 is blank, 11 to 255 are unused by default)</param>
        /// <param name="Signal">The byte of data that needs to be sent to the SPI bus for this character</param>
        public void ChangeSignal(byte Index, byte Signal)
        {
            this._Values[Index] = Signal;
        }

        /// <summary>
        /// Changes all characters (also removes custom-made characters created with <see cref="ChangeSignal"/>)
        /// </summary>
        /// <param name="Signals">A new array of index versus signal</param>
        public void ChangeSignals(byte[] Signals)
        {
            this._Values = Signals;
        }

        /// <summary>
        /// Changes the bit used for the dot (by default: 4)
        /// </summary>
        /// <param name="Bit">The bit in which the dot is (1 to 8)</param>
        public void ChangeDotSignal(byte Bit)
        {
            if (Bit == 0 || Bit > 8)
                throw new IndexOutOfRangeException();

            byte Value = 1;
            for (int b = 1; b < Bit; ++b)
                Value = (byte)(Value * 2);

            this._DotBit = Value;
        }

        /// <summary>
        /// Initalises a 7-segment display as part of a chain of 74HC595 IC's
        /// </summary>
        /// <param name="Chain">Reference to a 74HC595 chain</param>
        /// <param name="IcIndex">The index of the IC (start counting at 0!)</param>
        /// <param name="CommonAnode">Specifies if the 7-segment is common anode</param>
        public Bitshift7Segment(Ic74HC595Chain Chain, uint IcIndex = 0, bool CommonAnode = true)
        {
            this._Chain = Chain;
            this._IcIndex = IcIndex;
            this._CommonAnode = CommonAnode;
        }

        /// <summary>
        /// Initalises a 7-segment display connected with a single 74HC595 IC
        /// </summary>
        /// <param name="SPI_Module">The SPI interface it's connected to</param>
        /// <param name="LatchPin">The slave select pin connected to the IC</param>
        /// <param name="CommonAnode">Specifies if the 7-segment is common anode</param>
        public Bitshift7Segment(SPI.SPI_module SPI_Module, Cpu.Pin LatchPin, bool CommonAnode = true)
        {
            this._Chain = new Ic74HC595Chain(SPI_Module, LatchPin, 1);
            this._IcIndex = 0;
            this._CommonAnode = CommonAnode;
        }

        /// <summary>
        /// Sets the digit to a specific number
        /// </summary>
        /// <param name="Digit">The number (0 to 9, 10 for blank)</param>
        public void SetDigit(byte Digit)
        {
            this._Digit = Digit;
            this._Write();
        }

        /// <summary>
        /// Gets the current displayed digit
        /// </summary>
        /// <returns>The displayed number (0 to 9, 10 for blank)</returns>
        public byte GetDigit()
        {
            return this._Digit;
        }

        /// <summary>
        /// Sets or unsets the dot
        /// </summary>
        /// <param name="State">True when the dot must be on</param>
        public void SetDot(bool State)
        {
            this._Dot = State;
            this._Write();
        }

        /// <summary>
        /// Gets the current state of the dot
        /// </summary>
        /// <returns></returns>
        public bool GetDot()
        {
            return this._Dot;
        }

        /// <summary>
        /// Writes the byte to the 74HC595 IC
        /// </summary>
        private void _Write()
        {
            // Fetches the byte that corresponds with the digit
            byte Value = this._Values[this._Digit];
            // Adds the dot if required
            if (_Dot) Value += this._DotBit;
            // Makes 0 to 1 and visa versa
            if (this._CommonAnode) Value = (byte)(Value ^ 0xff);
            // Actually writes the data
            this._Chain.SetValue(this._IcIndex, Value);
        }
    }
}