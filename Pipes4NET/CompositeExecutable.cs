using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pipes4NET {
    public class CompositeExecutable<T> : Executable<T, object[]> {
        private IExecutable[] _executables;
        private IEnumerator[] _executablesEnumerators;
        protected new IEnumerable _input;

        public CompositeExecutable(params IExecutable[] tasks) {
            _executables = tasks;
            _executablesEnumerators = new IEnumerator[_executables.Length];
 
            for (int i = 0; i < _executables.Length; i++) {
                _executablesEnumerators[i] = _executables[i].GetEnumerator();
            }
        }

        public override object[] Mapper(T input) {
            throw new NotImplementedException();
        }

        public override void SetInput(IEnumerable input) {
            base.SetInput(input);

            for (int i = 0; i < _executables.Length; i++) {
                _executables[i].SetInput(input);
            }
        }

        public override bool TryGetItem(int index, out object[] item) {

            if (!this.hasInput) {
                throw new InvalidOperationException("This pipeline has no source!");
            }

            if (this.TryGetCacheItem(index, out item)) {
                return true;
            }

            item = new object[_executablesEnumerators.Length];
            bool hasMoreItems = false;

            // move all enumerators forward
            for (int i = 0; i < _executablesEnumerators.Length; i++) {
                // we have new items from source, bundle them as an array
                if (hasMoreItems = _executablesEnumerators[i].MoveNext()) {
                    item[i] = _executablesEnumerators[i].Current;
                }
            }

            if (hasMoreItems) {
                // cache the item
                this.SetCacheItem(index, item);
            }

            return hasMoreItems;
        }
    }
}
