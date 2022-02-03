using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class ListItemImagesCommand : Command
    {
        public ListItemImagesCommand() : base("ListItemImages")
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

            SendOKMessage(_helper);

            List<byte[]> images = i.Bilder;

            _helper.SendInt(images.Count);
            foreach(byte[] img in images)
            {
                _helper.SendByteArray(img);
            }
        }
    }
}
