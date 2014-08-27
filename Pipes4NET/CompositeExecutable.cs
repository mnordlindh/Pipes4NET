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
            // is this function used on composite-executables?
            return null;
        }


        public override void SetInput(IEnumerable input) {
            //_input = input;
            //IEnumerator _inputEnumerator = input.GetEnumerator();

            base.SetInput(input);

            for (int i = 0; i < _executables.Length; i++) {
                //Type type = _executables[i].GetType();
                //MethodInfo method = type.GetMethod("SetInput");
                //if (method != null) {
                //    method.Invoke(_executables[i], new object[] { input });
                //}
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

        //public new System.Collections.IEnumerator GetEnumerator() {
        //    return new ExecutableEnumerator<T, object[]>(this);
        //}
    }

    //public class CompositeExecutableEnumerator<T> : IEnumerator<object[]> {

    //    private CompositeExecutable<T> _current;
    //    private int _index;
    //    private object[] _currentItem;

    //    public CompositeExecutableEnumerator(CompositeExecutable<T> tasks) {
    //        _current = tasks;
    //        _index = -1;
    //        _currentItem = null;
    //    }

    //    public object[] Current {
    //        get { return _currentItem; }
    //    }

    //    public void Dispose() {

    //    }

    //    object System.Collections.IEnumerator.Current {
    //        get { return Current; }
    //    }

    //    public bool MoveNext() {
    //        // move the cursor forward
    //        _index++;

    //        // ask the executable for a cached item or the next item from source
    //        return _current.GetItem(_index, out _currentItem);
    //    }

    //    public void Reset() {
    //        _index = -1;
    //    }
    //}
}
