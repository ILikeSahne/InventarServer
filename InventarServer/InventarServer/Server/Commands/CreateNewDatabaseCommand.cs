using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InventarServer
{
    /// <summary>
    /// Creates a new database
    /// </summary>
    class CreateNewDatabaseCommand : Command
    {
        public CreateNewDatabaseCommand() : base("CreateNewDatabase")
        { }

        /// <summary>
        /// Creates the database, if the admin password is correct and the database doesn't already exist
        /// </summary>
        public override void Execute(StreamHelper _helper, Client _c)
        {
            LoginError error = AdminLogin(_helper);
            if (error != LoginError.NONE)
            {
                _helper.SendString(error.ToString());
                _c.Close();
                return;
            }
            _helper.SendString("OK");
            CreateDatabase(_helper);
        }

        /// <summary>
        /// Login as Admin User
        /// </summary>
        /// <param name="_helper">Allows you to send an receive messages from the client</param>
        /// <returns>An error, if the admin login wasn't successful</returns>
        public LoginError AdminLogin(StreamHelper _helper)
        {
            string adminUsername = _helper.ReadString();
            string adminPassword = _helper.ReadString();

            DatabaseHelper helper = new DatabaseHelper("admin_user", adminUsername, adminPassword);

            return helper.Login();
        }

        /// <summary>
        /// Creates the database
        /// </summary>
        /// <param name="_helper"></param>
        public void CreateDatabase(StreamHelper _helper)
        {
            string databaseName = _helper.ReadString();
            string adminEmail = _helper.ReadString();
            string adminUsername = _helper.ReadString();
            string adminPassword = _helper.ReadString();

            Validator val = new Validator();
            if(!val.ValidateEmail(adminEmail))
            {
                _helper.SendString("Email is wrong");
                return;
            }
            if (!val.ValidateUsername(adminUsername))
            {
                _helper.SendString("Username is wrong");
                return;
            }
            if (!val.ValidatePassword(adminPassword))
            {
                _helper.SendString("Password is wrong");
                return;
            }
            DatabaseHelper dbHelper = new DatabaseHelper(databaseName, adminUsername, adminPassword);
            List<string> databases = dbHelper.ListDatabases();
            foreach(string db in databases)
            {
                if(db.ToLower().Equals(databaseName.ToLower()))
                {
                    _helper.SendString("Databases already Exists");
                    return;
                }
            }
            if(!dbHelper.AddUser(adminEmail))
            {
                _helper.SendString("User already Exists");
                return;
            }
            _helper.SendString("OK");
        }
    }
}
