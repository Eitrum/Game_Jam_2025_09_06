using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Toolkit {
    public static partial class CommandLineArguments {
        #region Variables

        private const string TAG = ColorTable.RichTextTags.GREY + "[Command Line Arguments]</color> - ";
        private static string appPath;
        private static Dictionary<string, string> cachedArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        public static string AppPath => appPath;
        public static IReadOnlyDictionary<string, string> CachedArgumentsWithValues => cachedArguments;

        #endregion

        #region Constructor

        static CommandLineArguments() {
            var args = System.Environment.GetCommandLineArgs();
            if(args.Length == 0)
                return;

            if(System.IO.File.Exists(args[0])) // First argument is app location
                appPath = args[0];

            List<string> argumentsCleaned = new List<string>();
            for(int i = 0, len = args.Length; i < len; i++) {
                if(args[i].StartsWith('-'))
                    argumentsCleaned.Add(args[i]);
                else {
                    // Check if part of previous argument
                    if(argumentsCleaned.Count == 0)
                        continue;
                    if(i == 0)
                        continue;
                    if(!argumentsCleaned.Last().Contains("=")) {
                        argumentsCleaned[argumentsCleaned.Count - 1] = argumentsCleaned[argumentsCleaned.Count - 1] + "=" + args[i];
                    }
                }
            }

            argumentsCleaned.Foreach(Load);
            LoadPresets();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init() {
            UnityEngine.Debug.Log(TAG + $"Launched with {cachedArguments.Count} arguments: \n{string.Join('\n', cachedArguments.Select(x => $"{x.Key}<=>{x.Value}"))}");
        }

        #endregion

        #region Loading

        private static void Load(string argument) {
            if(!argument.StartsWith("-")) {
                UnityEngine.Debug.LogWarning(TAG + $"argument issue: '{argument}'");
                return;
            }
            if(!FormatArgument(argument, out string formatted, out string value)) {
                UnityEngine.Debug.LogWarning(TAG + $"Unable to format argument value: '{argument}'");
                return;
            }

            if(string.IsNullOrEmpty(formatted)) {
                UnityEngine.Debug.LogError(TAG + $"Unable to add argument ({argument}) as '{formatted}' = '{value}'");
                return;
            }

            if(string.IsNullOrEmpty(value))
                value = string.Empty;

            if(!cachedArguments.TryAdd(formatted, value)) {
                UnityEngine.Debug.LogError(TAG + $"Unable to add argument ({argument}) as '{formatted}' = '{value}'");
            }
        }

        private static bool FormatArgument(string arg, out string argumentFormatted, out string value) {
            value = default;
            argumentFormatted = default;
            if(!arg.StartsWith("-")) // All arguments have to start with a dash
                return false;
            arg = arg.TrimStart('-');
            if(string.IsNullOrEmpty(arg)) // Check so something is written
                return false;
            var index = arg.IndexOf('='); // Check if has a value field
            if(index == -1) {
                argumentFormatted = arg;
                return true;
            }
            arg.SplitAt(index, out argumentFormatted, out value);
            value = value.TrimStart('=');
            //UnityEngine.Debug.Log(TAG + $"Post split: {argumentFormatted} <=> {value}");
            if(value.StartsWith('\"') && !value.EndsWith('\"')) // Broken value
                return false;
            value = value.Trim('\"');

            if(argumentFormatted.Contains('-'))
                argumentFormatted = argumentFormatted.Replace('-', '_');

            return true;
        }

        #endregion

        #region Argument Checks

        private static string GetArgumentInput(string argument) {
            if(argument.StartsWith('-'))
                argument = argument.TrimStart('-');
            if(argument.Contains('-'))
                argument = argument.Replace('-', '_');

            return argument;
        }

        public static bool HasArgument(string argument) {
            return cachedArguments.ContainsKey(GetArgumentInput(argument));
        }

        public static bool TryGetValue(string argument, out int value) {
            value = default;
            if(!cachedArguments.TryGetValue(GetArgumentInput(argument), out string argumentValue))
                return false;
            return int.TryParse(argumentValue, out value);
        }

        public static bool TryGetValue(string argument, out float value) {
            value = default;
            if(!cachedArguments.TryGetValue(GetArgumentInput(argument), out string argumentValue))
                return false;
            return float.TryParse(argumentValue, out value);
        }

        public static bool TryGetValue(string argument, out bool value) {
            value = default;
            if(!cachedArguments.TryGetValue(GetArgumentInput(argument), out string argumentValue))
                return false;
            if(bool.TryParse(argumentValue, out value))
                return value;
            return true;
        }

        public static bool TryGetValue(string argument, out string value) {
            return cachedArguments.TryGetValue(GetArgumentInput(argument), out value) && !string.IsNullOrEmpty(value);
        }

        #endregion

        #region Adding

        public static void Add(string arguments) {
            int isInside = 0;
            int start = -1;
            for(int i = 0; i < arguments.Length; i++) {
                var c = arguments[i];
                if(c == '-' && isInside == 0) {
                    start = i;
                    isInside++;
                }
                switch(c) {
                    case '-':
                        if(isInside == 0) {
                            start = i;
                            isInside++;
                        }
                        break;
                    case ' ':
                        if(isInside == 1) {
                            isInside = 0;
                            Internal_AddArgument(arguments.Substring(start, i - start));
                        }
                        break;
                    case '\"':
                        if(isInside == 1) {
                            isInside++;
                        }
                        else if(isInside == 2) {
                            isInside--;
                        }
                        break;
                }
            }
            if(isInside == 1)
                Internal_AddArgument(arguments.Substring(start, arguments.Length - start));
        }

        private static void Internal_AddArgument(string argument) {
            UnityEngine.Debug.Log(TAG + $"Adding argument '{argument}'");
            Load(argument);
        }

        #endregion
    }
}
