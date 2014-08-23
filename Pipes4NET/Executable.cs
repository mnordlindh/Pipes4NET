using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET {

    public abstract class Executable<TInput, TOutput> : BaseExecutable<TInput, TOutput> {
        public abstract TOutput Mapper(TInput input);

        public override IEnumerator<TOutput> GetEnumerator() {
            return new ExecutableEnumerator<TInput, TOutput>(this);
        }

        // Caching
        public override bool TryGetItem(int index, out TOutput item) {
            if (!this.hasInput) {
                throw new InvalidOperationException("This pipeline has no source!");
            }

            bool isCached = _cache.ContainsKey(index);

            if (isCached) {
                item = _cache[index];
            } else {
                // ask the input enumerator for a new item
                isCached = _inputEnumerator.MoveNext();

                if (isCached) {
                    // we have a new item from source, send it to the mapper function
                    item = this.Mapper(_inputEnumerator.Current);
                    // cache it
                    _cache[index] = item;
                } else {
                    // we do not have any cached items nor any from source
                    item = default(TOutput);
                }
            }

            return isCached;
        }

    }

    public class ExecutableEnumerator<TInput, TOutput> : IEnumerator<TOutput> {

        protected BaseExecutable<TInput, TOutput> _current;
        protected int _index;
        protected TOutput _currentItem;

        public ExecutableEnumerator(BaseExecutable<TInput, TOutput> current) {
            _current = current;
            _index = -1;
            _currentItem = default(TOutput);
        }

        public virtual TOutput Current {
            get { return _currentItem; }
        }

        public void Dispose() {

        }

        object System.Collections.IEnumerator.Current {
            get { return Current; }
        }

        public virtual bool MoveNext() {
            // move the cursor forward
            _index++;

            // ask the executable for a cached item or the next item from source
            return _current.TryGetItem(_index, out _currentItem);
        }

        public virtual void Reset() {
            _index = -1;
        }
    }
}
