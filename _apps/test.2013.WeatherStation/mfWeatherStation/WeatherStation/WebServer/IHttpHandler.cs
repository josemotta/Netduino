using System;
using Microsoft.SPOT;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// HTTP interface for handler request
    /// </summary>
    public interface IHttpHandler
    {
        /// <summary>
        /// Process an HTTP request
        /// </summary>
        /// <param name="httpContext">HTTP context of request/response</param>
        void ProcessRequest(HttpContext httpContext);
    }
}
