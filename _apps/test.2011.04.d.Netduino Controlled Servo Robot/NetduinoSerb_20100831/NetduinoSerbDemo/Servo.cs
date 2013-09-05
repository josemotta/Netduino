/* 
 * Servo NETMF Driver 
 *      Coded by Chris Seto August 2010 
 *      <chris@chrisseto.com>  
 *       
 * Use this code for whatveer you want. Modify it, redistribute it, I don't care. 
 * I do ask that you please keep this header intact, however. 
 * If you modfy the driver, please include your contribution below: 
 *  
 * Chris Seto: Inital release (1.0) 
 * Chris Seto: Netduino port (1.0 -> Netduino branch) 
 *  
 *  
 * */

using System;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;

namespace NetduinoSerbDemo
{
    public class Servo : IDisposable
    {
        /// <summary> 
        /// PWM handle 
        /// </summary> 
        private readonly PWM _servo;

        /// <summary> 
        /// Timings range 
        /// </summary> 
        private readonly int[] range = new int[2];

        /// <summary> 
        /// Set Servo inversion 
        /// </summary> 
        private bool _inverted;

        private double _degree;

        public bool Inverted
        {
            get { return _inverted; }
            set
            {
                _inverted = value;
                SetPwmValue(_degree);
            }
        }

        /// <summary> 
        /// Create the PWM pin, set it low and configure timings 
        /// </summary> 
        /// <param name="pin"></param> 
        public Servo(Cpu.Pin pin)
            : this(pin, false)
        { }

        public Servo(Cpu.Pin pin, bool inverted)
        {
            // Init the PWM pin 
            _servo = new PWM(pin);
            _inverted = inverted;

            // See what the Netduino team say about this...  
            //servo.Set(false); 

            // Typical settings 
            // [sk] changed to match with http://www.arduino.cc/playground/ComponentLib/Servo
            range[0] = 544;
            range[1] = 2400;
        }

        public void Dispose()
        {
            _servo.Dispose();
        }

        /// <summary> 
        /// Allow the user to set cutom timings 
        /// </summary> 
        /// <param name="fullLeft"></param> 
        /// <param name="fullRight"></param> 
        public void SetRange(int fullLeft, int fullRight)
        {
            range[1] = fullLeft;
            range[0] = fullRight;
        }

        /// <summary> 
        /// Disengage the servo.  
        /// The Servo motor will stop trying to maintain an angle 
        /// </summary> 
        public void Disengage()
        {
            // See what the Netduino team say about this...  
            //servo.Set(false); 
        }

        /// <summary> 
        /// Set the Servo degree 
        /// </summary> 
        public double Degree
        {
            get { return _degree; }
            set
            {
                if (_degree != value)
                {
                    _degree = value;
                    SetPwmValue(_degree);
                }
            }
        }

        private void SetPwmValue(double value)
        {
            // Range checks 
            if (value > 180)
                value = 180;

            if (value < 0)
                value = 0;

            // Are we inverted? 
            if (_inverted)
                value = 180 - value;

            // Set the pulse 
            _servo.SetPulse(20000, (uint)Map((long)value, 0, 180, range[0], range[1]));
        }

        /// <summary> 
        /// Used internally to map a value of one scale to another 
        /// </summary> 
        /// <param name="x"></param> 
        /// <param name="in_min"></param> 
        /// <param name="in_max"></param> 
        /// <param name="out_min"></param> 
        /// <param name="out_max"></param> 
        /// <returns></returns> 
        private static long Map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}