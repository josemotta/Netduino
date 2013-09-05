using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace NetduinoPlus2RHT03
{
    // event handler delegate
    public delegate void IRCommandEventHandler(uint command);

    /// <summary>
    /// NEC Decoder from http://forums.netduino.com/index.php?/topic/1390-nec-protocol-ir-decoder/
    /// </summary>
    class NecProtocolDecoder
    {
        private static InterruptPort _input;
        public static event IRCommandEventHandler OnIRCommandReceived;

        private static long[] _pulses;
        private static int _currentIndex;
        private static Timer _timeout;
        
        public NecProtocolDecoder(Cpu.Pin irReceiverPin)
        {
            _timeout = new Timer(new TimerCallback(PulseTimedOut), null, Timeout.Infinite, Timeout.Infinite);

            _input = new InterruptPort(irReceiverPin, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            _input.OnInterrupt += new NativeEventHandler(OnInterrupt);

            _pulses = new long[200];
            _currentIndex = 0;
        }

        private void OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (_currentIndex >= 200)
            {
                _currentIndex = 0;
            }

            _pulses[_currentIndex++] = time.Ticks;

            _timeout.Change(10, Timeout.Infinite);
        }

        private void PulseTimedOut(object state)
        {
            const long toMicrosecondsDivisor = TimeSpan.TicksPerMillisecond / 1000;

            var firstValue = _pulses[0] / toMicrosecondsDivisor;
            var lastValue = firstValue;

            for (int i = 1; i < _currentIndex; i++)
            {
                var currentValue = _pulses[i] / toMicrosecondsDivisor;
                _pulses[i - 1] = currentValue - lastValue;
                lastValue = currentValue;
            }

            uint result = 0;

            int controlIndex = 0;
            int collectedLength = 0;
            bool isCollecting = false;

            for (int i = 0; i < _currentIndex; i++)
            {
                if (!isCollecting && (IsInRange(_pulses[i], 9000, 200) && IsInRange(_pulses[i + 1], 4500, 200)))
                {
                    controlIndex = i;
                    i = i + 2;
                    isCollecting = true;
                }
                else if (isCollecting && collectedLength < 32)
                {
                    if ((i - controlIndex) % 2 == 1)
                    {
                        result <<= 1;
                        collectedLength++;

                        if (IsInRange(_pulses[i], 1690, 200))
                        {
                            result |= 1;
                        }
                    }
                }
            }

            _currentIndex = 0;

            if (OnIRCommandReceived != null)
            {
                OnIRCommandReceived(result);
            }
        }

        private static bool IsInRange(long pulse, long expected, long tolerance)
        {
            return (expected + tolerance > pulse && expected - tolerance < pulse);
        }
    }

}
