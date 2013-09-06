using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using HttpLibrary;
using System.IO;

namespace HttpServerExample
{
    public class Program
    {
        private static HttpServer Server;
        private static OutputPort LedPin;
        private static Configuration ServerConfiguration;
        private static Credential ServerCredentials;

        public static void Main()
        {
            LedPin = new OutputPort(Pins.ONBOARD_LED, false);
            ServerConfiguration = new Configuration(80);
            ServerCredentials = new Credential("Administrator", "admin", "admin");
            Server = new HttpServer(ServerConfiguration, ServerCredentials, @"\SD\");
            Server.OnServerError += new OnServerErrorDelegate(Server_OnServerError);
            Server.OnRequestReceived += new OnRequestRecievedDelegate(Server_OnRequestReceived);
            Debug.Print(Server.Settings.IpAddress);
            Server.Start();

            while (true)
            {
                if (Server.IsServerRunning)
                {
                    LedPin.Write(true);
                    Thread.Sleep(1000);
                    LedPin.Write(false);
                    Thread.Sleep(1000);
                }
            }
        }

        static void Server_OnRequestReceived(HttpRequest Request, HttpResponse Response)
        {
            if (Request.RequestedFile != null)
            {
                string FullFileName = Request.FilesPath + Request.RequestedFile;
                if (File.Exists(FullFileName))
                {
                    Response.WriteFile(FullFileName);
                }
                else
                {
                    Response.WriteNotFound();
                }
            }
            else
            {
                Response.WriteFilesList();
            }
        }

        static void Server_OnServerError(ErrorEventArgs e)
        {
            Debug.Print(e.EventMessage);
        }
    }
}
