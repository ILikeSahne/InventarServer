using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    /// <summary>
    /// Adds a new item to a database
    /// </summary>
    class AddNewUserCommand : Command
    {
        public AddNewUserCommand() : base("AddNewUser")
        { }

        public override void Execute(StreamHelper _helper, Client _c)
        {
            string db = _helper.ReadString();
            string name = _helper.ReadString();
            string pw = _helper.ReadString();
            DatabaseHelper dbHelper = new DatabaseHelper(db, name, pw);
            LoginError error = dbHelper.Login();
            if (error != LoginError.NONE)
            {
                _helper.SendString("Invalid Login!");
                return;
            }
            _helper.SendString("OK");
            string username = _helper.ReadString();
            string email = _helper.ReadString();
            string password = _helper.ReadString();
            DatabaseHelper helper = new DatabaseHelper(db, username, password);
            if (!helper.UsernameOk())
            {
                _helper.SendString("Invalid Username");
                return;
            }
            if (!helper.EmailOk(email))
            {
                _helper.SendString("Invalid Email");
                return;
            }
            if(!helper.PasswordOk())
            {
                _helper.SendString("Invalid Password");
                return;
            }
            if (!helper.AddUser(email))
            {
                _helper.SendString("Unknown Error");
                return;
            }
            _helper.SendString("OK");
        }
    }
}
