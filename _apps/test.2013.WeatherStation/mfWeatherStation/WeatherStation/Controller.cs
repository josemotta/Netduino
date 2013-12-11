using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using uPLibrary.Hardware;
using WeatherStation.WebServer;
using WeatherStation.ErrorLogger;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.IO;
using System.Text;
using System.Net;

namespace WeatherStation
{
    /// <summary>
    /// Main controller of application
    /// </summary>
    public class Controller
    {
        // delay polling controller
        private const int CONTROLLER_DELAY = 1000;

        // hardware configuration

        // led signaling error
        private const Cpu.Pin LED_ERROR_PIN = Pins.ONBOARD_LED;

        // sensors
        private const Cpu.Pin SHT1X_CLK_PIN = Pins.GPIO_PIN_D0;
        private const Cpu.Pin SHT1X_DATA_PIN = Pins.GPIO_PIN_D1;
        private const Cpu.Pin ANEMOMETER_PIN = Pins.GPIO_PIN_D2;
        private const Cpu.AnalogChannel LDR_ANALOG_CHANNEL = AnalogChannels.ANALOG_PIN_A0;
        private const int LDR_ADC_RESOLUTION = 10;
        private const int LDR_LOAD_RESISTOR = 1500;

        // root for SD, Web Server web root directory, data logging and configuration
#if EMULATOR
        private const string SD_ROOT = @"\WINFS";
#else
        private const string SD_ROOT = @"\SD";
#endif
        private const string SD_WEBROOT = SD_ROOT + @"\webroot";
        private const string SD_DATA_LOGGING = SD_ROOT + @"\datalog";
        private const string SD_CONFIG = SD_ROOT + @"\config";

        // web server configuration
        private const int WS_BACKLOG = 10;
        private const int WS_HTTP_PORT = 80;
        private const int WS_MAX_WORKER_THREAD = 10;

        // data logger object
        private IDataLogger dataLogger;
        // error logger object
        private IErrorLogger errorLogger;
        // RTC (Real Time Clock) object
        private Rtc rtc;
        // anemometer object
        private Anemometer anemometer;
        // SHT1X object
        private SHT1X sht1x;
        // LDR object
        private Ldr ldr;
        // Web Server ocject
        private MicroWebServer uWebServer;

        // controller status data from sensors
        private double temperature;
        private double humidity;
        private double ldrResistance;
        private AmbientLight ambientLight;
        private double windSpeed;

        // configuration settings
        Configuration config;

        /// <summary>
        /// Constructor
        /// </summary>
        public Controller()
        {
            this.errorLogger = new LedErrorLogger(LED_ERROR_PIN);

#if !EMULATOR
            // check for SD presence
            this.CheckSD();
#endif

            // create sensors instances
            this.dataLogger = new SDDataLogger(SD_DATA_LOGGING);
            this.anemometer = new Anemometer(ANEMOMETER_PIN);
            this.sht1x = new SHT1X(SHT1X_DATA_PIN, SHT1X_CLK_PIN, SHT1X.Voltage.Vdd_3_5V);
            this.ldr = new Ldr(LDR_ANALOG_CHANNEL, LDR_ADC_RESOLUTION, LDR_LOAD_RESISTOR);

            // RTC
            this.rtc = Rtc.Instance;

            // configure and create Web Server
            MicroWebServerConfig uWebServerConfig = 
                new MicroWebServerConfig {
                    Backlog = WS_BACKLOG,
                    HttpPort = WS_HTTP_PORT,
                    MaxWorkerThread = WS_MAX_WORKER_THREAD,
                    WebRoot = SD_WEBROOT
                };

            this.uWebServer = new MicroWebServer(uWebServerConfig);
        }

        /// <summary>
        /// Controller initialization
        /// </summary>
        public void Init()
        {
            // load configuration
            this.config = Configuration.Load(SD_CONFIG);

            this.uWebServer.RegisterCgiCallback(this.CgiCallback);
            this.uWebServer.Start();

            this.anemometer.Start();
        }

        /// <summary>
        /// Check SD card presence
        /// </summary>
        private void CheckSD()
        {
            bool isSDCardInserted = false;
            do
            {
                VolumeInfo[] volumes = VolumeInfo.GetVolumes();
                isSDCardInserted = (volumes.Length > 0);
                if (!isSDCardInserted)
                    this.errorLogger.Log(ErrorCode.SDCardNotInserted);
                else
                    this.errorLogger.Log(ErrorCode.Success);
            }
            while (!isSDCardInserted);
        }

        public void CgiCallback(HttpContext httpContext)
        {
            // POST request
            if (httpContext.Request.HttpMethod == "POST")
            {
                // date & time configuration 
                if (httpContext.Request.URL == "config/datetime")
                {
                    // check that the body request (form parameters) contains date and time
                    // (date=YYYYMMDD&time=HHmmss)
                    if (!httpContext.Request.Form.ContainsKey("date") ||
                        !httpContext.Request.Form.ContainsKey("time"))
                        httpContext.Response.StatusCode = HttpStatusCode.BadRequest;
                    else
                    {
                        // extract date information
                        string date = httpContext.Request.Form["date"];
                        int year = Convert.ToInt32(date.Substring(0, 4));
                        int month = Convert.ToInt32(date.Substring(4, 2));
                        int day = Convert.ToInt32(date.Substring(6, 2));

                        // extract time information
                        string time = httpContext.Request.Form["time"];
                        int hour = Convert.ToInt32(time.Substring(0, 2));
                        int minute = Convert.ToInt32(time.Substring(2, 2));
                        int second = Convert.ToInt32(time.Substring(4, 2));

                        DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
                        // set datetime on RTC and into the system
                        this.rtc.SetDateTime(dateTime);
                    }
                }
                // configuration settings
                else if (httpContext.Request.URL == "config/settings")
                {
                    // check that the body request (form parameters) contains settings
                    // (ipAddress=...&subnetMask=...&gatewayAddress=....&isDhcpEnabled=...")
                    if (!httpContext.Request.Form.ContainsKey("ipAddress") ||
                        !httpContext.Request.Form.ContainsKey("subnetMask") ||
                        !httpContext.Request.Form.ContainsKey("gatewayAddress") ||
                        !httpContext.Request.Form.ContainsKey("isDhcpEnabled") ||
                        !httpContext.Request.Form.ContainsKey("isDataLogEnabled"))
                        httpContext.Response.StatusCode = HttpStatusCode.BadRequest;
                    else
                    {
                        this.config.IPAddress = IPAddress.Parse(httpContext.Request.Form["ipAddress"]);
                        this.config.SubnetMask = IPAddress.Parse(httpContext.Request.Form["subnetMask"]);
                        this.config.GatewayAddress = IPAddress.Parse(httpContext.Request.Form["gatewayAddress"]);
                        this.config.IsDhcpEnabled = (httpContext.Request.Form["isDhcpEnabled"] == "true");
                        this.config.IsDataLogEnabled = (httpContext.Request.Form["isDataLogEnabled"] == "true");
                    }

                    this.config.Save();
                }
            }
            // GET request
            else if (httpContext.Request.HttpMethod == "GET")
            {
                // get configuration settings
                if (httpContext.Request.URL == "config/settings")
                {
                    string jsonResp = "{ \"isDhcpEnabled\" : " + this.config.IsDhcpEnabled.ToString().ToLower() + "," +
                                      " \"ipAddress\" : \"" + this.config.IPAddress.ToString() + "\"," +
                                      " \"subnetMask\" : \"" + this.config.SubnetMask.ToString() + "\"," +
                                      " \"gatewayAddress\" : \"" + this.config.GatewayAddress.ToString() + "\"," +
                                      " \"isDataLogEnabled\" : " + this.config.IsDataLogEnabled.ToString().ToLower() + "}";

                    // set content type
                    //httpContext.Response.ContentType = "application/json";
                    httpContext.Response.Body = jsonResp;
                }
                // get weather station data
                else if (httpContext.Request.URL.StartsWith("sensors"))
                {
                    string jsonResp = "{ \"temperature\" : " + this.temperature + "," +
                                      " \"humidity\" : " + this.humidity + "," +
                                      " \"ldrResistance\" : " + this.ldrResistance + "," +
                                      " \"ambientLight\" : " + this.ambientLight + "," +
                                      " \"windSpeed\" : " + this.windSpeed + "}";

                    // set content type
                    //httpContext.Response.ContentType = "application/json";
                    httpContext.Response.Body = jsonResp;
                }
            }
        }

        /// <summary>
        /// Logic running into controller
        /// </summary>
        public void Run()
        {
            StringBuilder dataLogging = new StringBuilder();
#if EMULATOR
            Random rand = new Random();
#endif

            while (true)
            {
#if !EMULATOR
                // get humidity/temp
                this.temperature = System.Math.Round(this.sht1x.ReadTemperature(SHT1X.TempUnit.Celsius));
                this.humidity = System.Math.Round(this.sht1x.ReadRelativeHumidity());
                // get LDR data
                this.ldrResistance = this.ldr.GetStatus().Resistance;
                this.ambientLight = AmbientLightConverter.FromResistance(this.ldrResistance);
                // get anenometer data
                this.windSpeed = System.Math.Round(this.anemometer.WindSpeed);
#else
                // get humidity/temp
                this.temperature = rand.Next(30);
                this.humidity = rand.Next(100);
                // get LDR data
                this.ldrResistance = rand.Next(50);
                this.ambientLight = AmbientLightConverter.FromResistance(this.ldrResistance);
                // get anenometer data
                this.windSpeed = rand.Next(100);
#endif
                
                if (this.config.IsDataLogEnabled)
                {
                    if (!this.dataLogger.IsOpen)
                        this.dataLogger.Open();

                    dataLogging.Append(this.rtc.GetDateTime().ToString("HH:mm:ss"));
                    dataLogging.Append(",");
                    dataLogging.Append(humidity);
                    dataLogging.Append(",");
                    dataLogging.Append(temperature);
                    dataLogging.Append(",");
                    dataLogging.Append(ldrResistance);
                    dataLogging.Append(",");
                    dataLogging.Append(windSpeed);

                    this.dataLogger.Log(dataLogging.ToString());
                    dataLogging.Clear();
                }

                Thread.Sleep(CONTROLLER_DELAY);
            }
        }
    }
}
