using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    /// <summary>
    /// Adds a new item to a database
    /// </summary>
    class AddNewItemCommand : Command
    {
        public AddNewItemCommand() : base("AddNewItem")
        { }

        
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
            string json = _helper.ReadString();
            Item i = JsonSerializer.Deserialize<Item>(json);
            Server.WriteLine(i.ToString());
        }
    }
}
