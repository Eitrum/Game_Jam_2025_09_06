using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class StartupCommands {

        #region Variables

        private static List<string> commands = new List<string>();

        #endregion

        #region Init

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Initialize() {
            Commands.Add<string>("startup add", Add, Privilege.Normal);
            Commands.Add<string>("startup remove", Remove, Privilege.Normal);
            Commands.Add<int>("startup removeat", RemoveAt, Privilege.Normal);
            Commands.Add<int, int>("startup move", Move, Privilege.Normal);
            Commands.Add("startup list", ListStartup, Privilege.Normal);

            Load();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += LateInitialize;
#endif
        }

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        private static void LateInitialize() {
            foreach(var c in commands) {
                var ccr = new ChainedCommandRunner(c);
                if(ccr.IsValid)
                    ccr.Run();
            }
        }

        #endregion

        #region Add Remove

        private static void Add(string command) {
            commands.Add(command);
            Commands.PrintToConsole("Added startup command: " + command);
            Save();
        }

        private static void Remove(string command) {
            if(!commands.Remove(command)) {
                Commands.PrintToConsole("Failed to remove startup command: " + command);
                return;
            }
            Commands.PrintToConsole("Removed startup command: " + command);
            Save();
        }

        private static void RemoveAt(int index) {
            if(index < 0 || index >= commands.Count) {
                Commands.PrintToConsole("Removing startup command error: Index out of range!");
                return;
            }
            var cmd = commands[index];
            commands.RemoveAt(index);
            Commands.PrintToConsole("Removed startup command: " + cmd);
            Save();
        }

        #endregion


        private static void Move(int from, int to) {
            try {
                if(from < 0 || to < 0 || from >= commands.Count || to >= commands.Count) {
                    Commands.PrintToConsole($"error: failed to move startup command from '{from}' to '{to}'");
                    return;
                }
                var cmd = commands[from];
                commands.RemoveAt(from);
                commands.Insert(to, cmd);
                Save();
            }
            catch {
                Commands.PrintToConsole($"error: failed to move startup command from '{from}' to '{to}'");
            }
        }

        private static void ListStartup() {
            Commands.PrintToConsole("Startup Commands:");
            for(int i = 0; i < commands.Count; i++) {
                Commands.PrintToConsole($"{i}. {commands[i]}");
            }
        }

        #region Save / Load



        private static void Save() {
            try {
                TMLNode root = new TMLNode("startup");
                foreach(var c in commands) {
                    var child = root.AddNode($"{root.Children.Count}");
                    child.AddProperty("command", c);
                }
                var path = Application.persistentDataPath + "/startup.cfg";
                System.IO.File.WriteAllText(path, Toolkit.IO.TML.TMLParser.ToString(root, true));
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        private static void Load() {
            try {
                commands.Clear();
                var path = Application.persistentDataPath + "/startup.cfg";
                if(!System.IO.File.Exists(path))
                    return;
                if(!Toolkit.IO.TML.TMLParser.TryParse(System.IO.File.ReadAllText(path), out TMLNode root))
                    return;
                foreach(var c in root.Children) {
                    var command = c.GetString("command");
                    commands.Add(command);
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}
