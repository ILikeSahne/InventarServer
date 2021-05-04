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
        public Error StartServer()
        {
            IPAddress addr;
            try
            {
                addr = Dns.GetHostAddresses(domain)[0];
            } 
            catch(Exception e)
            {
                return new Error(ErrorType.SERVER_ERROR, ServerErrorType.INVALID_DOMAIN_ERROR, e);
            }
            try
            {
                server = new TcpListener(addr, port);
                server.Start();
            }
            catch(Exception e)
            {
                return new Error(ErrorType.SERVER_ERROR, ServerErrorType.SETUP_ERROR, e);
            }
            return Error.NO_ERROR;
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
    enum ServerErrorType
    {
        NO_ERROR,
        INVALID_DOMAIN_ERROR,
        SETUP_ERROR
    }
}
