using MemoryPenguin.CodeSync2.Server.Project;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Network
{
    class ProjectServer
    {
        public int Port
        {
            get
            {
                return server.Port;
            }
        }

        private List<Script> Scripts
        {
            get
            {
                List<Script> scripts = new List<Script>();

                foreach (SyncContext context in contexts)
                {
                    scripts.AddRange(context.GetScripts());
                }

                return scripts;
            }
        }

        private List<SyncContext> contexts;
        private HttpServer server;
        private Config config;

        public ProjectServer(Config config)
        {
            server = new HttpServer(config.Port);
            contexts = new List<SyncContext>();
            this.config = config;

            foreach (Mapping mapping in config.Mappings)
            {
                SyncContext context = new SyncContext(mapping);
                context.Start();
                contexts.Add(context);
            }

            server.AddRoute("read", RouteGetScriptContents);
            server.AddRoute("scripts", RouteGetScripts);
            server.AddRoute("changes", RouteGetChanges);
            server.AddRoute("info", RouteGetInfo);
        }

        private object RouteGetScriptContents(NameValueCollection args)
        {
            string target = args["location"];

            try
            {
                Script result = Scripts.First(s => s.RobloxPath == target);
                return result.GetFileContents();
            }
            catch
            {
                return "-- CodeSync couldn't find the file on the file system. Something is wrong.";
            }
        }

        private object RouteGetScripts(NameValueCollection args)
        {
            return Scripts;
        }

        private object RouteGetInfo(NameValueCollection args)
        {
            return config;
        }

        public object RouteGetChanges(NameValueCollection args)
        {
            List<IChange> changes = new List<IChange>();

            foreach (SyncContext context in contexts)
            {
                changes.AddRange(context.GetCurrentChanges());
                context.ClearChanges();
            }

            return changes;
        }

        public void Start()
        {
            server.Start();
        }
    }
}
