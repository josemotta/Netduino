using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using CW.NETMF;

namespace NetduinoPlus2RHT03
{
    public class Program
    {
        public void GetRequest(Socket socket, WebServer.WebServerEventArgs e)
        {
            WebServer.OutPutStream(socket, e.rawURL);
        }

        public static void Main()
        {
            var RHT03 = new Dht22Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.Internal);
            Thread.Sleep(2000);
            
            WebServer w = new WebServer(80, 10000);

            w.Start();
            w.CommandReceived += delegate(object o, WebServer.WebServerEventArgs e)
            {
                //WebServer.OutPutStream(e.response, e.rawURL);
                if (RHT03.Read())
                {
                    var temperatureCelsius = RHT03.Temperature;
                    var humidity = RHT03.Humidity;
                    string answer = "DHT Sensor: RH = " + humidity.ToString("F1") + "%  Temp = " + temperatureCelsius.ToString("F1") + "°C ";
                    Debug.Print(answer);
                    WebServer.OutPutStream(e.response, answer);
                }
            };

            while (true)
            {
                //if (RHT03.Read())
                //{
                //    var temperatureCelsius = RHT03.Temperature;
                //    var humidity = RHT03.Humidity;
                //    Debug.Print("DHT sensor Read() ok, RH = " + humidity.ToString("F1") + "%, Temp = " + temperatureCelsius.ToString("F1") + "°C " + (temperatureCelsius * 1.8 + 32).ToString("F1") + "°F");
                //}

                //    Thread.Sleep(2000);
            }

            //var RHT03 = new TemperatureSensor(Cpu.AnalogChannel.ANALOG_0);
            //var temp = RHT03.Temperature;
        }
    }
}