using System;
using System.Net;
using System.Net.Sockets;

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
namespace Toolbox.NETMF.NET
{
    /// <summary>
    /// Simplifies usage of TCP sockets in .NETMF
    /// </summary>
    public class SimpleSocket
    {
        /// <summary>Contains a reference to the socket</summary>
        private Socket _Sock;
        /// <summary>Stores the hostname connected to</summary>
        private string _Hostname;
        /// <summary>Stores the TCP port connected to</summary>
        private ushort _Port;
        /// <summary>Contains the buffer of the read data</summary>
        private string _Buffer = "";
        /// <summary>When LineEnding contains data, <see cref="Receive"/> will only return data when <see cref="LineEnding"/> is reached</summary>
        public string LineEnding { get; set; }

        /// <summary>
        /// Supported protocols
        /// </summary>
        public enum SocketProtocol
        {
            /// <summary>The socket will work as a TCP Stream</summary>
            TcpStream = 1,
            /// <summary>The socket will work as a UDP Datagram</summary>
            UdpDatagram = 2
        }

        /// <summary>
        /// Creates a new socket
        /// </summary>
        /// <param name="Hostname">The hostname to connect to</param>
        /// <param name="Port">The port to connect to</param>
        /// <param name="Protocol">The protocol to use (default: TCP Stream)</param>
        public SimpleSocket(string Hostname, ushort Port, SocketProtocol Protocol = SocketProtocol.TcpStream)
        {
            // Stores the values to the memory
            this._Hostname = Hostname;
            this._Port = Port;
            // Default line ending values
            this.LineEnding = "";
            // Creates a new socket
            if (Protocol == SocketProtocol.TcpStream)
                this._Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            else
                this._Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        /// <summary>
        /// Connects to the remote host
        /// </summary>
        public void Connect()
        {
            // Resolves the hostname to an IP address
            IPHostEntry address = Dns.GetHostEntry(this._Hostname);
            // Creates the new IP end point
            EndPoint Destination = new IPEndPoint(address.AddressList[0], (int)this._Port);
            // Connects to the socket
            this._Sock.Connect(Destination);
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public void Close()
        {
            this._Sock.Close();
        }

        /// <summary>
        /// Sends string data to the socket
        /// </summary>
        /// <param name="Data">The string to send</param>
        public void Send(string Data)
        {
            this._Sock.Send(Tools.Chars2Bytes(Data.ToCharArray()));
        }

        /// <summary>
        /// Sends binary data to the socket
        /// </summary>
        /// <param name="Data">The binary data to send</param>
        public void SendBinary(byte[] Data)
        {
            this._Sock.Send(Data);
        }

        /// <summary>
        /// Returns true when connected, otherwise false
        /// </summary>
        public bool IsConnected
        {
            get
            {
                // SelectRead returns true if:
                // - Listen has been called and a connection is pending; -or-
                // - if data is available for reading; -or-
                // - if the connection has been closed, reset, or terminated
                // We're not listening, so we only need to check if data is available for reading. If not, the connection is a goner.
                if (this._Sock.Poll(1, SelectMode.SelectRead) && this._Sock.Available == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>Returns the hostname this socket is configured for</summary>
        public string Hostname { get { return this._Hostname; } }
        /// <summary>Returns the port number this socket is configured for</summary>
        public ushort Port { get { return this._Port; } }

        /// <summary>
        /// Receives data from the socket
        /// </summary>
        /// <param name="Block">When true, this function will wait until there is data to return</param>
        /// <returns>The received data (may be empty)</returns>
        public string Receive(bool Block = false)
        {
            string RetValue = "";
            do
            {
                // Do we need to read data?
                if (this._Sock.Available > 0)
                {

                    // There is data, lets read it!
                    byte[] ReadBuffer = new byte[this._Sock.Available];
                    this._Sock.Receive(ReadBuffer);
                    // Lets add the data to the buffer
                    this._Buffer += new string(Tools.Bytes2Chars(ReadBuffer));
                }

                if (this.LineEnding.Length > 0)
                {
                    // We're going to do buffering
                    int Pos = this._Buffer.IndexOf(this.LineEnding);
                    // Appairently there's a line ending found, lets split the data up
                    if (Pos > -1)
                    {
                        RetValue = this._Buffer.Substring(0, Pos + this.LineEnding.Length);
                        this._Buffer = this._Buffer.Substring(Pos + this.LineEnding.Length);
                    }
                }
                else
                {
                    // We don't do buffering at this moment. We just send all data back.
                    RetValue = this._Buffer;
                    this._Buffer = "";
                }
            } while (Block && RetValue == "");

            return RetValue;
        }

        /// <summary>
        /// Receives binary data from the socket (line endings aren't used with this method)
        /// </summary>
        /// <param name="Length">The amount of bytes to receive</param>
        /// <returns>The binary data</returns>
        public byte[] ReceiveBinary(int Length)
        {
            byte[] RetValue = new byte[Length];
            this._Sock.Receive(RetValue);

            return RetValue;
        }
    }
}
