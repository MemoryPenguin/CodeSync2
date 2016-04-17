using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MemoryPenguin.CodeSync2.Server.Utility
{
    public static class RobloxPathHelper
    {
        private static Regex removeInnerSeparators = new Regex(@"\b\.{2,}\b", RegexOptions.Compiled);
        private static Regex removeOuterSeparators = new Regex(@"(?:^\.+)|(?:\.+$)", RegexOptions.Compiled);

        public static string Normalize(string path)
        {
            path = removeInnerSeparators.Replace(path, @".");
            path = removeOuterSeparators.Replace(path, "");

            return path;
        }

        public static string FromFsPath(string fsPath)
        {
            string robloxPath = fsPath;
            
            while (!string.IsNullOrEmpty(Path.GetExtension(robloxPath)))
            {
                robloxPath = robloxPath.Replace(Path.GetExtension(robloxPath), "");
            }

            if (!string.IsNullOrEmpty(Path.GetPathRoot(robloxPath)))
            {
                robloxPath = robloxPath.Replace(Path.GetPathRoot(robloxPath), "");
            }
            
            robloxPath = robloxPath.Replace(Path.DirectorySeparatorChar, '.');
            robloxPath = robloxPath.Replace(Path.AltDirectorySeparatorChar, '.');
            robloxPath = Normalize(robloxPath);

            return robloxPath;
        }

        /// <summary>
        /// Creates a relative path.
        /// </summary>
        /// <param name="path">The path to make relative. Must be rooted in the root path; cannot backtrack.</param>
        /// <param name="rootPath">The path to use as the root.</param>
        /// <returns>A path rooted in <c>rootPath</c> instead of <c>game</c></returns>
        /// <exception cref="ArgumentException">Thrown if <c>path</c> is null or empty.</exception>
        public static string MakeRelativePath(string path, string rootPath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path");
            }

            if (string.IsNullOrEmpty(rootPath))
            {
                return Normalize(path);
            }

            path = Normalize(path);
            rootPath = Normalize(rootPath);

            Console.WriteLine(path);
            Console.WriteLine(rootPath);

            return Normalize(path.Replace(rootPath, ""));
        }

        /// <summary>
        /// Converts a ROBLOX path to a file system path segment.
        /// </summary>
        /// <param name="robloxPath"></param>
        /// <returns></returns>
        public static string ToFsPath(string robloxPath)
        {
            return robloxPath.Replace('.', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Joins two paths.
        /// </summary>
        /// <param name="path1">The first path</param>
        /// <param name="path2">The second path</param>
        /// <returns>A path joined together</returns>
        public static string Join(string path1, string path2)
        {
            return Normalize(path1) + '.' + Normalize(path2);
        }

        /// <summary>
        /// Gets the name of the instance that a path refers to.
        /// </summary>
        /// <param name="path">The path to retrieve a name from</param>
        /// <returns>The name that the path ends in</returns>
        public static string GetName(string path)
        {
            string name = path.Substring(path.LastIndexOf('.') + 1);
            return name;
        }
    }
}
