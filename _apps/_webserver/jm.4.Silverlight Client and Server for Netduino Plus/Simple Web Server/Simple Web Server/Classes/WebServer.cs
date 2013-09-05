using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace SimpleWebServer
{
    class WebServer : IDisposable
    {
        #region Constants

        /// <summary>
        /// Default port for HTTP.
        /// </summary>
        public const int HTTP_PORT = 80;

        /// <summary>
        /// Maximum size of the buffer / request allowed.
        /// </summary>
        private const int RECEIVE_BUFFER_SIZE = 1024;

        /// <summary>
        /// Size of the buffer used to send any files to the client.
        /// </summary>
        private const int SEND_BUFFER_SIZE = 256;

        /// <summary>
        /// HTTP response for file not found.
        /// </summary>
        private const string HTTP_404_PAGE_NOT_FOUND = "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";

        /// <summary>
        /// HTTP response code for unsupported medis (used for unknown file types).
        /// </summary>
        private const string HTTP_415_UNSUPPORTED_MEDIA_TYPE = "HTTP/1.1 415 Unsupported Media Type\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";

        /// <summary>
        /// HTTP resonse for internal server error.
        /// </summary>
        private const string HTTP_500_INTERNAL_SERVER_ERROR = "HTTP/1.1 500 Internal Server Error\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";

        /// <summary>
        /// HTTP response for requests which have not been implmented.
        /// </summary>
        private const string HTTP_501_NOT_IMPLEMENTED = "HTTP/1.1 501 Not Implemented\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";

        /// <summary>
        /// HTTP reponse for HTTP version not supported.
        /// </summary>
        private const string HTTP_505_HTTP_VERSION_NOT_SUPPORTED = "HTTP/1.1 505 HTTP Version Not Supported\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";

        #endregion

        #region Private variables.

        /// <summary>
        /// Location of the files holding the web content.
        /// </summary>
        private string _webRoot;

        /// <summary>
        /// Socket we are using to listen for requests.
        /// </summary>
        private Socket _socket;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// This declaration stops the user from using / declaring a default constructor.
        /// </remarks>
        private WebServer()
        {
            _webRoot = string.Empty;
            _socket = null;
        }

        /// <summary>
        /// Constructor for the WebServer class.
        /// </summary>
        /// <param name="webFilesLocation">Directory where the web files can be found.</param>
        /// <param name="portNumber">Port to listen on for requests.</param>
        public WebServer(string webFilesLocation, int portNumber)
        {
            _webRoot = webFilesLocation;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            _socket.Listen(int.MaxValue);
            new Thread(Listen).Start();
        }

        #endregion

        #region IDisposable members

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
                _socket.Close();
                _socket = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Listen for requests and respond to the client request.
        /// </summary>
        private void Listen()
        {
            while (true)
            {
                using (Socket client = _socket.Accept())
                {
                    int requestSize;
                    byte[] buffer;
                    int amountRead;
                    string request;

                    requestSize = client.Available;
                    buffer = new byte[RECEIVE_BUFFER_SIZE];
                    Debug.Print("Request received from " + client.RemoteEndPoint.ToString() + " at " + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
                    amountRead = client.Receive(buffer, RECEIVE_BUFFER_SIZE, SocketFlags.None);
                    request = new string(Encoding.UTF8.GetChars(buffer));
                    Debug.Print(request);
                    ProcessRequest(client, request);
                    buffer = null;
                    request = null;
                    client.Close();
                }
            }
        }

        /// <summary>
        /// Process the request from the user.
        /// </summary>
        /// <remarks>
        /// A typical request will look something like this:
        /// 
        ///     GET /default.html HTTP/1.1
        ///     Host: 192.168.5.100
        ///     Connection: keep-alive
        ///     Cache-Control: no-cache
        ///     Pragma: no-cache
        ///     Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
        ///     User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.237 Safari/534.10
        ///     Accept-Encoding: gzip,deflate,sdch
        ///     Accept-Language: en-GB,en-US;q=0.8,en;q=0.6
        ///     Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
        /// 
        /// </remarks>
        /// <param name="client">Client which is making the request.</param>
        /// <param name="request">Request from the client web browser.</param>
        private void ProcessRequest(Socket client, string request)
        {
            string[] firstLine;

            firstLine = request.Substring(0, request.IndexOf('\n')).Split(' ');
            if (firstLine[0].ToLower() != "get")
            {
                Send(client, HTTP_501_NOT_IMPLEMENTED);
            }
            else
            {
                if (firstLine[2].ToLower() != "http/1.1\r")
                {
                    Send(client, HTTP_505_HTTP_VERSION_NOT_SUPPORTED);
                }
                else
                {
                    SendFile(client, firstLine[1]);
                }
            }
        }

        /// <summary>
        /// Send a response to the client.
        /// </summary>
        /// <param name="client">Client which is making the request.</param>
        /// <param name="response">Response to send to the client</param>
        private void Send(Socket client, string response)
        {
            client.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
        }

        /// <summary>
        /// Send the file to the client.
        /// </summary>
        /// <param name="client">Client which is making the request.</param>
        /// <param name="file">File being requested by the client.</param>
        private void SendFile(Socket client, string file)
        {
            if ((file.Length > 14) && (file.Substring(0, 14).ToLower() == "/command.html?"))
            {
                ProcessCommand(client, file.Substring(14));
            }
            else
            {
                string fullPath;
                string[] fileNameComponents;

                fileNameComponents = file.Split('/');
                fullPath = _webRoot;
                foreach (string fnc in fileNameComponents)
                {
                    if (fnc.Length > 0)
                    {
                        fullPath += @"\" + fnc;
                    }
                }
                Debug.Print("File requested: " + fullPath);
                if (File.Exists(fullPath))
                {
                    string fileType;

                    #region Work out the file type

                    int period;

                    period = fullPath.LastIndexOf('.');
                    fileType = null;
                    if (period > 0)
                    {
                        switch (fullPath.Substring(period + 1).ToLower())
                        {
                            case "html":
                                fileType = "text/html";
                                break;
                            case "css":
                                fileType = "text/css";
                                break;
                            case "js":
                                fileType = "text/javascript";
                                break;
                            case "xap":
                                fileType = "application/x-silverlight-app";
                                break;
                            case "xml":
                                fileType = "text/xml";
                                break;
                            case "jpg":
                                fileType = "image/jpeg";
                                break;
                            case "gif":
                                fileType = "image/gif";
                                break;
                        }
                    }

                    #endregion

                    #region Send the file

                    if (fileType != null)
                    {
                        Debug.Print("Sending file " + fullPath);
                        using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                        {
                            int amountRead;
                            byte[] buffer;

                            Send(client, "HTTP/1.0 200 OK\r\nContent-Type: " + fileType + "; charset=utf-8\r\nContent-Length: " + fs.Length.ToString() + "\r\n\r\n");
                            buffer = new byte[SEND_BUFFER_SIZE];
                            amountRead = fs.Read(buffer, 0, SEND_BUFFER_SIZE);
                            while (amountRead != 0)
                            {
                                client.Send(buffer, amountRead, SocketFlags.None);
                                amountRead = fs.Read(buffer, 0, SEND_BUFFER_SIZE);
                            }
                        }
                    }
                    else
                    {
                        Send(client, HTTP_415_UNSUPPORTED_MEDIA_TYPE);
                    }

                    #endregion
                }
                else
                {
                    Send(client, HTTP_404_PAGE_NOT_FOUND);
                }
            }
        }

        /// <summary>
        /// Process the command from the user.
        /// </summary>
        /// <param name="client">Client making the request.</param>
        /// <param name="command">Command being issued by the client.</param>
        private void ProcessCommand(Socket client, string command)
        {
            string[] commands;

            commands = command.Split('&');
            foreach (string c in commands)
            {
                string[] commandAndParameter;

                commandAndParameter = c.Split('=');
                switch (commandAndParameter[0].ToLower())
                {
                    case "sayhello":
                        Send(client, "<html><body><p>Hello, world</p></body></html>");
                        break;
                    case "gettime":
                        TimeSpan time;

                        time = Utility.GetMachineTime();
                        Send(client, "Time " + time.Hours + ":" + time.Minutes + ":" + time.Seconds);
                        break;
                    case "gettemperature":
                        double temperature;

                        temperature = ((double) Utility.GetMachineTime().Milliseconds) / 100;
                        Send(client, "Temperature:" + temperature.ToString());
                        break;
                    default:
                        Send(client, HTTP_501_NOT_IMPLEMENTED);
                        break;
                }
            }
        }

        #endregion
    }
}