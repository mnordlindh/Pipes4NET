﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public abstract class BaseExecutable<TInput, TOutput> : IExecutable<TInput, TOutput> {
        protected IExecutable<TInput> _input;
        protected IEnumerator<TInput> _inputEnumerator;
        protected IDictionary<int, TOutput> _cache;
        protected bool _inputIsSet = false;

        protected bool hasInput {
            get { return _input != null || _inputEnumerator != null; }
        }

        public BaseExecutable() {
            _cache = new Dictionary<int, TOutput>();
        }

        public virtual void SetInput(IEnumerable input) {
            if (!this.hasInput) {
                IEnumerable<TInput> i = (IEnumerable<TInput>)input;

                if (input is IExecutable<TInput>) {
                    _input = (IExecutable<TInput>)input;
                }
                _inputEnumerator = i.GetEnumerator();
            } else {
                if (_input != null) {
                    // if this executable already has a source, recursively trawel
                    // to the first executable in the chain and set its input
                    _input.SetInput(input);
                } else {
                    throw new InvalidOperationException("This pipeline already has an initial source!");
                }
            }
        }

        public IExecutable<TOut> Pipe<TOut>(IExecutable<TOutput, TOut> executable) {
            executable.SetInput(this);

            return executable;
        }

        public IEnumerable<TOutput> Exec() {
            foreach (var item in this) {
                yield return item;
            }
        }

        public abstract IEnumerator<TOutput> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // Caching
        public abstract bool TryGetItem(int index, out TOutput item);

    }
}
