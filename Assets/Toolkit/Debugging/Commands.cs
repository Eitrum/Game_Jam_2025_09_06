using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class Commands {

        #region Variables

        private const string TAG = "[Toolkit.Debugging.Commands] - ";
        private static List<Command> commands = new List<Command>();
        private static Dictionary<string, List<Command>> baseCommandsToCommandList = new Dictionary<string, List<Command>>();

        public static Privilege DefaultPrivilage = Privilege.Normal;

        #endregion

        #region Properties

        public static IReadOnlyList<Command> AllCommands => commands;
        public static IReadOnlyCollection<string> BaseCommands => baseCommandsToCommandList.Keys;

        #endregion

        #region Converter Registry

        public delegate bool TryParseCallback<T>(string input, out T value);

        public static void RegisterType<T>(Func<string, T> converter) {
            Converter<T>.Register(converter);
        }

        public static void RegisterType<T>(TryParseCallback<T> converter) {
            Converter<T>.Register(x => converter.Invoke(x, out var val) ? val : throw new ConvertException<T>(x));
        }

        internal static class ConverterRegistry {
            #region Init

            static ConverterRegistry() {
                Converter<float>.Register((s) => float.TryParse(s, out var value) ? value : throw new ConvertException<float>(s));
                Converter<int>.Register((s) => int.TryParse(s, out var value) ? value : throw new ConvertException<int>(s));
                Converter<double>.Register((s) => double.TryParse(s, out var value) ? value : throw new ConvertException<double>(s));
                Converter<long>.Register((s) => long.TryParse(s, out var value) ? value : throw new ConvertException<long>(s));
                Converter<string>.Register(s => s);
                Converter<bool>.Register((s) => bool.TryParse(s, out var value) ? value : throw new ConvertException<bool>(s));
            }

            #endregion

            #region Variables

            public static Dictionary<Type, Func<string, object>> toObjectConverter = new Dictionary<Type, Func<string, object>>();
            public static Dictionary<Type, Func<TMLNode, CommandValue>> toCommandValueConverter = new Dictionary<Type, Func<TMLNode, CommandValue>>();

            #endregion

            #region Try Convert

            public static bool TryConvert<T>(string input, out T value)
                => Converter<T>.TryConvert(input, out value);

            public static bool TryConvert(Type type, string input, out object value) {
                if(!toObjectConverter.TryGetValue(type, out var func)) {
                    value = default;
                    return false;
                }
                try {
                    value = func(input);
                    return true;
                }
                catch {
                    value = default;
                    return false;
                }
            }

            #endregion
        }

        internal class ConvertException<T> : Exception {
            public ConvertException(string s) : base($"Unable to convert '{s}' to type '{typeof(T).FullName}'") { }
        }

        internal static class Converter<T> {

            #region Variables

            public const string TAG = "[Toolkit.Debugging.Commands.Converter] - ";
            private static bool isRegistered;
            private static Func<string, T> cached;

            #endregion

            #region Register

            public static void Register(Func<string, T> callback) {
                if(isRegistered)
                    return;
                isRegistered = true;
                cached = callback;
                ConverterRegistry.toObjectConverter.Add(typeof(T), (s) => callback(s));
                ConverterRegistry.toCommandValueConverter.Add(typeof(T), (node) => new CommandValue<T>(node));
            }

            #endregion

            #region Convert

            public static T Convert(string s) {
                try {
                    return cached.Invoke(s);
                }
                catch(Exception e) {
                    Debug.LogException(e);
                    return default;
                }
            }

            public static bool TryConvert(string s, out T value) {
                try {
                    value = cached.Invoke(s);
                    return true;
                }
                catch {
                    value = default;
                    return false;
                }
            }

            public static T ConvertSafe(string s, T defaultValue) {
                try {
                    return cached.Invoke(s);
                }
                catch {
                    return defaultValue;
                }
            }

            #endregion
        }

        #endregion

        #region Run

        public static void Run(string command)
            => Run(command, DefaultPrivilage);

        public static void Run(string command, Privilege privilege) {
            if(!CommandParser.TryParse(command, out var commandNode)) {
                Debug.LogError(TAG + $"Failed to parse command: '{command}'");
                return;
            }

            if(!commandNode.TryGetNode("command", out var commandName)) {
                Debug.LogError(TAG + $"Command not found: '{command}'");
                return;
            }

            try {
                var baseCommand = commandName.GetString("value");
                var filtered = commands
                .Where(x=> x.BaseCommand.Equals(baseCommand, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.IsValid(commandNode, privilege))
                .ToArray();

                if(filtered.Length == 0) {
                    Debug.LogError(TAG + "Command not found: " + command);
                    return;
                }

                if(filtered.Length > 1) {
                    var bestMatch = BestMatch(command, filtered);
                    Debug.LogWarning(TAG + $"Command had multiple possible calls, using best match: '{command}'\n\t{string.Join("\n\t", filtered.Select(x => x.FullCommandVisual))}");
                    bestMatch.Run(commandNode, privilege);
                }
                else
                    filtered[0].Run(commandNode, privilege);
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        public static Command BestMatch(string inputCommand, Command[] commands) {
            int len = commands.Length;
            int[] counts = new int[len];
            for(int i = 0; i < len; i++)
                counts[i] = CountMatching(inputCommand, commands[i].FullCommandVisual);
            var highestIndex = 0;
            int highest = -1;
            for(int i = 0; i < len; i++) {
                if(counts[i] > highest) {
                    highest = counts[i];
                    highestIndex = i;
                }
            }

            return commands[highestIndex];
        }

        private static int CountMatching(ReadOnlySpan<char> inputCommand, ReadOnlySpan<char> commandRepresentation) {
            var len = Mathf.Min(inputCommand.Length, commandRepresentation.Length);
            for(int i = 0; i < len; i++) {
                if(inputCommand[i] != commandRepresentation[i])
                    return i - 1;
            }
            return 10000;
        }

        #endregion

        #region Add Commands

        private static Command AddInternal(string command, Action<object[]> genericWrapper, MethodInfo method, Privilege privilege = Privilege.Debug) {
            var c = new Command(command, genericWrapper, method, privilege);
            commands.Add(c);

            if(!baseCommandsToCommandList.TryGetValue(c.BaseCommand, out var list)) {
                list = new List<Command>();
                baseCommandsToCommandList.Add(c.BaseCommand, list);
            }
            list.Add(c);

            return c;
        }

        public static Command Add(string command, System.Action callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback(), callback.Method, privilege);

        public static Command Add<T0>(string command, System.Action<T0> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0]), callback.Method, privilege);

        public static Command Add<T0, T1>(string command, System.Action<T0, T1> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1]), callback.Method, privilege);

        public static Command Add<T0, T1, T2>(string command, System.Action<T0, T1, T2> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1], (T2)objs[2]), callback.Method, privilege);

        public static Command Add<T0, T1, T2, T3>(string command, System.Action<T0, T1, T2, T3> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1], (T2)objs[2], (T3)objs[3]), callback.Method, privilege);

        public static Command Add<T0, T1, T2, T3, T4>(string command, System.Action<T0, T1, T2, T3, T4> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1], (T2)objs[2], (T3)objs[3], (T4)objs[4]), callback.Method, privilege);

        public static Command Add<T0, T1, T2, T3, T4, T5>(string command, System.Action<T0, T1, T2, T3, T4, T5> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1], (T2)objs[2], (T3)objs[3], (T4)objs[4], (T5)objs[5]), callback.Method, privilege);

        public static Command Add<T0, T1, T2, T3, T4, T5, T6>(string command, System.Action<T0, T1, T2, T3, T4, T5, T6> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1], (T2)objs[2], (T3)objs[3], (T4)objs[4], (T5)objs[5], (T6)objs[6]), callback.Method, privilege);

        public static Command Add<T0, T1, T2, T3, T4, T5, T6, T7>(string command, System.Action<T0, T1, T2, T3, T4, T5, T6, T7> callback, Privilege privilege = Privilege.Debug)
            => AddInternal(command, (objs) => callback((T0)objs[0], (T1)objs[1], (T2)objs[2], (T3)objs[3], (T4)objs[4], (T5)objs[5], (T6)objs[6], (T7)objs[7]), callback.Method, privilege);

        public static bool Remove(Command command) {
            if(command == null)
                return false;
            if(!baseCommandsToCommandList.TryGetValue(command.BaseCommand, out var commands)) {
                return false;
            }
            var result = commands.Remove(command);
            if(!result) {
                return false;
            }
            var listRemoveResult = Commands.commands.Remove(command);
            if(!listRemoveResult) {
                commands.Add(command);
                return false;
            }
            return true;
        }

        #endregion

        #region Find

        public static void FindPossibleCommands(TMLNode parsed, List<Command> list)
            => FindPossibleCommands(parsed, list, DefaultPrivilage);

        public static void FindPossibleCommands(TMLNode parsed, List<Command> list, Privilege privilege) {
            list.Clear();

            if(!parsed.TryGetNode("command", out TMLNode commandNode) || !commandNode.TryGetString("value", out string baseCommand))
                return;

            if(parsed.Children.Count == 1) {
                // Only check base command
                foreach(var c in commands) {
                    if(c.BaseCommand.StartsWith(baseCommand) && c.Privilege <= privilege)
                        list.Add(c);
                }
                return;
            }

            foreach(var c in commands) {
                if(c.BaseCommand.Equals(baseCommand, StringComparison.OrdinalIgnoreCase) && c.IsParametersPossible(parsed, privilege)) {
                    list.Add(c);
                }
            }
        }

        public static void FindBaseCommands(List<string> list)
            => FindBaseCommands(list, DefaultPrivilage);

        public static void FindBaseCommands(List<string> list, Privilege privilege) {
            list.Clear();
            foreach(var b in baseCommandsToCommandList) {
                if(b.Value.Any(x => x.Privilege <= privilege)) {
                    list.Add(b.Key);
                }
            }
        }

        public static void FindAllCommands(string baseCommand, List<Command> commands)
            => FindAllCommands(baseCommand, commands, DefaultPrivilage);

        public static void FindAllCommands(string baseCommand, List<Command> commands, Privilege privilege) {
            commands.Clear();
            if(baseCommandsToCommandList.TryGetValue(baseCommand, out var list)) {
                foreach(var c in list)
                    if(c.Privilege <= privilege)
                        commands.Add(c);
            }
        }

        #endregion

        #region Print

        /// <summary>
        /// Used to print a message to the console.
        /// </summary>
        public static void PrintToConsole(string msg) {
            Debug.Log(msg);
        }

        #endregion
    }
}
