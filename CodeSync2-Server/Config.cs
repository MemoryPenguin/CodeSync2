using MemoryPenguin.CodeSync2.Server.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

    class Config
    {
        [JsonProperty("port")]
        public ushort Port { get; set; }

        [JsonProperty("mappings")]
        public Mapping[] Mappings { get; set; }

        [DefaultValue(SyncMode.TwoWay)]
        [JsonProperty("syncmode", DefaultValueHandling = DefaultValueHandling.Populate)]
        public SyncMode Mode { get; set; }

        [DefaultValue(SyncAuthority.FileSystem)]
        [JsonProperty("authority", DefaultValueHandling = DefaultValueHandling.Populate)]
        public SyncAuthority Authority { get; set; }

        public string FileLocation { get; set; }

        public ValidationResult Validate()
        {
            // Check port
            if (Port == 0)
            {
                return new ValidationResult { Valid = false, Message = "'port' cannot be zero" };
            }

            FileInfo info = new FileInfo(FileLocation);

            // Check mappings
            foreach (Mapping mapping in Mappings)
            {
                if (string.IsNullOrWhiteSpace(mapping.FsPath))
                {
                    return new ValidationResult { Valid = false, Message = "key 'disk' must be present" };
                }

                if (string.IsNullOrWhiteSpace(mapping.RobloxPath))
                {
                    return new ValidationResult { Valid = false, Message = "key 'roblox' must be present" };
                }

                string relative = mapping.FsPath;
                string path = relative;//Path.Combine(info.DirectoryName, relative);

                if (!Directory.Exists(path))
                {
                    return new ValidationResult { Valid = false, Message = $"'disk' value {path} does not reference a directory" };
                }
            }

            // Check mode
            if (!Enum.IsDefined(typeof(SyncMode), Mode))
            {
                return new ValidationResult { Valid = false, Message = "'mode' must be valid: 0 or 1" };
            }

            // Check authority
            if (!Enum.IsDefined(typeof(SyncAuthority), Authority))
            {
                return new ValidationResult { Valid = false, Message = "'authority' must be valid: 0 or 1" };
            }

            return new ValidationResult { Valid = true, Message = "all checks passed" };
        }
    }

    struct ValidationResult
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
