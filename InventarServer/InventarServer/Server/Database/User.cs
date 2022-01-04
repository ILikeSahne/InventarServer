using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventarServer
{
    class User
    {
        public Database Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Permissions { get; set; }

        private bool loggedIn;

        public User(Database _db, string _username, string _password)
        {
            Database = _db;
            Username = _username;
            Password = _password;
            LoadPermissions();
        }

        public User(string _db, string _username, string _password)
        {
            if(!string.IsNullOrWhiteSpace(_db))
                Database = new Database(_db);
            Username = _username;
            Password = _password;
            LoadPermissions();
        }

        private void LoadPermissions()
        {
            Permissions = new List<string>();
            if (Database == null)
                return;
            var user = GetUser().user;
            if (user == null)
                return;
            var permissions = user.GetValue("permissions").AsBsonArray;
            foreach (BsonValue val in permissions)
                Permissions.Add(val.AsString);
        }

        public LoginError Login()
        {
            if (loggedIn)
                return LoginError.NONE;
            loggedIn = false;
            if (Username == "")
                return LoginError.NONE;
            if (Database == null)
                return LoginError.WRONG_DATABASE;

            try
            {
                LoginError e = Database.Login(this);
                if (e == LoginError.NONE)
                    loggedIn = true;
                return e;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return LoginError.UNKNOWN;
            }
        }
        
        public bool HasPermission(string _perm)
        {
            if(Login() == LoginError.NONE)
                return Permissions.Contains(_perm);
            return false;
        }

        public bool IsSuperAdminUser()
        {
            if(Database.Name.Equals("super_admins") && Login() == LoginError.NONE)
                return true;
            return false;
        }

        public bool IsAdmin()
        {
            return HasPermission("admin");
        }

        public void AddPermission(string _permission)
        {
            Collection userCol = Database.GetCollection("users");

            var userObject = GetUser();
            var user = userObject.user;

            var perms = user.GetValue("permissions").AsBsonArray;
            perms.Add(_permission);

            if (userObject.isEmail)
                userCol.UpdateEntry("email", Username, user);
            else if (userObject.isUsername)
                userCol.UpdateEntry("username", Username, user);
        }

        public void RemovePermission(string _permission)
        {
            Collection userCol = Database.GetCollection("users");

            var userObject = GetUser();
            var user = userObject.user;

            var perms = user.GetValue("permissions").AsBsonArray;
            perms.Remove(_permission);

            if (userObject.isEmail)
                userCol.UpdateEntry("email", Username, user);
            else if (userObject.isUsername)
                userCol.UpdateEntry("username", Username, user);
        }

        public BsonDocument AsBson(string _email, string _salt)
        {
            string hashedPassword = Database.Hash(_salt, Password);

            BsonArray perms = new BsonArray();
            foreach (string p in Permissions)
                perms.Add(p);

            BsonDocument user = new BsonDocument
            {
                { "email", _email },
                { "username", Username },
                { "password", hashedPassword },
                { "password_salt", _salt},
                { "permissions", perms }
            };

            return user;
        }

        public (BsonDocument user, bool isEmail, bool isUsername) GetUser()
        {
            Collection userCol = Database.GetCollection("users");

            var byEmail = userCol.FindOne("email", Username);
            var byUsername = userCol.FindOne("username", Username);
            if (byEmail != null)
                return (byEmail, true, false);
            if(byUsername != null)
                return (byUsername, false, true);
            return (null, false, false); ;
        }
    }

    class UserData
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public List<string> Permissions { get; set; }

        public UserData()
        { }

        public UserData(string _email, string _username, List<string> _permissions)
        {
            Email = _email;
            Username = _username;
            Permissions = _permissions;
        }

        public static UserData FromBson(BsonDocument _bd)
        {
            string email = _bd.GetValue("email").AsString;
            string username = _bd.GetValue("username").AsString;
            List<string> permissions = new List<string>();
            BsonArray ba = _bd.GetValue("permissions").AsBsonArray;
            foreach (BsonValue b in ba)
            {
                permissions.Add(b.AsString);
            }
            return new UserData(email, username, permissions);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
