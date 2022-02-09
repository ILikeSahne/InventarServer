using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class ChangeItemImagesCommand : Command
    {
        public ChangeItemImagesCommand() : base("ChangeItemImages")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            string collectionName = _helper.ReadString();
            string id = _helper.ReadString();

            ItemCollection c = _u.Database.GetItemCollection(collectionName);

            if (c == null)
            {
                SendNoPermissionMessage(_helper);
                return;
            }

            if (!SendPermissionMessage(_u, _helper, c.GetPermission()))
                return;

            Item i = c.GetItem(_u, id);

            if (i == null)
            {
                SendNoPermissionMessage(_helper);
                return;
            }

            List<byte[]> images = new List<byte[]>();

            int amount = _helper.ReadInt();
            for(int j = 0; j < amount; j++)
            {
                images.Add(_helper.ReadByteArray());
            }

            i.Bilder = images;
            c.RemoveItem(i.ItemCollectionName, i.ID, _u);
            c.AddItem(i);
        }
    }
}
