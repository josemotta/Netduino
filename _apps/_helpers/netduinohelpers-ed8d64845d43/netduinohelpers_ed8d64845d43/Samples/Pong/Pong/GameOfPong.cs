using System;
using netduino.helpers.Fun;
using netduino.helpers.Imaging;
using System.Threading;

namespace Pong {
    /*
    Copyright (C) 2011 by Fabien Royer & Bertrand Le Roy

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
    */
    public class GameOfPong : Game {
        private const int ScreenSize = 8;
        private const int StickActiveZoneSize = 300;
        private const int StickRange = 1024;
        private const uint BeepFrequency = 10000;
        private const uint BoopFrequency = 3000;
        private const int PaddleAmplitude = ScreenSize - Paddle.Size;
        private const int StickActiveAmplitude = StickActiveZoneSize - Paddle.Size*StickActiveZoneSize/ScreenSize;
        private const int StickMin = (StickRange - StickActiveAmplitude)/2;
        private const int StickMax = (StickRange + StickActiveAmplitude)/2;
        private const int MaxScore = 1;

        bool _ballGoingDown;

        public int LeftScore;
        public int RightScore;

        public bool BallGoingRight { get; set; }

        public PlayerMissile Ball { get; private set; }
        public Paddle LeftPaddle { get; private set; }
        public Paddle RightPaddle { get; private set; }

        public GameOfPong(ConsoleHardwareConfig config) : base(config) {
            World = new Composition(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, ScreenSize, ScreenSize);
            Ball = new PlayerMissile("ball", 0, 0, World);
            LeftPaddle = new Paddle(Side.Left, this);
            RightPaddle = new Paddle(Side.Right, this);
            BallGoingRight = true;
            ResetBall();
        }

        public override void Loop() {
            var effectiveLeftPaddleY = Hardware.JoystickLeft.Y < StickMin
                                           ? StickMin
                                           : Hardware.JoystickLeft.Y > StickMax
                                                 ? StickMax
                                                 : Hardware.JoystickLeft.Y;
            LeftPaddle.Y = (effectiveLeftPaddleY - StickMin) * PaddleAmplitude / StickActiveAmplitude;
            var effectiveRightPaddleY = Hardware.JoystickRight.Y < StickMin
                                           ? StickMin
                                           : Hardware.JoystickRight.Y > StickMax
                                                 ? StickMax
                                                 : Hardware.JoystickRight.Y;
            RightPaddle.Y = (effectiveRightPaddleY - StickMin) * PaddleAmplitude / StickActiveAmplitude;

            Ball.X += BallGoingRight ? 1 : -1;
            if (Ball.X < 0) {
                RightScore++;
                DisplayScores(LeftScore, RightScore);
                ResetBall();
            }
            if (Ball.X >= 8) {
                LeftScore++;
                DisplayScores(LeftScore, RightScore);
                ResetBall();
            }
            Ball.Y += _ballGoingDown ? 1 : -1;
            if (Ball.Y < 0) {
                Ball.Y = 1;
                _ballGoingDown = true;
                Beep(BeepFrequency);
            }
            if (Ball.Y >= 8) {
                Ball.Y = 7;
                _ballGoingDown = false;
                Beep(BeepFrequency);
            }
         }

        private void DisplayScores(int leftScore, int rightScore) {
            Hardware.Matrix.Display(SmallChars.ToBitmap(leftScore, rightScore));
            if (leftScore >= MaxScore || rightScore >= MaxScore) {
                WaitForClick();
                LeftScore = 0;
                RightScore = 0;
            }
            else {
                Thread.Sleep(2000);
            }
        }

        private void WaitForClick() {
            while(!(Hardware.LeftButton.IsPressed || Hardware.RightButton.IsPressed)) {
                Thread.Sleep(100);
            }
        }

        public void ResetBall() {
            Ball.X = 0;
            Ball.Y = Random.Next(8);
            BallGoingRight = true;
            _ballGoingDown = Random.Next(2) == 0;
            Beep(BoopFrequency);
        }

        public void Beep(uint frequency) {
            var period = 1000000 / frequency; 
            Hardware.Speaker.SetPulse(period, period / 2);
            Thread.Sleep(50);
            Hardware.Speaker.SetPulse(0, 0);
        }
    }
}
