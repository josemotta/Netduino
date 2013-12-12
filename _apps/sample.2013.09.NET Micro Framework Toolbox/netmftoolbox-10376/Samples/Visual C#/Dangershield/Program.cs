using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Toolbox.NETMF;
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
namespace Dangershield_Demo
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !  W A R N I N G   W A R N I N G   W A R N I N G    W A R N I N G   W A R N I N G   W A R N I N G  !
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // ! The Dangershield analog ports are connected to 5V since it's originally designed for Arduino.    !
    // ! The Netduino analog inputs work on 3.3V. Herefor a modification to the shield is required.       !
    // ! See http://netmftoolbox.codeplex.com/wikipage?title=Danger%20Shield for more details.            !
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // ! This sample has two states:                                                                      !
    // !                                                                                                  !
    // ! State 1 (press button 1 to activate): The three potentiometers are linked to the two leds and    !
    // !                                       7-segment display                                          !
    // ! State 2 (press button 2 to activate): The photocell, knock sensor and temperature sensor are     !
    // !                                       linked to the two leds and 7-segment display               !
    // ! If you hold button 3, the buzzer will annoy you                                                  !
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class Program
    {
        public static void Main()
        {
            // Outputs:
            // - Buzzer (GPIO D3)
            // - Segment7 (7-Segment display/74HC595 IC) (GPIO D4, D7, D8)
            // - Led1 (GPIO D5)
            // - Led2 (GPIO D6)

            #region "Output definitions"
            // The buzzer is connected directly to GPIO pin D3
            BitBangBuzzer Buzzer = new BitBangBuzzer(Pins.GPIO_PIN_D3);

            // The 7-segment display is connected with a 74HC595 bitshift IC over GPIO pins D4 (MOSI), D7 (CS) and D8 (SCLK)
            Bitshift7Segment Segment7 = new Bitshift7Segment(new Ic74HC595Chain(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D7));
            // The DangerShield has the digits defined differently; these bits are used:
            //    Top = 1
            //    UpperRight = 2
            //    LowerRight = 3
            //    Bottom = 4
            //    LowerLeft = 5
            //    UpperLeft = 6
            //    Middle = 7
            //    Dot = 8
            Segment7.ChangeSignals(new byte[] { //    (87654321)
                0x3f, // 0 brights up: 0 1 2 3 4 5    (00111111)
                0x06, // 1 brights up: 1 2            (00000110)
                0x5b, // 2 brights up: 0 1 3 4 6      (01011011)
                0x4f, // 3 brights up: 0 1 2 3 6      (01001111)
                0x66, // 4 brights up: 1 2 5 6        (01100110)
                0x6d, // 5 brights up: 0 2 3 5 6      (01101101)
                0x7d, // 6 brights up: 0 2 3 4 5 6    (01111101)
                0x07, // 7 brights up: 0 1 2          (00000111)
                0x7f, // 8 brights up: 0 1 2 3 4 5 6  (01111111)
                0x6f, // 9 brights up: 0 1 2 3 5 6    (01101111)
                0x00, // all go down: 0 1 2 4 5 6 7   (00000000)
            });
            Segment7.ChangeDotSignal(8);

            // Both leds
            PWM Led1 = new PWM(Pins.GPIO_PIN_D5);
            PWM Led2 = new PWM(Pins.GPIO_PIN_D6);
            #endregion

            // Inputs:
            // - PotentioMeter1 (GPIO A0)
            // - PotentioMeter2 (GPIO A1)
            // - PotentioMeter3 (GPIO A2)
            // - Photocell (GPIO A3)
            // - TemperatureSensor (GPIO A4)
            // - KnockSensor (GPIO A5)
            // - PushButton1 (GPIO D10)
            // - PushButton2 (GPIO D11)
            // - PushButton3 (GPIO D12)

            #region "Input definitions"
            // Potentio meters
            SecretLabs.NETMF.Hardware.AnalogInput PotentioMeter1 = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A0);
            PotentioMeter1.SetRange(0, 100); // Same range as Led1.SetDutyCycle()
            SecretLabs.NETMF.Hardware.AnalogInput PotentioMeter2 = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A1);
            PotentioMeter2.SetRange(0, 100); // Same range as Led2.SetDutyCycle()
            SecretLabs.NETMF.Hardware.AnalogInput PotentioMeter3 = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A2);
            PotentioMeter3.SetRange(0, 9);   // Same range as Segment7.SetDigit()
            // Photocell
            SecretLabs.NETMF.Hardware.AnalogInput Photocell = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A3);
            Photocell.SetRange(0, 100); // Same range as Led2.SetDutyCycle()
            // Temperature Sensor
            Tmp36 TemperatureSensor = new Tmp36(Pins.GPIO_PIN_A4);
            // Knock Sensor
            SecretLabs.NETMF.Hardware.AnalogInput KnockSensor = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A5);
            KnockSensor.SetRange(0, 200); // Bigger range as Led1.SetDutyCycle() but you really need to smash hard to reach this value
            // Push buttons
            InputPort PushButton1 = new InputPort(Pins.GPIO_PIN_D10, false, Port.ResistorMode.Disabled);
            InputPort PushButton2 = new InputPort(Pins.GPIO_PIN_D11, false, Port.ResistorMode.Disabled);
            InputPort PushButton3 = new InputPort(Pins.GPIO_PIN_D12, false, Port.ResistorMode.Disabled);
            #endregion

            // This value contains which demo is currently active
            int Demo = 1;

            // Contains the last second, so we can switch between two numbers on the 7-segment display (to display the temperature)
            int LastSecond = Utility.GetMachineTime().Seconds;

            // This digit should be currently shown
            bool ShowSecondDigit = false;

            // Infinite loop
            while (true)
            {
                // Switches the demo, when required (NOT statement because of the pullup resistors)
                if (!PushButton1.Read())
                    Demo = 1;
                if (!PushButton2.Read())
                    Demo = 2;

                if (Demo == 1)
                {
                    // First demo is currently active
                    Led1.SetDutyCycle((uint)PotentioMeter1.Read());
                    Led2.SetDutyCycle((uint)PotentioMeter2.Read());
                    Segment7.SetDigit((byte)PotentioMeter3.Read());
                }
                else
                {
                    // Second demo is currently active
                    uint Knocking = (uint)KnockSensor.Read();
                    Led1.SetDutyCycle((uint)(Knocking > 100 ? 100 : Knocking)); // We want to limit to 100
                    Led1.SetDutyCycle((uint)KnockSensor.Read());
                    Led2.SetDutyCycle((uint)Photocell.Read());

                    // Okay, we want two temperature digits seprated, for display purposes
                    float Temp = TemperatureSensor.Temperature;
                    byte Digit1 = (byte)(Temp / 10);
                    byte Digit0 = (byte)(Temp - (10 * Digit1));

                    // Switch the digit to be displayed
                    if (LastSecond != Utility.GetMachineTime().Seconds)
                    {
                        LastSecond = Utility.GetMachineTime().Seconds;
                        Segment7.SetDigit(ShowSecondDigit ? Digit1 : Digit0);
                        Segment7.SetDot(!ShowSecondDigit);
                        ShowSecondDigit = !ShowSecondDigit;
                    }
                }


                // Links the buzzer to the 3rd pushbutton's value
                Buzzer.Write(!PushButton3.Read());

            }
        }

    }
}
