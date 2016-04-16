using MemoryPenguin.CodeSync2.Server.Network;
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
            server.Start();

            return 0;
        }

        private static string TestRoute(NameValueCollection args)
        {
            return "Hello, world!";
        }
    }
}
