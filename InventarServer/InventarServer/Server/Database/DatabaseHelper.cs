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

        /// <summary>
        /// Database name
        /// </summary>
        public string DB { get; }
        /// <summary>
        /// Username
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Used if you only want to use some helper functions
        /// </summary>
        public DatabaseHelper()
        { }

        /// <summary>
        /// Used if you only want to hash a password
        /// </summary>
        /// <param name="_password">User password</param>
        public DatabaseHelper(string _password)
        {
            Password = _password;
        }

        /// <summary>
        /// Used to login into a database
        /// </summary>
        /// <param name="_db">Database name</param>
        /// <param name="_name">Username</param>
        /// <param name="_password">User password</param>
        public DatabaseHelper(string _db, string _name, string _password)
        {
            if (_db == "")
                _db = "_";
            DB = _db;
            Name = _name;
            Password = _password;
        }

        /// <summary>
        /// Hashes a password using a salt
        /// </summary>
        /// <param name="_salt">Salt to add to the password</param>
        /// <returns>Hashed password</returns>
        public string Hash(string _salt)
        {
            byte[] hash = Encoding.UTF8.GetBytes(Password + pepper);
            byte[] salt = Encoding.UTF8.GetBytes(_salt);

            var byteResult = new Rfc2898DeriveBytes(hash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }

        /// <summary>
        /// Generates the Hash of the password, with the salt from the database
        /// </summary>
        /// <returns>The salted Hash</returns>
        public string Hash()
        {
            return Hash(GetSalt());
        }

        /// <summary>
        /// Reads the salt from the database
        /// </summary>
        /// <returns>The salt from the database</returns>
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

        /// <summary>
        /// Gets a collection from the database
        /// </summary>
        /// <param name="_collection">Collection name</param>
        /// <returns>The collection</returns>
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

        /// <summary>
        /// Gets the user from the database, based on his/her name
        /// </summary>
        /// <returns></returns>
        public BsonDocument GetUser()
        {
            var users = GetCollection("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", Name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", Name);

            var user = users.Find(filter).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        /// <param name="_email">User email</param>
        /// <returns>True, if the user was added successful</returns>
        public bool AddUser(string _email)
        {
            if (GetUser() != null)
                return false;
            var collection = GetCollection("users");
            User user = new User(_email, Name, Password);
            collection.InsertOne(user.GetUserAsBson());
            return true;
        }

        /// <summary>
        /// Logs into the database
        /// </summary>
        /// <returns>An error, if the login failed</returns>
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

        /// <summary>
        /// Lists all databases
        /// </summary>
        /// <returns>A list of databases</returns>
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

        public void AddItem(Item _i)
        {
            var collection = GetCollection("items");
            collection.InsertOne(_i.GetItemAsBson());
        }

        public List<Item> ListItems() {
            var collection = GetCollection("items");
            var rawItems = collection.Find(_ => true).ToList();
            List<Item> items = new List<Item>();
            foreach(var rawItem in rawItems)
            {
                items.Add(new Item(rawItem));
            }
            return items;
        }

    }

    /// <summary>
    /// Possible login errors
    /// </summary>
    enum LoginError
    {
        NONE, WRONG_DATABASE, WRONG_USERNAME, WRONG_PASSWORD
    }
}
