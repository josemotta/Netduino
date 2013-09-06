using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Net.Sockets;
using System.Net;

namespace IRWebServer
{
    class NTP
    {
        public static Boolean timeSet;
        public static DateTime GetNetworkTime(int UTC_offset)
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
            int offset;
            if (ST_DST <= 50 && ST_DST >= 1)
            {
                DST = 1;
                offset = ((UTC_offset + DST) * 100);
            }
            else
            {
                offset = UTC_offset;
            }
            if (offset < 0)
            {
                Program.offset = "-0" + offset.ToString();
            }
            else
            {
                Program.offset = "0" + offset.ToString();
            }
            TimeSpan offsetAmount = new TimeSpan(UTC_offset + DST, 0, 0);
            DateTime LocalDateTime = (UTC_Time + offsetAmount);
            return LocalDateTime;
        }

        public static void SetDeviceNTP(int TimeZone)
        {
            //Set the time on the Netduino.
            Utility.SetLocalTime(GetNetworkTime(TimeZone));
        }
    }
}
