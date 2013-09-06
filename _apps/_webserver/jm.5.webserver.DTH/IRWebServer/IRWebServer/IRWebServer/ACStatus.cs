using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace IRWebServer
{
    class ACStatus
    {
        private InputPort ACPower, ACTimer;
        public ACStatus(Cpu.Pin Power, Cpu.Pin Timer)
        {
            ACPower = new InputPort(Power, false, Port.ResistorMode.PullUp);
            ACTimer = new InputPort(Timer, false, Port.ResistorMode.PullUp);
        }

        public bool isOn
        {
            get
            {
                return !ACPower.Read();
            }
        }

        public bool timerOn
        {
            get
            {
                return !ACTimer.Read();
            }
        }
    }
}
