using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    /// <summary>
    /// Adds a new item to a database
    /// </summary>
    class AddNewItemCollectionCommand : Command
    {
        public AddNewItemCollectionCommand() : base("AddNewItemCollection")
        { }
        
        public override void Execute(StreamHelper _helper, Client _c)
        {
            string db = _helper.ReadString();
            string name = _helper.ReadString();
            string pw = _helper.ReadString();
            string itemCollectionName = _helper.ReadString();
            DatabaseHelper dbHelper = new DatabaseHelper(db, name, pw);
            LoginError error = dbHelper.Login();
            if (error != LoginError.NONE)
            {
                _helper.SendString("Invalid Login!");
                return;
            }

            if (string.IsNullOrWhiteSpace(itemCollectionName))
            {
                _helper.SendString("Item collection wrong");
            }

            dbHelper.AddItemCollection(itemCollectionName);

            _helper.SendString("OK");
        }
    }
}
