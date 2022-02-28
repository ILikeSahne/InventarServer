using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace InventarServer
{
    class Client
    {
        private const string okResponse = "OK";

        private TcpClient client;
        private NetworkStream stream;

        private CommandManager cmdManager;

        private RSAHelper rsaHelper;
        private StreamHelper helper;

        /// <summary>
        /// Creates a new client and starts the routine on its own thread
        /// </summary>
        /// <param name="_client">Used to send/receive messages</param>
        /// <param name="_cmdManager">Handles command bases communication beetwen server and client</param>
        public Client(TcpClient _client, CommandManager _cmdManager)
        {
            client = _client;
            cmdManager = _cmdManager;
            stream = client.GetStream();

            Server.WriteLine("New Client from: {0}", client.Client.RemoteEndPoint.ToString());
            new Thread(ClientRoutine).Start();
        }

        /// <summary>
        /// Performs command based communication
        /// </summary>
        private void ClientRoutine()
        {
            try
            {
                rsaHelper = new RSAHelper(stream);
                try
                {
                    rsaHelper.SetupServer();
                }
                catch (Exception e)
                {
                    Server.WriteLine("Error: {0}", e.ToString());
                    Close();
                }

                helper = new StreamHelper(rsaHelper);
                string database = helper.ReadString();
                string username = helper.ReadString();
                string password = helper.ReadString();

                User u = new User(database, username, password);
                LoginError error = u.Login();
                helper.SendString(error.ToString());
                if (error != LoginError.NONE)
                {
                    if (u.Username == "")
                    {
                        string commandType = helper.ReadString();
                        if (commandType == "ListDatabases")
                            cmdManager.Commands[0].Call(u, helper, this);
                    }
                    Close();
                }

                while (client.Connected)
                {
                    try
                    {
                        string commandType = helper.ReadString();
                        foreach (Command c in cmdManager.Commands)
                        {
                            if (c.CMD.Equals(commandType))
                            {
                                c.Call(u, helper, this);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Close();
                    }
                }
            }
            catch(Exception e)
            {
                Server.WriteLine("Unexpected Error: {0}", e.ToString());
            }
        }

        /// <summary>
        /// Closes the connection from the client
        /// </summary>
        public void Close()
        {
            Server.WriteLine("Connection from {0} closed!", client.Client.RemoteEndPoint.ToString());
            client.Close();
        }
    }
}
