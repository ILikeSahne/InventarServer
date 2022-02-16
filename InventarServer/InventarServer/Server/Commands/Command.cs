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
            Commands.Add(new AddUserCommand());
            Commands.Add(new ListUserCommand());
            Commands.Add(new AddPermissionCommand());
            Commands.Add(new RemovePermissionCommand());
            Commands.Add(new AddItemCommand());
            Commands.Add(new ListItemCollectionsCommands());
            Commands.Add(new AddItemCollectionCommand());
            Commands.Add(new ListItemsCommand());
            Commands.Add(new RemoveItemCommand());
            Commands.Add(new RemoveItemCollectionCommand());
            Commands.Add(new ListItemImagesCommand());
            Commands.Add(new AddExcelItemsCommand());
            Commands.Add(new CopyItemCollectionCommand());
            Commands.Add(new GeneratePDFCommand());
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

        public bool SendPermissionMessage(User _u, StreamHelper _helper, bool _hasRights)
        {
            if (!_hasRights)
            {
                SendNoPermissionMessage(_helper);
                return false;
            }
            SendOKMessage(_helper);
            return true;
        }

        public bool SendPermissionMessage(User _u, StreamHelper _helper, string _perm)
        {
            return SendPermissionMessage(_u, _helper, _u.HasPermission(_perm));
        }

        public bool IsAdmin(User _u, StreamHelper _helper)
        {
            return SendPermissionMessage(_u, _helper, _u.IsAdmin());
        }

        public bool HasItemAddPermission(User _u, StreamHelper _helper)
        {
            return SendPermissionMessage(_u, _helper, _u.HasItemAddPermission());
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
