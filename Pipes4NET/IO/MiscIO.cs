using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.IO {
    class MiscIO {
        public static string[] GetFiles(string sourceFolder, string filter, System.IO.SearchOption searchOption) {
            // ArrayList will hold all file names
            List<string> allFiles = new List<string>();

            // Create an array of filter string
            string[] multipleFilters = filter.Split('|');

            // for each filter find mathing file names
            foreach (string fileFilter in multipleFilters) {
                // add found file names to array list
                allFiles.AddRange(Directory.GetFiles(sourceFolder, fileFilter, searchOption));
            }

            // returns string array of relevant file names
            return allFiles.ToArray();
        }
    }
}
