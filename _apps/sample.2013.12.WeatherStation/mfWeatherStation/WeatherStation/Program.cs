using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using WeatherStation.WebServer;
using Microsoft.SPOT.IO;
using System.IO;
using uPLibrary.Hardware;

namespace WeatherStation
{
    public class Program
    {
        public static void Main()
        {
            
            Controller controller = new Controller();
            controller.Init();
            controller.Run();
            
            Thread.Sleep(Timeout.Infinite);
        }

        
    }

    
}
