using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public class FlattenExecutable<TInput> : BaseExecutable<IEnumerable<TInput>, TInput> {

        private IEnumerator<TInput> _currentInnerEnumerator;

        public override IEnumerator<TInput> GetEnumerator() {
            return new ExecutableEnumerator<IEnumerable<TInput>, TInput>(this);
        }


        public override bool TryGetItem(int index, out TInput item) {
            bool isCached = _cache.ContainsKey(index);

            // if we have a cached value for this index just return it
            if (isCached) {
                item = _cache[index];
                return true;
            }

            while (_currentInnerEnumerator == null || !_currentInnerEnumerator.MoveNext()) {
                // either we do not have a iterator or its out of items
                // move to the next source in the input
                if (!_inputEnumerator.MoveNext()) {
                    // no more sources
                    item = default(TInput);
                    return false;
                }

                // get the next enumerator
                _currentInnerEnumerator = _inputEnumerator.Current.GetEnumerator();
            }

            // here we have an enumerator and it has been moved into the right position
            item = _currentInnerEnumerator.Current;

            // cache it
            _cache[index] = item;

            return true;
        }
    }
}
