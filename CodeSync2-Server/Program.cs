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
        static Config config;

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: codesync <config_file>");
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

            ProjectServer server = new ProjectServer(config);
            Console.WriteLine($"CodeSync server starting on port {config.Port}.");
            server.Start();

            return 0;
        }
    }
}
