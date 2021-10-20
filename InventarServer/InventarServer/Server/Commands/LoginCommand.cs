using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Login into a database
    /// </summary>
    class LoginCommand : Command
    {
        public LoginCommand() : base("Login")
        { }

        /// <summary>
        /// Login into a database
        /// </summary>
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
            } else
            {
                _c.LoggedIn = false;
                Server.WriteLine("Login failed: {0}, with: {1}:{2}", db, name, pw);
            }
            _helper.SendString(error.ToString());
        }

        /// <summary>
        /// Helper function to login
        /// </summary>
        /// <param name="_db">Database name</param>
        /// <param name="_name">User name</param>
        /// <param name="_pw">User password</param>
        /// <returns></returns>
        private LoginError Login(string _db, string _name, string _pw)
        {
            DatabaseHelper helper = new DatabaseHelper(_db, _name, _pw);
            return helper.Login();
        }
    }
}
