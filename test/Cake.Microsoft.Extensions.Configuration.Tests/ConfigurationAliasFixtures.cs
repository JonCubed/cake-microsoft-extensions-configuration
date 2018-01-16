using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.Microsoft.Extensions.Configuration.Tests
{
    public sealed class ConfigurationAliasFixtures
    {
        [Fact]
        public void Test1()
        {
            var fileSystem = new Core.IO.FileSystem();
            var log = new Core.Diagnostics.CakeBuildLog(new Core.CakeConsole());
            var environment = new Core.CakeEnvironment(new Core.CakePlatform(), new Core.CakeRuntime(), log);
            var globber = new Core.IO.Globber(fileSystem, environment);
            var arguments = new Arguments();
            var context = new Core.CakeContext(fileSystem, environment, globber, log, arguments, new Core.IO.ProcessRunner(environment, log), new Core.IO.WindowsRegistry(), new Core.Tooling.ToolLocator(environment, new Core.Tooling.ToolRepository(environment), new Core.Tooling.ToolResolutionStrategy(fileSystem, environment, globber, new Core.Configuration.CakeConfiguration(new Dictionary<string, string>()))));
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
