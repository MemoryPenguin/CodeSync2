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
        public string Path { get; private set; }
        public ChangeType Type { get; private set; }

        public Change(string fullPath, ChangeType type) : this()
        {
            Path = fullPath;
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
            return Path.GetHashCode() * 17 + Type.GetHashCode();
        }

        public static bool Equals(Change a, Change b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a.Path == b.Path && a.Type == b.Type;
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
