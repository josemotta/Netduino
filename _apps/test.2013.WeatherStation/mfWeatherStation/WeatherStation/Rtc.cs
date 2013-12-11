using System;
using Microsoft.SPOT;
using uPLibrary.Hardware;
using Microsoft.SPOT.Hardware;

namespace WeatherStation
{
    /// <summary>
    /// Class for Rtc
    /// </summary>
    internal class Rtc
    {
        // singleton Rtc instance
        private static Rtc instance;

#if !EMULATOR
        // reference to DS1307 driver
        private DS1307 ds1307;
#endif

        /// <summary>
        /// Singleton Rtc instance
        /// </summary>
        internal static Rtc Instance
        {
            get
            {
                if (instance == null)
                    instance = new Rtc();
                return instance;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private Rtc()
        {
#if !EMULATOR
            this.ds1307 = new DS1307();
#endif
        }

        /// <summary>
        /// Set date/time into the system
        /// </summary>
        /// <param name="dateTime">Date/time to set</param>
        internal void SetDateTime(DateTime dateTime)
        {
#if !EMULATOR
            this.ds1307.SetDateTime(dateTime);
#else
            Utility.SetLocalTime(dateTime);
#endif
        }

        /// <summary>
        /// Get date/time from the system
        /// </summary>
        /// <returns>Date/time from system</returns>
        internal DateTime GetDateTime()
        {
#if !EMULATOR
            return this.ds1307.GetDateTime();
#else
            return DateTime.Now;
#endif
        }
    }
}
