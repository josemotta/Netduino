using System;
using System.Threading;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoSerbDemo
{
    public class RandomTest
    {
        private Serb serb = new Serb(Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6);

        private Random rand = new Random();

        public void Run()
        {
            while (true)
            {
                TurnRandom(100, 1000);
                //Turns randomly left or right for a random time period between .1 second and one second
                GoForwardRandom(1000, 2000); //Goes forward for a random time period between 1 and 2 seconds

                serb.Stop(); //Stops the robot
                Thread.Sleep(2000); //pauses for 2 seconds (whilst stopped)
            }
        }

        /*
        * turns the robot randomly left or right for a random time period between
        * minTime (milliseconds) and maxTime (milliseconds)
        */
        void TurnRandom(int minTime, int maxTime)
        {
            int choice = rand.Next(2);                     //Random number to decide between left (1) and right (0)
            int turnTime = minTime + rand.Next(maxTime - minTime);     //Random number for the pause time

            if (choice == 1) { serb.GoLeft(); }                 //If random number = 1 then turn left
            else { serb.GoRight(); }                           //If random number = 0 then turn right
            Thread.Sleep(turnTime);                            //delay for random time                         
        }

        /*
         * goes forward for a random time period between minTime (milliseconds)
         * and maxTime (milliseconds)
         */
        void GoForwardRandom(int minTime, int maxTime)
        {
            int forwardTime = minTime + rand.Next(maxTime - minTime);     //determine a random time to go forward
            serb.GoForward();                                    //sets the SERB forward
            Thread.Sleep(forwardTime);                             //delays for random time period
        }
    }
}