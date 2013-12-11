using System;
using Microsoft.SPOT;
using System.IO;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Handler for files on file system
    /// </summary>
    internal class FileSystemHandler : IHttpHandler
    {
        /// <summary>
        /// Return if handler can process the request
        /// </summary>
        /// <param name="httpContext">HTTP context of request/response</param>
        /// <returns>Handler can process or not the request</returns>
        public bool CanProcessRequest(HttpContext httpContext)
        {
            // no URL, process default page
            if (httpContext.Request.URL == String.Empty)
                return true;

            // get real file path in webroot directory
            string filePath = httpContext.Server.MapPath(httpContext.Request.URL.TrimStart('/'));    

            return File.Exists(filePath);
        }

        #region IHttpHandler ...

        public void ProcessRequest(HttpContext httpContext)
        {
            string filePath;
            // no URL, process default page
            if (httpContext.Request.URL == String.Empty)
                filePath = httpContext.Server.MapPath(MicroWebServer.DEFAULT_PAGE);
            else
                // get real file path in webroot directory
                filePath = httpContext.Server.MapPath(httpContext.Request.URL.TrimStart('/'));

            // set content type
            httpContext.Response.ContentType = httpContext.Server.MapContentType(Path.GetExtension(filePath));

            // content stream
            httpContext.Response.Stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            /*
            // read file content and set response body
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            TextReader tr = new StreamReader(fs);
            httpContext.Response.Body = tr.ReadToEnd();
            tr.Close();
            fs.Close();
            */ 
        }

        #endregion
    }
}
