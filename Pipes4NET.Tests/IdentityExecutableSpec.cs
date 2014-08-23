using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pipes4NET.Tests {
    [TestClass]
    public class IdentityExecutableSpec {
        [TestMethod]
        public void Mapper_ShouldReturnArgument() {
            var id = new IdentityExecutable<object>();
            var obj = new object();
            var res = id.Mapper(obj);

            Assert.AreEqual(obj, res);
        }
    }
}
