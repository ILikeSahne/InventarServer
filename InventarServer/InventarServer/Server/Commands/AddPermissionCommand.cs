using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class AddPermissionCommand : Command
    {
        public AddPermissionCommand() : base("AddPermission")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string username = _helper.ReadString();
            string permission = _helper.ReadString();
            AddPermission(_u.Database, username, permission);
        }

        public void AddPermission(Database _d, string _username, string _permission)
        {
            Collection userCol = _d.GetCollection("users");

            var user = userCol.FindOne("username", _username);
            var perms = user.GetValue("permissions").AsBsonArray;
            perms.Add(_permission);

            userCol.UpdateEntry("username", _username, user);
        }

    }
}
