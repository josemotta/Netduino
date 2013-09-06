using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HttpLibrary
{
    /// <summary>
    /// Software Guarddog class (Watchdog is already used in Microsoft.SPOT.Hardware.Watchdog)
    /// </summary>
    public class Guarddog : IDisposable
    {
        /// <summary>
        /// Used by program to check-in with the watchdog
        /// </summary>
        public bool running = false;

        private Timer wdtimer;

        /// <summary>
        /// Creates a guarddog
        /// </summary>
        /// <param name="delay">Number of milliseconds before the watchdog starts running</param>
        /// <param name="period">Number of milliseconds between each watchdog check-in</param>
        public Guarddog(int delay, int period = Timeout.Infinite)
        {
            wdtimer = new Timer(_watchdog, null, delay, period);
        }

        /// <summary>
        /// The internal watchdog process
        /// </summary>
        /// <param name="state">Required by the Timer class, but not used</param>
        private void _watchdog(object state)
        {
            if (!running)
            {
                Debug.Print("Rebooting...");
                PowerState.RebootDevice(false);
            }
            else
                running = false;
        }

        /// <summary>
        /// Disables and disposes of the watchdog
        /// </summary>
        public void Dispose()
        {
            running = true;
            wdtimer.Dispose();
        }

        ~Guarddog()
        {
            Dispose();
        }
    }
}
