using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class RemoveUserCommand : Command
    {
        public RemoveUserCommand() : base("RemoveUser")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string username = _helper.ReadString();
            User target = new User(_u.Database, username, "");
            var user = target.GetUser();

            if(user.user == null)
            {
                _helper.SendString("User does not exist");
                return;
            }

            var userCol = _u.Database.GetCollection("users");
            if (user.isEmail)
                userCol.RemoveOne("email", username);
            if (user.isUsername)
                userCol.RemoveOne("username", username);

            SendOKMessage(_helper);
        }
    }
}
