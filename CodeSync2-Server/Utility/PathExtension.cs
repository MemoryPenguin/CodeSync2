using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Utility
{
    /// <summary>
    /// Extends the <c>Path</c> class with more methods.
    /// </summary>
    public static class PathExtension
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// Only supports absolute paths rooted in the root path; does not support backtracking.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from <c>rootPath</c> to <c>absolutePath</c>.</returns>
        public static string MakeRelativePath(string rootPath, string absolutePath)
        {
            string common = Path.Combine(absolutePath, rootPath);
            string relativePath = absolutePath.Replace(common, "");
            // common does not include the trailing \ in the root, so it has to be removed separately
            relativePath = relativePath.Substring(1);
            return relativePath;
        }

        /// <summary>
        /// Converts a relative path to an absolute path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static string MakeAbsolutePath(string relativePath, string rootPath)
        {
            string temp = Path.Combine(rootPath, relativePath);
            return Path.GetFullPath(temp);
        }
    }
}
