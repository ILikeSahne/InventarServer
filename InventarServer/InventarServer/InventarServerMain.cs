using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class InventarServerMain
    {
        public static InventarServerMain Instance { get; set; }

        public static void Main(string[] args)
        {
            Instance = new InventarServerMain();
        }

        public static MongoClient GetMongoDB()
        {
            return Instance.MongoDB;
        }

        public Server Server { get; }
        public MongoClient MongoDB { get; }

        public InventarServerMain()
        {
            MongoDB = new MongoClient("mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb");
            Server = new Server(10000);
        }
    }
}
