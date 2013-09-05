using System;
using Microsoft.SPOT;

namespace SimpleWebServer.Classes
{
    /// <summary>
    /// Command instruction from the client.
    /// </summary>
    class Command
    {
        #region Properties

        /// <summary>
        /// Name of the command.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter to pass to the commd.
        /// </summary>
        public string Parameter { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a command object.
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="parameter">Parameters for the command</param>
        public Command(string name, string parameter)
        {
            Name = name;
            Parameter = parameter;
        }

        #endregion
    }
}
