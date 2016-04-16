using MemoryPenguin.CodeSync2.Server.Network;
using MemoryPenguin.CodeSync2.Server.Project;
using System;
using System.Collections.Specialized;

namespace MemoryPenguin.CodeSync2.Server
{
    class Program
    {
        static int Main(string[] args)
        {
            /*if (args.Length != 1)
            {
                Console.WriteLine("Usage: codesync <config_file>");
                return 1;
            }

            string configContents = File.ReadAllText(args[0]);
            Config config = JsonConvert.DeserializeObject<Config>(configContents);*/

            Config config = new Config();
            config.Port = 8080;
            config.Mappings = new Mapping[]
            {
                new Mapping(@"D:\Documents\Visual Studio 2015\Projects\CodeSync2\TestProject\Server", "game.ServerScriptService")
            };
            config.Mode = SyncMode.TwoWay;


            HttpServer server = new HttpServer(config.Port);
            server.AddRoute("test", TestRoute);
            //server.Start();

            SyncContext context = new SyncContext(@"D:\Documents\Visual Studio 2015\Projects\CodeSync2\TestProject\Server");

            Console.ReadLine();

            return 0;
        }

        private static string TestRoute(NameValueCollection args)
        {
            return "Hello, world!";
        }
    }
}
