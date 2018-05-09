using System;
using System.Net;

namespace AutoScribeClient.ServerAccess
{
    /// <summary>
    /// An exception acessing the server or during the server access
    /// </summary>
    public class ServerAccessException : Exception
    {
        /// <summary>
        /// Creates a new server access exception
        /// </summary>
        /// <param name="status">The HTTP status code of the response</param>
        public ServerAccessException(HttpStatusCode status)
            :base("Exception accessing the server (" + status + ")")
        {
            this.status = status;
        }

        private HttpStatusCode status;

        /// <summary>
        /// The HTTP status code of the response that caused the exception
        /// </summary>
        public HttpStatusCode Status { get => status; set => status = value; }
    }
}
