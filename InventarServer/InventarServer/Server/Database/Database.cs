﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class Database
    {
        private const string pepper = "h8xGOPtOmwCPSqnSYjcNVJc1tzpAyvZr";

        public string Name { get; set; }
        public IMongoDatabase MongoDatabase { get; set; }

        public Database(string _name)
        {
            Name = _name;
            if(Exists())
                MongoDatabase = Mongo().GetDatabase(_name);
        }

        public LoginError Login(User _u)
        {
            Collection userCollection = GetCollection("users");

            BsonDocument foundUser = userCollection.FindOne("username", _u.Username);
            if (foundUser == null)
                foundUser = userCollection.FindOne("email", _u.Username);

            if (foundUser == null)
                return LoginError.WRONG_USERNAME;

            string salt = foundUser.GetValue("password_salt").AsString;
            string hashedPassword = Hash(salt, _u.Password);
            string password = foundUser.GetValue("password").AsString;

            if (password.Equals(hashedPassword))
                return LoginError.NONE;
            return LoginError.WRONG_PASSWORD;
        }

        public string Hash(string _salt, string _password)
        {
            byte[] hash = Encoding.UTF8.GetBytes(_password + pepper);
            byte[] salt = Encoding.UTF8.GetBytes(_salt);

            var byteResult = new Rfc2898DeriveBytes(hash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }

        /// <summary>
        /// Generates a random salt
        /// </summary>
        /// <returns>A random salt</returns>
        public string GenerateRandomSalt()
        {
            var bytes = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public Collection GetCollection(string _name)
        {
            if (!Exists())
                return null;
            var col = MongoDatabase.GetCollection<BsonDocument>(_name);
            if(col == null)
            {
                MongoDatabase.CreateCollection(_name);
                col = MongoDatabase.GetCollection<BsonDocument>(_name);
            }
            return new Collection(col);
        }

        public MongoClient Mongo()
        {
            return InventarServerMain.GetMongoDB();
        }

        public bool Exists()
        {
            var databases = DatabaseHelper.ListAllDatabases();
            return databases.Contains(Name);
        }

        public void CreateNewDatabase()
        {
            if (Exists())
                return;
            MongoDatabase = Mongo().GetDatabase(Name);
            MongoDatabase.CreateCollection("users");
        }

        public ValidateError AddUser(string _email, string _username, string _password)
        {
            if (!Validator.ValidateEmail(_email))
                return ValidateError.EMAIL;
            if (!Validator.ValidateUsername(_username))
                return ValidateError.USERNAME;
            if (!Validator.ValidatePassword(_password))
                return ValidateError.PASSWORD;

            Collection c = GetCollection("users");

            string salt = GenerateRandomSalt();

            User u = new User(this, _username, _password);

            c.AddOne(u.AsBson(_email, salt));

            return ValidateError.NONE;
        }

        public List<UserData> GetUserData()
        {
            List<UserData> users = new List<UserData>();
            var userCol = GetCollection("users");
            var allDocs = userCol.GetAll();
            foreach (BsonDocument bd in allDocs)
            {
                users.Add(UserData.FromBson(bd));
            }
            return users;
        }
    }
}