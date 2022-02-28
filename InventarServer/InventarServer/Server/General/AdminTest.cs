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
        public static void CreateAdminUserDatabase()
        {
            Database db = new Database("super_admins");
            if (db.Exists())
                return;
            db.CreateNewDatabase();
            string email = "Test@gmx.at";
            string username = "TestUser";
            string password = "Test123!!!";
            db.AddUser(email, username, password);

            User u = new User(db, username, password);
            u.AddPermission("admin");
        }
    }
}
