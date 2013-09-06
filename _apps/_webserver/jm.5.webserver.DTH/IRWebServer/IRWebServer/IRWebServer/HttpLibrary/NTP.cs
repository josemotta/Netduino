using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Net.Sockets;
using System.Net;

namespace HttpLibrary
{
    static class NTP
    {
        /// <summary>
        /// Returns if the last time set was successful
        /// </summary>
        public static Boolean timeSet;
        /// <summary>
        /// The current timezone for the server
        /// </summary>
        public static int timezone = -5;
        private static DateTime GetNetworkTime(int UTC_offset)
        {
            //Set the time server here.
            timeSet = false;
            string S = "";
            while (!timeSet)
            {
                EndPoint ep = new IPEndPoint(Dns.GetHostEntry("time.nist.gov").AddressList[0], 13);
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                byte[] msg = new Byte[50];
                s.Connect(ep);
                s.Send(new byte[1], 0, 0);
                s.Receive(msg);
                S = new String(System.Text.Encoding.UTF8.GetChars(msg));
                s.Close();
                if (S != null && S != "")
                {
                    timeSet = true;
                }
                else
                {
                    Thread.Sleep(250);
                }
            }
            DateTime UTC_Time = new DateTime(Convert.ToInt32("20" + S.Substring(7, 2)), Convert.ToInt32(S.Substring(10, 2)), Convert.ToInt32(S.Substring(13, 2)), Convert.ToInt32(S.Substring(16, 2)), Convert.ToInt32(S.Substring(19, 2)), Convert.ToInt32(S.Substring(22, 2)));

            int ST_DST = Convert.ToInt32(S.Substring(25, 2));

            //Correct for the time zone and daylight savings.
            int DST = 0;
            if (ST_DST <= 50 && ST_DST >= 1)
                DST = 1;
            TimeSpan offsetAmount = new TimeSpan(UTC_offset + DST, 0, 0);
            DateTime LocalDateTime = (UTC_Time + offsetAmount);
            return LocalDateTime;
        }

        /// <summary>
        /// Sets the server time
        /// </summary>
        /// <param name="timezone">Timezine ofthe server</param>
        public static void SetDeviceNTP(int timezone)
        {
            //Set the time on the Netduino.
            Utility.SetLocalTime(GetNetworkTime(timezone));
        }

        /// <summary>
        /// Use this to create a periodic time update thread
        /// </summary>
        /// <param name="period">The update period</param>
        public static void UpdateTime(int period = 43200000)
        {
            while (true)
            {
                //Set the server date/time via NTP
                Debug.Print("Setting server time");
                Guarddog guard = new Guarddog(10000);
                guard.running = false;
                SetDeviceNTP(timezone);
                guard.Dispose();
                Debug.Print("Server time set to: " + DateTime.Now.ToString());
                Thread.Sleep(period);
            }
        }
    }
}
