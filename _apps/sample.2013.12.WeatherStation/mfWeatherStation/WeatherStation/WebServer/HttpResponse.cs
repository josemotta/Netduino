using System;
using Microsoft.SPOT;
using System.IO;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Class for HTTP response
    /// </summary>
    public class HttpResponse : HttpBase
    {
        /// <summary>
        /// Status code of HTTP response
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// HTTP response content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Response stream
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpResponse()
            : base()
        {
            // set default status code and content type
            this.StatusCode = HttpStatusCode.OK;
            this.ContentType = "text/html";
        }

        /// <summary>
        /// Close response stream
        /// </summary>
        public void CloseStream()
        {
            if (this.Stream != null)
            {
                this.Stream.Close();
                this.Stream.Dispose();
            }
        }
    }
}
