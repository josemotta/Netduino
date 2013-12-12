using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT.Net.NetworkInformation;
using Microsoft.SPOT;


using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

/*
 * Copyright 2012 Laurent Ellerbach (http://blogs.msdn.com/laurelle)
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

namespace NetduinoPlus2IR
{
    public class IrRx
    {
        #region Param
        public const char ParamSeparator = '&';
        public const char ParamStart = '?';
        public const char ParamEqual = '=';
        const int MAX_BUFF = 1024;

        public class Param
        {
            public string Name { get; set;  }
            public string Value { get; set; }
        }

        public static Param[] decryptParam(String Parameters)
        {
            Param[] retParams = null;
            int i = Parameters.IndexOf(ParamStart);
            int j = i;
            int k;
            
            if (i >= 0)
            {
                //look at the number of = and ;

                while ((i < Parameters.Length) || (i == -1))
                {
                    j = Parameters.IndexOf(ParamEqual, i);
                    if (j > i)
                    {
                        //first param!
                        if (retParams == null)
                        {
                            retParams = new Param[1];
                            retParams[0] = new Param();
                        }
                        else
                        {
                            Param[] rettempParams = new Param[retParams.Length + 1];
                            retParams.CopyTo(rettempParams, 0);
                            rettempParams[rettempParams.Length - 1] = new Param();
                            retParams = new Param[rettempParams.Length];
                            rettempParams.CopyTo(retParams, 0);
                        }
                        k = Parameters.IndexOf(ParamSeparator, j);
                        retParams[retParams.Length - 1].Name = Parameters.Substring(i + 1, j - i - 1);
                        //case'est la fin et il n'y a rien
                        if (k == j)
                        {
                            retParams[retParams.Length - 1].Value = "";
                        } // cas normal
                        else if (k > j)
                        {
                            retParams[retParams.Length - 1].Value = Parameters.Substring(j + 1, k - j - 1);
                        } //c'est la fin
                        else
                        {
                            retParams[retParams.Length - 1].Value = Parameters.Substring(j + 1, Parameters.Length - j - 1);
                        }
                        if (k > 0)
                            i = Parameters.IndexOf(ParamSeparator, k);
                        else
                            i = Parameters.Length;
                    }
                    else
                        i = -1;
                }
            }
            return retParams;
        }
        #endregion 

        #region internal objects
        private bool cancel = false;
        private Thread serverThread = null;
        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new webserver.
        /// </summary>
        /// <param name="port">Port number to listen on.</param>
        /// <param name="timeout">Timeout to listen and respond to a request in millisecond.</param>
        public IrRx(int port, int timeout)
        {
            this.Timeout = timeout;
            this.Port = port;
            this.serverThread = new Thread(StartServer);
            Debug.Print("IR Receiver started on port " + port.ToString());
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate for the CommandReceived event.
        /// </summary>
        public delegate void GetRequestHandler(object obj, IrRxEventArgs e);
        public class IrRxEventArgs: EventArgs
        {
            public IrRxEventArgs(Socket mresponse, string mcommand)
            {
                this.response = mresponse;
                this.command = mcommand;
            }
            public Socket response { get; protected set;  }
            public string command { get; protected set; }

        }

        /// <summary>
        /// CommandReceived event is triggered when a valid command (plus parameters) is received.
        /// Valid commands are defined in the AllowedCommands property.
        /// </summary>
        public event GetRequestHandler CommandReceived;

        #endregion

        #region Public and private methods

        /// <summary>
        /// Start the multithreaded server.
        /// </summary>
        public bool Start()
        {
            bool bStarted = true;
            // List ethernet interfaces, so we can determine the server's address
            //ListInterfaces();
            // start server           
            try
            {
                cancel = false;
                serverThread.Start();
                Debug.Print("Started IrRx in thread " + serverThread.GetHashCode().ToString());
            }
            catch
            {   //if there is a problem, maybe due to the fact we did not wait engouth
                cancel = true;
                bStarted = false;
            }
            return bStarted;
        }

        /// <summary>
        /// Restart the server.
        /// </summary>
        private bool Restart()
        {
            Stop();
            return Start();
        }

        /// <summary>
        /// Stop the multithreaded server.
        /// </summary>
        public void Stop()
        {
            cancel = true;
            Thread.Sleep(100);
            serverThread.Suspend();
            Debug.Print("Stopped IrRx in thread ");
        }

        /// <summary>
        /// Output a stream
        /// </summary>
        public static string OutPutStream(Socket response, string strResponse)
        {
            byte[] messageBody = Encoding.UTF8.GetBytes(strResponse);
            //if (!response.Poll(0, SelectMode.SelectError))
            if (response !=null)
                response.Send(messageBody, 0, messageBody.Length, SocketFlags.None);
            //allow time to physically send the bits
            Thread.Sleep(10);
            return "";
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        private void StartServer()
        {
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                //set a receive Timeout to avoid too long connection 
                server.ReceiveTimeout = this.Timeout*10;
                server.Bind(new IPEndPoint(IPAddress.Any, this.Port));
                server.Listen(int.MaxValue);
                while (!cancel)
                {
                    try
                    {
                        using (Socket connection = server.Accept())
                        {
                            if (connection.Poll(-1, SelectMode.SelectRead))
                            {
                                // Create buffer and receive raw bytes.
                                byte[] bytes = new byte[connection.Available];
                                int count = connection.Receive(bytes);
                                Debug.Print("Request received from " + connection.RemoteEndPoint.ToString() + " at " + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
                                //setup some time for send timeout as 10s.
                                //necessary to avoid any problem when multiple requests are done the same time.
                                connection.SendTimeout = this.Timeout; ;
                                // Convert to string, will include HTTP headers.
                                string rawData = new string(Encoding.UTF8.GetChars(bytes));
                                string mURI;

                                // Remove GET + Space
                                // pull out uri and remove the first /
                                if (rawData.Length > 5)
                                {
                                    int uriStart = rawData.IndexOf(' ') + 2;
                                    mURI = rawData.Substring(uriStart, rawData.IndexOf(' ', uriStart) - uriStart);
                                }
                                else
                                    mURI = "";
                                // return a simple header in the code like this in the handling fuction
                                string header = "HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\nCache-Control: no-cache\r\nConnection: close\r\n\r\n";
                                connection.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                                //and then you can return HTML code
                                if (CommandReceived != null)
                                    CommandReceived(this, new IrRxEventArgs(connection, mURI));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //this may be due to a bad IP address
                        Debug.Print(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// List all IP address, useful for debug only
        /// </summary>
        private void ListInterfaces()
        {
            NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
            Debug.Print("Number of Interfaces: " + ifaces.Length.ToString());
            foreach (NetworkInterface iface in ifaces)
            {
                Debug.Print("IP:  " + iface.IPAddress + "/" + iface.SubnetMask);
            }
        }

        /// <summary>
        /// Dispose of any resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        /// <param name="disposing">Dispose of resources?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                serverThread = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the port the server listens on.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Read the timeout for a request to be send.
        /// </summary>
        public int Timeout { get; protected set; }
        #endregion


    }
}