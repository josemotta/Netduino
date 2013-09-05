using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SilverlightOnNetduino
{
    public partial class MainPage : UserControl
    {
        #region Properties

        /// <summary>
        /// Timer which determines when readings are collected from the N+.
        /// </summary>
        private DispatcherTimer DataTimer { get; set; }

        /// <summary>
        /// Web server hosted on the N+
        /// </summary>
        private WebClient NetduinoWebServer { get; set; }

        /// <summary>
        /// ViewModel containing the temperature readings.
        /// </summary>
        private TemperatureViewModel TemperatureReadings { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainPage()
        {
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Page has loaded and the components are ready to be manipulated.
        /// </summary>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            NetduinoWebServer = new WebClient();
            NetduinoWebServer.DownloadStringCompleted += new DownloadStringCompletedEventHandler(NetduinoWebServer_DownloadStringCompleted);
            DataTimer = new DispatcherTimer();
            DataTimer.Tick += new EventHandler(DataTimer_Tick);
            DataTimer.Interval = new TimeSpan(0, 0, 1);
            TemperatureReadings = new TemperatureViewModel();
            LayoutRoot.DataContext = TemperatureReadings;
        }

        /// <summary>
        /// Timer has fired so get the data from the server.
        /// </summary>
        private void DataTimer_Tick(object sender, EventArgs e)
        {
            if (!NetduinoWebServer.IsBusy)
            {
                NetduinoWebServer.DownloadStringAsync(new Uri("http://192.168.5.100/Command.html?gettemperature"));
            }
        }

        /// <summary>
        /// Data download is complete, add the data to the view model (if valid).
        /// </summary>
        private void NetduinoWebServer_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string[] data;

                data = ((string) e.Result).Split(':');
                //
                //  TODO: Check the data is valid here and work out which sensor.
                //
                TemperatureReadings.Add(new TemperatureReading(DateTime.Now, double.Parse(data[1])));
            }
        }

        /// <summary>
        /// Start or stop the timer depending upon the current state.
        /// </summary>
        private void btnStartStopTimer_Click(object sender, RoutedEventArgs e)
        {
            if (DataTimer.IsEnabled)
            {
                DataTimer.Stop();
                btnStartStopTimer.Content = "Start";
            }
            else
            {
                DataTimer.Start();
                btnStartStopTimer.Content = "Stop";
            }
        }

        #endregion
    }
}
