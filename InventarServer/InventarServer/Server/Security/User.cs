using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class User
    {
        public string Email { get; }
        public string Username { get; }
        public string Password { get; }
        public string PasswordSalt { get; }

        public User(string _email, string _username, string _password)
        {
            Email = _email;
            Username = _username;
            PasswordSalt = Salt();
            Password = new DatabaseHelper(_password).Hash(PasswordSalt);
        }

        public string Salt()
        {
            var bytes = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public BsonDocument GetUserAsBson()
        {
            return new BsonDocument
            {
                { "email", Email },
                { "username", Username },
                { "password", Password },
                { "password_salt", PasswordSalt }
            };
        }
    }
}
