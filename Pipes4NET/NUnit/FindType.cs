using Pipes4NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.NUnit {
    public class FindType : Executable<string, Type> {

        Type _typeToLookFor;

        public FindType(Type typeToLookFor) {
            _typeToLookFor = typeToLookFor;
        }

        public override Type Mapper(string input) {
            var res = SearchImplementingType(new DirectoryInfo(input));

            return res != null ? res.Item2 : null;
        }

        /// <summary>
        /// Search recursively through the directory structure after a type implementing the interface.
        /// </summary>
        /// <param name="dir">The directory to recursively traverse.</param>
        private Tuple<Assembly, Type, string> SearchImplementingType(DirectoryInfo dir) {
            var file =
                this.SearchDirectory(
                dir,
                (f) => {

                    if (f.Name.EndsWith(".exe") || f.Name.EndsWith(".dll") &&
                        // TODO: check this...
                        !f.Name.Contains("vshost")) {

                        return GetInfoFromImplementingType(new List<string>() { f.FullName }) != null;
                    } else
                        return false;
                });

            if (file == null)
                return null;

            return GetInfoFromImplementingType(new List<string>() { file.FullName });
        }

        /// <summary>
        /// Search recursively through the directory structure for the first file matching the criterion function.
        /// </summary>
        /// <param name="dir">The directory to recursively traverse.</param>
        /// <param name="criterion">The criterion function.</param>
        private FileInfo SearchDirectory(DirectoryInfo dir, Func<FileInfo, bool> criterion) {

            var directories = dir.EnumerateDirectories();
            FileInfo result;
            foreach (var d in directories) {
                if ((result = SearchDirectory(d, criterion)) != null)
                    return result;
            }

            var files = dir.EnumerateFiles();

            foreach (var file in files) {
                if (criterion(file))
                    return file;
            }

            return null;
        }

        /// <summary>
        /// Get the first type in any of the assemblies that implements the interface.
        /// </summary>
        /// <param name="filePaths">filepaths to assemblies.</param>
        public Tuple<Assembly, Type, string> GetInfoFromImplementingType(IEnumerable<string> filePaths) {
            foreach (var filePath in filePaths) {
                var fileInfo = new FileInfo(filePath);

                var currentAssembly = Assembly.LoadFile(fileInfo.FullName);
                var typesInAssembly = currentAssembly.GetTypes();

                var type = typesInAssembly.FirstOrDefault(t => !t.IsInterface && t.GetInterfaces().Contains(_typeToLookFor));

                if (type != null && !type.IsInterface)
                    return new Tuple<Assembly, Type, string>(currentAssembly, type, filePath);
            }

            return null;
        }
    }
}
