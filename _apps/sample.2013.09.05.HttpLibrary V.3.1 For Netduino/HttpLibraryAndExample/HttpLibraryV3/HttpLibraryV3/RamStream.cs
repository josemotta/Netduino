using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace HttpLibrary
{
    /// <summary>
    /// Class for handling 23K640 Serial Ram
    /// </summary>
    public class RamStream
    {
        private SPI spi_driver;
        private OutputPort hold_port;

        /// <summary>
        /// Constructor intializes read and write at 10mhz
        /// </summary>
        /// <param name="ChipSelectPin">CS pin</param>
        /// <param name="HoldPin">HOLD pin</param>
        public RamStream(Cpu.Pin ChipSelectPin, Cpu.Pin HoldPin)
        {
            hold_port = new OutputPort(HoldPin, true);
            hold_port.Write(true);
            SPI.Configuration config = new SPI.Configuration(ChipSelectPin, false, 0, 0, false, true, 10000, SPI.SPI_module.SPI1);
            spi_driver = new SPI(config);
        }

        /// <summary>
        /// Constructor advanced configuration
        /// </summary>
        /// <param name="SpiConfiguration">SPI configuration</param>
        /// <param name="HoldPin">HOLD pin</param>
        public RamStream(SPI.Configuration SpiConfiguration, Cpu.Pin HoldPin)
        {
            spi_driver = new SPI(SpiConfiguration);
            hold_port = new OutputPort(HoldPin, true);
        }

        /// <summary>
        /// Writes a byte into a specific address
        /// </summary>
        /// <param name="Address">Ram address</param>
        /// <param name="Data">Data to store</param>
        public void Write(int Address, byte Data)
        {
            byte[] buffer = new byte[] { (byte)0x02, (byte)((Address >> 8) & 0x00ff), (byte)(Address & 0x00ff), Data };
            spi_driver.Write(buffer);
        }

        /// <summary>
        /// Writes an array of bytes begining from a specified address
        /// </summary>
        /// <param name="Address">Ram address</param>
        /// <param name="buffer">Data array to store</param>
        public void Write(int Address, byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i, ++Address)
                Write(Address, buffer[i]);
        }

        /// <summary>
        /// Writes an array of bytes begining from a specified address 
        /// </summary>
        /// <param name="Address">Ram address</param>
        /// <param name="buffer">Data array to store</param>
        /// <param name="offset">Buffer index</param>
        /// <param name="count">Number of bytes to store</param>
        public void Write(int Address, byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; ++i, ++Address)
                Write(Address, buffer[offset + i]);
        }

        /// <summary>
        /// Reads a byte from a specified address
        /// </summary>
        /// <param name="Address">Ram address</param>
        /// <returns>A single byte</returns>
        public byte Read(int Address)
        {
            byte[] buffer = new byte[] { (byte)0x03, (byte)((Address >> 8) & 0x00ff), (byte)(Address & 0x00ff), (byte)0x00 };
            byte[] output = new byte[4];
            spi_driver.WriteRead(buffer, output);
            return output[3];
        }

        /// <summary>
        /// Reads an array of bytes begining from a specified address
        /// </summary>
        /// <param name="Address">Ram address</param>
        /// <param name="buffer">Buffer to read</param>
        public void Read(int Address, byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i, ++Address)
                buffer[i] = Read(Address);
        }

        /// <summary>
        /// Reads an array of bytes begining from a specified address 
        /// </summary>
        /// <param name="Address">Ram address</param>
        /// <param name="buffer">Buffer to read</param>
        /// <param name="offset">Buffer index</param>
        /// <param name="count">Number of bytes to read</param>
        public void Read(int Address, byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; ++i, ++Address)
                buffer[offset + i] = Read(Address);
        }
    }
}

