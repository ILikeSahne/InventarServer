using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace InventarServer
{
    class IServer
    {
        private TcpListener server;
        private string domain;
        private int port;

        private Thread serverThread;
        private List<Client> clients;

        /// <summary>
        /// Saves values
        /// </summary>
        /// <param name="_domain">Domain of the Server, can also be an IP-Adress</param>
        /// <param name="_port">Port of the Server</param>
        public IServer(string _domain, int _port)
        {
            domain = _domain;
            port = _port;
        }

        /// <summary>
        /// Starts the Server
        /// </summary>
        /// <returns>Returns an Error if the Server can't be started</returns>
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
                server.Start();
                return new ServerError(ServerErrorType.NO_ERROR, null);
            }
            catch(Exception e)
            {
                return new ServerError(ServerErrorType.SETUP_ERROR, e);
            }
        }

        /// <summary>
        /// Starts the Server Routine - the Server accepts Client connections
        /// </summary>
        public void StartServerRoutine()
        {
            clients = new List<Client>();
            serverThread = new Thread(ServerRoutine);
            serverThread.Start();
        }

        /// <summary>
        /// Accepts new Clients, until the Server is stopped
        /// </summary>
        private void ServerRoutine()
        {
            while (serverThread.IsAlive)
            {
                InventarServer.WriteLine("Waiting for Client...");
                TcpClient client = server.AcceptTcpClient();
                clients.Add(new Client(client));
            }
        }

        /// <summary>
        /// Stops the Server Routine - the Server stops accepts Client connections
        /// </summary>
        public void StopServerRoutine()
        {
            serverThread.Abort();
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
            InventarServer.WriteLine("ServerError: {0}", ToString());
            StackFrame stackFrame = new StackFrame(1, true);
            string filename = stackFrame.GetFileName();
            int line = stackFrame.GetFileLineNumber();
            string method = stackFrame.GetMethod().ToString();
            InventarServer.WriteLine("{0}:{1}, {2}", Path.GetFileName(filename), line, method);
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
