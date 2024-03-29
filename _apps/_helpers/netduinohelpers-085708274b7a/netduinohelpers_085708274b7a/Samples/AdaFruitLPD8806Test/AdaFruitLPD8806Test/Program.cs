﻿using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoMini;
using netduino.helpers.Hardware;

namespace AdaFruitLPD8806Test {
    public class Program {
        public const int x = 16;
        public const int y = 10;        
        public static AdaFruitLPD8806 strip = new AdaFruitLPD8806(x, y, Pins.GPIO_PIN_13, SPI.SPI_module.SPI1,10000);
        public static SerialPort comPort = new SerialPort(Serial.COM1, 115200, Parity.None, 8, StopBits.One);
        private static UTF8Encoding _encoding = new UTF8Encoding();

        public static void Main() {            
            comPort.ReadTimeout = 20;
            comPort.WriteTimeout = 20;
            comPort.Open();

            Display("Start\r\n");

            strip.Reset();
            strip.Refresh(0);

            strip.CopyVideoStripBitmap(Pumpkin);
            strip.Refresh(0);
            Thread.Sleep(1000);

            DisplayAnimation(PacMan);
            Thread.Sleep(1000);
            
            DisplayAnimation(Ghost);
            Thread.Sleep(1000);

            DisplayGradient();
            Thread.Sleep(1000);

            DisplayShift();
            Thread.Sleep(1000);

            DisplayGradient();
            DisplayShift(false);
            DisplayGradient();

            Display("Stop\r\n");

            comPort.Close();
        }

        public static void DisplayGradient() {
            strip.Gradient(
                255, 0, 0,
                128, 0, 128,
                0, strip.PixelCount - 1);
        }

        public static void DisplayShift(bool goRight = true) {
            var count = 160;
            while (count >= 0) {
                strip.Refresh(0);
                if (goRight) strip.ShiftRight(true);
                else strip.ShiftLeft(true);
                count--;
            }
        }

        public static void DisplayAnimation(byte[] frames, int cycles = 6) {
            var frame = -1;
            var maxFrame = (frames.Length / strip.FrameSize) - 1;
            var count = maxFrame * 2 * cycles;
            var direction = 1;

            while (count-- > 0) {
                if (frame >= maxFrame) {
                    direction = -1;
                } else {
                    if (frame == 0) {
                        direction = 1;
                    }
                }
                frame += direction;
                strip.CopyVideoStripBitmap(frames, strip.FrameSize * frame);
                strip.Refresh(100);
            }
        }
        public static byte[] Pumpkin = {
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xe6,0xb5,0x9d,0xe6,0xb5,0x9d,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xe6,0xb5,0x9d,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0xe6,0xb5,0x9d,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0xbc,0xb7,0x80,0xbc,0xb7,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80
                                     };

        public static byte[] Ghost = {
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0xff,0x80,0x80,0xff,0x80,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,

        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0xff,0x80,0x80,0xff,0x80,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,

        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0xff,0x80,0x80,0xff,0x80,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0x80,0xff,0x80,0x80,0xff,0x80,0xc3,0xc3,0xc3,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,

        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xff,0xff,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0xff,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0xff,0x80,0x80,0xff,0x80,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0xff,0xc3,0xc3,0xc3,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
        0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
                                       };

        public static byte[] PacMan = {
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,

        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xae,0xff,0xc9,0xae,0xff,0xc9,0xae,0xff,0xc9,0xae,0xff,0xc9,0xae,0xff,0xc9,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,

        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0x80,0xff,0xff,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,

        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,

        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,

        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0x80,0x80,0x80,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
        0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xf2,0xff,0x80,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                                      };

        public static void Display(string line) {    
            byte[] bytes = _encoding.GetBytes(line);
            comPort.Write(bytes, 0, bytes.Length);
        }
    }
}
