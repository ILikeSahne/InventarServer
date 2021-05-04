using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class DatabaseTest
    {
        public DatabaseTest()
        {

        }

        public void CreateTestDatabase()
        {
            string name = "testdb";
            DatabaseManager dm = InventarServer.GetDatabase();
            DatabaseLocation loc = new DatabaseLocation(name, "C:/Users/ILikeSahne/AppData/Roaming/InventarServer/Databases/", 0);
            Error e = dm.AddDatabase(new Database(loc));
            if(!e)
            {
                e.PrintAllErrors();
            }
            Database d = dm.GetDatabase(name);
            d.AddEquipment(new Equipment(0, "12345", "99", "434", "29/04/2020", "Große Gurke", "ABC123", 50, 10, '€', "No", "BT8/1-55", "Bauteil 8 Lehrsaal 813B"));
        }
    }
}
