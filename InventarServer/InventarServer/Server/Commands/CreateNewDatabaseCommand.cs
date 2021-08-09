using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class CreateNewDatabaseCommand : Command
    {
        public CreateNewDatabaseCommand() : base("CreateNewDatabase")
        { }

        public override void Execute(StreamHelper _helper, Client _c)
        {
            Console.WriteLine("Admin Login: " + AdminLogin(_helper));
            //if(AdminLogin(_helper))
            //    CreateDatabase(_helper);
        }

        public bool AdminLogin(StreamHelper _helper)
        {
            string adminUsername = _helper.ReadString();
            string adminPassword = _helper.ReadString();

            DatabaseHelper helper = new DatabaseHelper("admin_user", adminUsername, adminPassword);

            return helper.Login();
        }

        public void CreateDatabase(StreamHelper _helper)
        {
            string databaseName = _helper.ReadString();
            string adminEmail = _helper.ReadString();
            string adminUsername = _helper.ReadString();
            string adminPassword = _helper.ReadString();

        }
    }
}
