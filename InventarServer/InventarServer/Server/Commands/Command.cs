using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class CommandManager
    {
        public List<Command> Commands { get; }

        public CommandManager()
        {
            Commands = new List<Command>();
            Commands.Add(new LoginCommand());
            Commands.Add(new ListDatabasesCommand());
            Commands.Add(new CloseCommand());
        }
    }

    class Command
    {
        public string CMD { get; }

        public Command(string _command)
        {
            CMD = _command;
            Server.WriteLine("Loading Command: {0}", CMD);
        }

        public virtual void Execute(StreamHelper _helper, Client _c)
        { }
    }
}
