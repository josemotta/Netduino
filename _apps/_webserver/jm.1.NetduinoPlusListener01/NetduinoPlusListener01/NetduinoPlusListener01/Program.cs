/*
 * NetDuino Web Listener by Hari Wiguna, Sep 2010
 * Requires NetduinoPlus
 * 
 * Responds to: http://x.x.x.x/led/1 by turning onboard LED on.
 * http://x.x.x.x/led/0 turns it off.
 * 
 */

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

using Microsoft.SPOT.Net.NetworkInformation;

namespace NetduinoPlusListener01
{
    public class Program
    {
        static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

        static string receivedStr = "";
        static string responseStr = "";

        public static void Main()
        {
            WebServer webServer = new WebServer();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                networkInterface.EnableStaticIP("192.168.1.200", "255.255.255.0", "192.168.1.1");
                //networkInterface.EnableStaticIP("10.10.10.200", "255.255.255.0", "10.10.10.1");

                //networkInterface.EnableDhcp();
                //networkInterface.RenewDhcpLease();

                Debug.Print("Gateway Address: " + networkInterface.GatewayAddress);
                Debug.Print("IP Address: " + networkInterface.IPAddress);
                Debug.Print("Subnet mask " + networkInterface.SubnetMask);
            }


            while (true)
            {
                int requestLength = webServer.WaitForRequest(new RequestReceivedDelegate(RequestReceived));
                if (requestLength > 0)
                {
                    //-- Build response --
                    
                    // A well-designed browser won't invoke /led/0 or /led/1 more than once in a short period of time
                    // because it fetches the previous responses from its client-side cache. Adding a Cache-Control
                    // directive to the HTTP response header like this fixes the problem:
                    responseStr = "HTTP/1.1 200 OK\nContent-Type: text/html\n\n";
                    //responseStr = "HTTP/1.1 200 OK\nContent-Type: text/html\nCache-Control: no-cache\n\n";
                    responseStr += ProcessRequest(receivedStr);
                    
                    // IMPORTANT: SendResponse() sends response AND closes the connection, please call only once per request.
                    webServer.SendResponse(responseStr); 
                    receivedStr = "";
                }
                Thread.Sleep(100);
            }
        }

        private static string ProcessRequest(string receivedStr)
        {
            //-- Parse the first line of the request: "GET /led/1 HTTP/1.1\r" --
            string firstLine = receivedStr.Substring(0,receivedStr.IndexOf('\n'));
            string[] words = firstLine.Split(' ');
            string[] parts = words[1].Split('/');
            string cmd = parts.Length > 1 ? parts[1] : "";
            string param1 = parts.Length > 2 ? parts[2] : "";
            string param2 = parts.Length > 3 ? parts[3] : "";

            //-- Add more commands and param handling here --
            if (cmd == "led" )
            {
                led.Write(param1 == "1");
            }

            //-- Optional string to return to caller --
            return "Command executed at " + DateTime.Now.ToString();
        }

        private static void RequestReceived(object param)
        {
            receivedStr += (string)param;
        }
    }
}