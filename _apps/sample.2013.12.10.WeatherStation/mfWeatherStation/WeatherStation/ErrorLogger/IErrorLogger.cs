using System;
using Microsoft.SPOT;

namespace WeatherStation.ErrorLogger
{
    /// <summary>
    /// Interface for error logging
    /// </summary>
    public interface IErrorLogger
    {
        /// <summary>
        /// Log error condition
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="msg">Optional error message</param>
        void Log(ErrorCode errorCode, string msg = null);
    }

    /// <summary>
    /// Errors code
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// No error
        /// </summary>
        Success,

        /// <summary>
        /// SD card not inserted
        /// </summary>
        SDCardNotInserted
    }
}
