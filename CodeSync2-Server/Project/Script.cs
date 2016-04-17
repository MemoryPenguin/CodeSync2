using MemoryPenguin.CodeSync2.Server.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    public enum ScriptType
    {
        Server = 0,
        Local = 1,
        Module = 2,
    }

    public class Script
    {
        public ScriptType Type { get; private set; }
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public string RobloxPath { get; private set; }

        public Script(string path, string rootPath, string robloxMappingRoot)
        {
            FilePath = path;

            string info = Path.GetFileNameWithoutExtension(path);
            Name = Path.GetFileNameWithoutExtension(info);
            Type = GetScriptType(Path.GetExtension(info));

            //RobloxPath = PathExtension.JoinRobloxPaths(robloxMappingRoot, PathExtension.FsToRobloxPath(path, rootPath));
        }

        public Script(string robloxPath, ScriptType type, string mappingRoot)
        {
            Type = type;
            RobloxPath = robloxPath;
            //FilePath = PathExtension.MakeAbsolutePath(PathExtension.RobloxToFsPath(robloxPath, mappingRoot, GetExtension(type) + ".lua"), mappingRoot);
            Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(FilePath));
        }

        public string GetFileContents()
        {
            return File.ReadAllText(FilePath);
        }

        private ScriptType GetScriptType(string typeString)
        {
            typeString = typeString.ToLower().Substring(1);
            
            switch (typeString)
            {
                case "mod":
                    return ScriptType.Module;
                case "module":
                    return ScriptType.Module;
                case "local":
                    return ScriptType.Local;
                default:
                    return ScriptType.Server;
            }
        }

        private string GetExtension(ScriptType type)
        {
            switch (type)
            {
                case ScriptType.Local:
                    return ".local";
                case ScriptType.Module:
                    return ".mod";
                default:
                    return "";
            }
        }
    }
}
