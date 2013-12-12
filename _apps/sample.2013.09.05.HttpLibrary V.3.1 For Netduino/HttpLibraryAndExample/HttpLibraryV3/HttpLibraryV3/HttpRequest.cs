using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Text;
using System.Net;

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
        private bool use_ram;
        private RamString ram_string;

        /// <summary>
        /// Gets the http request entire string
        /// </summary>
        public string QueryString
        {
            get
            {
                if (!use_ram)
                {
                    return this.request_buffer;
                }
                else
                {
                    return ram_string.ToString();
                }
            }
        }
        /// <summary>
        /// Gets where server files are located
        /// </summary>
        public string FilesPath { get { return this.files_path; } }
        /// <summary>
        /// Gets the http method GET or POST or null
        /// </summary>
        public string HttpMethod { get { return this.http_method; } }
        /// <summary>
        /// Gets the request length
        /// </summary>
        public int Length { get { return this.request_length; } }
        /// <summary>
        /// Gets the remote host address
        /// </summary>
        public string HostAddress { get { return this.host_address; } }
        /// <summary>
        /// Gets the current active socket
        /// </summary>
        public Socket Connection { get { return this.connection; } }
        /// <summary>
        /// Gets the requested page or file
        /// </summary>
        public string RequestedFile
        {
            get
            {
                if (!use_ram)
                {
                    int Count;
                    int IndexOfSlash = request_buffer.IndexOf('/') + 1;
                    for (Count = IndexOfSlash; request_buffer[Count] != ' ' && request_buffer[Count] != '?'; Count++) ;
                    int Length = Count - IndexOfSlash;
                    string Name = this.request_buffer.Substring(IndexOfSlash, Length);
                    return (Name == "") ? null : Name;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    int Count;
                    int IndexOfSlash = ram_string.IndexOf("/") + 1;
                    for (Count = IndexOfSlash; ram_string[Count] != ' ' && ram_string[Count] != '?'; Count++) ;
                    int Length = Count - IndexOfSlash;
                    for (int i = IndexOfSlash; i < IndexOfSlash + Length; i++)
                        sb.Append(ram_string[i]);
                    string Name = sb.ToString();
                    return (Name == "") ? null : Name;
                }
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
            this.ram_string = null;
            this.use_ram = false;
            this.request_buffer = new string(UTF8Encoding.UTF8.GetChars(buffer));
            this.files_path = FilesPath;
            if (this.request_buffer.IndexOf("GET") >= 0)
                this.http_method = "GET";
            else if (this.request_buffer.IndexOf("POST") >= 0)
                this.http_method = "POST";
            else
                throw new Exception("Only GET And POST Methods Are Supported");
            this.request_length = this.request_buffer.Length;
            this.host_address = ((IPEndPoint)Connection.RemoteEndPoint).Address.ToString();
            this.connection = Connection;
        }

        /// <summary>
        /// Class constructor which initializes used with an external ram 23K640
        /// </summary>
        /// <param name="Connection">Current socket</param>
        /// <param name="RamBuffer">RamString passed</param>
        /// <param name="DataSize">Available read size</param>
        /// <param name="FilesPath">Files storage path</param>
        public HttpRequest(Socket Connection, RamString RamBuffer, int DataSize, string FilesPath)
        {
            this.ram_string = RamBuffer;
            this.use_ram = true;
            this.files_path = FilesPath;
            this.host_address = ((IPEndPoint)Connection.RemoteEndPoint).Address.ToString();
            this.connection = Connection;
            
            byte[] b = new byte[1];
            
            ram_string.BeginWrite(0);
            for (int i = 0; i < DataSize; i++)
            {
                Connection.Receive(b, 0, b.Length, SocketFlags.None);
                ram_string.WriteSingle(b[0]);
            }
            ram_string.EndWrite();

            if (this.ram_string.IndexOf("GET") >= 0)
                this.http_method = "GET";
            else if (this.ram_string.IndexOf("POST") >= 0)
                this.http_method = "POST";
            else
                throw new Exception("Only GET And POST Methods Are Supported");
            this.request_length = this.ram_string.Length;
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
                if (!use_ram)
                {
                    int IndexOfValue = request_buffer.IndexOf(value + "=");
                    int i;
                    StringBuilder sb = new StringBuilder();
                    if (IndexOfValue >= 0)
                    {
                        for (i = IndexOfValue; request_buffer[i] != '='; ++i) ;
                        for (++i; request_buffer[i] != '&' && request_buffer[i] != ' '; i++)
                            sb.Append(request_buffer[i]);
                        return sb.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    int IndexOfValue = ram_string.IndexOf(value + "=");
                    int i;
                    StringBuilder sb = new StringBuilder();
                    if (IndexOfValue >= 0)
                    {
                        for (i = IndexOfValue; ram_string[i] != '='; ++i) ;
                        for (++i; ram_string[i] != '&' && ram_string[i] != ' '; i++)
                            sb.Append(ram_string[i]);
                        return sb.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
