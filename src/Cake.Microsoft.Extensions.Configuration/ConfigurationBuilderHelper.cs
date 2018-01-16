using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cake.Microsoft.Extensions.Configuration
{
    public static class ConfigurationBuilderHelper
    {
        public static IConfiguration LoadConfiguration(Action<IConfigurationBuilder, string[]> initialiseAction, ICollection<string> args = null)
        {
            var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs(args);

            foreach (var arg in invalidArgs)
            {
                Console.WriteLine($"{arg} is not in the correct format for Microsoft.Extensions.Configuration.CommandLine and has been ignored.");
            }

            var builder = new ConfigurationBuilder();

            initialiseAction?.Invoke(builder, scriptArgs.ToArray());

            return builder.Build();
        }


        /// <summary>
        /// Default configuration builder setup
        /// </summary>
        /// <param name="localConfiguration">Dictionary of configuration to initalise configuration with</param>
        /// <param name="commandLineSwitchMappings">Dictionary of command line arguments to replace if present</param>
        /// <returns>Return action for setting up configuration builder</returns>
        public static Action<IConfigurationBuilder, string[]> DefaultLoadConfigurationStrategy(IDictionary<string, string> localConfiguration = null, IDictionary<string, string> commandLineSwitchMappings = null, string settingsPath = null)
        {
            return (builder, args) =>
            {
                builder
                    .AddInMemoryCollection(localConfiguration)
                    .AddJsonFile(settingsPath ?? "build-settings.json", true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args, commandLineSwitchMappings)
                ;
            };
        }
    }
}
