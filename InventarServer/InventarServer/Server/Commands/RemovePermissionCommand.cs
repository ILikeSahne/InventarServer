﻿using System;
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
            User target = new User(_u.Database, username, "");
            
            target.RemovePermission(permission);
        }
    }
}
