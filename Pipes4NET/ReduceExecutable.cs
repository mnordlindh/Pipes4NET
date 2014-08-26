using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public abstract class ReduceExecutable<TInput, TOutput> : Executable<TInput, TOutput> {

        protected TOutput _memo;
        protected new TOutput _cache;
        protected bool _cacheSet = false;

        public ReduceExecutable(TOutput memo) {
            _memo = memo;
        }

        public override TOutput Mapper(TInput input) {
            // if we already have a calculated value for
            // this reduce function return that
            if (_cacheSet) return _cache;

            TOutput last = _memo;

            foreach (var item in _input) {
                last = ReduceFunction(last, item);
            }

            _cache = last; _cacheSet = true;

            return last;
        }

        public abstract TOutput ReduceFunction(TOutput memo, TInput current);
    }
}
