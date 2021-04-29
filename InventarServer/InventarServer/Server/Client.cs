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
            SetupEncryption();
        }

        /// <summary>
        /// Setup RSA communication
        /// </summary>
        public void SetupEncryption()
        {
            rsaHelper = new RSAHelper(stream);
            RSAError e = rsaHelper.SetupServer();
            ASCIIEncoding en = new ASCIIEncoding();
            string message = en.GetString(rsaHelper.ReadByteArray());
            Console.WriteLine(message);
        }

        /// <summary>
        /// Stops the Thread for the Client
        /// </summary>
        public void Close()
        {
            clientThread.Abort();
            client.Close();
        }
    }
}
