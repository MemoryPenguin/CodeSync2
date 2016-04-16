using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemoryPenguin.CodeSync2.Server.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Utility.Tests
{
    [TestClass()]
    public class PathConverterTests
    {
        [TestMethod()]
        public void MakeRelativePathTest()
        {
            string rootPath = @"C:\Test\Test2";
            string absolute1 = @"C:\Test\Test2\Test4\Test6";
            string absolute2 = @"C:\Test\Test3";

            string relative1 = PathConverter.MakeRelativePath(absolute1, rootPath, PathConverter.FILE_ATTRIBUTE, PathConverter.DIRECTORY_ATTRIBUTE);
            Assert.AreEqual(@".\Test4\Test6", relative1);

            string relative2 = PathConverter.MakeRelativePath(absolute2, rootPath, PathConverter.FILE_ATTRIBUTE, PathConverter.DIRECTORY_ATTRIBUTE);
            Assert.AreEqual(@"..\Test3", relative2);
        }

        [TestMethod()]
        public void MakeAbsolutePathTest()
        {
            string rootPath = @"C:\Test\Test2";
            string relative1 = @".\Test3\Test4";
            string relative2 = @"..\Test3";

            string absolute1 = PathConverter.MakeAbsolutePath(relative1, rootPath);
            Assert.AreEqual(@"C:\Test\Test2\Test3\Test4", absolute1);

            string absolute2 = PathConverter.MakeAbsolutePath(relative2, rootPath);
            Assert.AreEqual(@"C:\Test\Test3", absolute2);
        }

        [TestMethod()]
        public void RobloxToFsPathTest()
        {
            string rootPath = @"C:\Test\Test2";
            string robloxPath = @"game.ServerScriptService";
            string fsPath = PathConverter.RobloxToFsPath(robloxPath, rootPath, "");

            Assert.AreEqual(@"C:\Test\Test2\game\ServerScriptService", fsPath);
        }

        [TestMethod()]
        public void FsToRobloxPathTest()
        {
            Assert.Fail();
        }
    }
}