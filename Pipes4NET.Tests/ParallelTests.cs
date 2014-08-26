using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pipes4NET.Tests {
    [TestClass]
    public class ParallelTests {
        [TestMethod]
        public void AsParallel() {
            int cores = Environment.ProcessorCount;

            var spy = new ParallelSpyExecutable<int>();
            int items = 1000;

            Enumerable.Range(0, items).Pipe(spy).AsParallel().Where(x => true).ToList();

            Assert.AreEqual(items, spy.MapperHitCount.Count());
            var allItemsHitOnce = spy.MapperHitCount.All(kvp => kvp.Value == 1);

            Assert.IsTrue(allItemsHitOnce);
        }

        [TestMethod]
        public void AsParallel_WithReduceExecutable() {
            int cores = Environment.ProcessorCount;

            var spy = new ParallelSpyExecutable<int>();
            int items = 1000;

            var res =
                Enumerable.Range(0, items)
                    .Pipe(spy)
                    .AsParallel()
                    .Where(x => true)
                    .AsParallel()
                    .Pipe(new CompositeExecutable<int>(
                        new IdentityExecutable<int>(),
                        new IdentityExecutable<int>(),
                        new ReduceFuncExecutable<int, int>(0, (memo, curr) => memo + curr)
                    ))
                    .AsParallel()
                    .Where(x => true)
                    //.Where(x => (int)x[0] > 500)
                    .ToList();

            Assert.AreEqual(items, spy.MapperHitCount.Count());
            var allItemsHitOnce = spy.MapperHitCount.All(kvp => kvp.Value == 1);

            Assert.IsTrue(allItemsHitOnce);
        }
    }
}
