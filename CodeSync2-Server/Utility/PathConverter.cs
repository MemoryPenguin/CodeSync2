using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Utility
{
    class PathConverter
    {
        [DllImport("shlwapi.dll", SetLastError = true)]
        private static extern int PathRelativePathTo(StringBuilder pszPath, string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);

        private static int GetPathAttribute(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists)
            {
                // directory file attribute
                return 0x10;
            }
            
            if (fileInfo.Exists)
            {
                // normal file attribute
                return 0x80;
            }

            throw new FileNotFoundException();
        }

        /// <summary>
        /// Makes a relative path.
        /// Depends on the DLL shlwapi.dll.
        /// </summary>
        /// <param name="absolutePath">An absolute path</param>
        /// <param name="rootPath">The root path of the relative path</param>
        /// <returns>A relative path, relative to rootPath.</returns>
        public static string MakeRelativePath(string absolutePath, string rootPath)
        {
            int pathAttribute = GetPathAttribute(absolutePath);
            int rootAttribute = GetPathAttribute(rootPath);
            StringBuilder pathBuilder = new StringBuilder(256);

            if (PathRelativePathTo(pathBuilder, rootPath, rootAttribute, absolutePath, pathAttribute) == 0)
            {
                throw new ArgumentException("Paths lack a common prefix");
            }

            return pathBuilder.ToString();
        }

        public static string MakeAbsolutePath(string relativePath, string rootPath)
        {
            string temp = Path.Combine(rootPath, relativePath);
            return Path.GetFullPath(temp);
        }

        /// <summary>
        /// Converts a ROBLOX path string, in the form <c>Parent.Child.Child</c>, to an absolute file system path.
        /// </summary>
        /// <param name="robloxPath">The ROBLOX path</param>
        /// <param name="rootPath">The root of the path</param>
        /// <param name="extension">The extension of the file</param>
        /// <returns>An absolute path.</returns>
        public static string RobloxToFsPath(string robloxPath, string rootPath, string extension)
        {
            string relativePath = robloxPath.Replace(".", "\\");
            relativePath = Path.ChangeExtension(relativePath, extension);
            return MakeAbsolutePath(relativePath, rootPath);
        }

        /// <summary>
        /// Converts an absolute file system path to a ROBLOX path string.
        /// </summary>
        /// <param name="absolutePath">The absolute path</param>
        /// <param name="rootPath">Where the path is rooted</param>
        /// <returns>A ROBLOX path string</returns>
        public static string FsToRobloxPath(string absolutePath, string rootPath)
        {
            string relativePath = MakeRelativePath(absolutePath, rootPath);
            return relativePath.Replace("\\", ".");
        }
    }
}
