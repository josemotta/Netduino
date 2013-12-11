using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace uPLibrary.Hardware
{
    /// <summary>
    /// Driver for a generic LDR (Light Dependent Resistor)
    /// </summary>
    public class Ldr
    {
        #region Constants ...

        // Default voltage reference for ADC
        public const double DEFAULT_AREF = 3.3;

        #endregion

        #region Fields ...

        // analog input for LDR
        private AnalogInput input;

        // ADC resolution and max value for analog channel
        private int adcResolution;
        private int maxValue;
        // Load resistor in the LDR circuit
        private int loadResistor;
        // Voltage reference for ADC
        private double aRef;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channel">Analog channel used for reading LDR value</param>
        /// <param name="adcResolution">ADC resolution for analog channel</param>
        /// <param name="loadResistor">Load resistor in the LDR circuit (Ohm)</param>
        /// <param name="aRef">Voltage reference for ADC</param>
        public Ldr(Cpu.AnalogChannel channel, int adcResolution, int loadResistor, double aRef = DEFAULT_AREF)
        {
            this.input = new AnalogInput(channel);
            this.adcResolution = adcResolution;
            this.maxValue = (int)System.Math.Pow(2, adcResolution) - 1;
            this.loadResistor = loadResistor;
            this.aRef = aRef;
        }

        /// <summary>
        /// Get LDR status
        /// </summary>
        /// <returns>LDR status</returns>
        public LdrStatus GetStatus()
        {
            LdrStatus status = new LdrStatus();
            status.Voltage = (this.input.ReadRaw() * this.aRef) / this.maxValue;
            status.Current = (status.Voltage / this.loadResistor);

            if (status.Voltage != 0)
                status.Resistance = this.loadResistor * (this.aRef - status.Voltage) / status.Voltage;
            else
                status.Resistance = Double.MaxValue;
            
            return status;
        }
    }

    /// <summary>
    /// LDR status
    /// </summary>
    public struct LdrStatus
    {
        /// <summary>
        /// Voltage (V)
        /// </summary>
        public double Voltage { get; set; }

        /// <summary>
        /// Resistor (Ohm)
        /// </summary>
        public double Resistance { get; set; }

        /// <summary>
        /// Current (A)
        /// </summary>
        public double Current { get; set; }
    }
}
