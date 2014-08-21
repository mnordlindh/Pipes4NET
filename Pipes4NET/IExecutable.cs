using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public interface IExecutable : IEnumerable {
        void SetInput(IEnumerable input);
        //IExecutable Pipe(IExecutable executable);
    }

    public interface IExecutable<TIn, TOut> : IExecutable<TOut> {
        //void SetInput(IEnumerable<TIn> input);
    }

    public interface IExecutable<TOut> : IExecutable, IEnumerable<TOut> {
        IExecutable<TOut2> Pipe<TOut2>(IExecutable<TOut, TOut2> executable);
        IEnumerable<TOut> Exec();
    }

}
