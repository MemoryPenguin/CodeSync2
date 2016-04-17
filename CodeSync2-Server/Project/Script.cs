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

        public static Script FromFileSystem(string filePath, Mapping mapping)
        {
            string info = Path.GetFileNameWithoutExtension(filePath);
            string name = Path.GetFileNameWithoutExtension(info);
            string typeString = Path.GetExtension(info);
            string relativeFsPath = PathExtension.MakeRelativePath(mapping.FsPath, filePath);
            string relativeRbxPath = RobloxPathHelper.FromFsPath(relativeFsPath);
            string rbxPath = RobloxPathHelper.Join(mapping.RobloxPath, relativeRbxPath);

            Script script = new Script();
            script.RobloxPath = rbxPath;
            script.FilePath = filePath;
            script.Name = name;
            script.Type = GetScriptType(typeString);

            return script;
        }

        public static Script FromRoblox(string rbxPath, ScriptType rbxType, Mapping mapping)
        {
            string relativeRbxPath = RobloxPathHelper.MakeRelativePath(rbxPath, mapping.RobloxPath);
            string relativeFsPath = RobloxPathHelper.ToFsPath(relativeRbxPath);
            string absoluteFsPath = PathExtension.MakeAbsolutePath(relativeFsPath, mapping.FsPath);
            string ext = GetExtension(rbxType) + ".lua";
            absoluteFsPath = absoluteFsPath + ext;

            Script script = new Script();
            script.RobloxPath = rbxPath;
            script.FilePath = absoluteFsPath;
            script.Name = RobloxPathHelper.GetName(rbxPath);
            script.Type = rbxType;

            return script;
        }

        public string GetFileContents()
        {
            return File.ReadAllText(FilePath);
        }

        public static ScriptType GetScriptType(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
            {
                return ScriptType.Server;
            }

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

        public static string GetExtension(ScriptType type)
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
