using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class DatabaseHelper
    {
        private static string[] blacklistedDatabases =
        {
            "admin",
            "config",
            "local"
        };

        public static List<string> ListAllDatabases()
        {
            List<string> databases = new List<string>();
            using (var cursor = InventarServerMain.GetMongoDB().ListDatabases())
            {
                while (cursor.MoveNext())
                {
                    foreach (var db in cursor.Current)
                    {
                        databases.Add(db["name"].ToString());
                    }
                }
            }
            return databases;
        }

        public static List<string> ListUseableDatabases()
        {
            List<string> databases = new List<string>();
            foreach (string s in ListAllDatabases())
            {
                bool ok = true;
                foreach (string b in blacklistedDatabases)
                {
                    if (s.Equals(b))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                    databases.Add(s);
            }
            return databases;
        }
    }

    /// <summary>
    /// Possible login errors
    /// </summary>
    enum LoginError
    {
        NONE, WRONG_DATABASE, WRONG_USERNAME, WRONG_PASSWORD, UNKNOWN
    }
}
