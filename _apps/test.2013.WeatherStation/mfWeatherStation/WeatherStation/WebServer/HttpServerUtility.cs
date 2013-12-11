using System;
using Microsoft.SPOT;
using System.IO;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Utility class for HTML/URL encoding/decoding
    /// </summary>
    public class HttpServerUtility
    {
        // Web Server configuration
        private MicroWebServerConfig config;

        /// <summary>
        /// Execute URL encode
        /// </summary>
        /// <param name="url">String to encode</param>
        /// <returns>String encoded</returns>
        internal static string UrlEncode(string url)
        {
            return url.Replace(" ", "%20");
        }

        /// <summary>
        /// Execute URL decode
        /// </summary>
        /// <param name="url">String to decode</param>
        /// <returns>String decoded</returns>
        internal static string UrlDecode(string url)
        {
            return url.Replace("%20", " ");
        }

        /// <summary>
        /// Execute HTML encoding
        /// </summary>
        /// <param name="html">String to encode</param>
        /// <returns>String encoded</returns>
        internal static string HtmlEncode(string html)
        {
            string result = html.Replace("/", "%2F");
            result = result.Replace(" ", "+");
            return result;
        }

        /// <summary>
        /// Execute HTML decoding
        /// </summary>
        /// <param name="html">String to decode</param>
        /// <returns>String decoded</returns>
        internal static string HtmlDecode(string html)
        {
            string result = html.Replace("%2F", "/");
            result = result.Replace("+", " ");
            return result;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Web Server configuration</param>
        internal HttpServerUtility(MicroWebServerConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Returns the physical file path that corresponds to the specified virtual path on the Web server 
        /// </summary>
        /// <param name="path">Virtual path for mapping</param>
        /// <returns>Physical file path on the Web server</returns>
        public string MapPath(string path)
        {
            return Path.Combine(this.config.WebRoot, path.Replace("/", @"\"));
        }

        /// <summary>
        /// Return a content type based on file extension
        /// </summary>
        /// <param name="fileExtension">File extension to map</param>
        /// <returns>Content type related to file extension</returns>
        public string MapContentType(string fileExtension)
        {
            string contentType = String.Empty;

            switch (fileExtension.ToLower())
            {
                // HTML files
                case ".html":
                case ".htm":
                    contentType = "text/html";
                    break;
                // CSS files
                case ".css":
                    contentType = "text/css";
                    break;
                // Javascript files
                case ".js":
                    contentType = "text/javascript";
                    break;
                // Image files
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".bmp":
                    contentType = "image/bmp";
                    break;
                // TXT files
                case ".txt":
                    contentType = "text/plain";
                    break;
            }

            return contentType;
        }
    }
}
