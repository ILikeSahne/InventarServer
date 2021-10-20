using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace InventarServer
{
    /// <summary>
    /// Server, handles new clients
    /// </summary>
    class Server
    {
        private TcpListener server;
        private Thread serverThread;

        private CommandManager cmdManager;

        /// <summary>
        /// Starts the server on a specific domain and port
        /// </summary>
        /// <param name="_domain">Domain of the server</param>
        /// <param name="_port">Port of the server</param>
        public Server(string _domain, int _port)
        {
            StartServer(_domain, _port);
        }

        /// <summary>
        /// Starts the server on localhost (127.0.0.1)
        /// </summary>
        /// <param name="_port">Port of the server</param>
        public Server(int _port)
        {
            StartServer("127.0.0.1", _port);
        }

        /// <summary>
        /// Starts the server on a specific domain and port, also starts a thread for the serv routine
        /// </summary>
        /// <param name="_domain">Domain of the server</param>
        /// <param name="_port">Port of the server</param>
        private void StartServer(string _domain, int _port)
        {
            cmdManager = new CommandManager();
            IPAddress addr = Dns.GetHostAddresses(_domain)[0];
            server = new TcpListener(addr, _port);
            server.Start();
            serverThread = new Thread(ServerRoutine);
            serverThread.Start();
        }

        /// <summary>
        /// Server routine, accepts new clients
        /// </summary>
        private void ServerRoutine()
        {
            while (serverThread.IsAlive)
            {
                WriteLine("Waiting for Client...");
                new Client(server.AcceptTcpClient(), cmdManager);
            }
        }

        /// <summary>
        /// Writes a line to the console (only in DEBUG mode)
        /// </summary>
        /// <param name="_s">Message to send</param>
        /// <param name="_args">Parameters of the message</param>
        public static void WriteLine(string _s, params object[] _args)
        {
            #if DEBUG
                Console.WriteLine(_s, _args);
            #endif
        }

    }
}
