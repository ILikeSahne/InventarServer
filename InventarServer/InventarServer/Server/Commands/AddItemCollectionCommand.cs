using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class AddItemCollectionCommand : Command
    {
        public AddItemCollectionCommand() : base("AddItemCollection")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string name = _helper.ReadString().Trim();
            string permission = _helper.ReadString();

            List<string> collections = _u.Database.ListItemCollections();
            foreach(string s in collections)
            {
                if (name == s)
                {
                    _helper.SendString("Collection already exists!");
                    return;
                }
            }

            Collection c = _u.Database.GetCollection("items");
            c.AddOne(ItemCollection.CreateNew(name, permission));

            SendOKMessage(_helper);
        }
    }
}
