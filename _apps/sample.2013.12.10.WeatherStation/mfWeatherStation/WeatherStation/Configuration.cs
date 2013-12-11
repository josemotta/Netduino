using System;
using Microsoft.SPOT;
using System.Net;
using System.IO;
using Microsoft.SPOT.Net.NetworkInformation;

namespace WeatherStation
{
    /// <summary>
    /// Weather station configuration
    /// </summary>
    public class Configuration
    {

        #region Constants...

        // configuration file name
        private string CONFIG_FILE_NAME = "config.dat";

        // default configuration
        private string DEFAULT_IP_ADDRESS = "192.168.1.201";
        private string DEFAULT_SUBNETMASK_ADDRESS = "255.255.255.0";
        private string DEFAULT_GATEWAY_ADDRESS = "192.168.1.1";
        private bool DEFAULT_DHCP_ENABLED = false;

        private bool DEFAULT_ENABLE_DATALOG = false;

        #endregion

        #region Properties...

        /// <summary>
        /// IP address
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Subnet mask
        /// </summary>
        public IPAddress SubnetMask { get; set; }

        /// <summary>
        /// Gateway address
        /// </summary>
        public IPAddress GatewayAddress { get; set; }

        /// <summary>
        /// DHCP status (enabled/disabled)
        /// </summary>
        public bool IsDhcpEnabled { get; set; }

        /// <summary>
        /// Data Logging status (enabled/disabled)
        /// </summary>
        public bool IsDataLogEnabled { get; set; }

        #endregion

        // singleton class instance
        private static Configuration instance;
        // configuration file path
        private string filePath;
                
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storagePath">Storage path for configuration information</param>
        private Configuration(string storagePath)
        {
            string filePath = Path.Combine(storagePath, CONFIG_FILE_NAME);
            this.filePath = filePath;

            // create configuration folder
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            // create file with default configuration
            if (!File.Exists(filePath))
            {
                FileStream fs = File.Create(filePath);
                TextWriter tw = new StreamWriter(fs);
                tw.WriteLine(DEFAULT_IP_ADDRESS);
                tw.WriteLine(DEFAULT_SUBNETMASK_ADDRESS);
                tw.WriteLine(DEFAULT_GATEWAY_ADDRESS);
                tw.WriteLine("false");
                tw.WriteLine("false");
                tw.Close();
                fs.Close();

                this.IPAddress = IPAddress.Parse(DEFAULT_IP_ADDRESS);
                this.SubnetMask = IPAddress.Parse(DEFAULT_SUBNETMASK_ADDRESS);
                this.GatewayAddress = IPAddress.Parse(DEFAULT_GATEWAY_ADDRESS);
                this.IsDhcpEnabled = DEFAULT_DHCP_ENABLED;
                this.IsDataLogEnabled = DEFAULT_ENABLE_DATALOG;
            }
            // load configuration from file
            else
            {
                FileStream fs = File.Open(filePath, FileMode.Open);
                TextReader tr = new StreamReader(fs);

                this.IPAddress = IPAddress.Parse(tr.ReadLine());
                this.SubnetMask = IPAddress.Parse(tr.ReadLine());
                this.GatewayAddress = IPAddress.Parse(tr.ReadLine());
                this.IsDhcpEnabled = (tr.ReadLine() == "true");
                this.IsDataLogEnabled = (tr.ReadLine() == "true");

                tr.Close();
                fs.Close();
            }

#if !EMULATOR
            // configure network settings
            this.ConfigureNetwork();
#endif
        }

        /// <summary>
        /// Load Configuration singleton instance
        /// </summary>
        /// <param name="storagePath">Storage path for configuration information</param>
        /// <returns>Singleton Configuration instance</returns>
        public static Configuration Load(string storagePath)
        {
            if (instance == null)
                instance = new Configuration(storagePath);
            return instance;
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        public void Save()
        {
            FileStream fs = File.Open(this.filePath, FileMode.Open);
            TextWriter tw = new StreamWriter(fs);
            tw.WriteLine(this.IPAddress.ToString());
            tw.WriteLine(this.SubnetMask.ToString());
            tw.WriteLine(this.GatewayAddress.ToString());
            tw.WriteLine(this.IsDhcpEnabled ? "true" : "false");
            tw.WriteLine(this.IsDataLogEnabled ? "true" : "false");
            tw.Close();
            fs.Close();

#if !EMULATOR
            // configure network settings
            this.ConfigureNetwork();
#endif
        }

        /// <summary>
        /// Configure network settings
        /// </summary>
        private void ConfigureNetwork()
        {
            // get network interfaces but consider only the first interface
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // DHCP setting
            if (networkInterfaces[0].IsDhcpEnabled != this.IsDhcpEnabled)
            {
                // if DHCP enabled
                if (this.IsDhcpEnabled)
                    networkInterfaces[0].EnableDhcp();
            }

            // without DHCP, static network settings
            if (!this.IsDhcpEnabled)
            {
                // if IP address is changed
                if (networkInterfaces[0].IPAddress != this.IPAddress.ToString())

                    networkInterfaces[0].EnableStaticIP(this.IPAddress.ToString(),
                                                        this.SubnetMask.ToString(),
                                                        this.GatewayAddress.ToString());
            }
        }
    }
}
