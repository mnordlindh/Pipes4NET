using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Pipes4NET.Tests
{
    [TestClass]
    public class ReduceExecutableSpec
    {
        [TestMethod]
        public void Enumerating_ShouldApplyMapperToAllItemsInSource()
        {
            var spy = new SpyExecutable<int>();

            var res =
                Enumerable.Range(0, 10)
                    .Pipe(spy)
                    .Pipe(new ReduceFuncExecutable<int, int>(0, (memo, curr) => memo + curr))
                    .Take(1)
                    .ToList();

            Assert.AreEqual(1, res.Count());
            Assert.AreEqual(10, spy.MapperHitCount.Count());
        }

        [TestMethod]
        public void Enumerating_ShouldCacheResult()
        {
            var spy = new SpyExecutable<int>();
            int numOfItems = 10;

            var res =
                Enumerable.Range(0, numOfItems)
                    .Pipe(spy)
                    .Pipe(new ReduceFuncExecutable<int, int>(0, (memo, curr) => memo + curr));

            res.ToList();

            int hitcount = spy.GetItemHitCount.Count();

            res.ToList();

            Assert.AreEqual(hitcount, spy.GetItemHitCount.Count());
        }

        [TestMethod]
        public void Enumerating_ShouldReturnTheSameItemForAllPositions()
        {
            Enumerable.Range(0, 10)
                .Pipe(new ReduceFuncExecutable<int, int>(0, (memo, curr) => memo + curr))
                .Aggregate((last, current) => {
                    Assert.AreEqual(last, current);
                    return current;
                });
        }
    }
}
