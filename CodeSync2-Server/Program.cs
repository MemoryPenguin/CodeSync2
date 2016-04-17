using MemoryPenguin.CodeSync2.Server.Network;
using MemoryPenguin.CodeSync2.Server.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace MemoryPenguin.CodeSync2.Server
{
    class Program
    {
        static List<SyncContext> contexts = new List<SyncContext>();
        static Config config;

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: codesync <config_file>");
                Console.ReadLine();
                return 1;
            }

            FileInfo fileInfo = new FileInfo(args[0]);

            if (!fileInfo.Exists)
            {
                Console.WriteLine($"Can't access file at {args[0]}.");
                return 1;
            }

            string directoryPath = new FileInfo(args[0]).DirectoryName;
            string absoluteConfigPath = Path.GetFullPath(args[0]);

            Directory.SetCurrentDirectory(directoryPath);
            Console.WriteLine($"Set working directory to {directoryPath}.");

            string configContents = File.ReadAllText(absoluteConfigPath);
            config = JsonConvert.DeserializeObject<Config>(configContents);
            config.FileLocation = args[0];

            Console.WriteLine($"Config loaded from {args[0]}.");

            ValidationResult validationResult = config.Validate();
            if (!validationResult.Valid)
            {
                Console.WriteLine("Malformed config: {0}", validationResult.Message);
                Console.ReadLine();
                return 1;
            }

            foreach (Mapping mapping in config.Mappings)
            {
                contexts.Add(new SyncContext(mapping));
            }

            HttpServer server = new HttpServer(config.Port);
            server.AddRoute("test", TestRoute);
            server.AddRoute("read", ReadRoute);
            server.AddRoute("scripts", GetScriptsRoute);

            Console.WriteLine($"CodeSync server started on port {config.Port}.");
            server.Start();

            Console.ReadLine();

            return 0;
        }

        private static object TestRoute(NameValueCollection args)
        {
            return "Hello, world!";
        }

        private static object ReadRoute(NameValueCollection args)
        {
            string path = args.Get("path");
            return File.ReadAllText(path);
        }

        private static object GetScriptsRoute(NameValueCollection args)
        {
            List<Script> scripts = new List<Script>();

            foreach (SyncContext context in contexts)
            {
                scripts.AddRange(context.GetScripts());
            }

            return scripts;
        }
    }
}
