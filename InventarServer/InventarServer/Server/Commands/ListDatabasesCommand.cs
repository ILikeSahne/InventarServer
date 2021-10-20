using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Lists all Databases
    /// </summary>
    class ListDatabasesCommand : Command
    {
        public ListDatabasesCommand() : base("ListDatabases")
        { }

        /// <summary>
        /// Sends a list of databases to the client
        /// </summary>
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
