using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    struct WriteChange : IChange
    {
        public Script ChangedScript { get; }
        public ChangeType Type
        {
            get
            {
                return ChangeType.Write;
            }
        }
        public string NewContent { get; }

        public WriteChange(Script script, string newContents) : this()
        {
            ChangedScript = script;
            NewContent = newContents;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(IChange))
            {
                return Equals(this, (IChange) obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ChangedScript.GetHashCode() * 17 + Type.GetHashCode();
        }

        public static bool Equals(WriteChange a, WriteChange b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a.ChangedScript == b.ChangedScript && a.Type == b.Type && a.NewContent == b.NewContent;
        }

        public static bool operator ==(WriteChange a, WriteChange b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(WriteChange a, WriteChange b)
        {
            return !Equals(a, b);
        }
    }
}
