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

        public DatabaseHelper()
        { }

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

        public string GetSalt()
        {
            var users = GetCollection("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", Name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", Name);

            var user = users.Find(filter).FirstOrDefault();
            if (user == null)
                return null;

            BsonValue salt = user.GetValue("password_salt");

            return salt.ToString();
        }

        public IMongoCollection<BsonDocument> GetCollection(string _collection)
        {
            IMongoDatabase db = InventarServerMain.GetMongoDB().GetDatabase(DB);
            var collection = db.GetCollection<BsonDocument>(_collection);

            if(collection == null)
            {
                db.CreateCollection(_collection);
                collection = db.GetCollection<BsonDocument>(_collection);
            }

            return collection;
        }

        public BsonDocument GetUser()
        {
            var users = GetCollection("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", Name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", Name);

            var user = users.Find(filter).FirstOrDefault();
            return user;
        }

        public bool AddUser(string _email)
        {
            if (GetUser() != null)
                return false;
            var collection = GetCollection("users");
            User user = new User(_email, Name, Password);
            collection.InsertOne(user.GetUserAsBson());
            return true;
        }

        public LoginError Login()
        {
            var user = GetUser();

            if (user == null)
                return LoginError.WRONG_USERNAME;

            string generatedHash = user.GetValue("password").ToString();
            string hash = Hash();

            if (!hash.Equals(generatedHash))
                return LoginError.WRONG_PASSWORD;
            return LoginError.NONE;
        }

        public List<string> ListDatabases()
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

    }

    enum LoginError
    {
        NONE, WRONG_DATABASE, WRONG_USERNAME, WRONG_PASSWORD
    }
}
