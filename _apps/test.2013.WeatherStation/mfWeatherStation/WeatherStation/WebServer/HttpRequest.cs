using System;
using System.Net.Sockets;
using Microsoft.SPOT;
using System.Text;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Class for HTTP request
    /// </summary>
    public class HttpRequest : HttpBase
    {
        // size of request buffer
        private const int BUFFER_REQUEST_SIZE = 1024;

        /// <summary>
        /// HTTP request method
        /// </summary>
        public string HttpMethod { get; internal set; }

        /// <summary>
        /// URL request
        /// </summary>
        public string URL { get; internal set; }

        /// <summary>
        /// HTTP request protocol
        /// </summary>
        public string HttpProtocol { get; internal set; }

        /// <summary>
        /// HTTP query string
        /// </summary>
        public NameValueCollection QueryString { get; internal set; }

        /// <summary>
        /// HTTP form data
        /// </summary>
        public NameValueCollection Form { get; internal set; }

        /// <summary>
        /// Cnstructor
        /// </summary>
        public HttpRequest()
            : base()
        {
            this.QueryString = new NameValueCollection();
            this.Form = new NameValueCollection();
        }

        /// <summary>
        /// Execute parsing of a request 
        /// </summary>
        /// <param name="requestSocket">Socket bind to request for parsing</param>
        internal static HttpRequest Parse(Socket requestSocket)
        {
            byte[] buffer = new byte[BUFFER_REQUEST_SIZE];
            // TODO : evaluate use Poll for receiving alla bytes

            int bytesRead = 0;
            do
            {
                //int bytesRead = requestSocket.Receive(buffer);
                bytesRead += requestSocket.Receive(buffer, bytesRead, BUFFER_REQUEST_SIZE - bytesRead, SocketFlags.None);
            } while (requestSocket.Available > 0);

            // TODO : evaluate elaboration on bytes and not get all string result
            string request = new String(Encoding.UTF8.GetChars(buffer));

            HttpRequest httpRequest = new HttpRequest();
            
            // split request lines on line feed
            string[] lines = request.Split(LF);

            int i = 0;
            // trim request line on carriage return
            lines[i] = lines[i].TrimEnd(CR);

            // process request line (method, url, protocol version)
            string[] requestLineTokens = lines[i].Split(REQUEST_LINE_SEPARATOR);
            // method
            httpRequest.HttpMethod = requestLineTokens[0];
            // url, find start of query string (if exists)
            int idxQueryString = requestLineTokens[1].IndexOf(QUERY_STRING_SEPARATOR);
            httpRequest.URL = (idxQueryString != -1) ? 
                requestLineTokens[1].Substring(0, idxQueryString).Trim('/') : requestLineTokens[1].Trim('/');
            
            // protocol
            httpRequest.HttpProtocol = requestLineTokens[2];

            // parsing query string
            if (idxQueryString != -1)
            {
                string queryString = requestLineTokens[1].Substring(idxQueryString + 1);
                if (queryString != String.Empty)
                {
                    string[] queryStringParams = queryString.Split(QUERY_STRING_PARAMS_SEPARATOR);
                    foreach (string queryStringParam in queryStringParams)
                    {
                        string[] queryStringParamTokens = queryStringParam.Split(QUERY_STRING_VALUE_SEPARATOR);
                        string queryStringParamValue = null;
                        // if there is key-value pair
                        if (queryStringParamTokens.Length == 2)
                            queryStringParamValue = queryStringParamTokens[1];

                        httpRequest.QueryString.Add(queryStringParamTokens[0], HttpServerUtility.HtmlDecode(queryStringParamValue));
                    }
                }
            }

            // next line (header start)
            i++;
            // trim end carriage return of each line
            lines[i] = lines[i].TrimEnd(CR);

            // headers end with empty string
            while (lines[i] != String.Empty)
            {
                int separatorIndex = lines[i].IndexOf(HEADER_VALUE_SEPARATOR);

                if (separatorIndex != -1)
                    httpRequest.Headers.Add(lines[i].Substring(0, separatorIndex), lines[i].Substring(separatorIndex + 1).Trim());

                i++;
                // trim end carriage return of each line
                lines[i] = lines[i].TrimEnd(CR);
            }

            // next line (body start)
            i++;

            // content length specified
            if (httpRequest.Headers.ContainsKey("Content-Length"))
            {
                httpRequest.Body = lines[i].TrimEnd(CR).Substring(0, Convert.ToInt32(httpRequest.Headers["Content-Length"].ToString()));
            }

            // fill form parameters collection
            if ((httpRequest.Headers["Content-Type"] != null) && 
                (httpRequest.Headers["Content-Type"].StartsWith("application/x-www-form-urlencoded")))
            {
                string[] formParams = httpRequest.Body.Split(FORM_PARAMS_SEPARATOR);
                foreach (string formParam in formParams)
                {
                    string[] formParamTokens = formParam.Split(FORM_VALUE_SEPARATOR);
                    string formParamValue = null;
                    // if there is key-value pair
                    if (formParamTokens.Length == 2)
                        formParamValue = formParamTokens[1];

                    httpRequest.Form.Add(formParamTokens[0], HttpServerUtility.HtmlDecode(formParamValue));
                }
            }
            
            return httpRequest;
        }
    }
}
