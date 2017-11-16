using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace Cake.Microsoft.Extensions.Configuration.Tests
{
    public sealed class CommandLineHelperFixtures
    {
        public sealed class TheParseCommandLineArgsMethod
        {
            [Theory]
            [InlineData(null)]
            [InlineData(new object[] { new string[] { } })]
            public void Should_ReturnEmptyLists_If_NoArgs(ICollection<string> args)
            {
                // act
                var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(args);

                // assert
                cakeArgs.Should().NotBeNull();
                scriptArgs.Should().NotBeNull();
                invalidArgs.Should().NotBeNull();

                cakeArgs.Should().BeEmpty("Because there are no arguments");
                scriptArgs.Should().BeEmpty("Because there are no arguments");
                invalidArgs.Should().BeEmpty("Because there are no arguments");
            }

            [Theory]
            [InlineData("build.cake")]
            [InlineData("build.csx")]
            public void Should_ReturnEmptyLists_If_NoOnlyArgumentIsBuildScript(string buildScript)
            {
                // act
                var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(new[] { buildScript });

                // assert
                cakeArgs.Should().NotBeNull();
                scriptArgs.Should().NotBeNull();
                invalidArgs.Should().NotBeNull();

                cakeArgs.Should().BeEmpty("because the only argument was the build script");
                scriptArgs.Should().BeEmpty("because the only argument was the build script");
                invalidArgs.Should().BeEmpty("because the only argument was the build script");
            }

            [Theory]
            [InlineData("build.ps1")]
            [InlineData("build.sh")]
            public void Should_ReturnItemInInvalidArgsAndRestEmptyLists_If_NoOnlyArgumentIsBuildScriptWithUnknownExtension(string buildScript)
            {
                // act
                var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(new[] { buildScript });

                // assert
                cakeArgs.Should().NotBeNull();
                scriptArgs.Should().NotBeNull();
                invalidArgs.Should().NotBeNull();

                cakeArgs.Should().BeEmpty("because the only argument was the build script");
                scriptArgs.Should().BeEmpty("because the only argument was the build script");
                invalidArgs.Should().HaveCount(1, "because the only argument was the build script with an unknown extension");
                invalidArgs.Should().HaveElementAt(0, buildScript, "because it has an unknown extension");
            }            
        }

        [Fact]
        public void Test1()
        {
            var fileSystem = new Core.IO.FileSystem();
            var log = new Core.Diagnostics.CakeBuildLog(new Core.CakeConsole());
            var environment = new Core.CakeEnvironment(new Core.CakePlatform(), new Core.CakeRuntime(), log);
            var globber = new Core.IO.Globber(fileSystem, environment);
            var arguments = new Arguments();
            var context = new Cake.Core.CakeContext(fileSystem, environment, globber , log, arguments, new Core.IO.ProcessRunner(environment, log), new Core.IO.WindowsRegistry(), new Core.Tooling.ToolLocator(environment, new Core.Tooling.ToolRepository(environment), new Core.Tooling.ToolResolutionStrategy(fileSystem,environment,globber, new Core.Configuration.CakeConfiguration(new Dictionary<string, string>()))));
            var settings = ConfigurationAlias.GetConfiguration<MyClass>(context);
        }

        private class MyClass
        {
            public string Value { get; set; }
        }

        private class Arguments : Core.ICakeArguments
        {
            public string GetArgument(string name)
            {
                throw new NotImplementedException();
            }

            public bool HasArgument(string name)
            {
                throw new NotImplementedException();
            }
        }
    }
}
