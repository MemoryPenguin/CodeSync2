using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    struct Mapping
    {
        [JsonProperty("disk")]
        public string FilePath { get; set; }
        [JsonProperty("roblox")]
        public string RobloxPath { get; set; }

        public Mapping(string filePath, string robloxPath)
        {
            FilePath = filePath;
            RobloxPath = robloxPath;
        }
    }
}
