using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        public string RootPath { get; private set; }
        public string RobloxPath { get; private set; }

        private HashSet<Change> changes;
        private FileSystemWatcher watcher;

        /// <summary>
        /// Creates a new SyncContext.
        /// </summary>
        /// <param name="rootPath">The path to watch</param>
        /// <param name="fileTypesInput">A list of file extensions to use</param>
        public SyncContext(string rootPath)
        {
            changes = new HashSet<Change>();

            watcher = new FileSystemWatcher(rootPath, "*.lua");
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.IncludeSubdirectories = true;

            watcher.Changed += OnFileSystemEvent;
            watcher.Created += OnFileSystemEvent;
            watcher.Deleted += OnDeleteEvent;
            watcher.Renamed += OnRenameEvent;

            Start();
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
