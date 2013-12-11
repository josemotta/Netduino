using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using Microsoft.SPOT.Net.NetworkInformation;
using System.IO;
using System.Text;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Main class for Web Server
    /// </summary>
    public class MicroWebServer
    {
        #region Constants ...

        // default HTTP port
        private const int HTTP_PORT_DEFAULT = 80;
        // default Backlog
        private const int BACKLOG = 10;
        //default webroot directory
        private const string WEBROOT_DEFAULT = @"SD\webroot\";
        // size of response buffer
        private const int BUFFER_RESPONSE_SIZE = 1024;
        // default start page
        public const string DEFAULT_PAGE = "index.htm";

        #endregion

        #region Fields ...

        // socket instance for listening incoming request
        private Socket listenerSocket;
        // local end point for Web Server
        private IPEndPoint localEndPoint;
        // Web Server configuration
        private MicroWebServerConfig config;

        // main thread for listening incoming request
        private Thread listenerThread;
        // Web Server running status
        private bool isRunning;

        // thread pool for incoming request
        private ArrayList workerThreadPool;
        // request socket queue
        private Queue requestQueue;

        // handlers for incoming request
        private Hashtable handlers;
        // handler for access to Web Server file system
        private FileSystemHandler fileSystemHandler;
        // handler for CGI calls (see also RESTful calls)
        private CgiHandler cgiHandler;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Web Server configuration</param>
        public MicroWebServer(MicroWebServerConfig config)
        {
            // check HTTP port
            if (config.HttpPort <= 0)
                throw new ArgumentException("Wrong HTTP port in configuration");

            // check backlog
            if (config.Backlog <= 0)
                throw new ArgumentException("Wrong Backlog in configuration");

            // if not defined, set WebRoot default directory
            if ((config.WebRoot == null) || (config.WebRoot == String.Empty))
                this.config.WebRoot = WEBROOT_DEFAULT;

            // set Web Server configuration
            this.config = config;

            // create Web Root directory if it doesn't exist
            if (!Directory.Exists(this.config.WebRoot))
                Directory.CreateDirectory(this.config.WebRoot);

            // listener thread not yet running
            this.isRunning = false;
            // create thread pool for request processing and request queue
            this.workerThreadPool = new ArrayList();
            this.requestQueue = new Queue();

            // create handlers collection and set defaults handler
            this.handlers = new Hashtable();
            this.fileSystemHandler = new FileSystemHandler();
            this.cgiHandler = new CgiHandler();
        }

        /// <summary>
        /// Start Web Server
        /// </summary>
        public void Start()
        {
            this.listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // create end point and bind socket
#if EMULATOR
            this.localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.200"), 8080);
#else
            this.localEndPoint = new IPEndPoint(IPAddress.Any, this.config.HttpPort);
#endif
            
            
            this.listenerSocket.Bind(this.localEndPoint);
            // set backlog for listening
            this.listenerSocket.Listen(this.config.Backlog);

            // create and start listening thread for incoming request
            this.listenerThread = new Thread(this.ListenerThread);
            this.isRunning = true;
            this.listenerThread.Start();
        }

        /// <summary>
        /// Main thread for listening incoming request
        /// </summary>
        private void ListenerThread()
        {
            while (this.isRunning)
            {
                // accept incoming request
                Socket requestSocket = this.listenerSocket.Accept();

                lock (this.requestQueue)
                {
                    // enqueue socket request
                    this.requestQueue.Enqueue(requestSocket);
                }

                // check on maximum dimension of worker thread pool
                if (this.workerThreadPool.Count < this.config.MaxWorkerThread)
                {
                    lock (this.workerThreadPool)
                    {
                        // create a new worker thread (adding to pool) for processing request
                        Thread workerThread = new Thread(this.WorkerThread);
                        this.workerThreadPool.Add(workerThread);
                        workerThread.Start();
                    }
                }
                
            }
        }

        /// <summary>
        /// Thread for processing incoming request
        /// </summary>
        private void WorkerThread()
        {
            // the same worker thread can handle more request in the queue
            // if it is fast to process each request
            while (this.requestQueue.Count > 0)
            {
                Socket requestSocket;
                lock (this.requestQueue)
                {
                    // dequeue socket request
                    requestSocket = (Socket)this.requestQueue.Dequeue();
                }

                // process request
                this.ProcessRequest(requestSocket);
                // closing request socket
                requestSocket.Close();
            }

            lock (this.workerThreadPool)
            {
                // remove worker thread from the pool
                this.workerThreadPool.Remove(Thread.CurrentThread);
            }
        }

        /// <summary>
        /// Process a request from the corrisponding socket
        /// </summary>
        /// <param name="requestSocket">Socket bind to request</param>
        private void ProcessRequest(Socket requestSocket)
        {
            HttpRequest httpRequest = null;
            bool badRequest = false;
            
            try
            {
                // parse incoming request
                httpRequest = HttpRequest.Parse(requestSocket);
            }
            catch
            {
                // error parsing incoming request (bad request)
                httpRequest = new HttpRequest();
                badRequest = true;
            }

            // create an HTTP context for the current request
            HttpContext httpContext = new HttpContext(httpRequest, new HttpServerUtility(this.config));
            
            if (badRequest)
            {
                httpContext.Response.StatusCode = HttpStatusCode.BadRequest;
                httpContext.Response.Body = null;
            }
            else
            {
                try
                {
                    // resolve handler to manage request
                    IHttpHandler handler = this.ResolveHandler(httpContext);

                    // no handler found
                    if (handler == null)
                    {
                        httpContext.Response.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        // process request by handler
                        handler.ProcessRequest(httpContext);
                    }
                }
                catch
                {
                    httpContext.Response.StatusCode = HttpStatusCode.InternalServerError;
                    httpContext.Response.Body = null;
                }
            }

            // TODO : evaluate initial capacity
            // build the status line
            StringBuilder responseBuilder = new StringBuilder("HTTP/1.1 ");
            responseBuilder.Append(httpContext.Response.StatusCode);
            responseBuilder.Append(" ");
            responseBuilder.AppendLine(this.MapStatusCodeToReason(httpContext.Response.StatusCode));
            
            // build header section
            httpContext.Response.Headers["Content-Type"] = httpContext.Response.ContentType;
            int bodyLength = ((httpContext.Response.Body != null) && (httpContext.Response.Body != String.Empty)) ? httpContext.Response.Body.Length : 0;
            if (bodyLength > 0)
                httpContext.Response.Headers["Content-Length"] = bodyLength.ToString();
            httpContext.Response.Headers["Connection"] = "close";

            foreach (string responseHeaderKey in httpContext.Response.Headers.Keys)
            {
                responseBuilder.Append(responseHeaderKey);
                responseBuilder.Append(": ");
                responseBuilder.AppendLine(httpContext.Response.Headers[responseHeaderKey]);
            }

            // line blank seperation header-body
            responseBuilder.AppendLine();
            // start sending status line and headers
            byte[] buffer = Encoding.UTF8.GetBytes(responseBuilder.ToString());
            requestSocket.Send(buffer);

            // send body, if it exists
            if (bodyLength > 0)
            {
                buffer = Encoding.UTF8.GetBytes(httpContext.Response.Body);
                requestSocket.Send(buffer);
            }
            // no body, streamed response
            else
            {
                if (httpContext.Response.Stream != null)
                {
                    byte[] sendBuffer = new byte[512];
                    int sendBytes = 0;
                    while ((sendBytes = httpContext.Response.Stream.Read(sendBuffer, 0, sendBuffer.Length)) > 0)
                    {
                        requestSocket.Send(sendBuffer, sendBytes, SocketFlags.None);
                    }
                    httpContext.Response.CloseStream();
                }
            }
        }

        /// <summary>
        /// Resolve and return the HTTP handler for the HTTP context of request/response
        /// </summary>
        /// <param name="httpContext">HTTP context of request/response</param>
        /// <returns>HTTP handler for processing the request</returns>
        private IHttpHandler ResolveHandler(HttpContext httpContext)
        {
            IHttpHandler handler = null;

            // check if it is a request for the file system
            if (this.fileSystemHandler.CanProcessRequest(httpContext))
            {
                handler = this.fileSystemHandler;
            }
            else
            {
                // if there are handlers    
                if (this.handlers.Count > 0)
                {
                    // check URL for handler
                    if (this.handlers.Contains(httpContext.Request.URL))
                        handler = (IHttpHandler)this.handlers[httpContext.Request.URL];
                    else
                    {
                        // no handler for URL, try to use CGI handler
                        if (this.cgiHandler.CanProcessRequest(httpContext))
                            handler = this.cgiHandler;
                    }
                }
                else
                {
                    // no handlers registered, try to use CGI handler
                    if (this.cgiHandler.CanProcessRequest(httpContext))
                        handler = this.cgiHandler;
                }
            }

            return handler;
        }

        /// <summary>
        /// Register an HTTP handler for incoming request
        /// </summary>
        /// <param name="url">URL for mapping the handler execution</param>
        /// <param name="handler">HTTP handler for the incoming request on specified URL</param>
        public void RegisterHandler(string url, IHttpHandler handler)
        {
            if ((url != null) && (url != String.Empty))
                this.handlers.Add(url.Trim('/').ToLower(), handler);
            else
                throw new ArgumentNullException("url parameter cannot be null or empty");
        }

        /// <summary>
        /// Register the callback for builtin CGI handler
        /// </summary>
        /// <param name="cgiCallback">Callback for CGI request</param>
        public void RegisterCgiCallback(CgiHandler.ProcessGgiRequestDelegate cgiCallback)
        {
            this.cgiHandler.CgiCallback = cgiCallback;
        }

        /// <summary>
        /// Stop Web Server
        /// </summary>
        public void Stop()
        {
            this.isRunning = false;

            // TODO : synchronize ListenThread end

            // TODO : close all worker thread in the pool

            // closing listening socket
            this.listenerSocket.Close();
        }

        /// <summary>
        /// Map an HTTP status code to reason string
        /// </summary>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Reason mapped</returns>
        private string MapStatusCodeToReason(HttpStatusCode statusCode)
        {
            string reason = null;
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    reason = "OK";
                    break;
                case HttpStatusCode.Redirect:
                    reason = "Found";
                    break;
                case HttpStatusCode.BadRequest:
                    reason = "Bad request";
                    break;
                case HttpStatusCode.UnAuthorized:
                    reason = "Unauthorized";
                    break;
                case HttpStatusCode.Forbidden:
                    reason = "Forbidden";
                    break;
                case HttpStatusCode.NotFound:
                    reason = "Not found";
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    reason = "Method not allowed";
                    break;
                case HttpStatusCode.InternalServerError:
                    reason = "Internal server error";
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    reason = "Service unavailable";
                    break;
            }
            return reason;
        }
    }
}