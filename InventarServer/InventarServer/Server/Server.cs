using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace InventarServer
{
    class Server
    {
        private TcpListener server;
        private Thread serverThread;

        private CommandManager cmdManager;

        public Server(string _domain, int _port)
        {
            StartServer(_domain, _port);
        }

        public Server(int _port)
        {
            StartServer("127.0.0.1", _port);
        }

        private void StartServer(string _domain, int _port)
        {
            cmdManager = new CommandManager();
            IPAddress addr = Dns.GetHostAddresses(_domain)[0];
            server = new TcpListener(addr, _port);
            server.Start();
            serverThread = new Thread(ServerRoutine);
            serverThread.Start();
        }

        private void ServerRoutine()
        {
            while (serverThread.IsAlive)
            {
                WriteLine("Waiting for Client...");
                new Client(server.AcceptTcpClient(), cmdManager);
            }
        }

        public static void WriteLine(string _s, params object[] _args)
        {
#if DEBUG
            Console.WriteLine(_s, _args);
#endif
        }

    }
}
