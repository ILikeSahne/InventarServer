using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class ListDatabasesCommand : Command
    {
        public ListDatabasesCommand() : base("ListDatabases")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            var databases = DatabaseHelper.ListUseableDatabases();
            _helper.SendInt(databases.Count);
            foreach (string d in databases)
                _helper.SendString(d);
        }
    }
}
