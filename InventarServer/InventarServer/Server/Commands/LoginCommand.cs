using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class LoginCommand : Command
    {
        public LoginCommand() : base("Login")
        { }

        public override void Execute(StreamHelper _helper, Client _c)
        {
            string db = _helper.ReadString();
            string email = _helper.ReadString();
            string pw = _helper.ReadString();

            _c.LoggedIn = true;

            Server.WriteLine("Login to: {0}, with: {1}:{2}", db, email, pw);
        }

    }
}
