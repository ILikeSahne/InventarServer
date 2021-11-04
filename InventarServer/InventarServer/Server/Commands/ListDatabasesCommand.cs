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
        private readonly string[] mongodbDbNames =
        {
            "admin", "local", "config", "admin_user"
        };

        public ListDatabasesCommand() : base("ListDatabases")
        { }

        /// <summary>
        /// Sends a list of databases to the client
        /// </summary>
        public override void Execute(StreamHelper _helper, Client _c)
        {
            List<string> allDatabases = new DatabaseHelper().ListDatabases();
            List<string> databases = new List<string>();
            foreach(string name in allDatabases)
            {
                bool nameOk = true;
                foreach(string mongoDbName in mongodbDbNames)
                {
                    if(name.Equals(mongoDbName))
                    {
                        nameOk = false;
                        break;
                    }
                }
                if(nameOk)
                    databases.Add(name);
            }
            _helper.SendInt(databases.Count);
            foreach(string name in databases)
            {
                _helper.SendString(name);
            }
        }
    }
}
