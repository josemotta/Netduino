using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace WeatherStation.ErrorLogger
{
    /// <summary>
    /// Class for signaling error via led
    /// </summary>
    public class LedErrorLogger : IErrorLogger
    {
        // output port for led error logging
        private OutputPort ledPort;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ledPin">Led used for logging error</param>
        public LedErrorLogger(Cpu.Pin ledPin)
        {
            this.ledPort = new OutputPort(ledPin, false);
        }

        public void Log(ErrorCode errorCode, string msg = null)
        {
            switch (errorCode)
            {
                case ErrorCode.Success:
                    this.LogSuccess();
                    break;
                case ErrorCode.SDCardNotInserted:
                    this.LogSDCardNotInserted();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Log SD card not inserted error
        /// </summary>
        private void LogSDCardNotInserted()
        {
            this.ledPort.Write(true);
        }

        /// <summary>
        /// Log success condition
        /// </summary>
        private void LogSuccess()
        {
            this.ledPort.Write(false);
        }
    }

    
}
