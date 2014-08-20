using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public class IdentityExecutable<T> : Executable<T, T> {
        public override T Mapper(T input) {
            return input;
        }
    }
}
