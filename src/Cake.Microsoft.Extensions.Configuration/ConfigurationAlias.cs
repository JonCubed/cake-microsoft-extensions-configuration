using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace Cake.Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Cake build aliases for creating stongly typed configuration with Microsoft.Extensions.Configuration
    /// </summary>
    [CakeAliasCategory("Configuration")]
    [CakeAliasCategory("Microsoft.Extensions.Configuration")]
    [CakeNamespaceImport("Microsoft.Extensions.Configuration")]
    public static class ConfigurationAlias
    {
        private static IConfiguration _configuration;

        /// <summary>
        /// Gets configuration of <typeparamref name="T"/> using the default configuration builder
        /// that will load configuration in the following order
        /// 1. Environment Variables (All)
        /// 2. Command line (without Cake arguments)
        /// </summary>
        /// <typeparam name="T">Type of configuration to return</typeparam>
        /// <param name="context">The cake context</param>
        /// <returns>Return configuration of <typeparamref name="T"/></returns>
        /// <example>
        /// <code>
        ///     var settings = GetConfiguration<MyScriptSettings>();
        /// </code>
        /// </example>
        [CakeAliasCategory("Get")]
        [CakeMethodAlias]
        public static T GetConfiguration<T>(this ICakeContext context) where T : new()
        {
            return context.GetConfiguration<T>(localConfiguration: null, commandLineSwitchMappings: null);
        }

        /// <summary>
        /// Gets configuration of <typeparamref name="T"/> using the default configuration builder
        /// that will load configuration in the following order
        /// 1. In-Memory Collection
        /// 2. Environment Variables (All)
        /// 3. Command line (without Cake arguments)
        /// </summary>
        /// <typeparam name="T">Type of configuration to return</typeparam>
        /// <param name="context">The cake context</param>
        /// <param name="localConfiguration">Dictionary of configuration to initalise configuration with</param>
        /// <param name="commandLineSwitchMappings">Dictionary of command line arguments to replace if present</param>
        /// <returns>Return configuration of <typeparamref name="T"/></returns>
        /// <example>
        /// <code>
        ///     var defaults = new Dictionary()
        ///     {
        ///         ["environment"] = "local",
        ///         ["concurrency"] = "10"
        ///     };
        ///
        ///     var switchMappings = new Dictionary()
        ///     {
        ///         ["-e"] = "--environment",
        ///         ["-c"] = "--concurrency"
        ///     };
        ///
        ///     var settings = GetConfiguration<MyScriptSettings>(defaults, switchMappings);
        /// </code>
        /// </example>
        [CakeAliasCategory("Get")]
        [CakeMethodAlias]
        public static T GetConfiguration<T>(this ICakeContext context, IDictionary<string, string> localConfiguration = null, IDictionary<string, string> commandLineSwitchMappings = null) where T : new()
        {
            var configuration = new T();
            context.GetConfiguration(configuration, localConfiguration, commandLineSwitchMappings);
            return configuration;
        }

        /// <summary>
        /// Binds the configuration to <paramref name="instance"/> using the default configuration builder
        /// that will load configuration in the following order
        /// 1. In-Memory Collection
        /// 2. Environment Variables (All)
        /// 3. Command line (without Cake arguments)
        /// </summary>
        /// <typeparam name="T">Type of configuration to return</typeparam>
        /// <param name="context">The cake context</param>
        /// <param name="instance">The instance to bind configuration to</param>
        /// <param name="localConfiguration">Dictionary of configuration to initalise configuration with</param>
        /// <param name="commandLineSwitchMappings">Dictionary of command line arguments to replace if present</param>
        /// <example>
        /// <code>
        ///     var defaults = new Dictionary()
        ///     {
        ///         ["environment"] = "local",
        ///         ["concurrency"] = "10"
        ///     };
        ///
        ///     var switchMappings = new Dictionary()
        ///     {
        ///         ["-e"] = "--environment",
        ///         ["-c"] = "--concurrency"
        ///     };
        ///
        ///     var settings = new MySettings
        ///     {
        ///         Environment = "Unknown",
        ///         Concurrency = "1"
        ///     }
        ///
        ///     GetConfiguration(settings, defaults, switchMappings);
        /// </code>
        /// </example>
        [CakeAliasCategory("Get")]
        [CakeMethodAlias]
        public static void GetConfiguration<T>(this ICakeContext context, T instance, IDictionary<string, string> localConfiguration = null, IDictionary<string, string> commandLineSwitchMappings = null)
        {
            var mappings = commandLineSwitchMappings?.Concat(CommandLineHelper.KnownCakeCommandLineShortNameArguments.Select(kvp => new KeyValuePair<string, string>($"-{kvp.Key}", $"--{kvp.Value}")));
            context.GetConfiguration(instance, DefaultLoadConfiguration(localConfiguration, (IDictionary<string, string>)mappings));
        }

        /// <summary>
        /// Gets configuration of <typeparamref name="T"/> using the given configuration builder
        /// </summary>
        /// <typeparam name="T">Type of configuration to return</typeparam>
        /// <param name="context">The cake context</param>
        /// <param name="initialiseAction">Action to setup configuration builder</param>
        /// <returns>Return configuration of <typeparamref name="T"/></returns>
        /// <example>
        /// <code>
        ///     var settings = GetConfiguration<MyScriptSettings>((builder, args) => {
        ///         builder
        ///             .AddEnvironmentVariables()
        ///             .AddCommandLine(args)
        ///     });
        /// </code>
        /// </example>
        [CakeAliasCategory("Get")]
        [CakeMethodAlias]
        public static T GetConfiguration<T>(this ICakeContext context, Action<IConfigurationBuilder, string[]> initialiseAction) where T : new()
        {
            var configuration = new T();
            context.GetConfiguration(configuration, initialiseAction);
            return configuration;
        }

        /// <summary>
        /// Binds the configuration to <paramref name="instance"/> using the given configuration builder
        /// </summary>
        /// <param name="context">The cake context</param>
        /// <param name="instance">The instance to bind configuration to</param>
        /// <param name="initialiseAction">Action to setup configuration builder</param>
        /// <example>
        /// <code>
        ///     var settings = new MySettings
        ///     {
        ///         Environment = "Unknown",
        ///         Concurrency = "1"
        ///     }
        ///
        ///     GetConfiguration(settings, (builder, args) => {
        ///         builder
        ///             .AddEnvironmentVariables()
        ///             .AddCommandLine(args)
        ///     });
        /// </code>
        /// </example>
        [CakeAliasCategory("Get")]
        [CakeMethodAlias]
        public static void GetConfiguration(this ICakeContext context, object instance, Action<IConfigurationBuilder, string[]> initialiseAction)
        {
            LoadConfiguration(context, initialiseAction);
            BindConfiguration(context, instance);
        }

        /// <summary>
        /// Gets configuration of <typeparamref name="T"/> using the already loaded <see cref="IConfiguration"/>
        /// </summary>
        /// <typeparam name="T">Type of configuration to return</typeparam>
        /// <param name="context">The cake context</param>
        /// <returns>Return configuration of <typeparamref name="T"/></returns>
        /// <example>
        /// <code>
        ///     LoadConfiguration((builder, args) => {
        ///         builder
        ///             .AddEnvironmentVariables()
        ///             .AddCommandLine(args)
        ///     });
        ///
        ///     var settings = BindConfiguration<MyScriptSettings>();
        /// </code>
        /// </example>
        [CakeAliasCategory("Bind")]
        [CakeMethodAlias]
        public static T BindConfiguration<T>(this ICakeContext context) where T : new()
        {
            var instance = new T();
            context.BindConfiguration(instance);
            return instance;
        }

        /// <summary>
        /// Binds the configuration to <paramref name="instance"/> using the already loaded <see cref="IConfiguration"/>
        /// </summary>
        /// <param name="context">The cake context</param>
        /// <param name="instance">The instance to bind configuration to</param>
        /// <example>
        /// <code>
        ///     LoadConfiguration((builder, args) => {
        ///         builder
        ///             .AddEnvironmentVariables()
        ///             .AddCommandLine(args)
        ///     });
        ///
        ///     var settings = new MySettings
        ///     {
        ///         Environment = "Unknown",
        ///         Concurrency = "1"
        ///     }
        ///
        ///     BindConfiguration(settings);
        /// </code>
        /// </example>
        [CakeAliasCategory("Bind")]
        [CakeMethodAlias]
        public static void BindConfiguration(this ICakeContext context, object instance)
        {
            _configuration?.Bind(instance);
        }

        /// <summary>
        /// Loads the configuration using the configuration builder
        /// </summary>
        /// <param name="context">The cake context</param>
        /// <param name="initialiseAction">Action to setup configuration builder</param>
        /// <example>
        /// <code>
        ///     LoadConfiguration((builder, args) => {
        ///         builder
        ///             .AddEnvironmentVariables()
        ///             .AddCommandLine(args)
        ///     });
        /// </code>
        /// </example>
        [CakeAliasCategory("Load")]
        [CakeMethodAlias]
        public static void LoadConfiguration(this ICakeContext context, Action<IConfigurationBuilder, string[]> initialiseAction)
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToList();

            var (cakeArgs, scriptArgs, invalidArgs) = CommandLineHelper.ParseCommandLineArgs();

            foreach (var arg in invalidArgs)
            {
                Console.WriteLine($"{arg} is not in the correct format for Microsoft.Extensions.Configuration.CommandLine and has been ignored.");
            }

            var builder = new ConfigurationBuilder();

            initialiseAction?.Invoke(builder, scriptArgs.ToArray());

            _configuration = builder.Build();
        }

        /// <summary>
        /// Gets the loaded <see cref="IConfiguration">configuration</see>
        /// </summary>
        /// <param name="context">The cake context</param>
        /// <returns>Returns the loaded configuration</returns>
        [CakePropertyAlias]
        public static IConfiguration ScriptConfiguration(this ICakeContext context)
        {
            return _configuration;
        }

        /// <summary>
        /// Default configuration builder setup
        /// </summary>
        /// <param name="localConfiguration">Dictionary of configuration to initalise configuration with</param>
        /// <param name="commandLineSwitchMappings">Dictionary of command line arguments to replace if present</param>
        /// <returns>Return action for setting up configuration builder</returns>
        private static Action<IConfigurationBuilder, string[]> DefaultLoadConfiguration(IDictionary<string, string> localConfiguration = null, IDictionary<string, string> commandLineSwitchMappings = null)
        {
            return (builder, args) =>
            {
                builder
                    .AddInMemoryCollection(localConfiguration)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args, commandLineSwitchMappings)
                ;
            };
        }
    }

    /// <summary>
    /// This namespace contains Microsoft.Extensions.Configure aliases and related members.
    /// </summary>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}
