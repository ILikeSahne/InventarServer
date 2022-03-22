using MongoDB.Driver;
using System;
using System.IO;
using System.Net;

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

        public string IP { get; private set; }
        public int Port { get; private set; }

        /// <summary>
        /// Starts the database connection and the server
        /// </summary>
        public InventarServerMain()
        {
            MongoDB = new MongoClient("mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb");
            LoadIpAndPort();
            Server.WriteLine("Server starting on IP: {0}, on Port: {1}", IP, Port);
            Server = new Server(IP, Port);
        }

        public void LoadIpAndPort()
        {
            string filename = "settings.txt";
            if (!File.Exists(filename))
                File.WriteAllText(filename, "IP: 127.0.0.1\nPort: 10000");
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (s.StartsWith("IP: "))
                {
                    int len = "IP: ".Length;
                    s = s.Substring(len, s.Length - len);
                    IP = s;
                }
                if (s.StartsWith("Port: "))
                {
                    int len = "Port: ".Length;
                    s = s.Substring(len, s.Length - len);
                    Port = int.Parse(s);
                }
            }
        }
    }
}
