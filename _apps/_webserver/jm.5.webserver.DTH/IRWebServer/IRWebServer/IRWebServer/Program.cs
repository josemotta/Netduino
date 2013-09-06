using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using VariableLabs.PowerManagment;
using HttpLibrary;

namespace IRWebServer
{
    public class Program
    {

        //Set your timezone here.
        public const int TimeZone = -5;
        public static string offset;
        private static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        //Create the inside temperature object.
        private static HIH6130 tempSensor = new HIH6130(0x27, power: Pins.GPIO_PIN_D8);
        //Create the IR controller object (create multiple objects if you have multiple protocols).
        private static IRController IR = new IRController(38.0f, 550, 545, new int[][] {new int[] {8650, 1}, new int[] {4250, 0}});
        //Add your IR commands here.
        private static readonly int[][] commands = {
            new int[]{1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1},
            new int[]{1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1},
        };
        private static ACStatus AC;
        private static IRCommands commander;
        private static Timers timers;
        private static requestHandler handler;
        private static HttpServer Server;
        private static Credential ServerCredential;
        private static Configuration ServerConfiguration;

        public static void Main()
        {
            //Start the server time setting thread.
            new Thread(() => NTP.UpdateTime()).Start(); 
            while (!NTP.timeSet)
                Thread.Sleep(100);
            Thread.Sleep(100);

            PowerManagment.SetPeripheralState(Peripheral.PowerLED, false);
            //Start the server itself.
            ServerConfiguration = new Configuration(8081);
            ServerCredential = new Credential(new string[] { "Test" }, "/auth.htm", new string[] { "/auth.htm", "/img/Log-In.png", "/css/main.css", "/lib/GetInTemp.js", "/lib/Login.js" });
            Server = new HttpServer(ServerConfiguration, ServerCredential, 1024, 1024, @"\SD\htdocs");
            Server.OnServerError += new OnServerErrorDelegate(Server_OnServerError);
            AC = new ACStatus(Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D10);
            commander = new IRCommands(IR, commands);
            timers = new Timers(AC, tempSensor, commander);
            handler = new requestHandler(@"\SD\htdocs", commander, tempSensor, AC, timers, ServerCredential);        
            Server.OnRequestReceived += new OnRequestRecievedDelegate(handler.processRequest);
            Server.Start();
            
            //Start the temp management thread.
            new Thread(timers.tempControl).Start();
            //Blink LED three times to show we're up and running.
            for (int i = 0; i < 6; i++)
            {
                led.Write(!led.Read());
                Thread.Sleep(350);
            }
            led.Write(false);
        }

        static void Server_OnServerError(ErrorEventArgs e)
        {
            Debug.Print(e.EventMessage);
            Server.Start();
        }
    }
}
