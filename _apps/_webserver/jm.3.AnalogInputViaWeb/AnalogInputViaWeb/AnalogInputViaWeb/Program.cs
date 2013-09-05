using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace AnalogInputViaWeb
{
    public class Program
    {
        const string WebFolder = "\\SD\\Web";
        public static string trimpot = "0";

        public static void Main()
        {
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            AnalogInput an0 = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);

            //an0. SetRange(0, 100);
            trimpot = an0.Read().ToString();

            Listener webServer = new Listener(RequestReceived);

            while (true)
            {
                // Blink LED to show we're still responsive 
                led.Write(!led.Read());
                trimpot = an0.Read().ToString();
                Thread.Sleep(500);
            }

        }

        private static void RequestReceived(Request request)
        {
            // Quick'n'dirty ajax call 
            if (request.URL == @"/analog0")
                request.SendResponse(trimpot);
            else
                request.Send404();
        } 
    }
}
