using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO;

namespace SDCardDirTest
{
    public class Program
    {
        public static void Main()
        {
            InterruptPort _onboardSW = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);
            _onboardSW.OnInterrupt += new NativeEventHandler(delegate(uint u1, uint u2, DateTime time)
            {
                String path = "\\SD\\www";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                Debug.Print("Int " + u1.ToString() + " " + u2.ToString() + " " + time.ToString());
                Debug.Print("CurrDir " + Directory.GetCurrentDirectory());

                PrintDir(Directory.GetDirectories("\\"));
                PrintDir(Directory.GetDirectories("\\SD"));
            });

            Thread.Sleep(Timeout.Infinite);
        }

        private static void PrintDir(string[] dirs)
        {
            Debug.Print("<");
            foreach (string dir in dirs)
            {
                Debug.Print(dir);
                foreach (string file in Directory.GetFiles(dir))
                {
                    Debug.Print(file);
                }
            }
            Debug.Print(">");
        }



    }
}
