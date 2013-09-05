using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoSerbRemote
{
    public class Program
    {
        private static OutputPort statusLed;
        private static SerialPort serbSerial;
        private static WiiChuck wiiChuck;

        public static void Main()
        {
            statusLed = new OutputPort(Pins.ONBOARD_LED, false);

            // setup output port
            serbSerial = new SerialPort(SerialPorts.COM1, 115200);
            serbSerial.Open();

            // initialize wii chuck
            wiiChuck = new WiiChuck(true);

            Debug.Print("SERB WiiRemote - Ready");

            while (true)
            {
                // try to read the data from nunchucku
                if (wiiChuck.GetData() == false)
                {
                    continue;
                }

#if DEBUG                
                WiiChuck.PrintData(wiiChuck);
#endif

                // signal data read
                statusLed.Write(true);

                float posX, posY;
                if (wiiChuck.ZButtonDown)
                {
                    // if Z-button is pressed take vales form acclerometer
                    posX = InRange(wiiChuck.AccelerationXGs*1.25f, -1.0f, 1.0f);
                    posY = InRange(wiiChuck.AccelerationYGs*1.5f, -1.0f, 1.0f);
                }
                else
                {
                    // otherwise take values from analog joystick
                    posX = wiiChuck.AnalogX;
                    posY = wiiChuck.AnalogY;
                }

                // send values to robot
                MoveDifferential((int) (posY * 100f), (int) (posX * 100f));

                statusLed.Write(false);

                // wait a little
                Thread.Sleep(20); 
            }
        }

        private static float InRange(float val, float min, float max)
        {
            if (val > max) return max;
            if (val < min) return min;
            return val;
        }

        private static void MoveDifferential(int speed, int direction)
        {
            speed = DeadBandFilter(speed);
            direction = DeadBandFilter(direction);

#if DEBUG
            Debug.Print("speed: " + speed + " \t dir: " + direction);
#endif
            if (speed != 0 || direction != 0)
            {
                // convert to motor speed
                byte x = (byte) (speed + direction + 127);
                byte y = (byte) (speed - direction + 127);

                //setSpeedLeft(speed1 + direction1);
                //setSpeedRight(speed1 - direction1);
                SendCommand("AAAX", x);
                SendCommand("AAAY", y);
            }
            else
            {
                SendCommand("AAAS",0); // stop
            }
        }

        static byte[] serbBuffer = new byte[5];

        private static void SendCommand(string cmd, byte arg)
        {
            for (byte i = 0; i < 4; i++)
                serbBuffer[i] = (byte) cmd[i];
            
            serbBuffer[4] = arg;
            
            if (serbSerial.IsOpen)
                serbSerial.Write(serbBuffer, 0, serbBuffer.Length);
        }

        // A percentage away from center that is interpretted as still being zero
        const int DeadBand = 20;

        private static int DeadBandFilter(int value)
        {
            if (value > -DeadBand && value < DeadBand)
            {
                value = 0;
            }
            else
            {
                if (value > 0)
                {
                    value = value - DeadBand*100/(100 - DeadBand);
                }
                else
                {
                    value = value + DeadBand*100/(100 - DeadBand);
                }
            }
            return value;
        }
    }
}
