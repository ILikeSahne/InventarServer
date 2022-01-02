using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class RemovePermissionCommand : Command
    {
        public RemovePermissionCommand() : base("RemovePermission")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string username = _helper.ReadString();
            string permission = _helper.ReadString();
            RemovePermission(_u.Database, username, permission);
        }

        public void RemovePermission(Database _d, string _username, string _permission)
        {
            Collection userCol = _d.GetCollection("users");

            var user = userCol.FindOne("username", _username);
            var perms = user.GetValue("permissions").AsBsonArray;
            perms.Remove(_permission);

            userCol.UpdateEntry("username", _username, user);
        }

    }
}
