﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace ButtonInterrupt
{
    public class Program
    {
        class SimpleTread
        {
            int state = 0;
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

            public SimpleTread()
            {
                new Thread(ReadSDCard).Start();
            }

            private void ReadSDCard()
            {
                _onBoardButton = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
                _onBoardButton.OnInterrupt += new NativeEventHandler(onBoardButton_OnInterrupt);
            }

            private void onBoardButton_OnInterrupt(uint data1, uint data2, DateTime time)
            {
                if (data2 == 0)
                {
                    if (++state % 2 == 0)
                    {
                        led.Write(false);
                        Debug.Print("Even Click");
                    }
                    else
                    {
                        led.Write(true);
                        Debug.Print(@"Odd Click");
                    }
                }
            }
        }

        private static InterruptPort _onBoardButton;

        public static void Main()
        {
            SimpleTread t = new SimpleTread();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
