using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        public Mapping ContextMapping { get; private set; }

        private List<Script> scripts;
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
            scripts = new List<Script>();

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

        public Script[] GetScripts()
        {
            string[] files = Directory.GetFiles(ContextMapping.FsPath, "*.lua", SearchOption.AllDirectories);
            Script[] scripts = new Script[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                Script script = Script.FromFileSystem(filePath, ContextMapping);
                scripts[i] = script;
            }

            return scripts;
        }

        private void PushChange(Change change)
        {
            changes.RemoveWhere(c => c.Path == change.Path);
            changes.Add(change);
        }

        private void OnFileSystemEvent(object source, FileSystemEventArgs args)
        {
            PushChange(new Change(args.FullPath, ChangeType.Write));
        }

        private void OnDeleteEvent(object source, FileSystemEventArgs args)
        {
            PushChange(new Change(args.FullPath, ChangeType.Delete));
        }

        private void OnRenameEvent(object source, RenamedEventArgs args)
        {
            string old = args.OldFullPath;
            changes.RemoveWhere(c => c.Path == old);
            PushChange(new Change(args.OldFullPath, ChangeType.Delete));
            PushChange(new Change(args.FullPath, ChangeType.Write));
        }
    }
}
