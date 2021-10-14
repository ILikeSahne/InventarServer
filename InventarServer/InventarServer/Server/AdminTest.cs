using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class AdminTest
    {
        public static string GenerateAdminUsersDatabase(string _email, string _username, string _password)
        {
            string name = "admin_user";
            Validator val = new Validator();
            if (!val.ValidateEmail(_email))
            {
                return "Email is wrong";
            }
            if (!val.ValidateUsername(_username))
            {
                return "Username is wrong";
            }
            if (!val.ValidatePassword(_password))
            {
                return "Password is wrong";
            }
            DatabaseHelper dbHelper = new DatabaseHelper(name, _username, _password);
            List<string> databases = dbHelper.ListDatabases();
            foreach (string db in databases)
            {
                if (db.ToLower().Equals(name.ToLower()))
                {
                    return "Databases already Exists";
                }
            }
            if (!dbHelper.AddUser(_email))
            {
                return "User already Exists";
            }
            return "OK";
        }

    }
}
