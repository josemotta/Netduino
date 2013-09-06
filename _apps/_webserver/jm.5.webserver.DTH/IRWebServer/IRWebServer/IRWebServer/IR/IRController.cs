using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace IRWebServer
{
    /*
     * A large portion of this (alomst all) was adapted from: 
     * http://highfieldtales.wordpress.com/2012/02/07/infrared-transmitter-driver-for-netduino/
     * Many thanks for the excellent code!
     */
    public class IRController
    {
        //Dummy object for the lock statement.
        private static object monitor = new object();
        
        //Declare some variables.
        private SPI spi;
        private int pulse_width;
        private int pulse_space;
        private ushort[] preamble;
        private float bitTime;
       
        /* The width is the number of microseconds a single
         * IR pulse lasts, while the space is the number of
         * microseconds between each pulse. Since most remotes
         * have a bit of preamble (a long on or off pulse) before
         * data is sent, the pre variable allows you to assign a preamble
         * where each entry in the outer int array will be another int
         * array of two items: the pulse length, and the value (0 or 1).
         * 
         * For example:
         * 
         * int[][] pre = {new int[]{150, 1}, new int[]{75, 0},...}
         */

        public IRController(float frequency, int width, int space, int[][] pre)
        {
            //Set some variables.
            pulse_width = width;
            pulse_space = space;

            //Calculates the actual period for pushing out one
            //ushort value, interleave including.
            float carrier = 1 / frequency;
            bitTime = carrier - 2e-3f;

            //calculates the equivalent SPI frequency,
            //note that an "unshort" is 16 bits.
            uint spi_freq = (uint)(16.0f / bitTime);
            //uint spi_freq = (uint)frequency;

            SPI.Configuration config = new SPI.Configuration(
               Pins.GPIO_NONE,    // SS-pin (not used, but it could be)
               false,             // SS-pin active state (not used)
               0,                 // The setup time for the SS port (not used)
               0,                 // The hold time for the SS port (not used)
               false,             // The idle state of the clock (not used)
               true,              // The sampling clock edge (not used)
               spi_freq,          // The SPI clock rate in KHz
               SPI_Devices.SPI1   // The used SPI bus (refers to a MOSI MISO and SCLK pinset)
            );

            spi = new SPI(config);

            //Create the preamble.
            int length = 0;
            int duration = 0;
            foreach (int[] item in pre)
            {
                length += (int)((float)item[0] / (bitTime * 1000f));
            }
            preamble = new ushort[length];
            int index = 0;

            foreach (int[] item in pre)
            {
                if (item[1] == 1)
                {
                    duration = (int)((float)item[0] / (bitTime * 1000f));
                    for (int i = 0; i < duration; i++)
                    {
                        preamble[index] = (ushort)0xff;
                        index++;
                    }
                }
                else
                {
                    duration = (int)((float)item[0] / (bitTime * 1000f));
                    for (int i = 0; i < duration; i++)
                    {
                        preamble[index] = (ushort)0x0;
                        index++;
                    }
                }
            }
        }

        //This reiecives an int array of 1 and 0, which is the IR command to send.
        public void Send(int[] command)
        {
            //Calculate some needed parameters.
            int index = preamble.Length;
            int pulse_length = (int)(pulse_width / (this.bitTime * 1000f));
            int space_length = (int)(pulse_space / (this.bitTime * 1000f));
            //Calculates the total length of the command buffer.
            ushort[] buffer = new ushort[preamble.Length + (command.Length * pulse_length) + ((command.Length - 1) * space_length)];

            //Add the preamble to the command buffer.
            preamble.CopyTo(buffer, 0);
            
            //Build the command buffer.
            foreach (int bit in command)
            {
                if (bit == 1)
                {
                    for (int i = 0; i < pulse_length; i++)
                    {
                        buffer[index] = (ushort)0xff;
                        index++;
                    }
                }
                else
                {
                    for (int i = 0; i < pulse_length; i++)
                    {
                        buffer[index] = (ushort)0x0;
                        index++;
                    }
                }
                //This prevents adding a space to the end of the command buffer.
                if (index < buffer.Length)
                {
                    //Add the space between IR pulses.
                    for (int i = 0; i < space_length; i++)
                    {
                        buffer[index] = (ushort)0x00;
                        index++;
                    }
                }
            }
            //Call the function to transmit the command buffer.
            pulseLED(buffer);
            Debug.Print("command sent");
        }

        private void pulseLED(ushort[] buffer)
        {
            //Apply a lock to prevent more than one command from sending at a time.
            lock (IRController.monitor)
            {
                Debug.Print("Sending IR command");
                //Transmit the signal.
                spi.Write(buffer);                
            }            
        }
    }
}
