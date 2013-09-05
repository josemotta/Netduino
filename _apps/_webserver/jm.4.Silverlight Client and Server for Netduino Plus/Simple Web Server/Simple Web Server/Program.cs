using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace SimpleWebServer
{
    public class Program
    {
        #region Constants

        /// <summary>
        /// Root directory for the web files.
        /// </summary>
        private const string WEB_ROOT = @"\SD\www";

        #endregion

        #region Private variables.

        /// <summary>
        /// Web server instance.
        /// </summary>
        private static WebServer _webServer;

        /// <summary>
        /// On board LED - used to let the user know the program is still alive.
        /// </summary>
        private static OutputPort _onBoardLED;

        /// <summary>
        /// On board button - used by user to request confirmation the board is still alive.
        /// </summary>
        private static InterruptPort _onBoardButton;

        #endregion

        /// <summary>
        /// Main program loop.
        /// </summary>
        public static void Main()
        {
            _onBoardLED = new OutputPort(Pins.ONBOARD_LED, false);
            _onBoardButton = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            _onBoardButton.OnInterrupt += new NativeEventHandler(onBoardButton_OnInterrupt);

            _webServer = new WebServer(WEB_ROOT, WebServer.HTTP_PORT);

            Thread.Sleep(Timeout.Infinite);
        }

        #region Events

        /// <summary>
        /// Flash the on board LED to let the user know we are still alive.
        /// </summary>
        private static void onBoardButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (data2 == 0)
            {
                _onBoardLED.Write(true);
                Thread.Sleep(250);
                _onBoardLED.Write(false);
                Thread.Sleep(250);
                _onBoardLED.Write(true);
                Thread.Sleep(250);
                _onBoardLED.Write(false);
            }
        }

        #endregion
    }
}
