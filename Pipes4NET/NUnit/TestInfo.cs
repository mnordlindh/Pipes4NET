using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.NUnit {
    class TestInfo {
        public List<MemberInfo> Members { get; set; }
        public MemberInfo Setup { get; set; }
        public MemberInfo TearDown { get; set; }
        public Dictionary<string, Type> ExpectedExceptions { get; set; }
        public Type TestFixtureType { get; set; }
        public TestInfo() {
            Members = new List<MemberInfo>();
            ExpectedExceptions = new Dictionary<string, Type>();
        }
    }
}
