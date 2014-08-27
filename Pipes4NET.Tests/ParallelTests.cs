using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

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

        [TestMethod]
        public void Parallel_Blah()
        {
            int cores = Environment.ProcessorCount;

            var spy = new ParallelSpyExecutable<int>();
            var spy2 = new ParallelSpyExecutable<object[]>();
            int items = 1000;

            var res =
                Enumerable.Range(0, items)
                    .Pipe(spy)
                    .AsParallel()
                    .Pipe(new CompositeExecutable<int>(
                        new IdentityExecutable<int>(),
                        new IdentityExecutable<int>(),
                        new ReduceFuncExecutable<int, int>(0, (memo, curr) => memo + curr)
                    ));
                    //.Pipe(spy2);

            List<object[]> results = new List<object[]>();
            List<string> str = new List<string>();

            //Parallel.ForEach(res, new ParallelOptions { MaxDegreeOfParallelism = cores },
            //(line, state, index) => {
            //    results.Add(line);
            //    str.Add(line[0] + " computed");
            //});

            var partition = Partitioner.Create(res);
            var staticPartitions = partition.GetPartitions(cores);
            int index = 0;
            
            Thread[] threads = new Thread[cores];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ThreadStart(() =>
                {
                    int myIndex = Interlocked.Increment(ref index) - 1; // compute your index 
                    var myItems = staticPartitions[myIndex]; // grab your static partition 
                    int id = Thread.CurrentThread.ManagedThreadId; // cache your thread id 

                    // Enumerate through your static partition 
                    while (myItems.MoveNext())
                    {
                        Thread.Sleep(10); // guarantees that multiple threads have a chance to run
                        lock (str)
                        {
                            str.Add(String.Format("  item = {0}, thread id = {1}", myItems.Current, id));
                        }
                        lock (results)
                        {
                            results.Add(myItems.Current);
                        }
                    }

                    myItems.Dispose();
                }));
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
                str.Add(String.Format("  All done!"));
            }

            Assert.AreEqual(items, spy.MapperHitCount.Count());
            var allItemsHitOnce = spy.MapperHitCount.All(kvp => kvp.Value == 1);
            Assert.IsTrue(allItemsHitOnce);
        }
    }
}
