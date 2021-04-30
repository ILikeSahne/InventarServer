using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

namespace InventarServer
{
    class DatabaseManager
    {
        /// <summary>
        /// The Folder of the Confgig
        /// </summary>
        public string ConfigFolder { get; set; }
        /// <summary>
        /// The Config File itself
        /// </summary>
        public string ConfigFile { get; set; }

        private List<Database> databases;

        /// <summary>
        /// Stores and initiates values
        /// </summary>
        /// <param name="_configFile"></param>
        public DatabaseManager(string _configFile)
        {
            ConfigFolder = _configFile;
            ConfigFile = _configFile + "/config.json";
            databases = new List<Database>();
        }

        /// <summary>
        /// Loads all Databases stored in the Config-File
        /// </summary>
        /// <returns>Returns an Error if it can't load the Config</returns>
        public DatabaseError LoadDatabases()
        {
            if (!File.Exists(ConfigFile))
                return new DatabaseError(DatabaseErrorType.CONFIG_FILE_NOT_FOUND, null);
            try {
                string json = File.ReadAllText(ConfigFile);
                List<DatabaseLocation> Locations = JsonSerializer.Deserialize<List<DatabaseLocation>>(json);
                foreach (DatabaseLocation dl in Locations)
                {
                    Database d = new Database(dl);
                    DatabaseError e = d.LoadDatabase();
                    InventarServer.WriteLine("Loading Database: {0}", dl.Name);
                    if (!e)
                    {
                        InventarServer.WriteLine("CRITICAL ERROR: ");
                        e.PrintError();
                    } else
                        databases.Add(d);
                }
            }
            catch (Exception e)
            {
                return new DatabaseError(DatabaseErrorType.DATABASE_CORRUPTED, e);
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Saves the Config-File
        /// </summary>
        /// <returns>Returns an Error if it can't save the Config</returns>
        public DatabaseError SaveConfig()
        {
            try
            {
                List<DatabaseLocation> Locations = new List<DatabaseLocation>();
                foreach(Database d in databases)
                {
                    Locations.Add(d.Loc);
                }
                string json = JsonSerializer.Serialize(Locations);
                File.WriteAllText(ConfigFile, json);
                InventarServer.WriteLine("Saving Config!");
            }
            catch (Exception e)
            {
                return new DatabaseError(DatabaseErrorType.CONFIG_FILE_UNSAVEABLE, e);
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Adds a Database to the Config
        /// </summary>
        /// <param name="_d">Database to add</param>
        /// <returns>Returns an Error if the Database is not valid or the Config is unsaveable</returns>
        public DatabaseError AddDatabase(Database _d)
        {
            InventarServer.WriteLine("Adding new Database, with name: \"{0}\"", _d.Loc.Name);
            DatabaseError de = ValidateDatabase(_d);
            if (!de)
                return de;
            databases.Add(_d);
            _d.CreateDatabase();
            de = SaveConfig();
            if (!de)
                return de;
            de = _d.LoadDatabase();
            if (!de)
                return de;
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Returns a Database based on its name
        /// </summary>
        /// <param name="name">Name of the Database</param>
        /// <returns>Returns the Database or null if the Database can't be found</returns>
        public Database GetDatabase(string _name)
        {
            foreach (Database d in databases)
            {
                if (d.Loc.Name.Equals(_name))
                    return d;
            }
            return null;
        }

        /// <summary>
        /// Checks if a Database is valid
        /// </summary>
        /// <param name="_d">Database to validate</param>
        /// <returns>Returns an Error if the Database is not valid</returns>
        private DatabaseError ValidateDatabase(Database _d)
        {
            foreach(Database d in databases)
            {
                if (d.Loc.Name.Equals(_d.Loc.Name))
                    return new DatabaseError(DatabaseErrorType.DATABASE_ALREADY_EXISTS, null);
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }

        /// <summary>
        /// Creates a new Config
        /// </summary>
        /// <returns>Returns an Error if it can't create a new Config</returns>
        public DatabaseError CreateNewConfig()
        {
            try
            {
                Directory.CreateDirectory(ConfigFolder);
                File.Create(ConfigFile).Close();
                DatabaseError e = SaveConfig();
                if (!e)
                    return e;
            }
            catch (Exception e)
            {
                return new DatabaseError(DatabaseErrorType.CONFIG_FILE_UNCREATEABLE, e);
            }
            return new DatabaseError(DatabaseErrorType.NO_ERROR, null);
        }
    }

    class DatabaseLocation
    {
        /// <summary>
        /// Name of the Database
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Folder of the Database
        /// </summary>
        public string Folder { get; set; }
        /// <summary>
        /// IDCounter of the Database
        /// </summary>
        public int IDCounter { get; set; }

        /// <summary>
        /// Is used to be able to store a DatabaseLocations in a json-File
        /// </summary>
        public DatabaseLocation() { }

        /// <summary>
        /// Saves values
        /// </summary>
        /// <param name="_name">Name of the Database</param>
        /// <param name="_folder">Folder of the Database</param>
        /// <param name="_IDCounter">IDCounter of the Database</param>
        public DatabaseLocation(string _name, string _folder, int _IDCounter)
        {
            Name = _name;
            Folder = _folder;
            IDCounter = _IDCounter;
        }
    }

    class DatabaseError
    {
        /// <summary>
        /// Type of Error
        /// </summary>
        public DatabaseErrorType Type { get; }
        /// <summary>
        /// Thrown Exception
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Saves values
        /// </summary>
        /// <param name="_type">Type of Error</param>
        /// <param name="_e">Thrown Exception</param>
        public DatabaseError(DatabaseErrorType _type, Exception _e)
        {
            Type = _type;
            Exception = _e;
        }

        /// <summary>
        /// Writes the Error to the Console (only when in DEBUG mode)
        /// </summary>
        public void PrintError()
        {
            InventarServer.WriteLine("DatabaseError: {0}", ToString());
            StackFrame stackFrame = new StackFrame(1, true);
            string filename = stackFrame.GetFileName();
            int line = stackFrame.GetFileLineNumber();
            string method = stackFrame.GetMethod().ToString();
            InventarServer.WriteLine("{0}:{1}, {2}", Path.GetFileName(filename), line, method);
        }

        /// <summary>
        /// Returns the Error as a String:
        ///     "TypeOfError: ExceptionMessage"
        /// </summary>
        /// <returns>"TypeOfError: ExceptionMessage"</returns>
        public override string ToString()
        {
            if (Exception != null)
                return Type + ": " + Exception.Message;
            else
                return Type.ToString();
        }

        /// <summary>
        /// Returns true if there was no Error, otherwise it returns false
        /// </summary>
        /// <param name="e">DatabaseError</param>
        public static implicit operator bool(DatabaseError e)
        {
            return e.Type == DatabaseErrorType.NO_ERROR;
        }
    }

    enum DatabaseErrorType
    {
        NO_ERROR,
        CONFIG_FILE_NOT_FOUND,
        CONFIG_FILE_UNCREATEABLE,
        CONFIG_FILE_UNSAVEABLE,
        DATABASE_FOLDER_NOT_FOUND,
        DATABASE_FILES_UNCREATEABLE,
        DATABASE_CORRUPTED,
        DATABASE_ALREADY_EXISTS,
        EQUIPMENT_FOLDER_NOT_FOUND,
        EQUIPMENT_FILE_CORRUPTED,
        EQUIPMENT_FILE_UNSAVEABLE
    }
}
