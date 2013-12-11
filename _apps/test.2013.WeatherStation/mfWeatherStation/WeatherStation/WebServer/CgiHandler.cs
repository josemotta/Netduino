using System;
using Microsoft.SPOT;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Handler for CGI calls (see also RESTful calls)
    /// </summary>
    public class CgiHandler : IHttpHandler
    {
        // delegate for processing CGI request
        public delegate void ProcessGgiRequestDelegate(HttpContext httpContext);

        /// <summary>
        /// CAllback for processing  CGI request
        /// </summary>
        internal ProcessGgiRequestDelegate CgiCallback { get; set; }

        /// <summary>
        /// Return if handler can process the request
        /// </summary>
        /// <param name="httpContext">HTTP context of request/response</param>
        /// <returns>Handler can process or not the request</returns>
        public bool CanProcessRequest(HttpContext httpContext)
        {
            return (this.CgiCallback != null);
        }       

        #region IHttpHandler ...

        public void ProcessRequest(HttpContext httpContext)
        {
            // if defined, calls the callback
            if (this.CgiCallback != null)
                this.CgiCallback(httpContext);
        }

        #endregion
    }
}
