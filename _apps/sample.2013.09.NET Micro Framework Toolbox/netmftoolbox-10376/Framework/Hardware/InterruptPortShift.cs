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
    /// InterruptPort class for Bitshift-IC's
    /// </summary>
    public class InterruptPortShift
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
        /// True when interrupts are enabled
        /// </summary>
        private bool _InterruptsEnabled;

        /// <summary>
        /// Interrupt mode
        /// </summary>
        Port.InterruptMode _Interrupt;

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
            set {
                // We don't have a glitch filter implemented
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the glitch filter is currently enabled. 
        /// </summary>
        public Port.InterruptMode Interrupt
        {
            get { return this._Interrupt; }
            set {
                if (value == Port.InterruptMode.InterruptEdgeLevelHigh || value == Port.InterruptMode.InterruptEdgeLevelLow)
                {
                    // No level stuff supported
                    throw new NotImplementedException();
                }
                this._Interrupt = value;
            }
        }

        /// <summary>
        /// Returns the current resistor mode
        /// </summary>
        public Port.ResistorMode Resistor
        {
            get { return Port.ResistorMode.Disabled; }
            set {
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
        /// <param name="Interrupt">Defines when interrupts should be triggered (EdgeLevels not supported)</param>
        public InterruptPortShift(Ic74HC165 Ic, Ic74HC165.Pins Pin, bool GlitchFilter, Port.ResistorMode Resistor, Port.InterruptMode Interrupt)
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
            if (Interrupt == Port.InterruptMode.InterruptEdgeLevelHigh || Interrupt == Port.InterruptMode.InterruptEdgeLevelLow)
            {
                // No level stuff supported
                throw new NotImplementedException();
            }

            // Copies all references
            this._Ic74HC165 = Ic;
            this._Pin = (uint)Pin;
            this._Interrupt = Interrupt;

            // Enables interrupts
            Ic.onPinChange += new NativeEventHandler(Ic_onPinChange);
        }

        /// <summary>
        /// Triggered when a pin's value is changed
        /// </summary>
        /// <param name="PinId">The number of the pin</param>
        /// <param name="Value">The new value</param>
        /// <param name="Time">Time and date when the event is triggered</param>
        private void Ic_onPinChange(uint PinId, uint Value, DateTime Time)
        {
            // No events when it's the wrong pin, no interrupt is defined or interrupts are defined
            if (PinId != this._Pin || this.OnInterrupt == null || this._InterruptsEnabled == false) return;

            // Set to High, lets trigger the event
            if (Value == 1 && (
                this._Interrupt == Port.InterruptMode.InterruptEdgeHigh
                ||
                this._Interrupt == Port.InterruptMode.InterruptEdgeBoth
            ))
            {
                this.OnInterrupt(PinId, Value, Time);
            }
            // Set to Low, lets trigger the event
            if (Value == 0 && (
                this._Interrupt == Port.InterruptMode.InterruptEdgeLow
                ||
                this._Interrupt == Port.InterruptMode.InterruptEdgeBoth
            ))
            {
                this.OnInterrupt(PinId, Value, Time);
            }
        }

        /// <summary>
        /// Clears the current interrupt on the interrupt port.
        /// </summary>
        public void ClearInterrupt()
        {
            // Since EdgeLevel aren't supported, this can not happen.
            throw new System.InvalidOperationException();
        }

        /// <summary>
        /// Disables the interrupt on this InterruptPort
        /// </summary>
        public void DisableInterrupt()
        {
            this._InterruptsEnabled = false;
        }

        /// <summary>
        /// Enables the interrupt on this InterruptPort
        /// </summary>
        public void EnableInterrupt()
        {
            // Makes sure the IC also has events enabled
            this._Ic74HC165.EventsEnabled = true;
            // Enables interrupts in this class
            this._InterruptsEnabled = true;
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
    }
}
