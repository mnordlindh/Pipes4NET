using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.IO {
    public class FindFiles : Executable<string, IEnumerable<string>> {
        protected string _pattern;
        protected SearchOption _option = SearchOption.TopDirectoryOnly;
            
        public FindFiles(string pattern) {
            _pattern = pattern;
        }

        public FindFiles(string pattern, SearchOption option)
            : this(pattern) {
            _option = option;
        }

        public override IEnumerable<string> Mapper(string input) {
            if (input == null) return null;

            return MiscIO.GetFiles(input, _pattern, _option);
        }
    }
}
