using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Pipes4NET.Tests {
    [TestClass]
    public class FlattenExecutableSpec {
        [TestMethod]
        public void Enumerating_WithNestedSource_ShouldFlattenOutput() {
            var nestedList = new List<List<int>>() {
                new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            var res = nestedList.Pipe(new FlattenExecutable<int>()).ToList();

            for (int i = 0; i < 10; ++i) {
                Assert.AreEqual(i, res[i]);
            }
        }

        [TestMethod]
        public void Enumerating_WithMultipleNestedSources_ShouldFlattenOutput() {
            var nestedList = new List<List<int>>() {
                new List<int>() { 0, 1, 2, 3, 4, 5 },
                new List<int>() { 6, 7, 8, 9 },
                new List<int>() { 10, 11, 12, 13},
                new List<int>() { 14, 15}
            };

            var res = nestedList.Pipe(new FlattenExecutable<int>()).ToList();

            for (int i = 0; i < 16; ++i) {
                Assert.AreEqual(i, res[i]);
            }
        }

        [TestMethod]
        public void Enumerating_WithEmptyNestedSources_ShouldFlattenOutput() {
            var nestedList = new List<List<int>>() {
                new List<int>() { 0, 1, 2, 3, 4, 5 },
                new List<int>() { },
                new List<int>() { 6, 7, 8, 9 },
                new List<int>() { },
                new List<int>() { },
                new List<int>() { 10, 11, 12, 13},
                new List<int>() { 14, 15},
                new List<int>() { },
                new List<int>() { },
            };

            var res = nestedList.Pipe(new FlattenExecutable<int>()).ToList();

            for (int i = 0; i < 16; ++i) {
                Assert.AreEqual(i, res[i]);
            }
        }

        [TestMethod]
        public void Enumerating_WithAllEmptyNestedSources_ShouldFlattenOutput() {
            var nestedList = new List<List<int>>() {
                new List<int>() { },
                new List<int>() { },
                new List<int>() { },
                new List<int>() { },
                new List<int>() { },
            };

            var res = nestedList.Pipe(new FlattenExecutable<int>()).ToList();

            Assert.AreEqual(0, res.Count());
        }
    }
}
