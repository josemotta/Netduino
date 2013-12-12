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
    /// InputPort class for Bitshift-IC's
    /// </summary>
    public class InputPortShift
    {
        /// <summary>
        /// Reference to the 74HC165 IC
        /// </summary>
        private Ic74HC165 _Ic74HC165;

        /// <summary>
        /// The used pin on the IC
        /// </summary>
        private uint _Pin;

        /// <summary>
        /// The Pin Id
        /// </summary>
        public int Id { get { return (int)this._Pin; } }

        /// <summary>
        /// Gets a value that indicates whether the glitch filter is currently enabled. 
        /// </summary>
        public bool GlitchFilter
        {
            get { return false; }
            set
            {
                // We don't have a glitch filter implemented
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the glitch filter is currently enabled. 
        /// </summary>
        public Port.InterruptMode Interrupt
        {
            get { throw new System.InvalidOperationException(); }
            set { throw new System.InvalidOperationException(); }
        }

        /// <summary>
        /// Returns the current resistor mode
        /// </summary>
        public Port.ResistorMode Resistor
        {
            get { return Port.ResistorMode.Disabled; }
            set
            {
                // You need to place your own resistors in circuit (when required)
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Adds or removes callback methods for interrupt changes
        /// </summary>
        public event NativeEventHandler OnInterrupt;

        /// <summary>
        /// Creates a new interrupt port. Requires Ic74HC165.EventsEnabled on true!
        /// </summary>
        /// <param name="Ic">A reference to the 74HC165 IC</param>
        /// <param name="Pin">The pin to which the interrupt port is connected</param>
        /// <param name="GlitchFilter">Not supported, so only False is an option (in here for compatibility reasons)</param>
        /// <param name="Resistor">Not supported, so only Disabled is an option (in here for compatibility reasons)</param>
        public InputPortShift(Ic74HC165 Ic, Ic74HC165.Pins Pin, bool GlitchFilter, Port.ResistorMode Resistor)
        {
            if (GlitchFilter)
            {
                // We don't have a glitch filter implemented
                throw new NotImplementedException();
            }
            if (Resistor != Port.ResistorMode.Disabled)
            {
                // You need to place your own resistors in circuit (when required)
                throw new NotImplementedException();
            }

            // Copies all references
            this._Ic74HC165 = Ic;
            this._Pin = (uint)Pin;
        }

        /// <summary>
        /// Releases used resources
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Returns the current pin state
        /// </summary>
        /// <returns>The pin state</returns>
        public bool Read()
        {
            return this._Ic74HC165.GetPinState((Ic74HC165.Pins)this._Pin);
        }

        /// <summary>Interrups won't work on InputPorts, therefore I throw an NotImplementedException, just like the built-in InputPorts does.</summary>
        public void ClearInterrupt() { throw new NotImplementedException(); }
        /// <summary>Interrups won't work on InputPorts, therefore I throw an NotImplementedException, just like the built-in InputPorts does.</summary>
        public void DisableInterrupt() { throw new NotImplementedException(); }
        /// <summary>Interrups won't work on InputPorts, therefore I throw an NotImplementedException, just like the built-in InputPorts does.</summary>
        public void EnableInterrupt() { throw new NotImplementedException(); }
    }
}
