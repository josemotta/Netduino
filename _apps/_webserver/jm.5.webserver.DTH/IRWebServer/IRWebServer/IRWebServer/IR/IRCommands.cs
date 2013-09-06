using System;
using Microsoft.SPOT;

namespace IRWebServer
{
    class IRCommands
    {
        private IRController _controller;
        private int[][] _commands;
        public IRCommands(IRController controller, int[][] commands)
        {
            _controller = controller;
            _commands = commands;
        }

        public void sendCommand(int command)
        {
            _controller.Send(_commands[command]);
        }

        public bool hasCommand(int command)
        {
            return (_commands.Length <= command || command < 0);
        }
    }
}
