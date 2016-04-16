using MemoryPenguin.CodeSync2.Server.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server
{
    struct Config
    {
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("mappings")]
        public Mapping[] Mappings { get; set; }
    }
}
