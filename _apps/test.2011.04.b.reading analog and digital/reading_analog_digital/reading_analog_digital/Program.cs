using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Diagnostics;

namespace NetduinoPlusApplication1
{
    public class Program
    {
        public static void Main()
        {
            var axisX = new AnalogInput(Pins.GPIO_PIN_A0);
            var axisY = new AnalogInput(Pins.GPIO_PIN_A1);
            var axisZ = new AnalogInput(Pins.GPIO_PIN_A2);
            var IrFloorSensor = new AnalogInput(Pins.GPIO_PIN_A3);


            var out1 = new OutputPort(Pins.GPIO_PIN_D1, false);
            var out2 = new OutputPort(Pins.GPIO_PIN_D12, false);


            var in1 = new InputPort(Pins.GPIO_PIN_D2, false, Port.ResistorMode.Disabled);
            var in2 = new InputPort(Pins.GPIO_PIN_D4, false, Port.ResistorMode.PullUp);


            var servo1 = new PWM(Pins.GPIO_PIN_D9);
            servo1.SetDutyCycle(0);
            var servo2 = new PWM(Pins.GPIO_PIN_D10);
            servo2.SetDutyCycle(0);


            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            int i = 0;
            bool digState = false;


            while (i < 5000)
            {
                axisX.Read();
                axisY.Read();
                axisZ.Read();
                IrFloorSensor.Read();


                in1.Read();
                in2.Read();


                digState = !digState;
                out1.Write(digState);
                out2.Write(digState);


                servo1.SetPulse(20000, 1500);
                servo2.SetPulse(20000, 1500);


                i++;
            }
            stopWatch.Stop();
            Debug.Print("Elapsed: " + stopWatch.ElapsedMilliseconds.ToString());

        }

    }
}
