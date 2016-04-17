﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemoryPenguin.CodeSync2.Server.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Utility.Tests
{
    [TestClass()]
    public class RobloxPathTests
    {
        [TestMethod()]
        public void NormalizeTest()
        {
            string test1 = "game.Workspace";
            string test2 = ".game.Workspace";
            string test3 = "game.Workspace.";
            string test4 = ".game.Workspace.";
            string test5 = "game..Workspace";
            string test6 = "game.Workspace...";
            string test7 = "..game.Workspace";
            string test8 = "..game.Workspace....";

            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test1));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test2));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test3));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test4));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test5));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test6));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test7));
            Assert.AreEqual("game.Workspace", RobloxPath.Normalize(test8));
        }

        [TestMethod()]
        public void FromFsPathTest()
        {
            string fsPath1 = @"C:\Test\Test1";
            string fsPath2 = @"Test\Test2\Test3";

            Assert.AreEqual("Test.Test1", RobloxPath.FromFsPath(fsPath1));
            Assert.AreEqual("Test.Test2.Test3", RobloxPath.FromFsPath(fsPath2));
        }

        [TestMethod()]
        public void MakeRelativePathTest()
        {
            string rootPath = @"game.ServerScriptService";
            string absPath1 = @"game.ServerScriptService.Test.Module";
            string absPath2 = @"game.ServerScriptService.Module";

            Assert.AreEqual("Test.Module", RobloxPath.MakeRelativePath(absPath1, rootPath));
            Assert.AreEqual("Module", RobloxPath.MakeRelativePath(absPath2, rootPath));
        }

        [TestMethod()]
        public void ToFsPathTest()
        {
            string rbxPath1 = @"game.ServerScriptService.Test.Module";
            string rbxPath2 = @"Test.Module";

            Assert.AreEqual(@"game\ServerScriptService\Test\Module", RobloxPath.ToFsPath(rbxPath1));
            Assert.AreEqual(@"Test\Module", RobloxPath.ToFsPath(rbxPath2));
        }

        [TestMethod()]
        public void JoinTest()
        {
            string path1 = "game.ServerScriptService";
            string path2 = "Test.Module";

            Assert.AreEqual("game.ServerScriptService.Test.Module", RobloxPath.Join(path1, path2));
        }
    }
}