using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace InventarServer
{
    class Server
    {
        private TcpListener server;
        private string domain;
        private int port;

        /// <summary>
        /// Saves values
        /// </summary>
        /// <param name="_domain">Domain of the Server, can also be an IP-Adress</param>
        /// <param name="_port">Port of the Server</param>
        public Server(string _domain, int _port)
        {
            domain = _domain;
            port = _port;
        }

        /// <summary>
        /// Starts the Server
        /// </summary>
        /// <returns>Is true if there was no Error, if there is one it returns the Error and the Exception</returns>
        public ServerError StartServer()
        {
            IPAddress addr;
            try
            {
                addr = Dns.GetHostAddresses(domain)[0];
            } 
            catch(Exception e)
            {
                return new ServerError(ServerErrorType.INVALID_DOMAIN_ERROR, e);
            }
            try
            {
                server = new TcpListener(addr, port);
                return new ServerError(ServerErrorType.NO_ERROR, null);
            }
            catch(ArgumentOutOfRangeException e)
            {
                return new ServerError(ServerErrorType.SETUP_ERROR, e);
            }
        }
    }

    class ServerError
    {
        /// <summary>
        /// Type of Error
        /// </summary>
        public ServerErrorType Type { get; }
        /// <summary>
        /// Thrown Exception
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Saves values
        /// </summary>
        /// <param name="_type">Type of Error</param>
        /// <param name="_e">Thrown Exception</param>
        public ServerError(ServerErrorType _type, Exception _e)
        {
            Type = _type;
            Exception = _e;
        }

        /// <summary>
        /// Writes the Error to the Console (only when in DEBUG mode)
        /// </summary>
        public void PrintError()
        {
            InventarServer.WriteLine(ToString());
        }

        /// <summary>
        /// Returns the Error as a String:
        ///     "TypeOfError: ExceptionMessage"
        /// </summary>
        /// <returns>"TypeOfError: ExceptionMessage"</returns>
        public override string ToString()
        {
            if (Exception != null)
                return Type + ": " + Exception.Message;
            else
                return Type.ToString();
        }

        /// <summary>
        /// Returns true if there was no Error, otherwise it returns false
        /// </summary>
        /// <param name="e">ServerError</param>
        public static implicit operator bool(ServerError e)
        {
            return e.Type == ServerErrorType.NO_ERROR;
        }
    }

    enum ServerErrorType
    {
        NO_ERROR,
        INVALID_DOMAIN_ERROR,
        SETUP_ERROR
    }
}
