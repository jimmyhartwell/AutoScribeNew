using System;

namespace AutoScribeClient.ServerAccess
{
    /// <summary>
    /// An exception indicating that a requested ressource could not be found on the server
    /// </summary>
    public class ServerRessourceNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new server ressource not found exception
        /// </summary>
        /// <param name="action">The action that tried to access a ressource on the server</param>
        /// <param name="id">The id of the requested ressource</param>
        public ServerRessourceNotFoundException(string action, string id)
            : base("server ressource not found (" + action + " with id " + id + ")")
        {
            this.action = action;
            this.id = id;
        }

        private string action;
        private string id;

        /// <summary>
        /// The action that tried to access a ressource on the server
        /// </summary>
        public string Action { get => action; set => action = value; }
        /// <summary>
        /// The id of the requested ressource
        /// </summary>
        public string Id { get => id; set => id = value; }
    }
}
