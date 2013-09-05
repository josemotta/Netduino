using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;
using Math = System.Math;

namespace NetduinoSerbDemo
{
    public class SerbRemoteClient
    {
        // Defining constants corresponding to each command (also the ascii code number) 
        private static class Command
        {
            public const char Unknown = '?';
            public const char Forward = 'F';
            public const char Backward = 'B';
            public const char Left = 'L';
            public const char Right = 'R';
            public const char SetPower = 'P';
            public const char Stop = 'S';
            public const char SetSpeedLeft = 'X';
            public const char SetSpeedRight = 'Y';
        }

        /* The three check bytes (used to keep the robot from responding to random serial data) 
         * currently "AAA"
         */
        private static readonly char[] HeaderBytes = new[] { 'A', 'A', 'A' };

        private readonly Serb _serb;
        private readonly StatusLed _errorLed;
        private readonly StatusLed _statusLed;
        private readonly SerialPort _serbSerial;

        // buffer can hold up to 4 messages
        private readonly byte[] _readBuffer = new byte[20];
        private int _headerCount;
        private SerbClientState _state;
        private char _command;
        private byte _param;

        public SerbRemoteClient()
        {
            _statusLed = new StatusLed(Pins.GPIO_PIN_D9);
            _errorLed = new StatusLed(Pins.GPIO_PIN_D8);

            // left servo connected to pin D5, right servo to pin D6
            _serb = new Serb(Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6);
            _serb.SetMaxPower(16);

            // setup output port
            _serbSerial = new SerialPort(SerialPorts.COM1, 115200);
            _serbSerial.DataReceived += SerbSerialDataReceived;
            _serbSerial.ErrorReceived += SerbSerialErrorReceived;

        }

        public void Run()
        {
            _serbSerial.Open();

            Thread.Sleep(Timeout.Infinite);
        }

        void SerbSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _errorLed.TurnOn(200);
            ResetState();
        }

        void SerbSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
                return;

            while (_serbSerial.BytesToRead > 0)
            {
                //Debug.Print("Read buffer size: " + _serbSerial.BytesToRead);

                var count = _serbSerial.Read(_readBuffer, 0,
                    Math.Min(_serbSerial.BytesToRead, _readBuffer.Length));

                for (int i = 0; i < count; i++)
                {
                    ProcessByte(_readBuffer[i]);
                }
            }

        }

        enum SerbClientState
        {
            WaitHeader,
            WaitCommand,
            WaitParam
        }

        private void ProcessByte(byte data)
        {
            switch (_state)
            {
                case SerbClientState.WaitHeader:
                    if (data == HeaderBytes[_headerCount])
                    {
                        _headerCount++;
                        if (_headerCount >= HeaderBytes.Length)
                        {
                            _state = SerbClientState.WaitCommand;
                        }
                    }
                    else
                    {
                        _errorLed.TurnOn(200);
                        ResetState();
                    }
                    break;
                case SerbClientState.WaitCommand:
                    _command = (char) data;
                    _state = SerbClientState.WaitParam;
                    break;
                case SerbClientState.WaitParam:
                    _param = data;
                    _statusLed.TurnOn(200);
                    InterpretCommand();
                    ResetState();
                    break;
                default:
                    _errorLed.TurnOn(200);
                    ResetState();
                    break;
            }
        }

        private void ResetState()
        {
            _headerCount = 0;
            _state = SerbClientState.WaitHeader;
            _command = Command.Unknown;
            _param = 0;
        }

        /*
        * Takes the command and parameter and passes it to the robot
        */
        private void InterpretCommand()
        {
            Debug.Print("Command: "+_command+" param: "+_param);

            switch (_command)
            {
                case Command.Forward:
                    _serb.GoForward();
                    Thread.Sleep(_param * 100);
                    _serb.Stop();
                    break;
                case Command.Backward:
                    _serb.GoBackward();
                    Thread.Sleep(_param * 100);
                    _serb.Stop();
                    break;
                case Command.Left:
                    _serb.GoLeft();
                    Thread.Sleep(_param * 100);
                    _serb.Stop();
                    break;
                case Command.Right:
                    _serb.GoRight();
                    Thread.Sleep(_param * 100);
                    _serb.Stop();
                    break;
                case Command.SetPower:
                    _serb.SetMaxPower(_param);
                    break;
                case Command.Stop:
                    _serb.Stop();
                    break;
                case Command.SetSpeedLeft:
                    _serb.SetSpeedLeft((_param - 127)/127f);
                    break;
                case Command.SetSpeedRight:
                    _serb.SetSpeedRight((_param - 127)/127f);
                    break;
                default:
                    Debug.Print("Command not recognized");
                    _errorLed.TurnOn(200);

/*
                    //if unrecognized command do a little shimmey
                    _serb.GoLeft();
                    Thread.Sleep(150);
                    _serb.GoRight();
                    Thread.Sleep(150);
                    _serb.Stop();
*/
                    break;
            }
        }

    }
}