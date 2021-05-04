﻿using System;

namespace InventarServer
{
    class InventarServer
    {
        private const string domain = "localhost"; //"192.168.178.56"
        private const int port = 10001;

        private static string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace('\\', '/') + "/InventarServer/";

        private static DatabaseManager database;
        private static IServer server;
        private static CommandManager cmdManager;

        /// <summary>
        /// Main()
        /// </summary>
        /// <param name="_args">Arguments from cmd</param>
        public static void Main(string[] _args)
        {
            LoadDatabases();
            cmdManager = new CommandManager();
            StartServer();
            DatabaseTest dt = new DatabaseTest();
            dt.CreateTestDatabase();
        }

        /// <summary>
        /// Loads all Databases
        /// </summary>
        private static void LoadDatabases()
        {
            WriteLine("Loading Databases...");
            database = new DatabaseManager(configPath);
            Error e = database.LoadDatabases();
            if (!e)
            {
                e.PrintAllErrors();
                if (e.Message.Equals(DatabaseErrorType.CONFIG_FILE_NOT_FOUND.ToString()))
                    CreateConfig();
                Environment.Exit(0);
            }
            WriteLine("Databases finished loading!");
        }

        /// <summary>
        /// Creates a new Config
        /// </summary>
        private static void CreateConfig()
        {
            WriteLine("Creating new Config-File at: \"{0}\"", configPath);
            WriteLine("Create new Config [y, n]?");
            string s = Console.ReadLine().ToLower();
            if (s[0] == 'y')
            {
                Error e = database.CreateNewConfig();
                if (!e)
                {
                    e.PrintAllErrors();
                    WriteLine("Couldn't create Config!");
                }
                else
                    WriteLine("Restart the program to try again!");
            }
        }

        /// <summary>
        /// Starts the Server
        /// </summary>
        private static void StartServer()
        {
            server = new IServer(domain, port);
            Error e = server.StartServer();
            if (!e)
            {
                e.PrintAllErrors();
                Environment.Exit(0);
            }
            WriteLine("Server started on domain(adress): {0}, on port: {1}", domain, port);
            server.StartServerRoutine();
        }

        /// <summary>
        /// Returns the Database-Manager
        /// </summary>
        /// <returns>The Database-Manager</returns>
        public static DatabaseManager GetDatabase()
        {
            return database;
        }

        public static CommandManager GetCommandManager()
        {
            return cmdManager;
        }

        /// <summary>
        /// Only writes to Console when in DEBUG mode
        /// </summary>
        /// <param name="_s">Message to write</param>
        /// <param name="_args">Objects to place in {} placeholders</param>
        public static void WriteLine(string _s, params object[] _args)
        {
#if DEBUG
            Console.WriteLine(_s, _args);
#endif
        }

    }
}
