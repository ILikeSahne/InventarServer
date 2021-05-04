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
        public Error LoadDatabases()
        {
            if (!File.Exists(ConfigFile))
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.CONFIG_FILE_NOT_FOUND);
            try {
                string json = File.ReadAllText(ConfigFile);
                List<DatabaseLocation> Locations = JsonSerializer.Deserialize<List<DatabaseLocation>>(json);
                foreach (DatabaseLocation dl in Locations)
                {
                    Database d = new Database(dl);
                    Error e = d.LoadDatabase();
                    InventarServer.WriteLine("Loading Database: {0}", dl.Name);
                    if (!e)
                    {
                        InventarServer.WriteLine("CRITICAL ERROR: ");
                        e.PrintAllErrors();
                    } else
                        databases.Add(d);
                }
            }
            catch (Exception e)
            {
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.DATABASE_CORRUPTED, e);
            }
            return Error.NO_ERROR;
        }

        /// <summary>
        /// Saves the Config-File
        /// </summary>
        /// <returns>Returns an Error if it can't save the Config</returns>
        public Error SaveConfig()
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
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.CONFIG_FILE_UNSAVEABLE, e);
            }
            return Error.NO_ERROR;
        }

        /// <summary>
        /// Adds a Database to the Config
        /// </summary>
        /// <param name="_d">Database to add</param>
        /// <returns>Returns an Error if the Database is not valid or the Config is unsaveable</returns>
        public Error AddDatabase(Database _d)
        {
            InventarServer.WriteLine("Adding new Database, with name: \"{0}\"", _d.Loc.Name);
            Error e = ValidateDatabase(_d);
            if (!e)
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.DATABASE_CORRUPTED, e);
            databases.Add(_d);
            _d.CreateDatabase();
            e = SaveConfig();
            if (!e)
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.CONFIG_FILE_UNSAVEABLE);
            e = _d.LoadDatabase();
            if (!e)
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.DATABASE_UNLOAD_ABLE, e);
            return Error.NO_ERROR;
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
        private Error ValidateDatabase(Database _d)
        {
            foreach(Database d in databases)
            {
                if (d.Loc.Name.Equals(_d.Loc.Name))
                    return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.DATABASE_ALREADY_EXISTS);
            }
            return Error.NO_ERROR;
        }

        /// <summary>
        /// Creates a new Config
        /// </summary>
        /// <returns>Returns an Error if it can't create a new Config</returns>
        public Error CreateNewConfig()
        {
            try
            {
                Directory.CreateDirectory(ConfigFolder);
                File.Create(ConfigFile).Close();
                Error e = SaveConfig();
                if (!e)
                    return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.CONFIG_FILE_UNSAVEABLE, e);
            }
            catch (Exception e)
            {
                return new Error(ErrorType.DATABASE_ERROR, DatabaseErrorType.CONFIG_FILE_UNCREATEABLE, e);
            }
            return Error.NO_ERROR;
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
        DATABASE_UNLOAD_ABLE,
        EQUIPMENT_FOLDER_NOT_FOUND,
        EQUIPMENT_FILE_CORRUPTED,
        EQUIPMENT_FILE_UNSAVEABLE,
        EQUIPMENT_FILE_UNLOADABLE,
        EQUIPMENT_CORRUPTED
    }
}
