using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class ListDatabasesCommand : Command
    {
        public ListDatabasesCommand() : base("ListDatabases")
        { }

        public override void Execute(StreamHelper _helper, Client _c)
        {
            List<string> databases = new DatabaseHelper().ListDatabases();
            _helper.SendInt(databases.Count);
            foreach(string name in databases)
            {
                _helper.SendString(name);
            }
        }
    }
}
