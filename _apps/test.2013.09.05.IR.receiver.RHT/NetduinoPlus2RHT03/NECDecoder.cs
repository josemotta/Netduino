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
        public static InterruptPort RemoteInputPin;
        public static event IRCommandEventHandler OnIRCommandReceived;

        private static long[] _pulses = new long[200];
        private static int _currentIndex = 0;
        private static Timer _timeout = new Timer(new TimerCallback(PulseTimedOut), null, Timeout.Infinite, Timeout.Infinite);

        public NecProtocolDecoder()
        {
            //_timeout = new timer(new timercallback(pulsetimedout), null, timeout.infinite, timeout.infinite);

            //remoteinputpin = new interruptport(irreceiverpin, false, port.resistormode.disabled, port.interruptmode.interruptedgeboth);
            //remoteinputpin.oninterrupt += new nativeeventhandler(oninterrupt);

            _currentIndex = 0;
        }

        /// <summary>
        /// This is the pulse recorder. It should be called from the interrupt handler routine of your main program
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="time"></param>
        public static void OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (_currentIndex >= 200)
            {
                _currentIndex = 0;
            }

            _pulses[_currentIndex++] = time.Ticks;

            _timeout.Change(150, Timeout.Infinite);
        }

        //static void PulseTimedOut(object state)
        //{
        //    const long toMicrosecondsDivisor = TimeSpan.TicksPerMillisecond / 1000;

        //    _timeout.Change(Timeout.Infinite, Timeout.Infinite);

        //    var firstValue = _pulses[0] / toMicrosecondsDivisor;
        //    var lastValue = firstValue;

        //    for (int i = 1; i < _currentIndex; i++)
        //    {
        //        var currentValue = _pulses[i] / toMicrosecondsDivisor;
        //        _pulses[i - 1] = currentValue - lastValue;
        //        lastValue = currentValue;
        //    }

        //    uint result = 0;

        //    int controlIndex = 0;
        //    int collectedLength = 0;
        //    bool isCollecting = false;

        //    for (int i = 0; i < _currentIndex; i++)
        //    {
        //        if (!isCollecting && (IsInRange(_pulses[i], 9000, 200) && IsInRange(_pulses[i + 1], 4500, 200)))
        //        {
        //            controlIndex = i;
        //            i = i + 2;
        //            isCollecting = true;
        //        }
        //        else if (isCollecting && collectedLength < 32)
        //        {
        //            if ((i - controlIndex) % 2 == 1)
        //            {
        //                result <<= 1;
        //                collectedLength++;

        //                if (IsInRange(_pulses[i], 1690, 200))
        //                {
        //                    result |= 1;
        //                }
        //            }
        //        }
        //    }
        //    uint x = (uint)_pulses[_currentIndex];

        //    _currentIndex = 0;

        //    if (OnIRCommandReceived != null)
        //    {
        //        //OnIRCommandReceived(result);
        //        OnIRCommandReceived(x);
        //    }
        //}

        static void PulseTimedOut(object state)
        {
            var numberOfPulses = _currentIndex;
            _currentIndex = 0;
            _timeout.Change(Timeout.Infinite, Timeout.Infinite);

            ConvertPulsesToMicroseconds(numberOfPulses);

            //if (numberOfPulses != 68) return;
            //if (!IsPulseMatch(_pulses[0], 9000) || !IsPulseMatch(_pulses[1], 4500))
            if (!IsPulseMatch(_pulses[0], 9000) || !IsPulseMatch(_pulses[1], 4500))
            {
                Debug.Print("Did not find the correct burst & space.");
                return;
            }

            UInt32 data = 0;
            for (int i = 65; i >= 3; i--)
            {
                if ((i % 2) == 1)
                {
                    data = data << 1;
                    if (IsPulseMatch(_pulses[i], 565))
                        data = data | 1;
                }
                else
                {
                    if (!IsPulseMatch(_pulses[i], 560))
                        return;
                }
            }

            if (OnIRCommandReceived != null)
            {
                OnIRCommandReceived(data);
            }
        }

        private static bool IsInRange(long pulse, long expected, long tolerance)
        {
            return (expected + tolerance > pulse && expected - tolerance < pulse);
        }

        private static bool IsPulseMatch(long actualValue, long expectedValue)
        {
            var marginOfError = 200;
            if ((expectedValue - marginOfError) < actualValue && (expectedValue + marginOfError) > actualValue)
                return true;
            return false;
        }

        private static void ConvertPulsesToMicroseconds(long numberOfPulses)
        {
            var firstMicrosecondsValue = _pulses[0] / (TimeSpan.TicksPerMillisecond / 1000);
            var lastMicrosecondsValue = firstMicrosecondsValue;
            for (int i = 1; i < numberOfPulses; i++)
            {
                var currentPulseMicrosecondsValue = _pulses[i] / (TimeSpan.TicksPerMillisecond / 1000);
                var currentPulseLength = currentPulseMicrosecondsValue - lastMicrosecondsValue;
                lastMicrosecondsValue = currentPulseMicrosecondsValue;
                _pulses[i - 1] = currentPulseLength;
            }
        }

    }

}
