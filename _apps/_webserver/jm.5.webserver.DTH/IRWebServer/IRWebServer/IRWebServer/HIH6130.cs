using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace IRWebServer
{
    /// <summary>
    /// Driver for the Honeywell H1H6130 humidity and temperature sensor
    /// Datasheets: http://sensing.honeywell.com/index.php?ci_id=3106&defId=44872
    /// </summary>
    class HIH6130
    {
        /// <summary>
        /// The last status code returned by an EEPROM read event or measurement (see the datasheet)
        /// </summary>
        public int status
        {
            get
            {
                return _status;
            }
        }

        /// <summary>
        /// The last response code returned by an EEPROM read event (1 is normal)
        /// </summary>
        public int EEPROM_Response
        {
            get
            {
                return _EEPROM_Response;
            }
        }

        /// <summary>
        /// The last diagnostic code returned by an EEPROM read event (0 is normal)
        public int EEPROM_daignostic
        {
            get
            {
                return _EEPROM_daignostic;
            }
        }

        /// <summary>
        /// The last measured temperature in degrees Celsius
        /// </summary>
        public double tempC
        {
            get 
            {
                return _temp;
            }
        }

        /// <summary>
        /// The last measured temperature in degrees Fahrenheit
        /// </summary>
        public double tempF
        {
            get
            {
                return _temp * 1.8 + 32;
            }
        }

        /// <summary>
        /// The last percent humidity measurement
        /// </summary>
        public double hum
        {
            get
            {
                return _hum;
            }
        }

        /// <summary>
        /// The heat index corrected temperature in degrees Celsius 
        /// </summary>
        public double heatIndexC
        {
            get
            {
                double temp = tempF;
                if (temp >= 80 && _hum >= 40)
                {
                    double tempSQ = System.Math.Pow(temp, 2);
                    double humSQ = System.Math.Pow(_hum, 2);
                    return ((-42.379 + (2.04901523 * temp) + (10.14333127 * _hum) + (-0.22475541 * _hum * temp) + (-0.00683783 * tempSQ) + (-.05481717 * humSQ) + (0.00122874 * _hum * tempSQ) + (0.00085282 * temp * humSQ) + (-0.00000199 * humSQ * tempSQ)) - 32) / 1.8;
                }
                else
                    return _temp;
            }
        }

        /// <summary>
        /// The heat index corrected temperature in degrees Fahrenheit 
        /// </summary>
        public double heatIndexF
        {
            get
            {
                double temp = tempF;
                if (temp >= 80 && _hum >= 40)
                {
                    double tempSQ = System.Math.Pow(temp, 2);
                    double humSQ = System.Math.Pow(_hum, 2);
                    return -42.379 + (2.04901523 * temp) + (10.14333127 * _hum) + (-0.22475541 * _hum * temp) + (-0.00683783 * tempSQ) + (-.05481717 * humSQ) + (0.00122874 * _hum * tempSQ) + (0.00085282 * temp * humSQ) + (-0.00000199 * humSQ * tempSQ);
                }
                else
                    return temp;
            }
        }

        /// <summary>
        /// Enumeration of EEPROM addresses
        /// </summary>
        public enum EEPROM
        {
            /// <summary>
            /// The temperature for the high alarm on
            /// </summary>
            Alarm_High_On = 0x18,
            /// <summary>
            /// The temperature for the high alarm off
            /// </summary>
            Alarm_High_Off = 0x19,
            /// <summary>
            /// The temperature for the low alarm on
            /// </summary>
            Alarm_Low_On = 0x1A,
            /// <summary>
            /// The temperature for the low alarm off
            /// </summary>
            Alarm_Low_Off = 0x1B,
            /// <summary>
            /// Current custom settings (see datasheet)
            /// </summary>
            Cust_Config = 0x1C,
            /// <summary>
            /// Custom storage space 1 (ID2)
            /// </summary>
            Cust_ID2 = 0x1E,
            /// <summary>
            /// Custom storage space 2 (ID3)
            /// </summary>
            Cust_ID3 = 0x1F 
        }

        /// <summary>
        /// Alarm active state
        /// </summary>
        public enum AlarmActive
        {
            High = 0x00,
            Low = 0x01
        }

        /// <summary>
        /// Alarm output configuration, use Open_Drain for multiple alarms on same line
        /// </summary>
        public enum AlarmOutputConfig
        {
            PushPull = 0x00,
            Open_Drain = 0x01
        }

        /// <summary>
        /// Enumeration of commands
        /// </summary>
        private enum Commands
        {
            MEASURE = 0x00,
            START_CM = 0xA0,
            START_NOM = 0x80
        }
        
        private int _status, _timeout, _EEPROM_Response, _EEPROM_daignostic;
        private double _temp, _hum;
        private I2CDevice _sensor;
        private byte _address;
        private OutputPort _power;
        private InterruptPort _AL_H, _AL_L;
        private DateTime lastEvent = DateTime.Now;
        private object locker = new object();

        /// <summary>
        /// Constructor for the driver
        /// </summary>
        /// <param name="address">The I2C address of the sensor</param>
        /// <param name="clock">The clock speed of the bus</param>
        /// <param name="timeout">The timeout for commands</param>
        /// <param name="AL_H">The optional alarm high pin</param>
        /// <param name="AL_L">The optional alarm low pin</param>
        public HIH6130(byte address, int clock = 300, int timeout = 1000, Cpu.Pin power = Pins.GPIO_NONE, Cpu.Pin AL_H = Pins.GPIO_NONE, Cpu.Pin AL_L = Pins.GPIO_NONE)
        {
            _address = address;
            _timeout = timeout;
            _sensor = new I2CDevice(new I2CDevice.Configuration(_address, clock));
            if (power != Pins.GPIO_NONE)
                _power = new OutputPort(power, true);
            if(AL_H != Pins.GPIO_NONE)
            {
                _AL_H = new InterruptPort(AL_H, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
                _AL_H.OnInterrupt += _AL_H_OnInterrupt;
            }
            if (AL_L != Pins.GPIO_NONE)
            {

                _AL_L = new InterruptPort(AL_L, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
                _AL_L.OnInterrupt += _AL_L_OnInterrupt;
            }
            Thread.Sleep(10);
        }

        /// <summary>
        /// The high alarm event
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Arguments sent to event</param>
        public delegate void highAlarmEventDelegate(object sender, AlarmArgs e);

        /// <summary>
        /// This is for creating interrupt events for high alarms, alarm will only trigger after taking a measurement
        /// </summary>
        public event highAlarmEventDelegate highAlarmEvent;

        /// <summary>
        /// The low alarm event
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Arguments sent to event</param>
        public delegate void lowAlarmEventDelegate(object sender, AlarmArgs e);
        /// <summary>
        /// This is for creating interrupt events for low alarms, alarm will only trigger after taking a measurement
        /// </summary>
        public event lowAlarmEventDelegate lowAlarmEvent;

        /// <summary>
        /// Arguments for an alarm event
        /// </summary>
        public class AlarmArgs : EventArgs
        {
            /// <summary>
            /// The current temperature
            /// </summary>
            public double TempC { get; internal set; }

            /// <summary>
            /// The current humidity
            /// </summary>
            public double hum { get; internal set; }

            /// <summary>
            /// The state of the alarm port
            /// </summary>
            public bool state { get; internal set; }
        }

        /// <summary>
        /// Used to trigger alarm high event
        /// </summary>
        /// <param name="pin">Not used</param>
        /// <param name="state">Not used</param>
        /// <param name="time">Not used</param>
        private void _AL_H_OnInterrupt(uint pin, uint state, DateTime time)
        {
            //This is for debouncing
            long differnce = (time - lastEvent).Ticks / TimeSpan.TicksPerMillisecond;
            if (differnce > 500)
            {
                lastEvent = time.AddMilliseconds(20);
                Thread.Sleep(20);
                AlarmArgs args = new AlarmArgs();
                args.TempC = _temp;
                args.hum = _hum;
                args.state = _AL_H.Read();
                highAlarmEvent(this, args);
            }
        }

        /// <summary>
        /// Used to trigger alarm low event
        /// </summary>
        /// <param name="pin">Not used</param>
        /// <param name="state">The state of the alarm pin</param>
        /// <param name="time">Not used</param>
        private void _AL_L_OnInterrupt(uint pin, uint state, DateTime time)
        {
            //This is for debouncing
            long differnce = (time - lastEvent).Ticks / TimeSpan.TicksPerMillisecond;
            if (differnce > 500)
            {
                lastEvent = time.AddMilliseconds(20);
                Thread.Sleep(20);
                AlarmArgs args = new AlarmArgs();
                args.TempC = _temp;
                args.hum = _hum;
                args.state = _AL_L.Read();
                lowAlarmEvent(this, args);
            }
        }

        /// <summary>
        /// Reads a segment of the EEPROM
        /// </summary>
        /// <param name="address">The EEPROM address</param>
        /// <param name="readBuffer">The buffer to read the data into (almost always 3 bytes in length)</param>
        /// <returns>The data read from the EEPROM</returns>
        public byte[] readEEPROM(EEPROM address, byte[] readBuffer)
        {
            write(new byte[] { (byte)address, 0x00, 0x00 });
            Thread.Sleep(5);
            read(readBuffer);
            _status = readBuffer[0] >> 6;
            _EEPROM_Response = readBuffer[0] & 0x3;
            _EEPROM_daignostic = (readBuffer[0] >> 2) & 0x15;
            return readBuffer;
        }
        
        /// <summary>
        /// Has the sensor take a measurement and store the results
        /// </summary>
        /// <returns>Total amount of data transferred</returns>
        public int takeMeasurement()
        {
            byte[] readBuffer = new byte[4];
            int result = write(new byte[] { (byte)Commands.MEASURE });
            Thread.Sleep(100);
            result += read(readBuffer);
            _status = readBuffer[0] >> 6;
            if (_status == 0)
            {
                _hum = (readBuffer[0] << 6 | readBuffer[1]) * 0.006104260774;
                _temp = (readBuffer[2] << 6 | readBuffer[3] >> 2) * 0.010072030277 - 40;
            }
            return result;
        }

        /// <summary>
        /// Configures the high humidity alarm
        /// </summary>
        /// <param name="humOn">The percent humidity to trip the alarm</param>
        /// <param name="humOff">The percent humidity to turn off the alarm</param>
        /// <param name="mode">Alarm active state</param>
        /// <param name="outputConfig">Alarm output configuration</param>
        /// <returns>Total amount of data transferred</returns>
        public int setAlarmHigh(int humOn, int humOff, AlarmActive mode = AlarmActive.High, AlarmOutputConfig outputConfig = AlarmOutputConfig.Open_Drain)
        {
            byte[] rawConfig= new byte[3];
            readEEPROM(EEPROM.Cust_Config, rawConfig);
            int config = rawConfig[1] << 8 | rawConfig[2];

            if (mode != AlarmActive.High)
                config |= 0x80;
            else
                config &= 0xFF7F;

            if (outputConfig == AlarmOutputConfig.Open_Drain)
                config |= 0x100;
            else
                config &= 0xFEFF;

            humOn = (int)(humOn * 168.83);
            humOff = (int)(humOff * 168.83);
            int result = writeEEPROM(EEPROM.Alarm_High_On, new byte[] { (byte)(humOn >> 8), (byte)(humOn &= 0xFF) });
            Thread.Sleep(20);
            result += writeEEPROM(EEPROM.Alarm_High_Off, new byte[] { (byte)(humOff >> 8), (byte)(humOff &= 0xFF) });
            Thread.Sleep(20);
            result += writeEEPROM(EEPROM.Cust_Config, new byte[] { (byte)(config >> 8), (byte)(config &= 0xFF) });
            Thread.Sleep(20);
            return result;
        }

        /// <summary>
        /// Configures the low humidity alarm
        /// </summary>
        /// <param name="humOn">The percent humidity to trip the alarm</param>
        /// <param name="humOff">The percent humidity to turn off the alarm</param>
        /// <param name="mode">Alarm active state</param>
        /// <param name="outputConfig">Alarm output configuration</param>
        /// <returns>Total amount of data transferred</returns>
        public int setAlarmLow(int humOn, int humOff, AlarmActive mode = AlarmActive.High, AlarmOutputConfig outputConfig = AlarmOutputConfig.Open_Drain)
        {
            byte[] rawConfig = new byte[3];
            readEEPROM(EEPROM.Cust_Config, rawConfig);
            int config = rawConfig[1] << 8 | rawConfig[2];

            if (mode != AlarmActive.High)
                config |= 0x200;
            else
                config &= 0xFDFF;

            if (outputConfig == AlarmOutputConfig.Open_Drain)
                config |= 0x400;
            else
                config &= 0xFBFF;

            humOn = (int)(humOn * 168.83);
            humOff = (int)(humOff * 168.83);
            int result = writeEEPROM(EEPROM.Alarm_Low_On, new byte[] { (byte)(humOn >> 8), (byte)(humOn &= 0xFF) });
            Thread.Sleep(20);
            result += writeEEPROM(EEPROM.Alarm_Low_Off, new byte[] { (byte)(humOff >> 8), (byte)(humOff &= 0xFF) });
            Thread.Sleep(20);
            result += writeEEPROM(EEPROM.Cust_Config, new byte[] { (byte)(config >> 8), (byte)(config &= 0xFF) });
            Thread.Sleep(20);
            return result;
        }

        /// <summary>
        /// Writes data to the custom EEPROM storage
        /// </summary>
        /// <param name="data">The data (two bytes) to write</param>
        /// <param name="ID">The ID of the ROM section (2 or 3)</param>
        /// <returns>Total amount of data transferred</returns>
        public int writeCustomROM(byte[] data, int ID)
        {
            EEPROM address;
            if (data.Length != 2)
                throw new System.ArgumentOutOfRangeException("data", "Data must contain two bytes");
            if (ID != 2 && ID != 3)
                throw new System.ArgumentOutOfRangeException("ID", "ID must be either 2 or 3");
            if (ID == 2)
                address = EEPROM.Cust_ID2;
            else
                address = EEPROM.Cust_ID3;

            int result = writeEEPROM(address, data);
            Thread.Sleep(20);
            return result;
        }

        /// <summary>
        /// Changes the I2C address of the sensor
        /// </summary>
        /// <param name="newAddress">The new address (0x00 to 0x7F)</param>
        /// <returns>Total amount of data transferred</returns>
        public int changeAddress(byte newAddress)
        {
            if (newAddress > 0x7F)
                throw new System.ArgumentOutOfRangeException("newAddress", "New address out of range");

            byte[] rawConfig = new byte[3];
            readEEPROM(EEPROM.Cust_Config, rawConfig);
            int config = rawConfig[1] << 8 | rawConfig[2];
            config |= newAddress;
            int result = writeEEPROM(EEPROM.Cust_Config, new byte[] { (byte)(config >> 8), (byte)(config &= 0xFF) });
            return result;
        }

        /// <summary>
        /// Change the the window to enter command mode during startup from 10 ms to 3 ms
        /// </summary>
        /// <param name="enable">Enable fast startup</param>
        /// <returns>Total amount of data transferred</returns>
        public int fastStartup(bool enable)
        {
            byte[] rawConfig = new byte[3];
            readEEPROM(EEPROM.Cust_Config, rawConfig);
            int config = rawConfig[1] << 8 | rawConfig[2];
            if (enable)
                config |= 0x2000;
            else
                config &= 0xDFFF;
            return writeEEPROM(EEPROM.Cust_Config, new byte[] { (byte)(config >> 8), (byte)(config &= 0xFF) });
        }

        /// <summary>
        /// Has the senor enter command mode (needs a power pin)
        /// </summary>
        public void enterCommandMode()
        {
            if (_power == null)
                throw new System.NullReferenceException("Power pin not defined");
            Debug.Print("Entering command mode");
            _power.Write(false);
            Thread.Sleep(10);
            _power.Write(true);
            byte[] command = new byte[] { (byte)Commands.START_CM, 0x00, 0x00 };
            write(command);
            Thread.Sleep(2);
        }

        /// <summary>
        /// Has the sensor exit commend mode and return to normal operation (needs a power pin)
        /// </summary>
        /// <param name="powerCycle">Power cycle the sensor, necessary for EEPROM changes to take effect</param>
        public void exitCommandMode(bool powerCycle = false)
        {
            if (_power == null)
                throw new System.NullReferenceException("Power pin not defined");
            Debug.Print("Exiting command mode");
            byte[] command = new byte[] { (byte)Commands.START_NOM, 0x00, 0x00 };
            write(command);
            Thread.Sleep(50);
            if (powerCycle)
            {
                _power.Write(false);
                Thread.Sleep(100);
                _power.Write(true);
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Writes data to the sensor
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <returns>Total amount of data transferred</returns>
        private int write(byte[] data)
        {
            lock (locker)
                return _sensor.Execute(new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(data) }, _timeout);
        }

        /// <summary>
        /// Writes data to the EEPROM
        /// </summary>
        /// <param name="address">The EEPROM address</param>
        /// <param name="data">The data to write</param>
        /// <returns>Total amount of data transferred</returns>
        private int writeEEPROM(EEPROM address, byte[] data)
        {
            byte[] wrtieBuffer = new byte[data.Length + 1];
            wrtieBuffer[0] = (byte)(0x40 | (int)address);
            Array.Copy(data, 0, wrtieBuffer, 1, data.Length);
            return write(wrtieBuffer);
        }

        /// <summary>
        /// Reads data from the sensor
        /// </summary>
        /// <param name="readBuffer">The buffer to read data into</param>
        /// <returns>Total amount of data transferred</returns>
        private int read(byte[] readBuffer)
        {
            lock (locker)
                return _sensor.Execute(new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(readBuffer) }, _timeout);
        }
    }
}