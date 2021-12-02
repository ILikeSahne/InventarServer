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

        private Validator validator;

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
        public DatabaseHelper() : this("", "", "")
        { }

        /// <summary>
        /// Used if you only want to hash a password
        /// </summary>
        /// <param name="_password">User password</param>
        public DatabaseHelper(string _password) : this("", "", _password)
        { }

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
            validator = new Validator();
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
        public BsonDocument GetUser(string _name)
        {
            var users = GetCollection("users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", _name);
            filter |= Builders<BsonDocument>.Filter.Eq("email", _name);

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
            if (!UsernameOk() || !EmailOk(_email))
                return false;
            var collection = GetCollection("users");
            User user = new User(_email, Name, Password);
            collection.InsertOne(user.GetUserAsBson());
            return true;
        }

        public bool EmailOk(string _email)
        {
            if (GetUser(_email) != null)
                return false;
            return validator.ValidateEmail(_email);
        }

        public bool PasswordOk()
        {
            return validator.ValidatePassword(Password);
        }

        public bool UsernameOk()
        {
            if (GetUser(Name) != null)
                return false;
            return validator.ValidateUsername(Name);
        }

        /// <summary>
        /// Logs into the database
        /// </summary>
        /// <returns>An error, if the login failed</returns>
        public LoginError Login()
        {
            var user = GetUser(Name);

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

        public void AddItem(Item _i, string _itemCollection)
        {
            var collection = GetCollection("items");
            var filter = Builders<BsonDocument>.Filter.Eq("itemCollectionName", _itemCollection);
            var itemCollection = collection.Find(filter).FirstOrDefault();
            if(itemCollection == null)
            {
                itemCollection = new BsonDocument("itemCollectionName", _itemCollection);
                BsonArray items = new BsonArray();
                items.Add(_i.GetItemAsBson());
                itemCollection.Add("items", items);
                collection.InsertOne(itemCollection);
                return;
            }
            var update = Builders<BsonDocument>.Update.Push("items", _i.GetItemAsBson());
            collection.UpdateOne(filter, update);
        }

        public void DeleteItem(string _id, string _itemCollection) {
            var collection = GetCollection("items");
            var filter = Builders<BsonDocument>.Filter.Eq("itemCollectionName", _itemCollection);
            var itemCollection = collection.Find(filter).FirstOrDefault();
            if (itemCollection == null)
                return;
            BsonArray ba = itemCollection.GetElement("items").Value.AsBsonArray;
            foreach (BsonValue be in ba)
            {
                Item item = new Item(be.AsBsonDocument);
                if (item.ID == _id)
                {
                    var update = Builders<BsonDocument>.Update.Pull("items", item.GetItemAsBson());
                    collection.UpdateOne(filter, update);
                    break;
                }
            }
        }
        
        public List<Item> ListItems(string _itemCollection) {
            var collection = GetCollection("items");
            var filter = Builders<BsonDocument>.Filter.Eq("itemCollectionName", _itemCollection);
            var itemCollection = collection.Find(filter).FirstOrDefault();
            List<Item> items = new List<Item>();
            if (itemCollection == null)
                return items;

            foreach(var rawItem in itemCollection.GetElement("items").Value.AsBsonArray)
            {
                items.Add(new Item(rawItem.AsBsonDocument));
            }
            return items;
        }

        public List<string> ListItemCollections()
        {
            var collection = GetCollection("items");
            var itemCollections = collection.Find(_ => true).ToList();
            List<string> names = new List<string>();
            foreach (var coll in itemCollections)
            {
                names.Add(coll.GetElement("itemCollectionName").Value.AsString);
            }
            return names;
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
