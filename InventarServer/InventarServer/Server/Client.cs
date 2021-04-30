using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace InventarServer
{
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        private Thread clientThread;

        private RSAHelper rsaHelper;

        /// <summary>
        /// Saves values
        /// Executes function Start
        /// </summary>
        /// <param name="_client">TCP Connection of the Client</param>
        public Client(TcpClient _client)
        {
            client = _client;
            stream = client.GetStream();
            InventarServer.WriteLine("New Client from: {0}", client.Client.RemoteEndPoint.ToString());
            Start();
        }

        /// <summary>
        /// Starts the Thread for the Client
        /// </summary>
        public void Start()
        {
            clientThread = new Thread(ClientRoutine);
            clientThread.Start();
        }

        /// <summary>
        /// Handles Communication beetween Server and Client
        /// </summary>
        public void ClientRoutine()
        {
            RSAError e = SetupEncryption();
            if(e != RSAError.NO_ERROR)
            {
                InventarServer.WriteLine("Couldn't setup RSA communication!");
                Close();
                return;
            }
            byte[] data = rsaHelper.ReadByteArray();
            CommandError cmdError = InventarServer.GetCommandManager().ExecuteCommand(data, rsaHelper);
            if (!cmdError)
            {
                cmdError.PrintError();
            }
            Close();
        }

        /// <summary>
        /// Setup RSA communication
        /// </summary>
        public RSAError SetupEncryption()
        {
            rsaHelper = new RSAHelper(stream);
            RSAError e = rsaHelper.SetupServer();
            if(e != RSAError.NO_ERROR)
            {
                return e;
            }
            return e;
        }

        /// <summary>
        /// Stops the Thread for the Client
        /// </summary>
        public void Close()
        {
            client.Close();
        }
    }
}
