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

    struct Change
    {
        public Script ChangedScript { get; private set; }
        public ChangeType Type { get; private set; }

        public Change(Script script, ChangeType type) : this()
        {
            ChangedScript = script;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Change))
            {
                return Equals(this, (Change) obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ChangedScript.GetHashCode() * 17 + Type.GetHashCode();
        }

        public static bool Equals(Change a, Change b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a.ChangedScript == b.ChangedScript && a.Type == b.Type;
        }

        public static bool operator ==(Change a, Change b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Change a, Change b)
        {
            return !Equals(a, b);
        }
    }
}
