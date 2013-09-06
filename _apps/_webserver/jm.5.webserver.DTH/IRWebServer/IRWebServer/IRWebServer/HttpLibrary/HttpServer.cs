using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections;
using Microsoft.SPOT.Net.NetworkInformation;


namespace HttpLibrary
{
    /// <summary>
    /// Delegate for Recieve event handling
    /// </summary>
    /// <param name="Request">The request object</param>
    /// <param name="Response">The response object</param>
    public delegate void OnRequestRecievedDelegate(HttpRequest Request, HttpResponse Response);
   
    /// <summary>
    /// Delegate for error event handling
    /// </summary>
    /// <param name="e">Error message</param>
    public delegate void OnServerErrorDelegate(ErrorEventArgs e);

    /// <summary>
    /// HttpServer class that handles Http requests and respoones
    /// </summary>
    public class HttpServer
    {
        private Thread server_thread;
        private Socket listen_socket;
        private Socket accepted_socket;
        private bool is_server_running;
        private string storage_path;
        private byte[] recieve_buffer;
        private byte[] send_buffer;
        private bool server_stop;
        private Configuration server_config;
        private Credential server_credential;
        private bool use_authentication;
        private const string authentication_header = "HTTP/1.1 401 Authorization Required \nWWW-Authenticate: Basic realm=";
        private const string Unauthorized_page = "<html><body><h1 align=center>" + "401 UNAUTHORIZED ACCESS</h1></body></html>";
        private const string error_header = "";
        
        private void process_request()
        {
            for (int i = 0; i < 5 && !(accepted_socket.Available > 0); i++)
                Thread.Sleep(5);
            if (accepted_socket.Available > 0)
            {
                accepted_socket.Receive(recieve_buffer);
                if (use_authentication)
                {
                    if (authenticate(recieve_buffer))
                    {
                        OnRequestReceivedFunction(new HttpRequest(recieve_buffer, storage_path, accepted_socket), new HttpResponse(send_buffer, storage_path, accepted_socket));
                    }
                    else
                    {
                        if (!server_credential.useCookie)
                        {
                            byte[] header = UTF8Encoding.UTF8.GetBytes(authentication_header + server_credential.ServerOwner + "\"\n\n");
                            accepted_socket.Send(header, 0, header.Length, SocketFlags.None);
                            accepted_socket.Send(UTF8Encoding.UTF8.GetBytes(Unauthorized_page), 0, Unauthorized_page.Length, SocketFlags.None);
                        }
                        else
                        {
                            HttpRequest received = new HttpRequest(recieve_buffer, storage_path, accepted_socket);
                            HttpResponse sending = new HttpResponse(send_buffer, storage_path, accepted_socket);
                            if (Array.IndexOf(server_credential.authList, received.RequestedFile) >= 0)
                            {
                                if (received["login"] != null)
                                {
                                    if (server_credential.Keys.Contains(Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(received["login"]))) && received.RequestedFile == "/auth.htm")
                                    {
                                        Hashtable cookie = new Hashtable(1);
                                        cookie["status"] = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(received["login"]));
                                        sending.Redirect("/", cookie);
                                    }
                                    else
                                        sending.Redirect("/auth.htm");
                                }
                                else
                                {
                                    OnRequestReceivedFunction(received, sending);
                                }
                            }
                            else
                            {
                                sending.Redirect("/auth.htm");
                            }
                        }
                    }
                }
                else
                {
                    OnRequestReceivedFunction(new HttpRequest(recieve_buffer, storage_path, accepted_socket), new HttpResponse(send_buffer, storage_path, accepted_socket));
                }
                for (int i = 0; i < recieve_buffer.Length; i++)
                    recieve_buffer[i] = 0;
                for (int i = 0; i < send_buffer.Length; i++)
                    send_buffer[i] = 0;
            }
            else
            {
                Debug.Print("Error");
                accepted_socket.Send(UTF8Encoding.UTF8.GetBytes(error_header), 0, error_header.Length, SocketFlags.None);
            }
        }

        private void run_server()
        {
            try
            {
                listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint BindingAddress = new IPEndPoint(IPAddress.Any, server_config.ListenPort);
                listen_socket.Bind(BindingAddress);
                listen_socket.Listen(1);
                is_server_running = true;
                while (!server_stop)
                {
                    accepted_socket = listen_socket.Accept();
                    Guarddog guard = new Guarddog(30000);
                    guard.running = false;
                    process_request();
                    guard.Dispose();
                    accepted_socket.Close();
                }
                Debug.Print("Server stopped");
            }
            catch (Exception)
            {
                is_server_running = false;
                OnServerErrorFunction(new ErrorEventArgs("Server Error\r\nCheck Connection Parameters"));
            }
        }

        private bool authenticate(byte[] request)
        {
            string reqstr = new string(UTF8Encoding.UTF8.GetChars(request));
            if (!server_credential.useCookie)
            {
                if (reqstr != null)
                    return (reqstr.IndexOf(server_credential.Key) >= 0);
                else
                    return false;
            }
            else
            {
                if (reqstr != null && reqstr.IndexOf('\n') >= 0)
                {
                    string[] lines = reqstr.Split('\n');
                    foreach (string line in lines)
                    {
                        if (line.Length > 7 && line.Substring(0, 7) == "Cookie:")
                        {
                            string[] parts = line.Substring(line.IndexOf(':') + 2).Split(';');
                            foreach (string part in parts)
                            {
                                //Assign key and remove pesky spaces.
                                string key = part.Substring(0, part.IndexOf('=')).Trim();
                                if (key == "status")
                                {
                                    //Remove linebreaks/spaces from value.
                                    string value = "";
                                    string[] values = part.Substring(part.IndexOf('=') + 1).Split('\n');
                                    for (int i = 0; i < values.Length; i += 1)
                                        value += values[i].Trim();
                                    if (server_credential.Keys.Contains(value))
                                        return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }        

        /// <summary>
        /// Used in event firing
        /// </summary>
        /// <param name="Error">Parameter</param>
        protected virtual void OnServerErrorFunction(ErrorEventArgs Error)
        {
            OnServerError(Error);
        }

        /// <summary>
        /// Used in event firing
        /// </summary>
        /// <param name="Request">Parameter</param>
        /// <param name="Response">Parameter</param>
        protected virtual void OnRequestReceivedFunction(HttpRequest Request, HttpResponse Response)
        {
            OnRequestReceived(Request, Response);
        }

        /// <summary>
        /// Gets if credentials are enabled
        /// </summary>
        public bool SecurityEnabled
        {
            get
            {
                return use_authentication;
            }
        }

        /// <summary>
        /// Gets the server configuration
        /// </summary>
        public Configuration Settings
        {
            get
            {
                return server_config;
            }
        }

        /// <summary>
        /// Gets if server is running
        /// </summary>
        public bool IsServerRunning
        {
            get { return is_server_running; }
        }

        /// <summary>
        /// Gets the current server running thread
        /// </summary>
        public Thread RunningThread
        {
            get { return server_thread; }
        }

        /// <summary>
        /// Gets the server credentials
        /// </summary>
        public Credential Security
        {
            get { return server_credential; }
        }

        /// <summary>
        /// ServerError event
        /// </summary>
        public event OnServerErrorDelegate OnServerError;

        /// <summary>
        /// RequestRecieved event
        /// </summary>
        public event OnRequestRecievedDelegate OnRequestReceived;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="Config">Server configuration</param>
        /// <param name="ReceiveBufferSize">Recieving buffer size</param>
        /// <param name="SendBufferSize">Sending buffer size</param>
        /// <param name="PagesDirectory">Location where pages are stored</param>
        public HttpServer(Configuration Config, int ReceiveBufferSize, int SendBufferSize, string PagesDirectory)
        {
            server_thread = null;
            listen_socket = null;
            accepted_socket = null;
            is_server_running = false;
            storage_path = PagesDirectory;
            recieve_buffer = new byte[ReceiveBufferSize];
            send_buffer = new byte[SendBufferSize];
            server_thread = new Thread(run_server);
            server_config = Config;
            use_authentication = false;
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
            if (server_config.IpAddress != "")
                networkInterface.EnableStaticIP(server_config.IpAddress, server_config.SubnetMask, server_config.DefaultGateWay);
            else
                networkInterface.EnableDhcp();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="Config">Server configuration</param>
        /// <param name="Security">Server credentials</param>
        /// <param name="ReceiveBufferSize">Recieving buffer size</param>
        /// <param name="SendBufferSize">Sending buffer size</param>
        /// <param name="PagesDirectory">Location where pages are stored</param>
        public HttpServer(Configuration Config, Credential Security, int ReceiveBufferSize, int SendBufferSize, string PagesDirectory)
        {
            server_thread = null;
            listen_socket = null;
            accepted_socket = null;
            is_server_running = false;
            storage_path = PagesDirectory;
            recieve_buffer = new byte[ReceiveBufferSize];
            send_buffer = new byte[SendBufferSize];
            server_thread = new Thread(run_server);
            server_config = Config;
            server_credential = Security;
            use_authentication = true;
         
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
            if (server_config.IpAddress != "")
                networkInterface.EnableStaticIP(server_config.IpAddress, server_config.SubnetMask, server_config.DefaultGateWay);
            else
                networkInterface.EnableDhcp();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Starts the server listener 
        /// </summary>
        public void Start()
        {
            server_stop = false;
            server_thread.Start();
        }

        /// <summary>
        /// Stops the server listener
        /// </summary>
        public void Stop()
        {
            server_stop = true;
            listen_socket.Close();
        }        
    }
}
