using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipes4NET.Tests {
    class ParallelSpyExecutable<TIn> : SpyExecutable<TIn> {
        internal int NumOfThreads = 0;
        internal Dictionary<int, int> ThreadsHitCount = new Dictionary<int, int>();
        internal Dictionary<TIn, int> ThreadsHitMapperCount = new Dictionary<TIn, int>();
        private object _lock = new object();

        public override TIn Mapper(TIn input) {

            lock (_lock) {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (ThreadsHitCount.ContainsKey(threadId)) {
                    ThreadsHitCount[threadId]++;
                } else {
                    ThreadsHitCount[threadId] = 1;

                    NumOfThreads++;
                }
            }

            if (ThreadsHitMapperCount.ContainsKey(input)) {
                ThreadsHitMapperCount[input]++;
            } else {
                ThreadsHitMapperCount[input] = 1;
            }

            return base.Mapper(input);
        }
    }
}
