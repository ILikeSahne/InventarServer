using System;
using System.Collections.Generic;
using System.Text.Json;

namespace InventarServer
{
    class EditItemCommand : Command
    {
        public EditItemCommand() : base("EditItem")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!HasItemAddPermission(_u, _helper))
                return;

            string json = _helper.ReadString();
            Item newItem = JsonSerializer.Deserialize<Item>(json);

            ItemCollection itemCollection = _u.Database.GetItemCollection(newItem.ItemCollectionName);

            if (!SendPermissionMessage(_u, _helper, itemCollection.GetPermission()))
                return;

            Item oldItem = itemCollection.GetItem(_u, newItem.ID);
            newItem.Bilder = oldItem.Bilder;

            itemCollection.RemoveItem(oldItem.ItemCollectionName, oldItem.ID, _u);
            itemCollection.AddItem(newItem);

            SendOKMessage(_helper);
        }

    }
}
