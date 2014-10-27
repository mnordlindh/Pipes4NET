using Pipes4NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipes4NET.Microsoft {
    public class MSBuildCompile : Executable<string, string> {
        string _defaultFlags = "/p:Configuration=Debug";
        string _flags, _msBuildPath;

        public MSBuildCompile(string msbuildPath, string flags)
            : this(msbuildPath) {
            _flags = flags ?? _defaultFlags;
        }

        public MSBuildCompile(string msbuildPath) {
            _msBuildPath = msbuildPath ?? getMsBuildPath();
        }

        public MSBuildCompile() {
            _msBuildPath = getMsBuildPath();
        }

        public override string Mapper(string input) {

            this.compile(input);

            return input;
        }

        string getMsBuildPath() {
            string windir = Environment.ExpandEnvironmentVariables("%windir%");
            string[] paths = new string[] {
                windir+"\\Microsoft.NET\\Framework\\v4.0.30319",
                windir+"\\Microsoft.NET\\Framework\\v3.5"
            };

            for (int i = 0; i < paths.Length; ++i) {
                if (Directory.Exists(paths[i])) {
                    return paths[i];
                }
            }

            throw new PlatformNotSupportedException("No directory with MSBuild.exe");
        }

        void compile(string pathToProjectFolder) {

            string cmd = String.Format("/C cd {0} && {1}", pathToProjectFolder, _msBuildPath + "\\MSBuild.exe");

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd;
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();
        }
    }
}
