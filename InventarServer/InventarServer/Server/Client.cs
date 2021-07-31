using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace InventarServer
{
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        private RSAHelper rsaHelper;
        private StreamHelper helper;

        public Client(TcpClient _client)
        {
            client = _client;
            stream = client.GetStream();
            Server.WriteLine("New Client from: {0}", client.Client.RemoteEndPoint.ToString());
            new Thread(ClientRoutine).Start();
        }

        private void ClientRoutine()
        {
            rsaHelper = new RSAHelper(stream);
            try
            {
                rsaHelper.SetupServer();
            } catch (Exception e)
            {
                Server.WriteLine("Error: {0}", e.ToString());
                Close();
            }
            helper = new StreamHelper(rsaHelper);
            string db = helper.ReadString();
            string email = helper.ReadString();
            string pw = helper.ReadString();
            Server.WriteLine("{0}, {1}, {2}", db, email, pw);
        }

        private void Close()
        {
            client.Close();
        }
    }
}
