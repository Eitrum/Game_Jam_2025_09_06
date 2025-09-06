using System;
using System.Diagnostics;
using UnityEditor;
using System.Threading;

namespace Toolkit {
    public static class CommandUtility {

        #region Consts

        private const string TAG = "Command Utility";

        private const string NEW_PATH = "Toolkit/Command Line/New";
        private const string NEW_ADMIN_PATH = "Toolkit/Command Line/New Administrative";
        private const int NEW_PRIORITY = 20;
        private const int NEW_ADMIN_PRIORITY = 21;

        #endregion

        #region Menu Items

        [MenuItem(NEW_PATH, priority = NEW_PRIORITY)]
        public static Process New() {
#if UNITY_EDITOR_WIN
            return Process.Start("cmd.exe");
#else
            Debug.LogWarning("Command Terminal Does only work on Windows currently.");
            return null;
#endif
        }

        [MenuItem(NEW_ADMIN_PATH, priority = NEW_ADMIN_PRIORITY)]
        public static Process NewAdministrative() {
#if UNITY_EDITOR_WIN
            return Process.Start(new ProcessStartInfo() {
                FileName = "cmd.exe",
                Verb = "runas"
            });
#else
            Debug.LogWarning("Command Terminal Does only work on Windows currently.");
            return null;
#endif
        }

        [MenuItem(NEW_PATH, priority = NEW_PRIORITY, validate = true)]
        [MenuItem(NEW_ADMIN_PATH, priority = NEW_ADMIN_PRIORITY, validate = true)]
        public static bool Available() {
#if UNITY_EDITOR_WIN
            return true;
#else
            return false;
#endif
        }

        public static Process New(bool hidden) {
            if(!hidden)
                return New();
#if UNITY_EDITOR_WIN
            return Process.Start(new ProcessStartInfo() {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            });
#else
            return null;
#endif
        }

        public static Process NewAdministrative(bool hidden) {
            if(!hidden)
                return NewAdministrative();
#if UNITY_EDITOR_WIN
            return Process.Start(new ProcessStartInfo() {
                FileName = "cmd.exe",
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            });
#else
            return null;
#endif
        }

        #endregion

        #region Commands

        public static string[] Run(Process process, string command, bool removeEmptyEntries = true) {
            if(process == null) {
                return new string[0];
            }
#if UNITY_EDITOR_WIN
            // This will remove first lines from the cmd (Windows)
            for(int i = 0; i < 3; i++)
                process.StandardOutput.ReadLine();
#endif

            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");

            return process.StandardOutput.ReadToEnd().Split(
                new string[] { System.Environment.NewLine },
                removeEmptyEntries ?
                    StringSplitOptions.RemoveEmptyEntries :
                    StringSplitOptions.None);
        }

        private static void RunAdminCommands(string command) {
#if UNITY_EDITOR_WIN
            var process = Process.Start(new ProcessStartInfo() {
                FileName = "cmd.exe",
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "/C " + command
            });
#else
            
#endif
        }

        public static string[] Run(string command, bool removeEmptyEntries = true) {
            return Run(New(true), command, removeEmptyEntries);
        }

        public static void RunBackground(string command, Action<string[]> callback = null, bool removeEmptyEntries = true) {
            Thread thread = new Thread(new ThreadStart(() => callback?.Invoke(Run(command, removeEmptyEntries))));
            thread.IsBackground = true;
            thread.Start();
        }

        #endregion
    }
}
