using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections;

namespace HttpLibrary
{
    /// <summary>
    /// HttpResponse Class for handeling Http responses
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Enumeration of supported file types
        /// </summary>
        public enum FileType 
        { 
            /// <summary>
            /// JPEG file
            /// </summary>
            JPEG = 0,
            /// <summary>
            /// GIF file
            /// </summary>
            GIF = 1,
            /// <summary>
            /// HTML file
            /// </summary>
            Html = 2,
            /// <summary>
            /// JavaScript file
            /// </summary>
            JS = 3,
            /// <summary>
            /// PNG file
            /// </summary>
            PNG = 4,
            /// <summary>
            /// ICO image file
            /// </summary>
            ICO = 5,
            /// <summary>
            /// CSS file
            /// </summary>
            CSS = 6,
            /// <summary>
            /// XML text file
            /// </summary>
            XML = 7,
            /// <summary>
            /// Plain text file
            /// </summary>
            TXT = 8,
            /// <summary>
            /// JSON text
            /// </summary>
            JSON = 9
            
        };

        /// <summary>
        /// Array of mimeTypes, use FileTypes to grab one
        /// </summary>
        public string[] mimeType = new string[] 
        {              
            "image/jpeg",
            "image/gif",
            "text/html",
            "text/javascript",
            "image/png",
            "image/vnd.microsoft.icon",
            "text/css",
            "text/xml",
            "text/plain",
            "application/json"
        };
        private Socket connection;
        private FileStream file_stream;
        private const string HtmlPageHeader = "HTTP/1.0 200 OK\r\n";
        private byte[] send_buffer;
        private string files_path;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="buffer">Send buffer</param>
        /// <param name="FilesPath">Server files Location</param>
        /// <param name="Connection">Current active socket</param>
        public HttpResponse(byte[] buffer, string FilesPath, Socket Connection)
        {
            send_buffer = buffer;
            files_path = FilesPath;
            connection = Connection;
        }

        /// <summary>
        /// Gets the current active socket
        /// </summary>
        public Socket Connection { get { return this.connection; } }

        /// <summary>
        /// Gets where server files are located
        /// </summary>
        public string FilesPath { get { return this.files_path; } }

        /// <summary>
        /// Gets the response buffer
        /// </summary>
        public string Buffer { get { return new string(UTF8Encoding.UTF8.GetChars(this.send_buffer)); } }

        /// <summary>
        /// Send a data byte array
        /// </summary>
        /// <param name="Data">Data to send</param>
        public void Write(byte[] Data)
        {
            int datalength = Data.Length;
            int i = 0;
            while (datalength > 256)
            {
                connection.Send(Data, i, 256, SocketFlags.None);
                i += 256;
                datalength -= 256;
            }
            connection.Send(Data, i, datalength, SocketFlags.None);
        }

        /// <summary>
        /// Send Data String
        /// </summary>
        /// <param name="Str">String to send</param>
        /// <param name="mime">Mime type of text</param>
        /// <param name="cookies">Any cookies to set</param>
        /// <param name="cache">Used to trigger caching of the data by the browser</param>
        public void Write(string Str, string mime = "text/html", Hashtable cookies = null, bool cache = false)
        {
            byte[] Data = UTF8Encoding.UTF8.GetBytes(Str);
            int datalength = Data.Length;
            
            //Set the date and expiration (used for caching).
            DateTime date = DateTime.Now;
            TimeSpan offsetAmount = new TimeSpan(0, -NTP.timezone, 0, 0, 0);
            date = (date + offsetAmount);
            DateTime expireOn = date.AddDays(1);
            string expires = "";

            //Enable or disable caching of page.
            if (cache)
                expires = expireOn.ToString("r");
            else
                expires = "-1";
            string cookie = "";
            if (cookies != null)
            {
                foreach (DictionaryEntry entry in cookies)
                {
                    cookie += "Set-Cookie: " + entry.Key + "=" + entry.Value + "\r\n";
                }
            }
            byte[] HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "Date: " + date.ToString("r") + "\r\nExpires: " + expires + "\r\nContent-Type: " + mime + "; charset=utf-8\r\n" + cookie + "Content-Length: " + datalength.ToString() + "\r\n\r\n");
            connection.Send(HEADER, 0, HEADER.Length, SocketFlags.None);
            int i = 0;
            while (datalength > 256)
            {
                connection.Send(Data, i, 256, SocketFlags.None);
                i += 256;
                datalength -= 256;
            }
            connection.Send(Data, i, datalength, SocketFlags.None);
        }
        
        /// <summary>
        /// Sends a file
        /// </summary>
        /// <param name="FileName">Full file path</param>
        /// <param name="cookies">Any cookies to set</param>
        /// <param name="cache">Used to trigger caching of the data by the browser</param>
        public void WriteFile(string FileName, Hashtable cookies = null, bool cache = false)
        {
            string FILE_EXTENTION = GetFileExtention(FileName.ToLower());
            switch (FILE_EXTENTION)
            {
                case "gif":
                    FragmentateAndSend(FileName, FileType.GIF, cookies, cache);
                    break;
                case "txt":
                    FragmentateAndSend(FileName, FileType.TXT, cookies, cache);
                    break;
                case "jpg":
                case "jpeg":
                    FragmentateAndSend(FileName, FileType.JPEG, cookies, cache);
                    break;
                case "htm":
                case "html":
                    FragmentateAndSend(FileName, FileType.Html, cookies, cache);
                    break;
                case "js":
                    FragmentateAndSend(FileName, FileType.JS, cookies, cache);
                    break;
                case "png":
                    FragmentateAndSend(FileName, FileType.PNG, cookies, cache);
                    break;
                case "ico":
                    FragmentateAndSend(FileName, FileType.ICO, cookies, cache);
                    break;
                case "css":
                    FragmentateAndSend(FileName, FileType.CSS, cookies, cache);
                    break;
                case "xml":
                case "xsl":
                    FragmentateAndSend(FileName, FileType.XML, cookies, cache);
                    break;
                default:
                    FragmentateAndSend(FileName, FileType.Html, cookies, cache);
                    break;
            }
        }

        /// <summary>
        /// Redirects client to a specified url
        /// </summary>
        /// <param name="Url">Url</param>
        /// <param name="cookies">Cookies to send</param>
        public void Redirect(string Url, Hashtable cookies = null)
        {
            string cookie = "";
            if (cookies != null)
            {
                foreach (DictionaryEntry entry in cookies)
                {
                    cookie += "Set-Cookie: " + entry.Key + "=" + entry.Value + "\r\n";
                }
            }
            DateTime date = DateTime.Now;
            TimeSpan offsetAmount = new TimeSpan(0, -NTP.timezone, 0, 0, 0);
            date = (date + offsetAmount);
            string rs = "<meta http-equiv=\"refresh\" content=\"0; url=" + Url + "\">";
            byte[] headerbytes = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "Date: " + date.ToString("r") + "\r\nExpires: -1 \r\nContent-Type: text/html" + "; charset=utf-8\r\n" + cookie + "Content-Length: " + rs.Length.ToString() + "\r\n\r\n");
            byte[] databytes = UTF8Encoding.UTF8.GetBytes(rs);
            connection.Send(headerbytes, 0, headerbytes.Length, SocketFlags.None);
            connection.Send(databytes, 0, databytes.Length, SocketFlags.None);
        }

        //Sends a file
        private void FragmentateAndSend(string file_name, FileType Type, Hashtable cookies, bool cache)
        {                    
            //Set the date and expiration (used for caching).
            DateTime date = DateTime.Now;
            TimeSpan offsetAmount = new TimeSpan(0, -NTP.timezone, 0, 0, 0);
            date = (date + offsetAmount);
            DateTime expireOn = date.AddDays(1);
            string expires = "";

            //Enable or disable caching of page.
            if (cache)
                expires = expireOn.ToString("r");
            else
                expires = "-1";
            string cookie = "";
            if (cookies != null)
            {
                foreach (DictionaryEntry entry in cookies)
                {
                    cookie += "Set-Cookie: " + entry.Key + "=" + entry.Value + "\r\n";
                }
            }
            using (file_stream = new FileStream(file_name, FileMode.Open, FileAccess.Read))
            {
                long FILE_LENGTH = file_stream.Length;
                byte[] HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "Date: " + date.ToString("r") + "\r\nExpires: " + expires + "\r\nContent-Type: " + mimeType[(int)Type] + "; charset=utf-8\r\n" + cookie + "Content-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");

                connection.Send(HEADER, 0, HEADER.Length, SocketFlags.None);

                while (FILE_LENGTH > send_buffer.Length)
                {
                    file_stream.Read(send_buffer, 0, send_buffer.Length);
                    connection.Send(send_buffer, 0, send_buffer.Length, SocketFlags.None);
                    FILE_LENGTH -= send_buffer.Length;
                }
                file_stream.Read(send_buffer, 0, (int)FILE_LENGTH);
                connection.Send(send_buffer, 0, (int)FILE_LENGTH, SocketFlags.None);
            }
        }
        
        
        private string GetFileExtention(string file_name)
        {
            string x = file_name;
            x = x.Substring(x.LastIndexOf('.') + 1);
            return x;
        }

        /// <summary>
        /// Writes a not found page
        /// </summary>
        public void WriteNotFound()
        {
            DateTime date = DateTime.Now;
            TimeSpan offsetAmount = new TimeSpan(0, -NTP.timezone, 0, 0, 0);
            date = (date + offsetAmount);
            string page = "<html><head><title>Page Not Found</title><body><h1 align=center>Page Not Found</h1></body></html>";
            byte[] pagebytes = UTF8Encoding.UTF8.GetBytes(page);
            byte[] headerbytes = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "Date: " + date.ToString("r") + "\r\nExpires: -1\r\nContent-Type: text/html; charset=utf-8\r\nContent-Length: " + page.Length.ToString() + "\r\n\r\n");
            connection.Send(headerbytes, 0, headerbytes.Length, SocketFlags.None);
            connection.Send(pagebytes, 0, pagebytes.Length, SocketFlags.None);
        }
    }
}
