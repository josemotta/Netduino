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
        //public static InterruptPort RC_In = new InterruptPort(Pins.GPIO_PIN_D7, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
        public static InterruptPort RC_In = new InterruptPort(Pins.GPIO_PIN_D7, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

        public static void Main()
        {
            // HUMIDITY & TEMPERATURE
            // ----------------------
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

            
            // INFRA RED
            // ---------
            //// Declare our remote control input pin
            RC_In.OnInterrupt += new NativeEventHandler(RC_In_OnInterrupt);

            //// Set the RC6 decoder's input pin to the one we just declared, and create an event handler for the code.
            //RC6_Decoder.RemoteInputPin = RC_In;
            //RC6_Decoder.CodeReceived += new CodeReceivedEventHandler(RC6_Decoder_CodeReceived);

            // NEC 

            //var necRemoteControlDecoder = new NecProtocolDecoder(Pins.GPIO_PIN_D7);

            NecProtocolDecoder.RemoteInputPin = RC_In;
            NecProtocolDecoder.OnIRCommandReceived += necRemoteControlDecoder_OnIrCommandReceived;

            
            // LOOP INFINITO
            // -------------
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

        static void necRemoteControlDecoder_OnIrCommandReceived(UInt32 irData)
        {
            Debug.Print("Ir Command Received: " + irData);
        }


        // Event handler for the RC6 pin's code event. Just pass everything to the handler in the RC6 decoder class
        static void RC_In_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            //RC6_Decoder.Record_Pulse(data1, data2, time);

            NecProtocolDecoder.OnInterrupt(data1, data2, time);
        }

        // Event handler for the code received event
        static void RC6_Decoder_CodeReceived(int mode, ulong data)
        {
            Debug.Print("Received code! Mode=" + mode.ToString() + " ... Code=0x" + UlongToHexString(data));
        }

        // Hex to string. Codes make more sense in hex as they are really four bytes
        public static string UlongToHexString(ulong val)
        {
            string s = "";
            for (int i = 0; i < 8; i++)
            {
                ulong x = val & (ulong)0x0000000F;
                val = val >> 4;
                switch (x)
                {
                    case 0: s = '0' + s; break;
                    case 1: s = '1' + s; break;
                    case 2: s = '2' + s; break;
                    case 3: s = '3' + s; break;
                    case 4: s = '4' + s; break;
                    case 5: s = '5' + s; break;
                    case 6: s = '6' + s; break;
                    case 7: s = '7' + s; break;
                    case 8: s = '8' + s; break;
                    case 9: s = '9' + s; break;
                    case 10: s = 'A' + s; break;
                    case 11: s = 'B' + s; break;
                    case 12: s = 'C' + s; break;
                    case 13: s = 'D' + s; break;
                    case 14: s = 'E' + s; break;
                    case 15: s = 'F' + s; break;
                }
            }
            return s;
        }

        //public static OutputPort outPort = new OutputPort(Pins.GPIO_PIN_D2, false);

        //public void GetRequest(Socket socket, WebServer.WebServerEventArgs e)
        //{
        //    WebServer.OutPutStream(socket, e.rawURL);
        //}
    }
}