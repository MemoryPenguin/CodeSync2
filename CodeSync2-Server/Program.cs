using MemoryPenguin.CodeSync2.Server.Network;
using MemoryPenguin.CodeSync2.Server.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            HttpServer server = new HttpServer(8080);
            server.AddRoute("test", TestRoute);
            //server.Start();

            SyncContext context = new SyncContext(@"D:\Documents\Visual Studio 2015\Projects\CodeSync2\TestProject\Server", new string[0]);

            Console.ReadLine();

            return 0;
        }

        private static string TestRoute(NameValueCollection args)
        {
            return "Hello, world!";
        }
    }
}
