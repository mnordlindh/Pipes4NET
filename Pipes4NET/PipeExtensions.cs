using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes4NET {
    public static class PipeExtensions {
        /// <summary>
        /// Pipes the source into the executable.
        /// </summary>
        public static IExecutable<T, TOut> Pipe<T, TOut>(this IEnumerable<T> source, IExecutable<T, TOut> executable) {
            IdentityExecutable<T> exec = new IdentityExecutable<T>();
            exec.SetInput(source);

            exec.Pipe(executable);

            return executable;
        }

        /// <summary>
        /// Transforms and pipes the source into the executable.
        /// </summary>
        public static IExecutable<T, TOut> Pipe<T, TOut>(this T source, IExecutable<T, TOut> executable) {
            IdentityExecutable<T> exec = new IdentityExecutable<T>();
            exec.SetInput(new List<T>() { source });

            exec.Pipe(executable);

            return executable;
        }
    }
}
