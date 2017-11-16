using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cake.Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Helper methods for command line arguments
    /// </summary>
    public static class CommandLineHelper
    {
        /// <summary>
        /// List of Cake long name command line arguments
        /// </summary>
        public readonly static ICollection<string> KnownCakeCommandLineArguments = new[]
        {
            "verbosity",
            "showdescription",
            "dryrun",
            "noop",
            "whatif",
            "help",
            "version",
            "debug",
            "mono",
            "nuget_useinprocessclient",
            "nuget_source",
            "paths_tools",
            "paths_addins",
            "paths_modules",
            "settings_skipverification",
            "nuget_loaddependencies",
            "roslyn_nugetsource"
        };

        /// <summary>
        /// Map of Cake short name command line arguments to their respective long name
        /// </summary>
        public readonly static IDictionary<string, string> KnownCakeCommandLineShortNameArguments = new Dictionary<string, string>
        {
            ["v"] = "verbosity",
            ["s"] = "showdescription",
            ["?"] = "help",
            ["ver"] = "version",
            ["d"] = "debug"
        };

        /// <summary>
        /// Parses the command line arguments and splits them up into Cake, Script or Invalid arguments
        /// </summary>
        /// <returns>Returns Tuple of Cake, Script or Invalid arguments</returns>
        public static (ICollection<string> CakeArgs, ICollection<string> ScriptArgs, ICollection<string> InvalidArgs) ParseCommandLineArgs(ICollection<string> args)
        {
            if (args == null || !args.Any())
            {
                return (CakeArgs: new string[] { }, ScriptArgs: new string[] { }, InvalidArgs: new string[] { });
            }

            // first argument is the dll so skip it
            ICollection<string> knownArgs = new List<string>();
            ICollection<string> scriptArgs = new List<string>();
            ICollection<string> invalidArgs = new List<string>();

            for (var i = 0; i < args.Count; i++)
            {
                string argName;
                var arg = args.ElementAt(i);

                if (i.Equals(0) && (arg.EndsWith(".cake") || arg.EndsWith(".csx")))
                {
                    // build script so ignore
                    continue;
                }

                // break argument up into its parts so we can process it
                var argParts = Regex.Match(arg, @"^(?<prefix>-|--|/)?(?<argName>\w+)(?:=(?<value>\w+))?$");

                if (!argParts.Success)
                {
                    // not valid command line argument format for Microsoft.Extensions.Configuration.CommandLine
                    // just ignore, don't break
                    invalidArgs.Add(arg);
                    continue;
                }

                // normalise short names to long names
                (argName, arg) = NormaliseArgument(argParts);

                // we need some info about the current argument so we know what to do next
                var (isSplit, isSwitch) = GetArgumentInfo(argParts, args.Count > i+1 ? args[i + 1] : null);                

                // pick the correct list to add argument to
                var list = KnownCakeCommandLineArguments.Contains(argName) ? knownArgs : scriptArgs;

                list.Add(arg);

                if (isSplit)
                {
                    // argument switch is split from it's value so we need to add the next argument as well
                    i++;
                    list.Add(args.ElementAt(i));
                }
                else if (isSwitch)
                {
                    // argument doesn't have a value and Microsoft.Extensions.Configuration does not like that!
                    list.Add($"{true}");
                }
            }

            return (knownArgs, scriptArgs, invalidArgs);
        }

        /// <summary>
        /// Replaces short name arguments with their long name if present
        /// </summary>
        private static (string name, string argument) NormaliseArgument(Match argParts)
        {
            var argName = argParts.Groups["argName"].Value;

            if (!KnownCakeCommandLineShortNameArguments.ContainsKey(argName))
            {
                return (argName, argParts.Value);
            }

            var prefix = argParts.Groups["prefix"].Value;
            var newArgName = KnownCakeCommandLineShortNameArguments[argName];
            var arg = argParts.Value.Replace($"{prefix}{argName}", $"{prefix}{newArgName}");

            return (newArgName, arg);
        }

        public static (bool IsSplitArgument, bool IsSwitch) GetArgumentInfo(Match argParts, string nextArgument)
        {
            bool hasValue = argParts.Groups["value"].Success ? argParts.Groups["value"].Value != null : false;

            if (hasValue)
            {
                // has a value so can't be the others
                return (IsSplitArgument: false, IsSwitch: false);
            }
            
            if (nextArgument == null)
            {
                // this is the last argument, so must be a switch
                return (IsSplitArgument: false, IsSwitch: true);
            }
             
            // let's figure out if we have a value or argument next
            var nextArgParts = Regex.Match(nextArgument, @"^(?<prefix>-|--|/)?(?<argName>\w+)(?:=(?<value>\w+))?");

            if (!nextArgParts.Success || nextArgParts.Groups["value"].Success)
            {
                // next is an argument so current must be a switch
                return (IsSplitArgument: false, IsSwitch: true);
            }

            // next is not an argument, so current must be a split argument
            return (IsSplitArgument: true, IsSwitch: false);
        }
    }
}
