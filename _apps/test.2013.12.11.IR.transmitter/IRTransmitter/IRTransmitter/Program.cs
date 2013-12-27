﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Diagnostics;

namespace IRTransmitter
{
    public class Program
    {
        /**
                 * Maerklin remote commander codes:
                 * light:   1.T.11000.1010000
                 * btn 1:   1.T.11000.1010001
                 * btn 2:   1.T.11000.1010010
                 * btn 3:   1.T.11000.1010011
                 * btn 4:   1.T.11000.1010100
                 * btn -:   1.T.11000.0010001
                 * btn +:   1.T.11000.0010000
                 * dir:     1.T.11000.0001101
                 **/
        private const int Address = 0x18;

        private const int SelectLight = 0x50;
        private const int SelectTrain1 = 0x51;
        private const int SelectTrain2 = 0x52;
        private const int SelectTrain3 = 0x53;
        private const int SelectTrain4 = 0x54;

        private const int SpeedDown = 0x11;
        private const int SpeedUp = 0x10;

        private const int Direction = 0x0D;


        public static void Main()
        {
            ////define the button for decrement speed
            //var btn_dec = new InterruptPort(
            //    Pins.GPIO_PIN_D0,
            //    true,
            //    Port.ResistorMode.PullUp,
            //    Port.InterruptMode.InterruptEdgeLow
            //    );

            //btn_dec.OnInterrupt += (a_, b_, dt_) =>
            //{
            //    codec.Send(Address, SpeedDown);
            //};

            ////define the button for increment speed
            //var btn_inc = new InterruptPort(
            //    Pins.GPIO_PIN_D1,
            //    true,
            //    Port.ResistorMode.PullUp,
            //    Port.InterruptMode.InterruptEdgeLow
            //    );

            //btn_inc.OnInterrupt += (a_, b_, dt_) =>
            //{
            //    codec.Send(Address, SpeedUp);
            //};


            //create the infrared transmitter driver
            var irtx = new InfraredTransmitter(Pins.GPIO_PIN_D8);

            //create the codec to be used
            var codec = new InfraredCodecNEC(irtx);
            //codec.ExtendedMode = true;

            //define button for sending IR command
            var btn_dir = new InterruptPort(
                Pins.GPIO_PIN_D2,
                true,
                Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLow
                );

            btn_dir.OnInterrupt += (a_, b_, dt_) =>
            {
                Debug.Print("sending ...");
                codec.Send(0xFF, 0xE0);
            };
            
            //Thread.Sleep(Timeout.Infinite);

            // send non-stop sequence
            
            int i = 0;
            //int[] x = {0xE0, 0xEA, 0x0E, 0x15};
            //int[] x = { 0xFF, 0xFF, 0xFF, 0xFF };
            //int[] x = { 0x0D, 0x1F, 0x0D, 0x1F };
            int[] x = { 0xF2, 0xE3, 0xEA, 0xE0 }; // on, yellow, white, off

            while (true)
            {
                //codec.Send(0x00, x[i++]);
                //codec.Send(x[i++], 0x00);
                Debug.Print("... cmd " + i);
                codec.Send(0x00, i++);
                i = i % 256;
                Thread.Sleep(2000);
            }
        }
    }
}
