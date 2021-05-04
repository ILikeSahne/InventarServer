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
            Error e = SetupEncryption();
            if(!e)
            {
                InventarServer.WriteLine("Couldn't setup RSA communication!");
                Close();
                return;
            }
            byte[] data = rsaHelper.ReadByteArray();
            Error cmdError = InventarServer.GetCommandManager().ExecuteCommand(data, rsaHelper);
            if (!cmdError)
            {
                cmdError.PrintAllErrors();
            }
            Close();
        }

        /// <summary>
        /// Setup RSA communication
        /// </summary>
        public Error SetupEncryption()
        {
            rsaHelper = new RSAHelper(stream);
            Error e = rsaHelper.SetupServer();
            if(!e)
            {
                return new Error(ErrorType.CLIENT_ERROR, RSAError.CONNECTION_ERROR, e);
            }
            return Error.NO_ERROR;
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
