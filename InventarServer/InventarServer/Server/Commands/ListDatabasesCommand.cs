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
            List<string> databases = new List<string>();
            using (var cursor = InventarServerMain.GetMongoDB().ListDatabases())
            {
                while (cursor.MoveNext())
                {
                    foreach (var db in cursor.Current)
                    {
                        databases.Add(db["name"].ToString());
                    }
                }
            }
            _helper.SendInt(databases.Count);
            foreach(string name in databases)
            {
                _helper.SendString(name);
            }
        }
    }
}
