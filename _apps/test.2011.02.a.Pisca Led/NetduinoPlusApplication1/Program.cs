﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NetduinoPlusApplication1
{
    public class Program
    {
        public static void Main()
        {
            // write your code here

            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

            while (true)
            {
                led.Write(true);
                Thread.Sleep(500);
                led.Write(false);
                Thread.Sleep(250);
                led.Write(true);
                Thread.Sleep(250);
                led.Write(false);
                Thread.Sleep(125);
            }
        }

    }
}
