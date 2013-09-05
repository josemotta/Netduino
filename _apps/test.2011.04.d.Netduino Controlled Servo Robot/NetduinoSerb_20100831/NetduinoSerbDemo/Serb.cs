using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoSerbDemo
{
    public class Serb : IDisposable
    {
        private const double LEFTSERVSTOP = 90;
        private const double RIGHTSERVSTOP = 90;

        private readonly Servo _leftServo;
        private readonly Servo _rightServo;
        private readonly Timer _timer;

        private double _maxPower;

        private double _leftServoPower = LEFTSERVSTOP;
        private double _rightServoPower = RIGHTSERVSTOP;

        public Serb(Cpu.Pin leftServoPin, Cpu.Pin righServoPin)
        {
            _leftServo = new Servo(leftServoPin, false);
            _rightServo = new Servo(righServoPin, true);
            _leftServo.Degree = LEFTSERVSTOP;
            _rightServo.Degree = RIGHTSERVSTOP;

            _timer = new Timer(UpdateMotors, null, 0, 15);
        }

        public void Dispose()
        {
            _timer.Dispose();
            _leftServo.Dispose();
            _rightServo.Dispose();
        }

        private void UpdateMotors(object state)
        {
            // execute changes on timer ramp up the servos
            RampUpServo(_leftServo, _leftServoPower);
            RampUpServo(_rightServo, _rightServoPower);
            
            // pass directly to servo
            //_leftServo.Degree = _leftServoPower;
            //_rightServo.Degree = _rightServoPower;
        }

        private const double ServoStep = 2;

        private static void RampUpServo(Servo servo, double desiredSpeed)
        {
            var currentSpeed = servo.Degree;
            if (desiredSpeed < currentSpeed)
            {
                if (currentSpeed - desiredSpeed < ServoStep)
                    servo.Degree = desiredSpeed;
                else
                    servo.Degree = currentSpeed - ServoStep;
            }
            else if (desiredSpeed > currentSpeed)
            {
                if (desiredSpeed - currentSpeed < ServoStep)
                    servo.Degree = desiredSpeed;
                else
                    servo.Degree = currentSpeed + ServoStep;
            }
        }

        /// <summary>
        /// Sets the maximum power that will be set to the motors.
        /// The values passet to SetSpeed method will be scalled by this factor.
        /// </summary>
        /// <param name="newMaxPower"></param>
        public void SetMaxPower(int newMaxPower)
        {
            if (newMaxPower > 90) { newMaxPower = 90; } 
            if (newMaxPower < 0)  { newMaxPower = 0; }  
            _maxPower = newMaxPower;                    
        }

        /// <summary>
        /// Sends the robot forwards
        /// </summary>
        public void GoForward()
        {
            SetSpeedLeft(1f);
            SetSpeedRight(1f);
        }

        /// <summary>
        /// Sends the robot backwards.
        /// </summary>
        public void GoBackward()
        {
            SetSpeedLeft(-1f);
            SetSpeedRight(-1f);
        }

        /// <summary>
        /// Sends the robot right.
        /// </summary>
        public void GoRight()
        {
            SetSpeedLeft(1f);
            SetSpeedRight(-1f);
        }

        /// <summary>
        /// Sends the robot left. 
        /// </summary>
        public void GoLeft()
        {
            SetSpeedLeft(-1f);
            SetSpeedRight(1f);
        }

        /// <summary>
        /// stops the robot
        /// </summary>
        public void Stop()
        {
            SetSpeedLeft(0);
            SetSpeedRight(0);
        }

        public void SetSpeedLeft(double speed)
        {
            _leftServoPower = LEFTSERVSTOP + SpeedToPower(speed);
            //Debug.Print("Left Speed: " + _leftServoPower);
        }

        public void SetSpeedRight(double speed)
        {
            _rightServoPower = RIGHTSERVSTOP + SpeedToPower(speed);
            // Debug.Print("Right Speed: " + _rightServoPower);
        }

        private double SpeedToPower(double speed)
        {
            if (speed > 1.0) speed = 1f;
            else if (speed < -1.0) speed = -1f;
            return speed * (float)_maxPower;
        }

        private const double A = 0;
        private const double B = -32.614;
        private const double C = 2569.7;
        private const double D = -38624;
        private const double E = -254034;
        private const double F = -761766;
        private const double G = 854581;

        // Maps speed to power using polynomial equation 
        // that fits curve from speed test 
        // This doesn't work yet.
        private double SpeedToPowerPoly(double speed)
        {
            // if close to zero don't calc
            if (speed == 0.0) return 0.0;

            var spd = speed*0.33;

            // equation is valid only for positive values
            if (speed < 0) spd = -spd;

            var pwr = A + spd*(B + spd*(C + spd*(D + spd*(E + spd*(F + spd*G)))));
            if (pwr > _maxPower) pwr = _maxPower;
            if (pwr < 0.0) pwr = 0.0;

            // update sign
            if (speed < 0) pwr = -pwr;
            return pwr;
        }

    }
}