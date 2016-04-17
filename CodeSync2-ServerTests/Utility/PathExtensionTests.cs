using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemoryPenguin.CodeSync2.Server.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryPenguin.CodeSync2.Server.Utility.Tests
{
    [TestClass()]
    public class PathExtensionTests
    {
        [TestMethod()]
        public void MakeRelativePathTest()
        {
            string root = @"C:\Test";
            string test1 = @"C:\Test\Test1";
            string test2 = @"C:\Test\Test1\Test2";

            Assert.AreEqual("Test1", PathExtension.MakeRelativePath(root, test1));
            Assert.AreEqual(@"Test1\Test2", PathExtension.MakeRelativePath(root, test2));
        }

        [TestMethod()]
        public void MakeAbsolutePathTest()
        {
            string root = @"C:\Test";
            string test1 = @"Test1";
            string test2 = @"Test1\Test2";

            Assert.AreEqual(@"C:\Test\Test1", PathExtension.MakeAbsolutePath(test1, root));
            Assert.AreEqual(@"C:\Test\Test1\Test2", PathExtension.MakeAbsolutePath(test2, root));
        }
    }
}