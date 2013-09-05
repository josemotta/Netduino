using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Toolbox.NETMF.Hardware;

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
namespace Multiplexing_GPIOs
{
    public class Program
    {
        // In the global scope so we can call the leds from the events
        public static OutputPortShift[] Leds = new OutputPortShift[16];

        public static void Main()
        {
            // Defining two 74HC165s daisychained on the SPI bus, pin 10 as latchpin
            Ic74HC165Chain IcInChain = new Ic74HC165Chain(SPI_Devices.SPI1, Pins.GPIO_PIN_D10, 2);

            // Defining two 74HC595s daisychained on the SPI bus, pin 9 as latchpin
            Ic74HC595Chain IcOutChain = new Ic74HC595Chain(SPI_Devices.SPI1, Pins.GPIO_PIN_D9, 2);

            // Splits up both 74HC165s. Detail: counting starts at 0
            Ic74HC165 IcIn1 = new Ic74HC165(IcInChain, 0);
            Ic74HC165 IcIn2 = new Ic74HC165(IcInChain, 1);

            // Splits up both 74HC595s. Detail: counting starts at 0
            Ic74HC595 IcOut1 = new Ic74HC595(IcOutChain, 0);
            Ic74HC595 IcOut2 = new Ic74HC595(IcOutChain, 1);

            // Defines all 16 leds
            for (uint Counter = 0; Counter < 8; ++Counter)
            {
                // Leds on the first Ic
                Leds[Counter] = new OutputPortShift(IcOut1, (Ic74HC595.Pins)Counter, false);
                // Leds on the second Ic
                Leds[Counter + 8] = new OutputPortShift(IcOut2, (Ic74HC595.Pins)Counter, false);
            }

            // Defines all 16 buttons
            InterruptPortShift[] Buttons = new InterruptPortShift[16];
            for (uint Counter = 0; Counter < 8; ++Counter)
            {
                // Buttons on the first Ic
                Buttons[Counter] = new InterruptPortShift(IcIn1, (Ic74HC165.Pins)Counter, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
                Buttons[Counter].OnInterrupt += new NativeEventHandler(IcIn1_OnInterrupt);
                Buttons[Counter].EnableInterrupt();
                // Buttons on the second Ic
                Buttons[Counter + 8] = new InterruptPortShift(IcIn2, (Ic74HC165.Pins)Counter, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
                Buttons[Counter + 8].OnInterrupt += new NativeEventHandler(IcIn2_OnInterrupt);
                Buttons[Counter + 8].EnableInterrupt();
            }

            // Wait infinite; let the events to their jobs
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Event triggered when a state changes
        /// </summary>
        /// <param name="PinId">The changed pin number</param>
        /// <param name="Value">The current value</param>
        /// <param name="Time">Time and date of the event</param>
        static void IcIn1_OnInterrupt(uint PinId, uint Value, DateTime Time)
        {
            Debug.Print("State changed of 74HC165 IC 1 port " + PinId.ToString() + " to " + Value.ToString());
            Leds[PinId].Write(Value == 0);
        }

        /// <summary>
        /// Event triggered when a state changes
        /// </summary>
        /// <param name="PinId">The changed pin number</param>
        /// <param name="Value">The current value</param>
        /// <param name="Time">Time and date of the event</param>
        static void IcIn2_OnInterrupt(uint PinId, uint Value, DateTime Time)
        {
            Debug.Print("State changed of 74HC165 IC 2 port " + PinId.ToString() + " to " + Value.ToString());
            Leds[8 + PinId].Write(Value == 0);
        }

    }
}
