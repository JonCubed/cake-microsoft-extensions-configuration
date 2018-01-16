using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using System.Linq;

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
            public void Should_ReturnItemInInvalidArgsAndRestAsEmptyLists_If_NoOnlyArgumentIsBuildScriptWithUnknownExtension(string buildScript)
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

            [Theory]
            [MemberData(nameof(KnownCakeArgs))]
            public void Should_ReturnItemInCakeArgsAndRestAsEmptyLists_If_ArgumentIsKnownCakeArgument(string arg)
            {
                // act
                var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(new[] { arg });

                // assert
                cakeArgs.Should().NotBeNull();
                scriptArgs.Should().NotBeNull();
                invalidArgs.Should().NotBeNull();

                cakeArgs.Should().HaveCount(1, "because the only argument is a known cake argument");
                cakeArgs.Should().HaveElementAt(0, arg, "because it is a known cake argument");
                scriptArgs.Should().BeEmpty("because the only argument is a known cake argument");
                invalidArgs.Should().BeEmpty("because the only argument was the build script");

            }

            [Theory]
            [InlineData("-")]
            [InlineData("--")]
            [InlineData("/")]
            [InlineData("")]
            public void Should_Not_ReturnItemInInvalidArgs_If_ArgumentFormatIsValid(string prefix)
            {
                // arrange
                var args = new[] { $"{prefix}arg1=true" };
                var expectedItems = 1;

                if (!prefix.Equals(string.Empty))
                {
                    args = args
                            .Concat(new[] { $"{prefix}arg2", $"{prefix}arg3", "true" })
                            .ToArray();

                    expectedItems = args.Length + 1;
                }

                // act
                var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(args);

                // assert
                cakeArgs.Should().NotBeNull();
                scriptArgs.Should().NotBeNull();
                invalidArgs.Should().NotBeNull();

                cakeArgs.Should().BeEmpty("because the arguments are not known cake argument");
                scriptArgs.Should().HaveCount(expectedItems, "because the arguments are in a valid fomat");
                scriptArgs.Should().HaveElementAt(0, $"{prefix}arg1=true", "because the argument is in a valid format");

                if (args.Length == 4)
                {
                    scriptArgs.Should().HaveElementAt(1, $"{prefix}arg2", "because the argument is in a valid format");
                    scriptArgs.Should().HaveElementAt(3, $"{prefix}arg3", "because the argument is in a valid format");
                }

                invalidArgs.Should().BeEmpty("because the arguments are in a valid fomat");
            }

            [Theory]
            [InlineData("")]
            public void Should_ReturnItemInInvalidArgs_If_ArgumentFormatIsInvalid(string prefix)
            {
                // arrange
                var args = new[] { $"{prefix}arg1" };
                
                // act
                var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(args);

                // assert
                cakeArgs.Should().NotBeNull();
                scriptArgs.Should().NotBeNull();
                invalidArgs.Should().NotBeNull();

                cakeArgs.Should().BeEmpty("because the arguments are not known cake argument");
                scriptArgs.Should().BeEmpty("because the arguments are in an invalid format");
                invalidArgs.Should().HaveCount(args.Length, "because the argument is in an invalid format");
                invalidArgs.Should().HaveElementAt(0, $"{prefix}arg1", "because the argument is in an invalid format");
            }

            public static IEnumerable<object[]> KnownCakeArgs()
            {
                return CommandLineHelper
                            .KnownCakeCommandLineArguments
                            .SelectMany(arg => new[] { $"--{arg}=true", $"/{arg}=true", $"{arg}=true" })
                            .Select(arg => new object[] { arg });
            }

        }        
    }
}
