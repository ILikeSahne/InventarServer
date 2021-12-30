using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class AddNewUserCommand : Command
    {
        public AddNewUserCommand() : base("AddNewUser")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string email = _helper.ReadString();
            string username = _helper.ReadString();
            string password = _helper.ReadString();

            ValidateError e = _u.Database.AddUser(email, username, password);
            if (e != ValidateError.NONE)
            {
                _helper.SendString(e.ToString());
                return;
            }

            SendOKMessage(_helper);
        }
    }
}
