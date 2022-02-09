using System.Collections.Generic;

namespace InventarServer
{
    class CopyItemCollectionCommand : Command
    {
        public CopyItemCollectionCommand() : base("CopyItemCollection")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string nameToCopy = _helper.ReadString().Trim();
            string newName = _helper.ReadString().Trim();
            string permission = _helper.ReadString();

            List<string> collections = _u.Database.ListItemCollections();
            foreach (string s in collections)
            {
                if (newName == s)
                {
                    _helper.SendString("Collection already exists!");
                    return;
                }
            }

            var items = _u.Database.GetItemCollection(nameToCopy);
            if (items == null)
            {
                _helper.SendString("Item Collection doesn't exist!");
                return;
            }

            var newItems = items.Clone(newName, permission);
            _u.Database.GetCollection("items").AddOne(newItems);

            SendOKMessage(_helper);
        }
    }
}
