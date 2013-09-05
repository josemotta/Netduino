/* Copyright (C) 2010 Secret Labs LLC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System;
using System.Runtime.CompilerServices;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace SecretLabs.NETMF.Hardware
{
    public class AnalogInput
    {
        Boolean _disposed = false;

        UInt32 _pin;

        // ADC constants and values
        const Int32 TEN_BIT_ADC_MAX_VALUE = 1023;   // 10-bit ADC maximum value
        Int32 _minValue = 0;                        // default instance minimum value
        Int32 _maxValue = TEN_BIT_ADC_MAX_VALUE;    // default instance maximum value

        public AnalogInput(Cpu.Pin pin)
        {
            _pin = (UInt32)pin;
            ADC_Enable(_pin);
        }

        ~AnalogInput()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            ADC_Disable(_pin);
            _disposed = true;
        }

        public void SetRange(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public Int32 Read()
        {
            if (_disposed)
                throw new System.ObjectDisposedException();

            // retrieve our 10-bit ADC reading (based on a range of 0 to AREF)
            // convert our reading to the desired range and return it to the user.
            return (Int32)((((Int64)ADC_Read(_pin) * (_maxValue - _minValue)) / TEN_BIT_ADC_MAX_VALUE) + _minValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void ADC_Enable(UInt32 pin);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void ADC_Disable(UInt32 pin);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern UInt32 ADC_Read(UInt32 pin);
    }
}
