using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace HttpLibrary
{
    /// <summary>
    /// HttpResponse Class for handeling Http responses
    /// </summary>
    public class HttpResponse
    {
        private Socket connection;
        private FileStream file_stream;
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
            this.send_buffer = buffer;
            this.files_path = FilesPath;
            this.connection = Connection;
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
                this.connection.Send(Data, i, 256, SocketFlags.None);
                i += 256;
                datalength -= 256;
            }
            this.connection.Send(Data, i, datalength, SocketFlags.None);
        }

        /// <summary>
        /// Send Data String
        /// </summary>
        /// <param name="Str">String to send</param>
        public void Write(string Str)
        {
            byte[] Data = UTF8Encoding.UTF8.GetBytes(Str);
            int datalength = Data.Length;
            int i = 0;
            while (datalength > 256)
            {
                this.connection.Send(Data, i, 256, SocketFlags.None);
                i += 256;
                datalength -= 256;
            }
            this.connection.Send(Data, i, datalength, SocketFlags.None);
        }

        /// <summary>
        /// Sends a byte array (header) followed by the data array
        /// </summary>
        /// <param name="Data">Data array</param>
        /// <param name="Header">Header Array</param>
        public void Write(byte[] Data, byte[] Header)
        {
            int datalength = Data.Length;
            int i = 0;
            connection.Send(Header, 0, Header.Length, SocketFlags.None);
            while (datalength > send_buffer.Length)
            {
                this.connection.Send(Data, i, send_buffer.Length, SocketFlags.None);
                i += send_buffer.Length;
                datalength -= send_buffer.Length;
            }
            this.connection.Send(Data, i, datalength, SocketFlags.None);
        }

        /// <summary>
        /// Send a header string followed by a data string 
        /// </summary>
        /// <param name="Str">Data string</param>
        /// <param name="Header">Header string</param>
        public void Write(string Str, string Header)
        {
            byte[] Data = UTF8Encoding.UTF8.GetBytes(Str);
            byte[] header = UTF8Encoding.UTF8.GetBytes(Header);
            int datalength = Data.Length;
            int i = 0;
            connection.Send(header, 0, header.Length, SocketFlags.None);
            while (datalength > send_buffer.Length)
            {
                this.connection.Send(Data, i, send_buffer.Length, SocketFlags.None);
                i += send_buffer.Length;
                datalength -= send_buffer.Length;
            }
            this.connection.Send(Data, i, datalength, SocketFlags.None);
        }
        
        /// <summary>
        /// Sends a file
        /// </summary>
        /// <param name="FileName">Full file path</param>
        public void WriteFile(string FileName)
        {
            string FILE_EXTENTION = GetFileExtention(FileName.ToLower());
            switch (FILE_EXTENTION)
            {
                case "gif":
                    FragmentateAndSend(FileName, HttpUtility.FileType.GIF);
                    break;
                case "txt":
                    FragmentateAndSend(FileName, HttpUtility.FileType.Html);
                    break;
                case "jpg":
                    FragmentateAndSend(FileName, HttpUtility.FileType.JPEG);
                    break;
                case "jpeg":
                    FragmentateAndSend(FileName, HttpUtility.FileType.JPEG);
                    break;
                case "htm":
                    FragmentateAndSend(FileName, HttpUtility.FileType.Html);
                    break;
                case "html":
                    FragmentateAndSend(FileName, HttpUtility.FileType.Html);
                    break;
                case "css":
                    FragmentateAndSend(FileName, HttpUtility.FileType.CSS);
                    break;
                case "png":
                    FragmentateAndSend(FileName, HttpUtility.FileType.PNG);
                    break;
                case "js":
                    FragmentateAndSend(FileName, HttpUtility.FileType.JAVASCRIPT);
                    break;
                default:
                    FragmentateAndSend(FileName, HttpUtility.FileType.Html);
                    break;
            }
        }

        /// <summary>
        /// Redirects client to a specified url
        /// </summary>
        /// <param name="Url">Url</param>
        public void Redirect(string Url)
        {
            string rs = "<meta http-equiv=\"refresh\" content=\"0; url=" + Url + "\">";
            byte[] header = HttpUtility.GenerateHeaderBytes(HttpUtility.FileType.Html, rs.Length);
            byte[] databytes = UTF8Encoding.UTF8.GetBytes(rs);
            connection.Send(header, 0, header.Length, SocketFlags.None);
            connection.Send(databytes, 0, databytes.Length, SocketFlags.None);
        }


        private void FragmentateAndSend(string file_name, HttpUtility.FileType Type)
        {
            byte[] HEADER;
            long FILE_LENGTH;
            file_stream= new FileStream(file_name, FileMode.Open, FileAccess.Read);
            FILE_LENGTH = file_stream.Length;
            HEADER = HttpUtility.GenerateHeaderBytes(Type, FILE_LENGTH);
            this.connection.Send(HEADER, 0, HEADER.Length, SocketFlags.None);

            while (FILE_LENGTH > send_buffer.Length)
            {
                file_stream.Read(send_buffer, 0, send_buffer.Length);
                this.connection.Send(send_buffer, 0, send_buffer.Length, SocketFlags.None);
                FILE_LENGTH -= send_buffer.Length;
            }
            file_stream.Read(send_buffer, 0, (int)FILE_LENGTH);
            this.connection.Send(send_buffer, 0, (int)FILE_LENGTH, SocketFlags.None);
            file_stream.Close();
        }
        
        
        private string GetFileExtention(string file_name)
        {
            return file_name.Split('.')[1];
        }

        /// <summary>
        /// Writes a not found page
        /// </summary>
        public void WriteNotFound()
        {
            string page = "<html><head><title>Page Not Found</title><body><h1 align=center>Page Not Found</h1></body></html>";
            byte[] pagebytes = UTF8Encoding.UTF8.GetBytes(page);
            byte[] headerbytes = HttpUtility.GenerateHeaderBytes(HttpUtility.FileType.Html, page.Length);
            this.connection.Send(headerbytes, 0, headerbytes.Length, SocketFlags.None);
            this.connection.Send(pagebytes, 0, pagebytes.Length, SocketFlags.None);
        }

        /// <summary>
        /// Sends all the files in the memory card as an index page  
        /// </summary>
        public void WriteFilesList()
        {
            string[] files = Directory.GetFiles(@"\SD");
            StringBuilder page = new StringBuilder();
            string file_name = "";
            page.Append("<html><head><title>File Directory</title></head>");
            page.Append("<body>");
            page.Append("<h2 align=\"center\">File Directory</h2>");
            page.Append("<hr>");
            foreach (string f in files)
            {
                file_name = f.Split('\\')[2];
                page.Append("<a href=\"/" + file_name + "\">" + file_name + "</a><br><hr>");
            }
            page.Append("</body>");
            page.Append("</html>");
            Write(page.ToString(), HttpUtility.GenerateHeaderString(HttpUtility.FileType.Html, page.Length));
        }
    }
}
