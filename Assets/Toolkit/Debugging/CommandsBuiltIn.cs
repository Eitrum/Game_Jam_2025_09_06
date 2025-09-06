using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class CommandsBuiltIn {

        #region Variables

        private const string TAG = "[Toolkit.Debugging.CommandsBuiltIn] - ";

        #endregion

        #region Init
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void InitBuiltIn() {
            /// HELP
            Commands.Add("help", Help, Privilege.None);
            Commands.RegisterType<Privilege>(FastEnum.TryParseIgnoreCase);
            Commands.Add<Privilege>("help", Help, Privilege.None);
            Commands.Add<string>("help", Help, Privilege.None);
            Commands.Add<string, Privilege>("help", Help, Privilege.None);

            /// TEST
            // Commands.Add<CommandValue<string>>("test", Test, Privilege.None);
            // Commands.Add<CommandValue<string>, float>("test <?> and set <?>", Test, Privilege.None);

            /// Time Scale
            Commands.Add<float>("timescale set", SetTimeScale, Privilege.Debug);

            /// Time System
            Commands.Add<TimeModify, float>("time", TimeSet, Privilege.Admin);
            Commands.Add<DayCycle.TimeOfDay>("time set", TimeSet, Privilege.Admin);

            /// Open Scene
            Commands.Add<SceneModify, int>("scene", SceneSet, Privilege.Debug);
            Commands.Add<SceneModify, string>("scene", SceneSet, Privilege.Debug);

            /// Print
            Commands.Add<string>("print", PrintCommand, Privilege.Normal);

            /// Quit
#if UNITY_EDITOR
            Commands.Add("quit", () => UnityEditor.EditorApplication.ExitPlaymode(), Privilege.Normal);
#else
            Commands.Add("quit", () => Application.Quit(), Privilege.Normal);
#endif
            /// File
            Commands.Add("open gamefolder", () => Application.OpenURL(System.IO.Path.GetDirectoryName("file.txt")), Privilege.Normal);
            Commands.Add("open storagefolder", () => Application.OpenURL(Application.persistentDataPath), Privilege.Normal);

            // Invoke
            Commands.Add<string>("invoke <method>", InvokeOnObject, Privilege.Debug);
        }

        private static void InvokeOnObject(string method) {
            var main = CameraInstance.MainCamera;
            if(!main) {
                Commands.PrintToConsole("error: no camera found");
                return;
            }

            if(main.ScreenPointToRay(Input.mousePosition).Hit(out RaycastHit hit))
                hit.collider.SendMessageUpwards(method, SendMessageOptions.DontRequireReceiver);
            else
                Commands.PrintToConsole("error: no object found");
        }

        private static void PrintCommand(string message) {
            Commands.PrintToConsole(message);
        }

        #endregion

        #region Help

        public static void Help() => Help(Commands.DefaultPrivilage);

        public static void Help(Privilege privilege) {
            List<string> baseCommands = new List<string>();
            Commands.FindBaseCommands(baseCommands, privilege);
            Debug.Log(TAG + $"Help '{privilege}'\n---{baseCommands.Count:0000}---\n{string.Join('\n', baseCommands)}\n-------");
        }

        public static void Help(string command) {
            List<Command> commands = new List<Command>();
            Commands.FindAllCommands(command, commands);
            Debug.Log(TAG + $"Help '{command}'\n---{commands.Count:0000}---\n{string.Join('\n', commands.Select(x => x.FullCommandVisual))}\n-------");
        }

        public static void Help(string command, Privilege privilege) {
            List<Command> commands = new List<Command>();
            Commands.FindAllCommands(command, commands, privilege);
            Debug.Log(TAG + $"Help '{command}' @ '{privilege}'\n---{commands.Count:0000}---\n{string.Join('\n', commands.Select(x => x.FullCommandVisual))}\n-------");
        }

        #endregion

        #region Test

        public static void Test(CommandValue<string> message) {
            Debug.Log(TAG + $"Test: '{message.Value}'\n" + Toolkit.IO.TML.TMLParser.ToString(message.Data, true));
        }

        public static void Test(CommandValue<string> message, float value) {
            Debug.Log(TAG + $"Test: '{message.Value}' + '{value}'\n" + Toolkit.IO.TML.TMLParser.ToString(message.Data, true));
        }

        #endregion

        #region TimeScale

        private static void SetTimeScale(float timePerSecond) {
            Time.timeScale = timePerSecond;
        }

        #endregion

        #region TimeSystem

        public enum TimeModify {
            Add,
            Set,
            Remove,
        }

        private static void TimeSet(TimeModify modify, float time) {
            switch(modify) {
                case TimeModify.Add:
                    Toolkit.DayCycle.TimeSystem.Add(time);
                    break;
                case TimeModify.Set:
                    Toolkit.DayCycle.TimeSystem.Set(time);
                    break;
                case TimeModify.Remove:
                    Toolkit.DayCycle.TimeSystem.Remove(time);
                    break;
            }
        }

        private static void TimeSet(Toolkit.DayCycle.TimeOfDay time) {
            Toolkit.DayCycle.TimeSystem.Set(time);
        }

        #endregion

        #region Scene

        public enum SceneModify {
            Add,
            Remove,
        }

        private static void SceneSet(SceneModify modify, int sceneIndex) {
            switch(modify) {
                case SceneModify.Add:
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case SceneModify.Remove:
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
                    break;
            }
        }

        private static void SceneSet(SceneModify modify, string sceneName) {
            switch(modify) {
                case SceneModify.Add:
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case SceneModify.Remove:
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
                    break;
            }
        }

        #endregion
    }

}
