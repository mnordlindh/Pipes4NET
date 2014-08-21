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

            int i = 0;
            for (; i < 10; ++i) {
                Assert.AreEqual(i, res[i]);
            }
            Assert.AreEqual(10, i);
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

            int i = 0;
            for (; i < 16; ++i) {
                Assert.AreEqual(i, res[i]);
            }
            Assert.AreEqual(16, i);
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

            int i = 0;
            for (; i < 16; ++i) {
                Assert.AreEqual(i, res[i]);
            }
            Assert.AreEqual(16, i);
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

        [TestMethod]
        public void GetItem_ShouldCacheItems() {
            // Setup
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

            var spy = new SpyExecutable<IEnumerable<int>>();

            // Act
            var expr =
                nestedList
                    .Pipe(spy)
                    .Pipe(new FlattenExecutable<int>());

            // execute the enumerator twice
            var res1 = expr.ToList();
            var res = expr.ToList();

            // Assert that the mapper function is hit
            // only once per item for the spy
            var allItemsHitOnce = spy.GetItemHitCount.All(kvp => kvp.Value == 1);

            Assert.IsTrue(allItemsHitOnce);
            Assert.AreEqual(16, res.Count());
        }
    }
}
