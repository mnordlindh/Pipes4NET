using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.Tests {
    internal class SpyExecutable<TIn> : Executable<TIn, TIn> {
        internal Dictionary<TIn, int> MapperHitCount = new Dictionary<TIn, int>();
        internal Dictionary<int, int> GetItemHitCount = new Dictionary<int, int>();

        public override bool GetItem(int index, out TIn item) {
            if (GetItemHitCount.ContainsKey(index)) {
                GetItemHitCount[index]++;
            } else {
                GetItemHitCount[index] = 1;
            }

            return base.GetItem(index, out item);
        }

        public override TIn Mapper(TIn input) {
            if (MapperHitCount.ContainsKey(input)) {
                MapperHitCount[input]++;
            } else {
                MapperHitCount[input] = 1;
            }

            return input;
        }
    }
}
