using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IRWebServer
{
    public delegate void RequestReceivedDelegate(Request request);

    public class Listener : IDisposable
    {
        const int maxRequestSize = 1024;
        //Set this to the port you want to listen on.
        readonly int portNumber = 8081;

        private Socket listeningSocket = null;
        private RequestReceivedDelegate requestReceived;

        public Listener(RequestReceivedDelegate RequestReceived)
            //You need to set the port here too.
            : this(RequestReceived, 8081) { }

        public Listener(RequestReceivedDelegate RequestReceived, int PortNumber)
        {
            portNumber = PortNumber;
            requestReceived = RequestReceived;
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            listeningSocket.Listen(10);

            /*
             * Start listener threads. Two aren't necessary, but multiple listening threads
             * allow multiple requests to be answered more quickly. Increase or decrease 
             * the number of threads to your liking.
             */
            new Thread(StartListening).Start();
           // new Thread(StartListening).Start();
        }

        ~Listener()
        {
            Dispose();
        }

        public void StartListening()
        {
            while (true)
            {
                using (Socket clientSocket = listeningSocket.Accept())
                {
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    Debug.Print("Received request from " + clientIP.ToString());
                    var x = clientSocket.RemoteEndPoint;

                    int availableBytes = clientSocket.Available;
                    Debug.Print(DateTime.Now.ToString() + " " + availableBytes.ToString() + " request bytes available");

                    int bytesReceived = (availableBytes > maxRequestSize ? maxRequestSize : availableBytes);
                    if (bytesReceived > 0)
                    {
                        byte[] buffer = new byte[bytesReceived + 8]; //Buffer is 8 bytes larger than the request, free to tweak.
                        int readByteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);

                        using (Request r = new Request(clientSocket, Encoding.UTF8.GetChars(buffer)))
                        {
                            Debug.Print(DateTime.Now.ToString() + " " + r.URL);
                            if (requestReceived != null) requestReceived(r);
                        }

                    }
                }
                Thread.Sleep(10);
            }

        }

        #region IDisposable Members

        public void Dispose()
        {
            if (listeningSocket != null) listeningSocket.Close();
        }

        #endregion
    }
}
