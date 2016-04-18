using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        public Mapping ContextMapping { get; private set; }
        public Dictionary<string, Script> Scripts { get; private set; }

        private HashSet<Change> changes;
        private FileSystemWatcher watcher;

        /// <summary>
        /// Creates a new SyncContext.
        /// </summary>
        /// <param name="mapping">The mapping to handle</param>
        public SyncContext(Mapping mapping)
        {
            ContextMapping = mapping;
            changes = new HashSet<Change>();
            Scripts = new Dictionary<string, Script>();

            watcher = new FileSystemWatcher(mapping.FsPath, "*.lua");
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.IncludeSubdirectories = true;

            watcher.Changed += OnFileSystemEvent;
            watcher.Created += OnFileSystemEvent;
            watcher.Deleted += OnDeleteEvent;
            watcher.Renamed += OnRenameEvent;
        }

        /// <summary>
        /// Starts watching for changes in the context's root directory.
        /// </summary>
        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            RepopulateScripts();
        }

        /// <summary>
        /// Stops watching for changes.
        /// </summary>
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Gets the current changes recorded by the SyncContext.
        /// </summary>
        /// <returns>All changes that have been recorded</returns>
        public Change[] GetCurrentChanges()
        {
            return changes.ToArray();
        }

        /// <summary>
        /// Clears the recorded changes.
        /// </summary>
        public void ClearChanges()
        {
            changes.Clear();
        }

        public void RepopulateScripts()
        {
            Scripts.Clear();

            string[] files = Directory.GetFiles(ContextMapping.FsPath, "*.lua", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Scripts[file] = Script.FromFileSystem(file, ContextMapping);
            }
        }

        private void PushChange(string path, ChangeType type)
        {
            if (!Scripts.ContainsKey(path))
            {
                Scripts[path] = Script.FromFileSystem(path, ContextMapping);
            }

            changes.RemoveWhere(c => c.ChangedScript.FilePath == path);
            changes.Add(new Change(Scripts[path], type));

            if (type == ChangeType.Delete)
            {
                Scripts.Remove(path);
            }
        }

        private void OnFileSystemEvent(object source, FileSystemEventArgs args)
        {
            PushChange(args.FullPath, ChangeType.Write);
        }

        private void OnDeleteEvent(object source, FileSystemEventArgs args)
        {
            PushChange(args.FullPath, ChangeType.Delete);
        }

        private void OnRenameEvent(object source, RenamedEventArgs args)
        {
            string old = args.OldFullPath;
            PushChange(args.OldFullPath, ChangeType.Delete);
            PushChange(args.FullPath, ChangeType.Write);
        }
    }
}
