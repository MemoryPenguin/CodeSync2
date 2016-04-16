using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        enum ScriptType
        {
            Local = 0,
            Server = 1,
            Module = 2,
        }

        private FileSystemWatcher watcher;
        private string[] fileTypes;

        public SyncContext(string rootPath, string[] fileTypesInput)
        {
            watcher = new FileSystemWatcher(rootPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            fileTypes = new string[fileTypesInput.Length];
            for (int i = 0; i < fileTypesInput.Length; i++)
            {
                string type = fileTypesInput[i];
                if (type.Substring(1, 1) != ".")
                {
                    type = '.' + type;
                }

                fileTypes[i] = type;
            }

            watcher.Changed += (source, e) =>
            {
                Console.WriteLine($"change: {e.FullPath} of type {e.ChangeType}");
            };

            StartWatching();
        }

        public void StartWatching()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }
    }
}
