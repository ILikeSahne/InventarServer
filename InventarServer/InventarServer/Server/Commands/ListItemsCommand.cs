using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    class ListItemsCommand : Command
    {
        public ListItemsCommand() : base("ListItems")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            string name = _helper.ReadString();
            var itemCollection = _u.Database.GetItemCollection(name);

            if (!SendPermissionMessage(_u, _helper, itemCollection.GetPermission()))
                return;

            var items = itemCollection.GetItems(_u);

            string json = JsonSerializer.Serialize(items);
            _helper.SendByteArray(Zip(json));
        }
    }
}
