using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.NUnit {
    public class TestRunResult {
        public enum TestOutcome {
            Failed = 0,
            Passed = 1
        }

        public string Name { get; set; }
        public Exception Reason { get; set; }
        public TestOutcome Result { get; set; }

        public bool Succeeded {
            get { return Result == TestOutcome.Passed; }
        }

        public override string ToString() {
            string output = String.Format("[TestRun: {0}, Result: {1}", this.Name, this.Result);

            if (Reason != null)
                output += String.Format("\r\nReason: \r\n{0}]", Reason.Message);
            else
                output += "]";

            return output;
        }
    }
}
