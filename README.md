Pipes4NET
=========

## Examples

### Run NUnit tests against a lot of zipped-projects 
```C#
string pathToAssignments = Application.StartupPath + "/../../Files/";
string pathToTestAssembly = Application.StartupPath + "/../../Assemblies/TestsAssembly.dll";
Type typeToLookFor = typeof(InterfaceTypeToLookFor);

var query =
	pathToAssignments
    .Pipe(new FindFiles("ZipFileWithStudentHandins.zip"))
    .Pipe(new FlattenExecutable<string>())
    .Pipe(new Unzip("unzipped"))
    .Pipe(new Folders())
    .Pipe(new FlattenExecutable<string>())
    .Pipe(
        new CompositeExecutable<string>(
            new FindFiles(@"*.zip|*.rar")
                .Pipe(new MaxExecutable<string>())
                .Pipe(new Unzip("unzipped"))
                .Pipe(new FindContainingFolder("*.sln|*.csproj", System.IO.SearchOption.AllDirectories))
                .Pipe(new MSBuildCompile())
                .Pipe(new FindType(typeToLookFor))
                .Pipe(new NUnitTestRunner(pathToTestAssembly)),
            new IdentityExecutable<string>()
    ))
    .Select(item => {
        var testResults = item[0] as IEnumerable<TestRunResult>;
        var path = item[1] as string;
        var name = path.Substring(path.LastIndexOf('\\') + 1);

        return new Tuple<string, bool>(name, testResults != null ? testResults.All(r => r.Result == TestRunResult.TestOutcome.Passed) : false);
    });

var passed = query.Where(t => t.Item2).ToList();
var failed = query.Where(t => !t.Item2).ToList();
```