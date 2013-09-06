using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;

namespace HttpLibrary
{
    /// <summary>
    /// HttpRequest class for handeling http requests
    /// </summary>
    public class HttpRequest
    {
        private string request_buffer;
        private string files_path;
        private string http_method;
        private int request_length;
        private string host_address;
        private Socket connection;
        private Hashtable _cookie;

        /// <summary>
        /// Gets the http request entire string
        /// </summary>
        public string QueryString { get { return request_buffer; } }

        /// <summary>
        /// Gets where server files are located
        /// </summary>
        public string FilesPath { get { return files_path; } }

        /// <summary>
        /// Gets the http method GET or POST or null
        /// </summary>
        public string HttpMethod { get { return http_method; } }

        /// <summary>
        /// Gets the request length
        /// </summary>
        public int Length { get { return request_length; } }

        /// <summary>
        /// Gets the remote host address
        /// </summary>
        public string HostAddress { get { return host_address; } }

        /// <summary>
        /// Gets the current active socket
        /// </summary>
        public Socket Connection { get { return connection; } }

        /// <summary>
        /// Gets all the cookies sent with the last request
        /// </summary>
        public Hashtable cookie { get { return _cookie; } }

        /// <summary>
        /// Gets the requested page or file
        /// </summary>
        public string RequestedFile
        {
            get
            {
                int Count;
                int IndexOfSlash = request_buffer.IndexOf('/');
                for (Count = IndexOfSlash; request_buffer[Count] != ' ' && request_buffer[Count] != '?'; Count++);
                int Length = Count - IndexOfSlash;
                string Name=request_buffer.Substring(IndexOfSlash, Length);
                return Name;
            }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="buffer">Recieved buffer</param>
        /// <param name="FilesPath">Server files Location</param>
        /// <param name="Connection">Current active socket</param>
        public HttpRequest(byte[] buffer, string FilesPath, Socket Connection)
        {
            request_buffer = new string(UTF8Encoding.UTF8.GetChars(buffer));
            files_path = FilesPath;
            if (request_buffer.IndexOf("GET") >= 0)
                http_method = "GET";
            //else if (request_buffer.IndexOf("POST") >= 0)
              //  http_method = "POST";
            else
                throw new Exception("Only GET Method Are Supported");
            _cookie = new Hashtable();
            string cookies = "";
            string[] lines = request_buffer.Split('\n');
            foreach (string line in lines)
            {
                if (line.Length > 7 && line.Substring(0, 7) == "Cookie:")
                {
                    cookies = line.Substring(line.IndexOf(':') + 2);
                    string[] parts = cookies.Split(';');
                    foreach (string part in parts)
                    {
                        //Assign key and remove pesky spaces.
                        string key = part.Substring(0, part.IndexOf('=')).Trim();
                        //Add remove linebreaks/spaces from value.
                        string value = "";
                        string[] values = part.Substring(part.IndexOf('=') + 1).Split('\n');
                        for (int i = 0; i < values.Length; i += 1)
                            value += values[i].Trim();
                        //Assign cookie value to the key.
                        _cookie[key] = value;

                        Debug.Print("Cookie: " + key + " = " + cookie[key]);
                    }
                    break;
                }
            }
            request_length = request_buffer.Length;
            host_address = ((IPEndPoint)Connection.RemoteEndPoint).Address.ToString();
            connection = Connection;
        }

        /// <summary>
        /// Indexer to find values submitted by a form html
        /// </summary>
        /// <param name="value">Value name</param>
        /// <returns></returns>
        public string this[string value]
        {
            get
            {
                string firstLine = request_buffer.Substring(0, request_buffer.IndexOf("\r\n"));
                int IndexOfAndOrSpace;
                Match contains = Regex.Match(firstLine, @"[?,&]" + value + "=");
                if (contains.Success)
                {
                    int IndexOfValue = firstLine.IndexOf(contains.Value);
                    for (IndexOfAndOrSpace = IndexOfValue + 1; firstLine[IndexOfAndOrSpace] != '&' && firstLine[IndexOfAndOrSpace] != ' '; IndexOfAndOrSpace++) ;
                    int Length = IndexOfAndOrSpace - (IndexOfValue + contains.Length);
                    return firstLine.Substring(IndexOfValue + contains.Length, Length);
                }
                else
                    return null;
            }
        }
    }
}
