using MongoDB.Driver;
using System;

namespace InventarServer
{
    class InventarServerMain
    {
        private static Server server;
        private static MongoClient mongodb;

        static void Main(string[] args)
        {
            mongodb = new MongoClient("mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb");
            //server = new Server(10000);
        }
    }
}
