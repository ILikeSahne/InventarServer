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
    class ListItemCollectionNamesCommand : Command
    {

        public ListItemCollectionNamesCommand() : base("ListItemCollectionNames")
        { }

        /// <summary>
        /// Sends a list of databases to the client
        /// </summary>
        public override void Execute(StreamHelper _helper, Client _c)
        {
            string db = _helper.ReadString();
            string name = _helper.ReadString();
            string pw = _helper.ReadString();
            DatabaseHelper dbHelper = new DatabaseHelper(db, name, pw);
            LoginError error = dbHelper.Login();
            if (error != LoginError.NONE)
            {
                _helper.SendString("Invalid Login!");
                return;
            }
            _helper.SendString("OK");

            List<string> itemCollections = new DatabaseHelper(db, name, pw).ListItemCollections();
            _helper.SendInt(itemCollections.Count);
            foreach(string itemCollectionName in itemCollections)
            {
                _helper.SendString(itemCollectionName);
            }
        }
    }
}
