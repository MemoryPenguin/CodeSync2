using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project
{
    struct DeleteChange : IChange
    {
        public Script ChangedScript { get; }
        public ChangeType Type
        {
            get
            {
                return ChangeType.Delete;
            }
        }

        public DeleteChange(Script script) : this()
        {
            ChangedScript = script;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(DeleteChange))
            {
                return Equals(this, (DeleteChange)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ChangedScript.GetHashCode() * 17 + Type.GetHashCode();
        }

        public static bool Equals(DeleteChange a, DeleteChange b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a.ChangedScript == b.ChangedScript && a.Type == b.Type;
        }

        public static bool operator ==(DeleteChange a, DeleteChange b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(DeleteChange a, DeleteChange b)
        {
            return !Equals(a, b);
        }
    }
}
