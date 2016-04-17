using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemoryPenguin.CodeSync2.Server.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Project.Tests
{
    [TestClass()]
    public class ScriptTests
    {
        [TestMethod()]
        public void FsScriptTest()
        {
            Script script = Script.FromFileSystem(@"C:\Test\Module.mod.lua", new Mapping(@"C:\Test", "game.ServerScriptService"));
            Assert.AreEqual(ScriptType.Module, script.Type);
            Assert.AreEqual("Module", script.Name);
            Assert.AreEqual("game.ServerScriptService.Module", script.RobloxPath);
            Assert.AreEqual(@"C:\Test\Module.mod.lua", script.FilePath);
        }

        [TestMethod()]
        public void RobloxScriptTest()
        {
            Script script = Script.FromRoblox("game.ServerScriptService.Module", ScriptType.Module, new Mapping(@"C:\Test", "game.ServerScriptService"));
            Assert.AreEqual(@"C:\Test\Module.mod.lua", script.FilePath);
            Assert.AreEqual("Module", script.Name);
            Assert.AreEqual("game.ServerScriptService.Module", script.RobloxPath);
            Assert.AreEqual(ScriptType.Module, script.Type);
        }
    }
}