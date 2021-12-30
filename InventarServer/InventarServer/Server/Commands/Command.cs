using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Registers and executes commands
    /// </summary>
    class CommandManager
    {
        /// <summary>
        /// Stores commands
        /// </summary>
        public List<Command> Commands { get; }

        /// <summary>
        /// Registers all commands
        /// </summary>
        public CommandManager()
        {
            Commands = new List<Command>();
            Commands.Add(new ListDatabasesCommand());
            Commands.Add(new LoginCommand());
            Commands.Add(new CreateNewDatabaseCommand());
            Commands.Add(new AddNewUserCommand());
            Commands.Add(new ListUserCommand());
        }
    }

    /// <summary>
    /// Stores command data and execute behaviour
    /// </summary>
    class Command
    {
        /// <summary>
        /// Stores command name
        /// </summary>
        public string CMD { get; }

        /// <summary>
        /// Registers command
        /// </summary>
        /// <param name="_command">Command name</param>
        public Command(string _command)
        {
            CMD = _command;
            Server.WriteLine("Loading Command: {0}", CMD);
        }

        /// <summary>
        /// Gets called when the corresponding command gets requested from the client
        /// </summary>
        /// <param name="_helper">Allows you to send an receive messages from the client</param>
        /// <param name="_c">Holds data about the client</param>
        public void Call(User _u, StreamHelper _helper, Client _c)
        {
            Execute(_u, _helper, _c);
        }

        public virtual void Execute(User _u, StreamHelper _helper, Client _c)
        {

        }

        public bool CheckForAdmin(User _u, StreamHelper _helper)
        {
            if (!_u.IsAdmin())
            {
                SendNoPermissionMessage(_helper);
                return false;
            }
            SendOKMessage(_helper);
            return true;
        }

        public void SendNoPermissionMessage(StreamHelper _helper)
        {
            _helper.SendString("Not enough Permissions!");
        }

        public void SendOKMessage(StreamHelper _helper)
        {
            _helper.SendString("OK");
        }
    }
}
