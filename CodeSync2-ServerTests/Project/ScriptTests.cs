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
            Script script = new Script(@"C:\Test\TestScript.mod.lua", @"C:\Test", "game.ServerScriptService");
            Assert.AreEqual(ScriptType.Module, script.Type);
            Assert.AreEqual("TestScript", script.Name);
            Assert.AreEqual("game.ServerScriptService.TestScript", script.RobloxPath);
        }

        [TestMethod()]
        public void RobloxScriptTest()
        {
            Assert.Fail();
        }
    }
}