using System;
using Microsoft.SPOT;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Configuration for Web Server
    /// </summary>
    public class MicroWebServerConfig
    {
        /// <summary>
        /// HTTP port value
        /// </summary>
        public int HttpPort { get; set; }

        /// <summary>
        /// Maximum number of incoming connections that can be queued for acceptance
        /// </summary>
        public int Backlog { get; set; }

        /// <summary>
        /// Maximum number of worker thread for processing request simultaneously
        /// </summary>
        public int MaxWorkerThread { get; set; }

        /// <summary>
        /// Root for the file system Web Server
        /// </summary>
        public string WebRoot { get; set; }
    }
}
