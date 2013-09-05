using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightOnNetduino
{
    public class TemperatureReading
    {
        /// <summary>
        /// Date & time the reading was recorded.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Temperature.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="when">When was the reding taken</param>
        /// <param name="temperature">Reading taken</param>
        public TemperatureReading(DateTime when, double temperature)
        {
            When = when;
            Temperature = temperature;
        }
    }
}
