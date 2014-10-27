using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.NUnit {
    public class NUnitTestRunner : Pipes4NET.Executable<Type, IEnumerable<TestRunResult>> {

        TestRunner _testRunner;
        string _pathToTestAssembly;

        public NUnitTestRunner(string pathToTestAssembly) {
            _pathToTestAssembly = pathToTestAssembly;
        }

        public override IEnumerable<TestRunResult> Mapper(Type input) {
            if (_testRunner == null)
                _testRunner = new TestRunner(_pathToTestAssembly);

            try {
                return _testRunner.RunAllTests(input);
            } catch (Exception) {
                return null;
            }
        }
    }
}
