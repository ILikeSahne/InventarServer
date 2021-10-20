using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Stores data about a user
    /// </summary>
    class User
    {
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; }
        /// <summary>
        /// User password salt
        /// </summary>
        public string PasswordSalt { get; }

        /// <summary>
        /// Stores data about a user
        /// </summary>
        /// <param name="_email">User email</param>
        /// <param name="_username">Username</param>
        /// <param name="_password">User passwords</param>
        public User(string _email, string _username, string _password)
        {
            Email = _email;
            Username = _username;
            PasswordSalt = Salt();
            Password = new DatabaseHelper(_password).Hash(PasswordSalt);
        }

        /// <summary>
        /// Generates a random salt
        /// </summary>
        /// <returns>A random salt</returns>
        public string Salt()
        {
            var bytes = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convers the user to a bson document
        /// </summary>
        /// <returns>The user as a bson document</returns>
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
