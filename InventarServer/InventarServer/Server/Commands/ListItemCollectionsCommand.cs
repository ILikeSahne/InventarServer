using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    class ListItemCollectionsCommands : Command
    {
        public ListItemCollectionsCommands() : base("ListItemCollections")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            List<string> itemCollections = _u.Database.ListItemCollections(_u);
            _helper.SendInt(itemCollections.Count);
            foreach (string col in itemCollections)
            {
                _helper.SendString(col);
            }
        }
    }
}
