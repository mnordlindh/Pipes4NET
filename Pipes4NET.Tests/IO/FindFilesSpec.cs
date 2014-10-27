using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipes4NET.IO;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pipes4NET.Tests.IO {
    [TestClass]
    public class FindFilesSpec {
        [TestMethod]
        public void Mapper() {
            var readFiles = new FindFiles("*.txt");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../files";
            var path = dir + "/file.txt";
            Directory.CreateDirectory(dir);
            File.Create(path);

            var res = readFiles.Mapper(dir);

            Assert.AreEqual(1, res.Count());
        }
    }
}
