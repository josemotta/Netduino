using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace NetduinoPlusListener01
{
    public delegate void RequestReceivedDelegate(object param);

    public class WebServer
    {
        private Socket newSocket = null;
        private Socket client = null;

        public WebServer()
        {
            newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newSocket.Bind(new IPEndPoint(IPAddress.Any, 80));
            newSocket.Listen(10);
        }

        ~WebServer()
        {
            if (client != null) client.Close();
            if (newSocket != null) newSocket.Close();
        }

        public int WaitForRequest(RequestReceivedDelegate requestReceived)
        {
            int totalBytesReceived = 0;
            client = newSocket.Accept(); //IMPORTANT: This is a blocking call. It would sit here until data arrives.
            Thread.Sleep(100);

            int availableByteCount = 0;
            do
            {
                availableByteCount = client.Available;
                if (availableByteCount > 0)
                {
                    totalBytesReceived += availableByteCount;
                    byte[] buffer = new byte[128]; // Buffer probably should be larger than this.
                    int readByteCount = client.Receive(buffer, 128, SocketFlags.None);
                    string receivedStr = new string(Encoding.UTF8.GetChars(buffer));
                    Debug.Print(receivedStr);
                    requestReceived(receivedStr);
                }
            } while (availableByteCount > 0);

            //client.Close(); // Can't close yet, we need to let caller process this request and send a Response.
            return totalBytesReceived;
        }

        public void SendResponse(string responseStr) // Sends responseStr then closes the connection.
        {
            if (client != null)
            {
                Byte[] responseBytes = Encoding.UTF8.GetBytes(responseStr);
                client.Send(responseBytes, responseStr.Length, SocketFlags.None);
                client.Close();
                client = null;
            }
        }
    }
}
