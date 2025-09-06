using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class BindCommands {
        #region Variables

        private static List<Command> searchCache = new List<Command>();
        private static Dictionary<string, Command> aliasCommands = new Dictionary<string, Command>();
        private static Dictionary<string, string> aliasRawCommands = new Dictionary<string, string>();

        private static Dictionary<KeyCode, ChainedCommandRunner> boundCommands = new Dictionary<KeyCode, ChainedCommandRunner>();

        #endregion

        #region Init

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Initialize() {
            Commands.Add<string>("alias remove", AliasRemove, Privilege.Normal);
            Commands.Add("alias list", AliasList, Privilege.Normal);
            Commands.Add<string, string>("alias", Alias, Privilege.Normal);
            Commands.Add("alias clear", AliasClear, Privilege.Normal);
            Commands.Add<KeyCode, string>("bind", Bind, Privilege.Normal);
            Commands.Add("bind list", BindList, Privilege.Normal);
            Commands.Add("help bind", HelpBind, Privilege.Normal);
            Commands.Add<KeyCode>("unbind", Unbind, Privilege.Normal);
            Commands.Add("unbind all", UnbindAll, Privilege.Normal);

            PlayerLoopUtilty.InsertBeforeByPath("Update/ScriptRunBehaviourUpdate", typeof(BindCommands), UpdateBoundKeys);

            LoadAlias();
            LoadBind();
        }

        private static void UpdateBoundKeys() {
            foreach(var b in boundCommands) {
                if(Input.GetKeyDown(b.Key)) {
                    b.Value.Run();
                }
            }
        }

        #endregion

        #region Alias

        public static void AliasClear() {
            foreach(var ac in aliasCommands)
                Commands.Remove(ac.Value);
            aliasCommands.Clear();
            aliasRawCommands.Clear();
            SaveAlias();
        }

        private static void Alias(string command, string execution)
            => Alias(command, execution, true);

        private static void Alias(string command, string execution, bool printtoconsole) {
            Commands.FindAllCommands(command, searchCache, Privilege.Admin);
            if(searchCache.Count > 0) {
                if(aliasCommands.TryGetValue(command, out var existingCommand)) {
                    Debug.LogError($"Alias command already exists '{command}'");
                    return;
                }
                Debug.LogError($"Attempting to add a command that already exist '{command}'");
                return;
            }
            var cmdRunner = new ChainedCommandRunner(execution);
            if(!cmdRunner.IsValid) {
                Commands.PrintToConsole($"Failed to alias '{command}' with commands '{execution}'");
                return;
            }
            var cmd = Commands.Add(command, cmdRunner.Run, Privilege.Normal);
            aliasCommands.Add(command, cmd);
            aliasRawCommands.Add(command, execution);
            if(printtoconsole)
                Commands.PrintToConsole($"Added command '{command}'");
            SaveAlias();
        }

        private static void AliasRemove(string command) {
            if(!aliasCommands.TryGetValue(command, out Command cmd)) {
                Debug.LogError($"Alias not found '{command}'");
                return;
            }
            aliasCommands.Remove(command);
            aliasRawCommands.Remove(command);
            if(Commands.Remove(cmd))
                Commands.PrintToConsole($"Removed alias '{command}'");
            else
                Commands.PrintToConsole($"Failed to remove alias '{command}'");
            SaveAlias();
        }

        private static void AliasList() {

            Commands.PrintToConsole($"Aliases:");
            foreach(var a in aliasRawCommands) {
                Commands.PrintToConsole($"{a.Key} = {a.Value}");
            }
        }

        #endregion

        #region Bind

        public static void UnbindAll() {
            boundCommands.Clear();
            SaveBind();
        }

        private static void HelpBind() {
            Commands.PrintToConsole($"Binds a key to run a command on click.");
            Commands.PrintToConsole($"Example: bind H \"help normal\"");
            Commands.PrintToConsole($"Example: unbind H");
            Commands.PrintToConsole($"-bind <keycode> <command>");
            Commands.PrintToConsole($"-bind list");
            Commands.PrintToConsole($"-unbind <keycode>");
            Commands.PrintToConsole($"-unbind <keycode> <command>");
        }

        private static void BindList() {
            Commands.PrintToConsole($"Bound Keys:");
            foreach(var b in boundCommands) {
                Commands.PrintToConsole($"{b.Key} : {(b.Value.Count > 1 ? "multiple commands" : b.Value[0])}");
            }
            Commands.PrintToConsole($"------ End --------");
        }

        private static void Bind(KeyCode code, string command) {
            if(boundCommands.TryGetValue(code, out var existingRunner)) {
                Commands.PrintToConsole($"Failed to bind key '{code}' as it already exists!");
                return;
            }
            var chainedCommandRunner = new ChainedCommandRunner(command);
            if(!chainedCommandRunner.IsValid) {
                Commands.PrintToConsole($"Failed to bind key '{code}' with commands '{command}'");
                return;
            }
            boundCommands.Add(code, chainedCommandRunner);
            Commands.PrintToConsole($"Bound key '{code}' with command '{command}'");
            SaveBind();
        }

        private static void Unbind(KeyCode code) {
            boundCommands.Remove(code);
            Commands.PrintToConsole($"Unbound all commands on key '{code}'");
            SaveBind();
        }

        #endregion

        #region Save / Load

        private static void SaveAlias() {
            try {
                TMLNode root = new TMLNode("alias");
                foreach(var c in aliasRawCommands) {
                    var child = root.AddNode(c.Key);
                    child.AddProperty("execution", c.Value);
                }
                var path = Application.persistentDataPath + "/alias.cfg";
                System.IO.File.WriteAllText(path, Toolkit.IO.TML.TMLParser.ToString(root, true));
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        private static void LoadAlias() {
            try {
                var path = Application.persistentDataPath + "/alias.cfg";
                if(!System.IO.File.Exists(path))
                    return;
                if(!Toolkit.IO.TML.TMLParser.TryParse(System.IO.File.ReadAllText(path), out TMLNode root)) {
                    Debug.Log("Failed to parse?");
                    return;
                }
                foreach(var c in root.Children) {
                    var command = c.Name;
                    var execution = c.GetString("execution");
                    if(string.IsNullOrEmpty(command) || string.IsNullOrEmpty(execution))
                        continue;
                    Alias(command, execution, false);
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        private static void SaveBind() {
            try {
                TMLNode root = new TMLNode("bind");
                foreach(var c in boundCommands) {
                    var child = root.AddNode(c.Key.ToStringFast());
                    child.AddProperty("raw", c.Value.RawCommands);
                }
                var path = Application.persistentDataPath + "/bind.cfg";
                System.IO.File.WriteAllText(path, Toolkit.IO.TML.TMLParser.ToString(root, true));
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        private static void LoadBind() {
            try {
                var path = Application.persistentDataPath + "/bind.cfg";
                if(!System.IO.File.Exists(path))
                    return;
                if(!Toolkit.IO.TML.TMLParser.TryParse(System.IO.File.ReadAllText(path), out TMLNode root))
                    return;
                foreach(var c in root.Children) {
                    if(!FastEnum<KeyCode>.TryParseIgnoreCase(c.Name, out KeyCode keycode))
                        continue;
                    var rawcommands = c.GetString("raw");
                    Bind(keycode, rawcommands);
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}
