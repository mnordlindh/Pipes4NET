using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET {
    public class MaxExecutable<TIn> : Executable<IEnumerable<TIn>, TIn>
        where TIn : IComparable<TIn> {
        public override TIn Mapper(IEnumerable<TIn> input) {
            return input.Max();
        }
    }
}
