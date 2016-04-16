using MemoryPenguin.CodeSync2.Server.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server
{
    enum SyncMode
    {
        TwoWay = 0,
        OneWay = 1,
    }

    enum SyncAuthority
    {
        FileSystem = 0,
        Studio = 1,
    }

    struct Config
    {
        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("mappings")]
        public Mapping[] Mappings { get; set; }

        [DefaultValue(SyncMode.TwoWay)]
        [JsonProperty("syncmode", DefaultValueHandling = DefaultValueHandling.Populate)]
        public SyncMode Mode { get; set; }

        [DefaultValue(SyncAuthority.FileSystem)]
        [JsonProperty("authority", DefaultValueHandling = DefaultValueHandling.Populate)]
        public SyncAuthority Authority { get; set; }
    }
}
