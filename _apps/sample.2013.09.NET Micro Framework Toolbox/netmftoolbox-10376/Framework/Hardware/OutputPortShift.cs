using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

/*
 * Copyright 2011 Stefan Thoolen (http://netmftoolbox.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Toolbox.NETMF.Hardware
{
    /// <summary>
    /// OutputPort class for Bitshift-IC's
    /// </summary>
    public class OutputPortShift
    {
        /// <summary>
        /// Contains the pin used, read only
        /// </summary>
        public uint Id
        {
            get { return this._Pin; }
        }

        /// <summary>
        /// Contains the initial state, read only
        /// </summary>
        public bool InitialState
        {
            get { return this._InitialState; }
        }

        /// <summary>
        /// A reference to the IC class
        /// </summary>
        private Ic74HC595 _IcOut74HC595;

        /// <summary>The pin used on the 74HC595 IC</summary>
        private uint _Pin;
        /// <summary>The initial state</summary>
        private bool _InitialState;

        /// <summary>
        /// OutputPort on a 74HC595 Bitshifter IC
        /// </summary>
        /// <param name="IcOut">Reference to the 74HC595 IC</param>
        /// <param name="Pin">The output pin on the IC</param>
        /// <param name="InitialState">The initial state</param>
        public OutputPortShift(Ic74HC595 IcOut, Ic74HC595.Pins Pin, bool InitialState)
        {
            // Stores the parameters in class globals
            this._InitialState = InitialState;
            this._Pin = (uint)Pin;
            this._IcOut74HC595 = IcOut;
            // Actually sets the pin to the right state
            this._IcOut74HC595.SetPinState((Ic74HC595.Pins)this._Pin, InitialState);
        }

        /// <summary>
        /// Changes the state of the current pin
        /// </summary>
        /// <param name="State">The new state</param>
        public void Write(bool State)
        {
            this._IcOut74HC595.SetPinState((Ic74HC595.Pins)this._Pin, State);
        }

        /// <summary>
        /// Reads the state of the current pin
        /// </summary>
        /// <returns>The current state</returns>
        public bool Read()
        {
            return this._IcOut74HC595.GetPinState((Ic74HC595.Pins)this._Pin);
        }

        /// <summary>
        /// Interrups won't work on OutputPorts, therefore I throw an InvalidOperationException, just like the built-in OutputPort does.
        /// </summary>
        public event NativeEventHandler OnInterrupt
        {
            add { throw new InvalidOperationException(); }
            remove { }

        }
        /// <summary>Interrups won't work on OutputPorts, therefore I throw an InvalidOperationException, just like the built-in OutputPort does.</summary>
        public void DisableInterrupt() { throw new InvalidOperationException(); }
        /// <summary>Interrups won't work on OutputPorts, therefore I throw an InvalidOperationException, just like the built-in OutputPort does.</summary>
        public void EnableInterrupt() { throw new InvalidOperationException(); }

        /// <summary>
        /// This class is actually just a wrapper for Ic74HC595, there is nothing to dispose
        /// </summary>
        public void Dispose() { }
    }
}
