using System;
using Microsoft.SPOT;

namespace HttpLibrary
{
    /// <summary>
    /// Utility Class used for generating headers for multiple file types
    /// </summary>
    public class HttpUtility
    {
        /// <summary>
        /// File type enumeration
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// Image jpeg
            /// </summary>
            JPEG = 1,
            /// <summary>
            /// Image gif
            /// </summary>
            GIF = 2,
            /// <summary>
            /// Html text
            /// </summary>
            Html = 3,
            /// <summary>
            /// CSS text
            /// </summary>
            CSS = 4,
            /// <summary>
            /// Image png
            /// </summary>
            PNG = 5,
            /// <summary>
            /// Java script
            /// </summary>
            JAVASCRIPT=6
        };
        /// <summary>
        /// Generates a mime type header string based on the file length and type
        /// </summary>
        /// <param name="Type">File type</param>
        /// <param name="FileLength">File length</param>
        /// <returns></returns>
        public static string GenerateHeaderString(FileType Type, long FileLength)
        {
            if (Type == FileType.Html)
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "text/html" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
            else if (Type == FileType.JPEG)
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "image/jpeg" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
            else if (Type == FileType.GIF)
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "image/gif" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
            else if (Type == FileType.CSS)
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "text/css" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
            else if (Type == FileType.PNG)
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "image/png" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
            else if (Type == FileType.JAVASCRIPT)
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "text/javascript" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
            else
                return "HTTP/1.0 200 OK\r\nContent-Type: " + "text/html" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n";
        }
        /// <summary>
        /// Generates a mime type header byte array based on the file length and type
        /// </summary>
        /// <param name="Type">File type</param>
        /// <param name="FileLength">File length</param>
        /// <returns></returns>
        public static byte[] GenerateHeaderBytes(FileType Type, long FileLength)
        {
            if (Type == FileType.Html)
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "text/html" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
            else if (Type == FileType.JPEG)
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "image/jpeg" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
            else if (Type == FileType.GIF)
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "image/gif" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
            else if (Type == FileType.CSS)
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "text/css" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
            else if (Type == FileType.PNG)
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "image/png" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
            else if (Type == FileType.JAVASCRIPT)
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "text/javascript" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
            else
                return System.Text.UTF8Encoding.UTF8.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + "text/html" + "; charset=utf-8\r\nContent-Length: " + FileLength.ToString() + "\r\n\r\n");
        }
    }
}
