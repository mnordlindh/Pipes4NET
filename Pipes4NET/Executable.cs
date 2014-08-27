using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET {

    public abstract class Executable<TInput, TOutput> : BaseExecutable<TInput, TOutput> {
        public abstract TOutput Mapper(TInput input);

        public override bool TryGetItem(int index, out TOutput item) {
            if (!this.hasInput) {
                throw new InvalidOperationException("This pipeline has no source!");
            }

            if (this.TryGetCacheItem(index, out item)) {
                return true;
            }

            bool hasMoreItems;
            if (hasMoreItems = this.MoveNext()) {
                // we have a new item from source, send it to the mapper function
                item = this.Mapper(_inputEnumerator.Current);
                // cache it
                this.SetCacheItem(index, item);
            } else {
                // we do not have any cached items nor any from source
                item = default(TOutput);
            }

            return hasMoreItems;
        }

        protected bool MoveNext() {
            // ask the input enumerator for a new item
            return _inputEnumerator.MoveNext();
        }
    }
}
