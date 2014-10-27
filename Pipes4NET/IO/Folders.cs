using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.IO {
    public class Folders : Executable<string, IEnumerable<string>> {
        public override IEnumerable<string> Mapper(string input) {
            if (input == null) return null;

            return Directory.EnumerateDirectories(input);
        }
    }
}
