using System;
using System.IO.Ports;

namespace SerbRemote
{
    public class SerbServer : IDisposable
    {
        public class Command
        {
            public const char Unknown = '?';
            public const char Forward = 'F';
            public const char Backward = 'B';
            public const char Left = 'L';
            public const char Right = 'R';
            public const char Stop = 'S';
            public const char SetPower = 'P';
            public const char SetSpeedLeft = 'X';
            public const char SetSpeedRight = 'Y';
        }

        private static readonly char[] HeaderBytes = new[] { 'A', 'A', 'A' };

        private readonly SerialPort _serial;
        private readonly object _syncRoot = new object();

        public SerbServer(string portName, int baudRate)
        {
            _serial = new SerialPort(portName, baudRate);
            _serial.Open();
        }

        public void Dispose()
        {
            _serial.Dispose();
        }

        public void SendCommand(char command, byte param)
        {
            lock (_syncRoot)
            {
                var buffer = new byte[5];
                for (int i = 0; i < 3; i++)
                {
                    // copy header bytes
                    buffer[i] = (byte) HeaderBytes[i];
                }
                buffer[3] = (byte) command;
                buffer[4] = param;
                _serial.Write(buffer, 0, buffer.Length);
            }
        }
    }

}
