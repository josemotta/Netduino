using System;

namespace WeatherStation
{
    /// <summary>
    /// Interface for data logging
    /// </summary>
    public interface IDataLogger
    {
        /// <summary>
        /// Open data logger
        /// </summary>
        void Open();

        /// <summary>
        /// Close data logger
        /// </summary>
        void Close();

        /// <summary>
        /// Log data
        /// </summary>
        /// <param name="text">Text for data logging</param>
        void Log(string text);

        /// <summary>
        /// Data logger is opened or not
        /// </summary>
        bool IsOpen { get; }
    }
}
