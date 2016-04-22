using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    public class Mapping
    {
        /// <summary>
        /// The path to the root directory that will be synced.
        /// </summary>
        [JsonProperty("disk")]
        public string RawFsPath { get; set; }

        [JsonProperty("absoluteDisk")]
        public string FsPath
        {
            get
            {
                return Path.GetFullPath(RawFsPath);
            }
        }

        /// <summary>
        /// The ROBLOX path to where to put the directory
        /// </summary>
        [JsonProperty("roblox")]
        public string RobloxPath { get; set; }

        /// <summary>
        /// Creates a new Mapping with a given target directory and ROBLOX location.
        /// </summary>
        /// <param name="fsPath">The path on the file system</param>
        /// <param name="robloxPath">The path in Studio</param>
        public Mapping(string fsPath, string robloxPath)
        {
            RawFsPath = fsPath;
            RobloxPath = robloxPath;
        }
    }
}
