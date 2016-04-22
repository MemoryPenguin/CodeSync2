using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    class SyncContext
    {
        public Mapping ContextMapping { get; private set; }

        private Dictionary<string, Script> scripts;
        private HashSet<IChange> changes;
        private FileSystemWatcher watcher;

        /// <summary>
        /// Creates a new SyncContext.
        /// </summary>
        /// <param name="mapping">The mapping to handle</param>
        public SyncContext(Mapping mapping)
        {
            ContextMapping = mapping;
            changes = new HashSet<IChange>();
            scripts = new Dictionary<string, Script>();

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
        public IChange[] GetCurrentChanges()
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
            scripts.Clear();

            string[] files = Directory.GetFiles(ContextMapping.FsPath, "*.lua", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                scripts[file] = Script.FromFileSystem(file, ContextMapping);
            }
        }

        public Script[] GetScripts()
        {
            return scripts.Values.ToArray();
        }

        private Script GetScript(string path)
        {
            if (!scripts.ContainsKey(path))
            {
                scripts[path] = Script.FromFileSystem(path, ContextMapping);
            }

            return scripts[path];
        }

        private void PushChange(string path, IChange change)
        {
            changes.RemoveWhere(c => c.ChangedScript.FilePath == path);
            changes.Add(change);

            if (change.Type == ChangeType.Delete)
            {
                scripts.Remove(path);
            }
        }

        private void OnFileSystemEvent(object source, FileSystemEventArgs args)
        {
            PushChange(args.FullPath, new WriteChange(GetScript(args.FullPath), ReadText(args.FullPath)));
        }

        private void OnDeleteEvent(object source, FileSystemEventArgs args)
        {
            PushChange(args.FullPath, new DeleteChange(GetScript(args.FullPath)));
        }

        private void OnRenameEvent(object source, RenamedEventArgs args)
        {
            string old = args.OldFullPath;
            PushChange(args.OldFullPath, new DeleteChange(GetScript(args.FullPath)));
            PushChange(args.FullPath, new WriteChange(GetScript(args.FullPath), ReadText(args.FullPath)));
        }

        /// <summary>
        /// Reads a file. Will block until the file is available.
        /// </summary>
        /// <param name="path">The path to read.</param>
        /// <returns>The contents of the file.</returns>
        private string ReadText(string path)
        {
            string contents = null;

            while (contents == null)
            {
                try
                {
                    using (Stream stream = new FileStream(path, FileMode.Open))
                    {
                        StreamReader reader = new StreamReader(stream);
                        contents = reader.ReadToEnd();
                    }
                }
                catch
                {
                    // no-op
                }
            }

            return contents;
        }
    }
}
