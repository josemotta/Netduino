using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace HttpLibrary
{
    /// <summary>
    /// Configuration class for holding server configuration
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Listening ip address
        /// </summary>
        public string IpAddress;
        /// <summary>
        /// Network mask
        /// </summary>
        public string SubnetMask;
        /// <summary>
        /// Network default gateway
        /// </summary>
        public string DefaultGateWay;
        /// <summary>
        /// Listening port
        /// </summary>
        public int ListenPort;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="listenport">Listening port</param>
        public Configuration(int listenport)
        {
            IpAddress = "";
            ListenPort = listenport;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="ipaddress">Listening ip address</param>
        /// <param name="subnetmask">Network mask</param>
        /// <param name="defaultgateWay">Default gateway</param>
        /// <param name="listenport">Listening port</param>
        public Configuration(string ipaddress, string subnetmask, string defaultgateWay, int listenport)
        {
            IpAddress = ipaddress;
            SubnetMask = subnetmask;
            DefaultGateWay = defaultgateWay;
            ListenPort = listenport;
        }
        /// <summary>
        /// Override of ToString() method
        /// </summary>
        /// <returns>A string with configuration parameters each followed by a new line</returns>
        public override string ToString()
        {
            return "IpAddress : " + IpAddress + "\n" +
                "SubnetMask : " + SubnetMask + "\n" +
                "Default GateWay : " + DefaultGateWay + "\n" +
                "Port : " + ListenPort.ToString();
        }
    }
}
