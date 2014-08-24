using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public class FlattenExecutable<TInput> : BaseExecutable<IEnumerable<TInput>, TInput> {

        private IEnumerator<TInput> _currentInnerEnumerator;

        //public override IEnumerator<TInput> GetEnumerator() {
        //    return new ExecutableEnumerator<IEnumerable<TInput>, TInput>(this);
        //}

        public override bool TryGetItem(int index, out TInput item) {
            if (!this.hasInput) {
                throw new InvalidOperationException("This pipeline has no source!");
            }

            if (this.TryGetCacheItem(index, out item)) {
                return true;
            }
            
            bool hasMoreItems;
            if (hasMoreItems = this.MoveNext()) {
                item = this._currentInnerEnumerator.Current;
                // cache it
                this.SetCacheItem(index, item);
            } else {
                // we do not have any cached items nor any from source
                item = default(TInput);
            }

            return hasMoreItems;
        }

        protected bool MoveNext() {
            // ask the input enumerator for a new item

            while (_currentInnerEnumerator == null || !_currentInnerEnumerator.MoveNext()) {
                // either we do not have a iterator or its out of items
                // move to the next source in the input
                if (!_inputEnumerator.MoveNext()) {
                    // no more sources
                    return false;
                }

                // get the next enumerator
                _currentInnerEnumerator = _inputEnumerator.Current.GetEnumerator();
            }

            return true;
        }
    }
}
