using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress;

namespace Pipes4NET.IO {
    public class Unzip : Executable<string, string> {
        private string _folder;

        public Unzip() {
        }

        public Unzip(string folder) {
            _folder = folder;
        }
        private void MakeFolderWritable(string Folder) {
            if (IsFolderReadOnly(Folder)) {
                System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo(Folder);
                oDir.Attributes = oDir.Attributes & ~System.IO.FileAttributes.ReadOnly;
            }
        }
        private bool IsFolderReadOnly(string Folder) {
            System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo(Folder);
            return ((oDir.Attributes & System.IO.FileAttributes.ReadOnly) > 0);
        }
        private void MakeFileWritable(string file) {
            if (IsFileReadOnly(file)) {
                System.IO.FileInfo oDir = new System.IO.FileInfo(file);
                oDir.Attributes = oDir.Attributes & ~System.IO.FileAttributes.ReadOnly;
            }
        }
        private bool IsFileReadOnly(string file) {
            System.IO.FileInfo oDir = new System.IO.FileInfo(file);
            return ((oDir.Attributes & System.IO.FileAttributes.ReadOnly) > 0);
        }
        public override string Mapper(string input) {

            try {
                FileInfo file = new FileInfo(input);
                var dirName = file.DirectoryName;
                dirName += _folder != null ? "\\" + _folder : "";

                if (file != null) {

                    using(var stream = File.OpenRead(input))
	                {
                        var reader = SharpCompress.Reader.ReaderFactory.Open(stream);

                        Directory.CreateDirectory(dirName);

	                    while (reader.MoveToNextEntry())
	                    {
                            try {
                                if (!reader.Entry.IsDirectory) {
                                    var filePath = dirName + "\\" + reader.Entry.FilePath;

                                    var fileDir = new DirectoryInfo(filePath);
                                    if (!fileDir.Parent.Exists) {
                                        fileDir.Parent.Create();
                                    }

                                    using (var writer = File.Create(filePath)) {
                                        reader.WriteEntryTo(writer);
                                    }
                                }
                            } catch (Exception e) {
                                Console.WriteLine("Unzip errror: " + e.Message);
                                return dirName;
                            }
	                    }
	                }

                    //var zip = ZipFile.Read(input);

                    //zip.ExtractAll(dirName, ExtractExistingFileAction.OverwriteSilently);

                    return dirName;
                }
            } catch (Exception e2) {
                // make it blow instead?
                Console.WriteLine("IO errror: " + e2.Message);
            }

            return null;
        }
    }
}
