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

        private CommandManager cmdManager;

        private RSAHelper rsaHelper;
        private StreamHelper helper;

        public bool LoggedIn { get; set; }

        public Client(TcpClient _client, CommandManager _cmdManager)
        {
            client = _client;
            cmdManager = _cmdManager;
            stream = client.GetStream();

            LoggedIn = false;

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
            while (client.Connected)
            {
                string commandType = "";
                try
                {
                    commandType = helper.ReadString();
                } catch(Exception e)
                {
                    Close();
                }
                foreach (Command c in cmdManager.Commands)
                {
                    if (c.CMD.Equals(commandType))
                    {
                        c.Execute(helper, this);
                        break;
                    }
                }
            }
        }

        public void Close()
        {
            Server.WriteLine("Connection from {0} closed!", client.Client.RemoteEndPoint.ToString());
            client.Close();
        }
    }
}
