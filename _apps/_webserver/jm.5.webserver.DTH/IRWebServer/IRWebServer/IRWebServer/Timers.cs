using System;
using System.IO;
using System.Threading;
using Microsoft.SPOT;

namespace IRWebServer
{
    class Timers
    {
        private ACStatus _AC;
        private HIH6130 _temp;
        private IRCommands _commander;
        private double _highTemp, _lowTemp;

        public Timers(ACStatus AC, HIH6130 temp, IRCommands commander)
        {
            _AC = AC;
            _temp = temp;
            _commander = commander;
            if (File.Exists(@"\SD\settings.txt"))
            {
                Debug.Print("Loading settings");
                using (StreamReader sr = new StreamReader(@"\SD\settings.txt"))
                {
                    string[] lines = sr.ReadToEnd().Split('\n');
                    int i = 0;
                    foreach (string line in lines)
                    {
                        lines[i] = line.Trim();
                        i++;
                    }

                    _highTemp = Double.Parse(lines[0]);
                    _lowTemp = Double.Parse(lines[1]);
                }
            }
            else
            {
                Debug.Print("No file");
                _highTemp = 0;                
                _lowTemp = 0;
            }
        }

        //onTimer variables
        public bool onAbort;
        public int onMinutes;

        public void onTimer()
        {
            onAbort = false;
            int counter = 0;
            int seconds = onMinutes * 60;
            Debug.Print("onTimer started with a time of " + onMinutes.ToString());
            while (!onAbort)
            {
                if (counter >= seconds)
                {
                    while (!_AC.isOn)
                    {
                        _commander.sendCommand(0);
                        Thread.Sleep(250);
                    }
                    while (!_AC.timerOn)
                    {
                        _commander.sendCommand(1);
                        Thread.Sleep(250);
                    }
                    Debug.Print("AC has been turned on");
                    break;
                }
                counter++;
                //Wait one minute.
                Thread.Sleep(60000);
            }
            Debug.Print("Exiting onTimer");
        }

        //offTimer variables
        public bool offAbort;
        public int offMinutes;

        public void offTimer()
        {
            offAbort = false;
            int counter = 0;
            int seconds = offMinutes * 60;
            Debug.Print("offTimer started with a time of " + offMinutes.ToString());
            while (!onAbort)
            {
                if (counter >= seconds)
                {
                    while (_AC.isOn)
                    {
                        _commander.sendCommand(0);
                        Thread.Sleep(250);
                    }
                    Debug.Print("AC has been turned off");
                    break;
                }
                counter++;
                //Wait one minute.
                Thread.Sleep(60000);
            }
            Debug.Print("Exiting offTimer");
        }

        public double highTemp
        {
            get
            {
                return _highTemp;
            }
            set
            {
                _highTemp = value;
                using (StreamWriter sw = new StreamWriter(@"\SD\settings.txt", false))
                {
                    sw.WriteLine(value.ToString());
                    sw.WriteLine(_lowTemp.ToString());
                }
            }
        }
        public double lowTemp
        {
            get
            {
                return _lowTemp;
            }
            set
            {
                _lowTemp = value;
                using (StreamWriter sw = new StreamWriter(@"\SD\settings.txt", false))
                {
                    sw.WriteLine(_highTemp.ToString());
                    sw.WriteLine(value.ToString());
                }
            }
        }
        public void tempControl()
        {
            while (true)
            {
                if (_highTemp != 0 && _lowTemp != 0)
                {
                    _temp.takeMeasurement();
                    if (_temp.heatIndexF > _highTemp)
                    {
                        while (!_AC.isOn)
                        {
                            _commander.sendCommand(0);
                            Thread.Sleep(250);
                        }
                        while (!_AC.timerOn)
                        {
                            _commander.sendCommand(1);
                            Thread.Sleep(250);
                        }
                        Debug.Print("AC has been turned on");
                        Thread.Sleep(600000);
                    }
                    else if (_temp.heatIndexF < _lowTemp)
                    {
                        while (_AC.isOn)
                        {
                            _commander.sendCommand(0);
                            Thread.Sleep(250);
                        }
                        Debug.Print("AC has been turned off");
                        Thread.Sleep(900000);
                    }
                }
                Thread.Sleep(60000);
            }
        }

    }
}
