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

        private List<SyncContext> contexts;
        private HashSet<Script> scripts;
        private HttpServer server;
        private Config config;

        public ProjectServer(Config config)
        {
            server = new HttpServer(config.Port);
            contexts = new List<SyncContext>();
            scripts = new HashSet<Script>();
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
            IEnumerable<Script> matchingScripts = scripts.Where(s => s.RobloxPath == target);
            
            if (matchingScripts.Count() > 0)
            {
                return matchingScripts.ElementAt(0).GetFileContents();
            }

            return string.Empty;
        }

        private object RouteGetScripts(NameValueCollection args)
        {
            List<Script> scripts = new List<Script>();

            foreach (SyncContext context in contexts)
            {
                scripts.AddRange(context.Scripts.Values);
            }

            return scripts;
        }

        private object RouteGetInfo(NameValueCollection args)
        {
            return config;
        }

        public object RouteGetChanges(NameValueCollection args)
        {
            List<Change> changes = new List<Change>();

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
