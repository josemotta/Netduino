using System;
using Microsoft.SPOT;

namespace WeatherStation
{
    /// <summary>
    /// Class for converting LDR data (resistance) into ambient light
    /// </summary>
    internal static class AmbientLightConverter
    {
        #region Constants...

        // value resistance for evaluating ambient light
        private const int LOW_RESISTANCE_LEVEL = 2000;
        private const int MEDIUM_RESISTANCE_LEVEL = 100000;
        private const int HIGH_RESISTANCE_LEVEL = 500000;

        #endregion

        /// <summary>
        /// Convert resistance value in ambient light
        /// </summary>
        /// <param name="resistance">Resistance value</param>
        /// <returns>Ambient light</returns>
        internal static AmbientLight FromResistance(double resistance)
        {
            if (resistance < LOW_RESISTANCE_LEVEL)
                return AmbientLight.FullDaylight;
            else if (resistance < MEDIUM_RESISTANCE_LEVEL)
                return AmbientLight.OvercastDay;
            else if (resistance < HIGH_RESISTANCE_LEVEL)
                return AmbientLight.DarkOvercastDay;
            else
                return AmbientLight.MoonlitNight;
        }
    }

    /// <summary>
    /// Ambient light
    /// </summary>
    internal enum AmbientLight
    {
        MoonlitNight,
        DarkOvercastDay,
        OvercastDay,
        FullDaylight
    }
}
