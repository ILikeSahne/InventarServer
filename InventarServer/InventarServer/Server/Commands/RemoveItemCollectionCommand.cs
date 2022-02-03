using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class RemoveItemCollectionCommand : Command
    {
        public RemoveItemCollectionCommand() : base("RemoveItemCollection")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            string collectionName = _helper.ReadString();
            
            ItemCollection c = _u.Database.GetItemCollection(collectionName);

            if (c == null)
            {
                SendNoPermissionMessage(_helper);
                return;
            }

            if (!IsAdmin(_u, _helper))
                return;

            c.RemoveCollection(collectionName);

            SendOKMessage(_helper);
        }
    }
}
