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

            LoginError error = Login(db, name, pw);
            if (error == LoginError.NONE)
            {
                _c.LoggedIn = true;
                Server.WriteLine("Login to: {0}, with: {1}:{2}", db, name, pw);
                _helper.SendString("OK");
            } else
            {
                _c.LoggedIn = false;
                Server.WriteLine("Login failed: {0}, with: {1}:{2}", db, name, pw);
                _helper.SendString(error.ToString());
            }
        }

        private LoginError Login(string _db, string _name, string _pw)
        {
            DatabaseHelper helper = new DatabaseHelper(_pw, _name, _pw);
            return helper.Login();
        }
    }
}
