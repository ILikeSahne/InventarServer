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
            string name = _helper.ReadString();
            string pw = _helper.ReadString();

            Login(db, name, pw);

            _c.LoggedIn = true;

            Server.WriteLine("Login to: {0}, with: {1}:{2}", db, name, pw);
        }

        private void Login(string _db, string _name, string _pw)
        {

        }
    }
}
