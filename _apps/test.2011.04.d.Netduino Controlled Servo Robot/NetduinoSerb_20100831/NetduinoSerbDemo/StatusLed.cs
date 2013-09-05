using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoSerbDemo
{
    public class StatusLed : IDisposable
    {
        private readonly OutputPort _port;
        private readonly ExtendedTimer _timer;

        public StatusLed(Cpu.Pin pin)
        {
            _port = new OutputPort(pin, false);
            _timer = new ExtendedTimer(OnTimeout, null, 0, 0);
        }

        public void TurnOn(int duration)
        {
            _port.Write(true);
            _timer.Change(duration, 0);
        }

        public void TurnOff()
        {
            _port.Write(false);
            _timer.Change(0, 0);
        }

        private void OnTimeout(object state)
        {
            _port.Write(false);
        }

        public void Dispose()
        {
            _port.Dispose();
            _timer.Dispose();
        }
    }
}