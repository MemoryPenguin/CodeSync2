using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    enum ChangeType
    {
        Write = 0,
        Delete = 1,
    }

    interface IChange
    {
        Script ChangedScript { get; }
        ChangeType Type { get; }
    }
}
