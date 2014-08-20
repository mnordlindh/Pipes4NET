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

        public override bool GetItem(int index, out TInput item) {
            bool isCached = _cache.ContainsKey(index);

            if (isCached) {
                item = _cache[index];
            } else {
                bool first = _currentInnerEnumerator == null;
                if (first || !(isCached = _currentInnerEnumerator.MoveNext())) {
                    // proceed to next inner enumerator
                    isCached = _inputEnumerator.MoveNext();

                    if (_inputEnumerator.Current == null) {
                        _currentInnerEnumerator = null;
                    } else if (isCached) {
                        _currentInnerEnumerator = _inputEnumerator.Current.GetEnumerator();
                    }
                }

                // take a step in the inner enumerator if we were at the first one AND
                // that we have an enumerator (we do not when the current item is a null value).
                if (first && _currentInnerEnumerator != null)
                    isCached = _currentInnerEnumerator.MoveNext();

                // get the item
                if (_currentInnerEnumerator != null) {
                    item = _currentInnerEnumerator.Current;
                } else {
                    item = default(TInput);
                }

                // cache it
                if (isCached)
                    _cache[index] = item;

                return isCached;
            }

            return isCached;
        }
    }
}
