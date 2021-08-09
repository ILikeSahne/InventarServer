using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class User
    {
        public string Database { get; }
        public string Email { get; }
        public string Username { get; }
        public string Password { get; }
        public string PasswordSalt { get; }

        public User(string _database, string _email, string _username, string _password)
        {
            Database = _database;
            Email = _email;
            Username = _username;
            PasswordSalt = Salt();
            Password = new DatabaseHelper(_password).Hash(PasswordSalt);
        }

        private string Salt()
        {
            var bytes = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
