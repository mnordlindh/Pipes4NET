using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Pipes4NET.Tests {
    [TestClass]
    public class FlattenExecutableSpec {
        [TestMethod]
        public void Should_Flatten_nested_source() {
            var nestedList = new List<List<int>>() {
                new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            var res = nestedList.Pipe(new FlattenExecutable<int>()).ToList();

            for (int i = 0; i < 10; ++i) {
                Assert.AreEqual(i, res[i]);
            }
        }
    }
}
