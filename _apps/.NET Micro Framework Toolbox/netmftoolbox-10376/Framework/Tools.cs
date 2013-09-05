using System;

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
namespace Toolbox.NETMF
{
    /// <summary>
    /// Generic, useful tools
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Converts a Hex string to a number
        /// </summary>
        /// <param name="HexNumber">The Hex string (ex.: "0F")</param>
        /// <returns>The decimal value</returns>
        public static uint Hex2Dec(string HexNumber)
        {
            // Always in upper case
            HexNumber = HexNumber.ToUpper();
            // Contains all Hex posibilities
            string ConversionTable = "0123456789ABCDEF";
            // Will contain the return value
            uint RetVal = 0;
            // Will increase
            uint Multiplier = 1;

            for (int Index = HexNumber.Length - 1; Index >= 0; --Index)
            {
                RetVal += (uint)(Multiplier * (ConversionTable.IndexOf(HexNumber[Index])));
                Multiplier = (uint)(Multiplier * ConversionTable.Length);
            }

            return RetVal;
        }

        /// <summary>
        /// Converts a byte array to a char array
        /// </summary>
        /// <param name="Input">The byte array</param>
        /// <returns>The char array</returns>
        public static char[] Bytes2Chars(byte[] Input)
        {
            char[] Output = new char[Input.Length];
            for (int Counter = 0; Counter < Input.Length; ++Counter)
                Output[Counter] = (char)Input[Counter];
            return Output;
        }

        /// <summary>
        /// Converts a char array to a byte array
        /// </summary>
        /// <param name="Input">The char array</param>
        /// <returns>The byte array</returns>
        public static byte[] Chars2Bytes(char[] Input)
        {
            byte[] Output = new byte[Input.Length];
            for (int Counter = 0; Counter < Input.Length; ++Counter)
                Output[Counter] = (byte)Input[Counter];
            return Output;
        }

        /// <summary>
        /// Changes a number into a string and add zeros in front of it, if required
        /// </summary>
        /// <param name="Number">The input number</param>
        /// <param name="Digits">The amount of digits it should be</param>
        /// <returns>A string with the right amount of digits</returns>
        public static string ZeroFill(string Number, int Digits)
        {
            bool Negative = false;
            if (Number.Substring(0, 1) == "-")
            {
                Negative = true;
                Number = Number.Substring(1);
            }

            for (int Counter = Number.Length; Counter < Digits; ++Counter)
            {
                Number = "0" + Number;
            }
            if (Negative) Number = "-" + Number;
            return Number;
        }

        /// <summary>
        /// Changes a number into a string and add zeros in front of it, if required
        /// </summary>
        /// <param name="Number">The input number</param>
        /// <param name="MinLength">The amount of digits it should be</param>
        /// <returns>A string with the right amount of digits</returns>
        public static string ZeroFill(int Number, int MinLength)
        {
            return ZeroFill(Number.ToString(), MinLength);
            // In 4.2 it should be possible to replace this with the following line,
            // but due to a bug (http://netmf.codeplex.com/workitem/1322) it isn't.
            // return Number.toString("d" + MinLength.toString());
        }

        /// <summary>
        /// URL-encode according to RFC 3986
        /// </summary>
        /// <param name="Input">The URL to be encoded.</param>
        /// <returns>Returns a string in which all non-alphanumeric characters except -_.~ have been replaced with a percent (%) sign followed by two hex digits.</returns>
        public static string RawUrlEncode(string Input)
        {
            string RetValue = "";
            for (int Counter = 0; Counter < Input.Length; ++Counter)
            {
                byte CharCode = (byte)(Input.ToCharArray()[Counter]);
                if (
                   CharCode == 0x2d                        // -
                   || CharCode == 0x5f                     // _
                   || CharCode == 0x2e                     // .
                   || CharCode == 0x7e                     // ~
                   || (CharCode > 0x2f && CharCode < 0x3a) // 0-9
                   || (CharCode > 0x40 && CharCode < 0x5b) // A-Z
                   || (CharCode > 0x60 && CharCode < 0x7b) // a-z
                   )
                {
                    RetValue += Input.Substring(Counter, 1);
                }
                else
                {
                    // Calculates the hex value in some way
                    RetValue += "%" + Dec2Hex(CharCode, 2);
                }
            }

            return RetValue;
        }

        /// <summary>
        /// Encodes a string according to the BASE64 standard
        /// </summary>
        /// <param name="Input">The input string</param>
        /// <returns>The output string</returns>
        public static string Base64Encode(string Input)
        {
            // Pairs of 3 8-bit bytes will become pairs of 4 6-bit bytes
            // That's the whole trick of base64 encoding :-)

            int Blocks = Input.Length / 3;           // The amount of original pairs
            if (Blocks * 3 < Input.Length) ++Blocks; // Fixes rounding issues; always round up
            int Bytes = Blocks * 4;                  // The length of the base64 output

            // These characters will be used to represent the 6-bit bytes in ASCII
            char[] Base64_Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".ToCharArray();

            // Converts the input string to characters and creates the output array
            char[] InputChars = Input.ToCharArray();
            char[] OutputChars = new char[Bytes];

            // Converts the blocks of bytes
            for (int Block = 0; Block < Blocks; ++Block)
            {
                // Fetches the input pairs
                byte Input0 = (byte)(InputChars.Length > Block * 3 ? InputChars[Block * 3] : 0);
                byte Input1 = (byte)(InputChars.Length > Block * 3 + 1 ? InputChars[Block * 3 + 1] : 0);
                byte Input2 = (byte)(InputChars.Length > Block * 3 + 2 ? InputChars[Block * 3 + 2] : 0);

                // Generates the output pairs
                byte Output0 = (byte)(Input0 >> 2);                           // The first 6 bits of the 1st byte
                byte Output1 = (byte)(((Input0 & 0x3) << 4) + (Input1 >> 4)); // The last 2 bits of the 1st byte followed by the first 4 bits of the 2nd byte
                byte Output2 = (byte)(((Input1 & 0xf) << 2) + (Input2 >> 6)); // The last 4 bits of the 2nd byte followed by the first 2 bits of the 3rd byte
                byte Output3 = (byte)(Input2 & 0x3f);                         // The last 6 bits of the 3rd byte

                // This prevents 0-bytes at the end
                if (InputChars.Length < Block * 3 + 2) Output2 = 64;
                if (InputChars.Length < Block * 3 + 3) Output3 = 64;

                // Converts the output pairs to base64 characters
                OutputChars[Block * 4] = Base64_Characters[Output0];
                OutputChars[Block * 4 + 1] = Base64_Characters[Output1];
                OutputChars[Block * 4 + 2] = Base64_Characters[Output2];
                OutputChars[Block * 4 + 3] = Base64_Characters[Output3];
            }

            return new string(OutputChars);
        }

        /// <summary>
        /// Converts a number to a Hex string
        /// </summary>
        /// <param name="Input">The number</param>
        /// <param name="MinLength">The minimum length of the return string (filled with 0s)</param>
        /// <returns>The Hex string</returns>
        public static string Dec2Hex(int Input, int MinLength = 0)
        {
            #if MF_FRAMEWORK_VERSION_V4_2
                // Since NETMF 4.2 int.toString() exists, so we can do this:
                return Input.ToString("x" + MinLength.ToString());
            #else
                // Contains all Hex posibilities
                string ConversionTable = "0123456789ABCDEF";
                // Starts the conversion
                string RetValue = "";
                int Current = 0;
                int Next = Input;
                do
                {
                    if (Next >= ConversionTable.Length)
                    {
                        // The current digit
                        Current = (Next / ConversionTable.Length);
                        if (Current * ConversionTable.Length > Next) --Current;
                        // What's left
                        Next = Next - (Current * ConversionTable.Length);
                    }
                    else
                    {
                        // The last digit
                        Current = Next;
                        // Nothing left
                        Next = -1;
                    }
                    RetValue += ConversionTable[Current];
                } while (Next != -1);

                return Tools.ZeroFill(RetValue, MinLength);
            #endif
        }
    }
}