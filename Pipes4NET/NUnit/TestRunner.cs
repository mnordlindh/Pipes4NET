using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pipes4NET.NUnit {
    /// <summary>
    /// Represents a class that can execute NUnit tests from an assembly against a type under test.
    /// </summary>
    class TestRunner {

        /// <summary>
        /// Information about the test-fixture.
        /// </summary>
        public TestInfo TestInfo { get; private set; }

        Type _typeUnderTest;
        object _testTypeInstance;
        object _instanceUnderTest;

        /// <summary>
        /// The type which at the moment is under test.
        /// </summary>
        public Type TypeUnderTest {
            set {
                if (value == null) throw new ArgumentNullException("value", "TypeUnderTest can not be null");

                _typeUnderTest = value;

                // Create an instance to test
                _instanceUnderTest = TestRunner.GetInstance(_typeUnderTest);

                // Create an instance of the test-class
                _testTypeInstance =
                    this.TestInfo.TestFixtureType.InvokeMember(
                    null, BindingFlags.CreateInstance, null, null, new object[] { _instanceUnderTest, TypeUnderTest });
            }
            get {
                return _typeUnderTest;
            }
        }

        /// <summary>
        /// Instantiate a new test-runner instance with the tests from the assembly.
        /// </summary>
        /// <param name="assemblyWithTests">The assembly with tests.</param>
        public TestRunner(string assemblyWithTests) {
            // Initialize tests
            LoadTests(assemblyWithTests);
        }

        /// <summary>
        /// Initiate the test-runner by loading all tests from the assembly.
        /// </summary>
        /// <param name="assemblyWithTests">The assembly with tests.</param>
        private void LoadTests(string assemblyWithTests) {
            //var testAssembly = Assembly.Load(assemblyWithTests);
            var testAssembly = Assembly.LoadFrom(assemblyWithTests);

            var testFixtures = GetTestFixturesFromAssembly(testAssembly);

            TestInfo testInfo = null;
            // TODO: now only loads the last testfixture, need to make this handle multiple test-fixture-classes
            foreach (var testFixtureType in testFixtures) {
                testInfo = GetTestInfoForTestFixture(testFixtureType);
            }

            TestInfo = testInfo;
        }

        /// <summary>
        /// Get all classes with the data-annotation [TestFixture] in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to be traversed.</param>
        private static IEnumerable<Type> GetTestFixturesFromAssembly(Assembly assembly) {
            var types = assembly.GetTypes();
            var typeList = new List<Type>();

            foreach (var type in types) {
                var attributes = type.GetCustomAttributes(true);
                if (attributes.Any(a => a is TestFixtureAttribute)) {
                    typeList.Add(type);
                }
            }

            return typeList;
        }

        /// <summary>
        /// Gets member information from the type with tests.
        /// </summary>
        /// <param name="testType">The type with tests.</param>
        private static TestInfo GetTestInfoForTestFixture(Type testType) {
            var testMembers = testType.GetMembers();

            // Create the TestInfo
            var testInfo = new TestInfo();
            testInfo.TestFixtureType = testType;
            //testInfo.Instance = testClassInstance;

            // Get all test associated members from the test-class-members
            foreach (var member in testMembers) {
                var memberAttributes = member.GetCustomAttributes(true);

                // Get SetUp                
                if (memberAttributes.Any(a => a is SetUpAttribute)) {
                    if (testInfo.Setup == null)
                        testInfo.Setup = member;
                    else
                        throw new CustomAttributeFormatException("More than one setup-method in test-class.");
                }

                // Get TearDown
                if (memberAttributes.Any(a => a is TearDownAttribute)) {
                    if (testInfo.TearDown == null)
                        testInfo.TearDown = member;
                    else
                        throw new CustomAttributeFormatException("More than one teardown-method in test-class.");
                }

                // Get all Tests
                //Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
                if (memberAttributes.Any(a => a is TestAttribute))
                    testInfo.Members.Add(member);

                // Get all methods with Exception-Excpected-attributes
                if (memberAttributes.Any(a => a is ExpectedExceptionAttribute)) {
                    foreach (var a in memberAttributes) {
                        var exceptionAttr = a as ExpectedExceptionAttribute;

                        if (exceptionAttr != null)
                            testInfo.ExpectedExceptions.Add(member.Name, exceptionAttr.ExpectedException);
                    }
                }
            }

            return testInfo;
        }

        /// <summary>
        /// Instantiate a object from a type.
        /// </summary>
        /// <param name="type">The type to instantiate an object from.</param>
        public static object GetInstance(Type type) {
            object instanceToTest = null;

            if (type.ContainsGenericParameters) {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                // If the implementing-type is generic 
                // set the generic type to be of type object
                Type[] typeArgs = { typeof(object) };

                type = type.MakeGenericType(typeArgs);
            }

            // create the instance
            instanceToTest = Activator.CreateInstance(type);

            return instanceToTest;
        }

        /// <summary>
        /// Use an assembly with tests to test a type implementing a common test-interface. 
        /// </summary>
        /// <param name="typeToTest">The type to be tested.</param>
        public IEnumerable<TestRunResult> RunAllTests(Type typeToTest) {
            // Set the current type to be tested
            this.TypeUnderTest = typeToTest;

            // Run the tests
            Dictionary<string, TestRunResult> result = InvokeTests(this.TestInfo, this._testTypeInstance);

            if (this.TestInfo != null)
                return result.Values;
            else
                return null;
        }

        /// <summary>
        /// Invoke all tests in a test-fixture class.
        /// </summary>
        /// <param name="testInfo">Test information.</param>
        /// <param name="testTypeInstance">Test type instance.</param>
        private Dictionary<string, TestRunResult> InvokeTests(TestInfo testInfo, object testTypeInstance) {
            Type testType = testInfo.TestFixtureType;
            // Create a new result-objekt
            var testRunResults = new Dictionary<string, TestRunResult>();

            // Invoke all test-members.
            // Invoking setup before and TearDown after each run.
            foreach (var member in testInfo.Members) {
                // Invoke SetUp before each test
                if (testInfo.Setup != null)
                    testType.InvokeMember(testInfo.Setup.Name, BindingFlags.InvokeMethod, null, testTypeInstance, null);

                // Initialize a testRunInfo presuming it will fail
                var testRunInfo = new TestRunResult() {
                    Name = member.Name,
                    Result = TestRunResult.TestOutcome.Failed
                };

                testRunResults.Add(member.Name, testRunInfo);

                // Run actual test
                try {
                    testType.InvokeMember(member.Name, BindingFlags.InvokeMethod, null, testTypeInstance, null);

                    // Checks if the test SHOULD throw an exception
                    if (!testInfo.ExpectedExceptions.ContainsKey(member.Name))
                        testRunInfo.Result = TestRunResult.TestOutcome.Passed;
                    else
                        testRunInfo.Reason = (Exception)testInfo.ExpectedExceptions[member.Name].InvokeMember(
                            null,
                            BindingFlags.CreateInstance,
                            null,
                            null,
                            new object[] { testInfo.ExpectedExceptions[member.Name].ToString() + " was expected" });
                }
                    // If test failes, store the exception, still marked as failed.
                catch (Exception e) {
                    Type expected = null;

                    if (testInfo.ExpectedExceptions.TryGetValue(member.Name, out expected)) {
                        var type1 = expected;
                        var type2 = e.InnerException.GetType();
                        if (Object.ReferenceEquals(type1, type2))
                            testRunInfo.Result = TestRunResult.TestOutcome.Passed;
                        else
                            testRunInfo.Reason = e.InnerException;
                    } else
                        testRunInfo.Reason = e.InnerException;
                }

                // Invoke TearDown after each test
                if (testInfo.TearDown != null)
                    testType.InvokeMember(testInfo.TearDown.Name, BindingFlags.InvokeMethod, null, testTypeInstance, null);
            }

            return testRunResults;
        }
    }
}
