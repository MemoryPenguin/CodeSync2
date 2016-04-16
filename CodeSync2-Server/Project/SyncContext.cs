using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        private HashSet<Change> changes;
        private FileSystemWatcher watcher;
        private string[] fileTypes;

        public SyncContext(string rootPath, string[] fileTypesInput)
        {
            changes = new HashSet<Change>();

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

            watcher.Changed += OnFileSystemEvent;
            watcher.Created += OnFileSystemEvent;
            watcher.Deleted += OnDeleteEvent;
            watcher.Renamed += OnRenameEvent;

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

        public Change[] GetCurrentChanges()
        {
            return changes.ToArray();
        }

        public void ClearChanges()
        {
            changes.Clear();
        }

        private bool IsTrackable(string fullPath)
        {
            string ext = Path.GetExtension(fullPath);
            return fileTypes.Contains(ext);
        }

        private void PushChange(Change change)
        {
            changes.RemoveWhere(c => c.Path == change.Path);
            changes.Add(change);
        }

        private void OnFileSystemEvent(object source, FileSystemEventArgs args)
        {
            if (IsTrackable(args.FullPath))
            {
                PushChange(new Change(args.FullPath, ChangeType.Write));
            }
        }

        private void OnDeleteEvent(object source, FileSystemEventArgs args)
        {
            if (IsTrackable(args.FullPath))
            {
                PushChange(new Change(args.FullPath, ChangeType.Delete));
            }
        }

        private void OnRenameEvent(object source, RenamedEventArgs args)
        {
            if (IsTrackable(args.OldFullPath))
            {
                string old = args.OldFullPath;
                changes.RemoveWhere(c => c.Path == old);
                PushChange(new Change(args.OldFullPath, ChangeType.Delete));
            }
            if (IsTrackable(args.FullPath))
            {
                PushChange(new Change(args.FullPath, ChangeType.Write));
            }
        }
    }
}
