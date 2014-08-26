using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET {

    public class ReduceFuncExecutable<TIn, TOut> : ReduceExecutable<TIn, TOut> {

        Func<TOut, TIn, TOut> _mapper;

        public ReduceFuncExecutable(TOut init, Func<TOut, TIn, TOut> mapper)
            : base(init) {
            _mapper = mapper;
        }

        public override TOut ReduceFunction(TOut memo, TIn current) {
            return _mapper(memo, current);
        }
    }
}
