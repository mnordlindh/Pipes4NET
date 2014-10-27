using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET {
    public class Last<T> : Executable<IEnumerable<T>, T> {
        public override T Mapper(IEnumerable<T> input) {
            return input.Last();
        }
    }
}
