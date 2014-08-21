using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.Tests {
    internal class SpyExecutable<TIn> : Executable<TIn, TIn> {
        internal Dictionary<TIn, int> HitCount = new Dictionary<TIn, int>();

        public override TIn Mapper(TIn input) {
            if (HitCount.ContainsKey(input)) {
                HitCount[input]++;
            } else {
                HitCount[input] = 1;
            }

            return input;
        }
    }
}
