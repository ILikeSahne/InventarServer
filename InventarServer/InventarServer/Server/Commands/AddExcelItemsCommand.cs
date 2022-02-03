using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    class AddExcelItemsCommand : Command
    {
        public AddExcelItemsCommand() : base("AddExcelItems")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!HasItemAddPermission(_u, _helper))
                return;

            string name = _helper.ReadString();

            ItemCollection c = _u.Database.GetItemCollection(name);

            if (c == null) {
                SendNoPermissionMessage(_helper);
                return;
            }

            if (!SendPermissionMessage(_u, _helper, c.GetPermission()))
                return;

            string filename = DateTime.Now.ToString().Replace('/', '_').Replace(' ', '_').Replace(':', '_') + ".xlsl";

            byte[] data = _helper.ReadByteArray();
            File.WriteAllBytes(filename, data);

            List<Item> items = ExcelHelper.LoadExcel(name, filename);
            _helper.SendInt(items.Count);
            foreach (Item i in items)
            {
                _u.Database.AddItem(i);
            }
        }
    }
}
