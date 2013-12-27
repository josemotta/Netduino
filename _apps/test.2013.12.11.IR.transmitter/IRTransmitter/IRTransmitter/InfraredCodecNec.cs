using System;
using Microsoft.SPOT;

/*
 * Copyright 2012 Mario Vernari (http://netmftoolbox.codeplex.com/)
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
namespace IRTransmitter
{
    /// <summary>
    /// Class acting as driver for the NEC IR protocol
    /// </summary>
    /// <seealso cref="http://www.sbprojects.com/knowledge/ir/nec.php"/>
    public class InfraredCodecNEC : InfraredCodecBase
    {
        // NEC Infrared Transmission Protocol
        // The NEC IR transmission protocol uses pulse distance encoding of the message bits. 
        // Each pulse burst (mark – RC transmitter ON) is 562.5µs in length (about 21 cycles),
        // at a carrier frequency of 38kHz (26.3µs). Logical bits are transmitted as follows:
        // • Logical '0' – a 562.5µs pulse burst followed by a 562.5µs space, with a total transmit time of 1.125ms
        // • Logical '1' – a 562.5µs pulse burst followed by a 1.6875ms space, with a total transmit time of 2.25ms
        // When a key is pressed on the remote controller, the message transmitted consists of, in order:
        // • a 9ms leading pulse burst (16 times the pulse burst length used for a logical data bit)
        // • a 4.5ms space
        // • the 8-bit address for the receiving device
        // • the 8-bit logical inverse of the address
        // • the 8-bit command
        // • the 8-bit logical inverse of the command
        // • a final 562.5µs pulse burst to signify the end of message transmission.
        // The four bytes of data bits are each sent least significant bit first. 
        // The recommended carrier duty-cycle is 1/4 or 1/3. 

        private const float CarrierFrequency = 38.0f;   //kHz
        private const float PulseDuty = 0.33f;
        private const int PulseBurstCount = 21;

        /// <summary>
        /// Create a new instance of codec
        /// </summary>
        /// <param name="transmitter">A valid reference for the transmitter to be used</param>
        public InfraredCodecNEC(InfraredTransmitter transmitter)
            : base(transmitter, CarrierFrequency, PulseDuty)
        {
        }

        /// <summary>
        /// Send a NEC message
        /// </summary>
        /// <param name="address">Specifies the address in the message</param>
        /// <param name="command">Specifies the command to be sent</param>
        public void Send(
            int address,
            int command)
        {
            int notaddress = address ^ 0xFF;    // inverte byte: address
            int notcommand = command ^ 0xFF;    // inverte byte: command

            //place the "START" pattern
            this.MarkStart();

            //address (8 bits, LSB first)
            for (int i = 0; i < 8; i++)
            {
                this.Modulate((address & 0x01) != 0);
                address >>= 1;
            }

            //notaddress (8 bits, LSB first)
            for (int i = 0; i < 8; i++)
            {
                this.Modulate((notaddress & 0x01) != 0);
                notaddress >>= 1;
            }
            
            //command (8 bits, LSB first)
            for (int i = 0; i < 8; i++)
            {
                this.Modulate((command & 0x01) != 0);
                command >>= 1;
            }

            //notcommand (8 bits, LSB first)
            for (int i = 0; i < 8; i++)
            {
                this.Modulate((notcommand & 0x01) != 0);
                notcommand >>= 1;
            }

            //send
            this.Transmitter
                .Send(this);

            //place the "END" pattern
            this.MarkEnd();
        }

        /// <summary>
        /// Mark the start pattern of the NEC message
        /// </summary>
        private void MarkStart()
        {
            //"START": 336 pulses + 168 blanks = 504 as total
            this.TotalPulseCount = (16 + 8) * PulseBurstCount;
            this.InitialPulseCount = 16 * PulseBurstCount;
            this.FinalPulseCount = 0;

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }
        
        /// <summary>
        /// Mark the start pattern of the NEC message
        /// </summary>
        private void MarkEnd()
        {
            //"END": 21 pulses + 21 blanks = 42 as total
            this.TotalPulseCount = PulseBurstCount * 2;
            this.InitialPulseCount = PulseBurstCount;
            this.FinalPulseCount = 0;

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }

        /// <summary>
        /// Provide the modulation for the logic bit
        /// </summary>
        /// <param name="value">The logic value to be modulated</param>
        private void Modulate(bool value)
        {
            if (value)
            {
                //logic "ONE": 21 pulses + 63 blanks = 84 as total
                this.TotalPulseCount = PulseBurstCount * 4;
                this.InitialPulseCount = PulseBurstCount;
                this.FinalPulseCount = 0;
            }
            else
            {
                //logic "ZERO": 21 pulses + 21 blanks = 42 as total
                this.TotalPulseCount = PulseBurstCount * 2;
                this.InitialPulseCount = PulseBurstCount;
                this.FinalPulseCount = 0;
            }

            //append the defined pattern to the stream
            this.Transmitter
                .Append(this);
        }
    }
}
