using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class InventarServerMain
    {
        /// <summary>
        /// Used to get from static to public
        /// </summary>
        public static InventarServerMain Instance { get; set; }

        /// <summary>
        /// Starts the Program
        /// </summary>
        public static void Main(string[] args)
        {
            ExcelHelper.Setup();
            Instance = new InventarServerMain();
            // AdminTest.DeleteItem("TestDB", "TestUser2", "Test123!!!", "Master");
            AdminTest.CreateAdminUserDatabase();
            /*List<Item> items = new List<Item>();
            Item i = new Item();
            i.Anlage = "312002794672";
            i.Anlagenbezeichnung = "Labortisch für CAN-System";
            i.BuchWert = 0;
            for(int j = 0; j < 10; j++)
                items.Add(i);*/
            //byte[] pdf = GeneratePDFCommand.generateAbschreibungsPDF("C:/Users/ILike/Desktop/Diplom/Server/InventarServer/InventarServer/InventarServer/bin/Debug/netcoreapp3.1/DocumentTypes/ABSCHREIBUNG.pdf", items);
            //File.WriteAllBytes("C:/Users/ILike/Downloads/test.pdf", pdf);
        }

        /// <summary>
        /// Returns the MongoClient
        /// </summary>
        /// <returns>The MongoClient</returns>
        public static MongoClient GetMongoDB()
        {
            return Instance.MongoDB;
        }

        /// <summary>
        /// Server
        /// </summary>
        public Server Server { get; }

        /// <summary>
        /// MongoDB database
        /// </summary>
        public MongoClient MongoDB { get; }

        /// <summary>
        /// Starts the database connection and the server
        /// </summary>
        public InventarServerMain()
        {
            MongoDB = new MongoClient("mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb");
            Server = new Server(10000);
        }
    }
}
