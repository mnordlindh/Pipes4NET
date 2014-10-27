using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.IO {
    public class FindContainingFolder : Executable<string, string> {
        protected string _pattern;
        protected SearchOption _option = SearchOption.TopDirectoryOnly;

        public FindContainingFolder(string pattern) {
            _pattern = pattern;
        }

        public FindContainingFolder(string pattern, SearchOption option)
            : this(pattern) {
            _option = option;
        }

        public override string Mapper(string input) {
            if (input == null) return null;

            var files = MiscIO.GetFiles(input, _pattern, _option);

            // get parent folder of the first match
            return new DirectoryInfo(files.First()).Parent.FullName;
        }
    }
}
