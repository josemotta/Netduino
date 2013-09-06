using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NetduinoFileServer
{
    public class Program
    {
        public static void Main()
        {
            using (FileServer server = new FileServer(@"\SD\", 1554))
            {
                while (true) { }
            }
        }
    }
}
