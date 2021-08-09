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
        private const string pepper = "h8xGOPtOmwCPSqnSYjcNVJc1tzpAyvZr";

        public string DB { get; }
        public string Name { get; }
        public string Password { get; }

        public DatabaseHelper(string _password)
        {
            Password = _password;
        }

        public DatabaseHelper(string _db, string _name, string _password)
        {
            DB = _db;
            Name = _name;
            Password = _password;
        }

        public bool UserExists()
        {
            var users = GetCollection("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", Name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", Password);

            var user = users.Find(filter).FirstOrDefault();
            if (user != null)
                return true;
            return false;
        }

        public string Hash(string _salt)
        {
            byte[] hash = Encoding.UTF8.GetBytes(Password + pepper);
            byte[] salt = Encoding.UTF8.GetBytes(_salt);

            var byteResult = new Rfc2898DeriveBytes(hash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }

        public string Hash()
        {
            return Hash(GetSalt());
        }

        private IMongoCollection<BsonDocument> GetCollection(string _collection)
        {
            IMongoDatabase db = InventarServerMain.GetMongoDB().GetDatabase(DB);
            var collection = db.GetCollection<BsonDocument>(_collection);

            return collection;
        }

        public string GetSalt()
        {
            var users = GetCollection("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", Name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", Password);

            var user = users.Find(filter).FirstOrDefault();
            if (user == null)
                return null;

            BsonValue salt = user.GetValue("password_salt");

            return salt.ToString();
        }

        public bool Login()
        {
            if (!UserExists())
                return false;
            var database = InventarServerMain.GetMongoDB().GetDatabase(DB);
            var userCollection = database.GetCollection<BsonDocument>("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", Name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", Password);

            var user = userCollection.Find(filter).FirstOrDefault();
            
            string generatedHash = user.GetValue("password").ToString();
            string hash = Hash();

            Console.WriteLine(hash);
            Console.WriteLine(generatedHash);

            return SaveEquals(generatedHash, hash);
        }

        private bool SaveEquals(string _s1, string _s2)
        {
            char[] s1 = _s1.ToCharArray();
            char[] s2 = _s2.ToCharArray();
            bool eq = true;
            if (s1.Length != s2.Length)
                eq = false;
            for(int i = 0; i < s1.Length; i++)
            {
                if (s1[i] != s2[i])
                    eq = false;
            }
            return eq;
        }
    }
}
