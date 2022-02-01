using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class RemoveItemCommand : Command
    {
        public RemoveItemCommand() : base("RemoveItem")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            string collectionName = _helper.ReadString();
            string id = _helper.ReadString();

            ItemCollection c = _u.Database.GetItemCollection(collectionName);

            if (c == null)
                SendNoPermissionMessage(_helper);

            if (!SendPermissionMessage(_u, _helper, c.GetPermission()))
                return;

            if (!c.RemoveItem(collectionName, id, _u))
            {
                SendNoPermissionMessage(_helper);
                return;
            }

            SendOKMessage(_helper);
        }
    }
}
