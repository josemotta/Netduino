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
    public class PWM
    {
        Boolean _disposed = false;

        UInt32 _pin;

        public PWM(Cpu.Pin pin)
        {
            _pin = (UInt32)pin;
            PWM_Enable(_pin);
        }

        ~PWM()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            PWM_Disable(_pin);
            _disposed = true;
        }

        public void SetDutyCycle(UInt32 dutyCycle)
        {
            if (_disposed)
                throw new System.ObjectDisposedException();

            PWM_SetDutyCycle(_pin, dutyCycle);
        }

        public void SetPulse(UInt32 period, UInt32 duration)
        {
            if (_disposed)
                throw new System.ObjectDisposedException();

            PWM_SetPulse(_pin, period, duration);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PWM_Enable(UInt32 pin);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PWM_Disable(UInt32 pin);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PWM_SetDutyCycle(UInt32 pin, UInt32 dutyCycle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PWM_SetPulse(UInt32 pin, UInt32 period, UInt32 duration);
    }
}
