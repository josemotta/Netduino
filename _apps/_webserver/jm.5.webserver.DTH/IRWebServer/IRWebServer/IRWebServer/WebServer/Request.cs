using System;
using System.Threading;
using System.Collections;
using Microsoft.SPOT;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace IRWebServer
{
    //Holds information about a web request.
    public class Request : IDisposable
    {
        private string method;
        private string url;
        private Socket client;
        private bool CookieSet;
        private Hashtable cookie = new Hashtable();

        const int fileBufferSize = 256;

        internal Request(Socket Client, char[] Data)
        {
            client = Client;
            ProcessRequest(Data);
        }

        //Request method.
        public string Method
        {
            get { return method; }
        }

        //URL of request.
        public string URL
        {
            get { return url; }
        }

        //Check if a cookie is set.
        public bool cookieSet
        {
            get { return CookieSet; }
        }

        //Cookie data.
        public Hashtable Cookie
        {
            get { return cookie; }
        }
        
        //Client IP address.
        public IPAddress Client
        {
            get
            {
                IPEndPoint ip = client.RemoteEndPoint as IPEndPoint;
                if (ip != null)
                    return ip.Address;
                return null;
            }
        }

        //Send a response back to the client.
        public void SendResponse(string response, bool cache = true, string cookies = "",  string type = "text/html")
        {
            if (client != null)
            {
                //Set the date and expiration (used for caching).
                DateTime date = DateTime.Now;
                TimeSpan offsetAmount = new TimeSpan(0, -Program.TimeZone, 0, 0, 0);
                date = (date + offsetAmount);
                DateTime expireOn = date.AddDays(1);
                string expires = "";

                //Enable or disable caching of page.
                if (cache)
                    expires = expireOn.ToString("r");
                else
                    expires = "-1";

                string header = "HTTP/1.1 200 OK\r\nDate: " + date.ToString("r") + "\r\nExpires: " + expires + "\r\nContent-Type: " + type + "; charset=utf-8\r\n" + cookies + "Content-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";

                client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                client.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);

                Debug.Print("Response of " + response.Length.ToString() + " sent.");
            }

        }

        //Process a cookie and append it to the appropriate header.
        public void SetCookie(Hashtable cookie, string response, bool sendFile = false, string type = "text/html")
        {
            
            string cookies = "";
            foreach (DictionaryEntry entry in cookie)
            {
                cookies += "Set-Cookie: " + entry.Key + "=" + entry.Value + "\r\n";
            }
            Debug.Print("Cookies being sent:\n" + cookies);

            //If a response sets a cookie it should not be cached, since a cached response won't set cookie data.
            if (!sendFile)
            {
                SendResponse(response, false, cookies);
                Debug.Print("Sending response with cookies");
            }
            else
            {
                SendFile(response, false, cookies);
                Debug.Print("Sending file with cookies");
            }
        }

        //Sends a file back to the client, assumes the application using this has checked whether it exists.
        public void SendFile(string filePath, bool cache = true, string cookies = "")
        {

            //Map the file extension to a mime type
            string type = "";
            int dot = filePath.LastIndexOf('.');
            if (dot != 0)
                switch (filePath.Substring(dot + 1))
                {
                    case "css":
                        type = "text/css";
                        break;
                    case "js":
                        type = "text/javascript";
                        break;
                    case "xml":
                    case "xsl":
                        type = "text/xml";
                        break;
                    case "jpg":
                    case "jpeg":
                        type = "image/jpeg";
                        break;
                    case "gif":
                        type = "image/gif";
                        break;
                    case "png":
                        type = "image/png";
                        break;
                    case "htm":
                    case "html":
                        type = "text/html";
                        break;
                    case "ico":
                        type = "image/vnd.microsoft.icon";
                        break;
                    default:
                        throw new System.ArgumentException("File mimetype not found", filePath.Substring(dot + 1));

                    //Not exhaustive. Extend this list as required.
                }

            using (FileStream inputStream = new FileStream(filePath, FileMode.Open))
            {
                //Set the date and expiration.
                DateTime date = DateTime.Now;
                TimeSpan offsetAmount = new TimeSpan(0, -Program.TimeZone, 0, 0, 0);
                date = (date + offsetAmount);
                DateTime expireOn = date.AddDays(1);
                string expires = "";

                //Enable or disable caching of page.
                if (cache)
                    expires = expireOn.ToString("r");
                else
                    expires = "-1";

                //Send the header
                string header = "HTTP/1.1 200 OK\r\nDate: " + date.ToString("r") + "\r\nExpires: " + expires + "\r\nContent-Type: " + type + ";charset=utf-8\r\n" + cookies + "Content-Length: " + inputStream.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);

                byte[] readBuffer = new byte[fileBufferSize];
                while (true)
                {
                    //Send the file a few bytes at a time
                    int bytesRead = inputStream.Read(readBuffer, 0, readBuffer.Length);
                    if (bytesRead == 0)
                        break;

                    client.Send(readBuffer, bytesRead, SocketFlags.None);
                    Debug.Print("Sending " + readBuffer.Length.ToString() + "bytes...");
                }
            }
            Debug.Print("Sent file " + filePath);
        }

        //Send a Not Found response.
        public void Send404()
        {
            string response = "<h1>404 Not found</h1>";
            string header = "HTTP/1.1 404 Not Found\r\nContent-Length: " + response.Length.ToString() + "\r\nContent-Type: text/html\r\nConnection: close\r\n\r\n";
            if (client != null)
            {
                client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                client.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
            }
            Debug.Print("Sent 404 Not Found");
        }

        //Process the request header.
        private void ProcessRequest(char[] data)
        {
            string content = new string(data);
            string[] lines = content.Split('\n');
            
            /*
            //This loop is just for debugging/printing incoming HTML requests, enable if necessary.
            int i = 1;
            foreach (string line in lines)
            {
                Debug.Print("Request header line " + i + ": " + line);
                i++;
            }
            */

            // Parse the first line of the request: "GET /path/ HTTP/1.1"
            string[] words = lines[0].Split(' ');
            method = words[0];
            url = words[1];

            //Check for a cookie.
            CookieSet = false;
            string cookies = "";

            foreach (string line in lines)
            {
                if (line.Length > 7 && line.Substring(0,7) == "Cookie:")
                {
                    CookieSet = true;
                    cookies = line.Substring(line.IndexOf(':') + 2);
                    string[] parts = cookies.Split(';');
                    foreach (string part in parts)
                    {
                        //Assign key and remove pesky spaces.
                        string key = part.Substring(0, part.IndexOf('=')).Trim(); 
                        //Add remove linebreaks/spaces from value.
                        string value = "";
                        string[] values = part.Substring(part.IndexOf('=') + 1).Split('\n');
                        for (int i = 0; i < values.Length; i+= 1)
                            value += values[i].Trim();
                        //Assign cookie value to the key.
                        cookie[key] = value;

                        Debug.Print("Cookie key " + key + " assigned to value " + cookie[key]);
                    }
                    //Exit the loop as all cookies should be sent on one line.
                    break;
                }
            }
            //Could look for any further headers in other lines of the request if required (e.g. User-Agent).
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (client != null)
            {
                //Pause a few milliseconds to make sure the browser has received the data.
                Thread.Sleep(50);
                client.Close();
                client = null;
            }
        }

        #endregion
    }
}
