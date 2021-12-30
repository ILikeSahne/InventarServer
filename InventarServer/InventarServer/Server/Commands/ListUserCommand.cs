using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class ListUserCommand : Command
    {
        public ListUserCommand() : base("ListUser")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!CheckForAdmin(_u, _helper))
                return;
            
            Database d = _u.Database;
            List<UserData> users = d.GetUserData();
            _helper.SendInt(users.Count);
            foreach (UserData u in users)
            {
                _helper.SendString(u.ToJson());
            }
        }
    }
}
