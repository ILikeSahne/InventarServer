using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Some Helper functions to make life easier
    /// </summary>
    class AdminTest
    {
        /// <summary>
        /// Generates a new admin user database
        /// </summary>
        /// <param name="_email">Admin email</param>
        /// <param name="_username">Admin username</param>
        /// <param name="_password">Admin password</param>
        /// <returns>An error string, if the database creation failed</returns>
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

        public static void AddItem(string _db, string _username, string _password, string _itemCollection)
        {
            Item i = new Item();
            DatabaseHelper dbHelper = new DatabaseHelper(_db, _username, _password);
            dbHelper.AddItem(i, _itemCollection);
        }

        public static void DeleteItem(string _db, string _username, string _password, string _itemCollection)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(_db, _username, _password);
            Item i = dbHelper.ListItems(_itemCollection)[0];
            dbHelper.DeleteItem(i.ID, _itemCollection);
        }
    }
}
