using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public abstract class BaseExecutable<TInput, TOutput> : IExecutable<TInput, TOutput> {
        protected IExecutable<TInput> _input;
        protected IEnumerator<TInput> _inputEnumerator;
        protected IDictionary<int, TOutput> _cache;

        public BaseExecutable() {
            _cache = new Dictionary<int, TOutput>();
        }

        public virtual void SetInput(IEnumerable input) {
            if (_input == null) {
                IEnumerable<TInput> i = (IEnumerable<TInput>)input;
                if (input is IExecutable)
                    _input = (IExecutable<TInput>)input;
                _inputEnumerator = i.GetEnumerator();
            } else {
                // if this executable already has a source, recursively trawel
                // to the first executable in the chain and set its input
                _input.SetInput(input);
            }
        }

        public IExecutable Pipe(IExecutable executable) {
            executable.SetInput(this);

            return executable;
        }

        public IExecutable<TOut> Pipe<TOut>(IExecutable<TOutput, TOut> executable) {
            executable.SetInput(this);

            return executable;
        }

        public IEnumerable<TOutput> Exec() {
            //IEnumerator iterator = this.GetEnumerator();
            //while (iterator.MoveNext()) {
            //    yield return (TOutput)iterator.Current;
            //}
            // the foreach will iterate better then above..
            foreach (var item in this) {
                yield return item;
            }
        }

        public abstract IEnumerator<TOutput> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // Caching
        public abstract bool GetItem(int index, out TOutput item);
    }
}
