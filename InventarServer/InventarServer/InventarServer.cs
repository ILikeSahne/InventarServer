using System;

namespace InventarServer
{
    class InventarServer
    {
        private const string domain = "ilikesahne.ddns.net";
        private const int port = 10001;

        private static Server server;

        /// <summary>
        /// Main()
        /// </summary>
        /// <param name="_args">Arguments from cmd</param>
        public static void Main(string[] _args)
        {
            StartServer();
            Console.ReadKey(); // Prevent Program from stopping
        }

        /// <summary>
        /// Starts the Server
        /// </summary>
        private static void StartServer()
        {
            server = new Server(domain, port);
            ServerError e = server.StartServer();
            if (!e)
            {
                e.PrintError();
                Environment.Exit(0);
            } else {
                WriteLine("Server started on domain(adress): {0}, on port: {1}", domain, port);
            }
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
