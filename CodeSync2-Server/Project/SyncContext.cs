using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        private FileSystemWatcher watcher;
        private string[] fileTypes;

        public SyncContext(string rootPath, string[] fileTypesInput)
        {
            watcher = new FileSystemWatcher(rootPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.IncludeSubdirectories = true;

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

            watcher.Changed += OnWatcherEvent;
            watcher.Deleted += OnWatcherEvent;
            watcher.Created += OnWatcherEvent;
            watcher.Renamed += OnWatcherEvent;

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

        private void OnWatcherEvent(object source, FileSystemEventArgs args)
        {
            Console.WriteLine($"change: {args.FullPath} of type {args.ChangeType}");
        }
    }
}
