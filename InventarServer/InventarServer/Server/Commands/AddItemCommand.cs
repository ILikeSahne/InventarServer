using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    class AddItemCommand : Command
    {
        public AddItemCommand() : base("AddItem")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!HasItemAddPermission(_u, _helper))
                return;

            string json = _helper.ReadString();
            Item i = JsonSerializer.Deserialize<Item>(json);
            i.GenerateID();
            _u.Database.AddItem(i);

            json = JsonSerializer.Serialize(i);
            _helper.SendString(json);
        }
    }
}
