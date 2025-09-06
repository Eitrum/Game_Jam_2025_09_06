using System;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Toolkit {
    [InitializeOnLoad]
    public static class RecompileUtility {

        #region Consts

        private const string TAG = "<color=#FF00FF>[Recompile]</color> - ";
        private const string RECOMPILE_LOG_PREFS_PATH = "Toolkit.Recompile_Log";
        private const string RECOMPILE_PREVENT_PREFS_PATH = "Toolkit.Recompile_Prevent";
        private const string CLEANUP_UNUSED_RESOURCES_PREFS_PATH = "Toolkit.CleanupUnusedResources";
        private const string RECOMPILE_CLEANUP_PREFS_PATH = "Toolkit.RecompileCleanup";

        public const string RECOMPILE_PATH = "Toolkit/Recompile";
        public const int RECOMPILE_PRIORITY = -100;

        public static event Action OnRecompile;

        #endregion

        #region Properties

        public static bool EnableLog {
            get => EditorPrefs.GetBool(RECOMPILE_LOG_PREFS_PATH, false);
            set {
                if(value != EnableLog) {
                    EditorPrefs.SetBool(RECOMPILE_LOG_PREFS_PATH, value);
                    LogSettings(value);
                }
            }
        }

        public static bool Prevent {
            get => EditorPrefs.GetBool(RECOMPILE_PREVENT_PREFS_PATH, false);
            set {
                if(value != Prevent) {
                    EditorPrefs.SetBool(RECOMPILE_PREVENT_PREFS_PATH, value);

                }
            }
        }

        public static bool RecompileCleanup {
            get => EditorPrefs.GetBool(RECOMPILE_CLEANUP_PREFS_PATH, true);
            set {
                if(value != RecompileCleanup) {
                    EditorPrefs.SetBool(RECOMPILE_CLEANUP_PREFS_PATH, value);
                }
            }
        }

        public static bool CleanupUnusedResources {
            get => EditorPrefs.GetBool(CLEANUP_UNUSED_RESOURCES_PREFS_PATH, false);
            set {
                if(value != CleanupUnusedResources) {
                    EditorPrefs.SetBool(CLEANUP_UNUSED_RESOURCES_PREFS_PATH, value);
                }
            }
        }

        #endregion

        #region Recompile Logging

        [InitializeOnLoadMethod]
        private static void Initialize() {
            ToolkitProjectSettings.RegisterEditor("Recompile", 1, ProjectSettingsGUI);
            if(RecompileCleanup)
                AssemblyReloadEvents.beforeAssemblyReload += RunRecompileCleanup;

            LogSettings(EnableLog);
            PreventUpdate(Prevent);

            if(CleanupUnusedResources)
                AssemblyReloadEvents.beforeAssemblyReload += () => Resources.UnloadUnusedAssets();

            AssemblyReloadEvents.beforeAssemblyReload += () => OnRecompile?.Invoke();
        }

        private static void RunRecompileCleanup() {
            Debug.Log("<color=#FF00FF>---------------- [   Cleanup   ] ----------------</color>");
            if(!AttributeStorage.TryFind(typeof(RecompileCleanupAttribute), out var storage)) {
                return;
            }
            foreach(var m in storage.MethodAttributes) {
                try {
                    m.MethodInfo.Invoke(null, null);
                }
                catch { };
            }
        }

        private static void ProjectSettingsGUI(string obj) {
            EnableLog = EditorGUILayout.Toggle("Log", EnableLog);
            Prevent = EditorGUILayout.Toggle("Prevent Unfocused", Prevent);
            RecompileCleanup = EditorGUILayout.Toggle("Cleanup (RecompileCleanupAttribute)", RecompileCleanup);
            CleanupUnusedResources = EditorGUILayout.Toggle("Cleanup (Resources.UnloadUnusedAssets)", CleanupUnusedResources);
        }

        private static void LogSettings(bool active) {
            AssemblyReloadEvents.beforeAssemblyReload -= Log;
            if(active)
                AssemblyReloadEvents.beforeAssemblyReload += Log;
            else
                AssemblyReloadEvents.beforeAssemblyReload -= Log;
        }

        private static void PreventUpdate(bool active) {
            EditorApplication.update -= PreventUpdate;
            if(active)
                EditorApplication.update += PreventUpdate;
            else
                EditorApplication.update -= PreventUpdate;
        }

        private static void Log() {
            Debug.Log("<color=#FF00FF>---------------- [ Recompile ] ----------------</color>");
        }

        #endregion

        #region Utility Methods

        [MenuItem(RECOMPILE_PATH, priority = RECOMPILE_PRIORITY)]
        public static void Recompile() {
            var mono = AssetDatabase.FindAssets(typeof(RecompileUtility).Name)[0];
            AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(mono), ImportAssetOptions.ForceUpdate);
        }

        #endregion

        #region Prevent Recompile

        private static bool blocked = false;

        private static void PreventUpdate() {
            if(EditorApplication.isCompiling) {
                if(!blocked && !ToolkitEditorUtility.IsApplicationFocused()) {
                    Debug.Log(TAG + "Lock assembly reload\n(If this doesn't trigger a 'Unlock' log, manually recompile using 'Toolkit/Recompile')");
                    blocked = true;
                    EditorApplication.LockReloadAssemblies();
                }
            }
            if(blocked) {
                if(ToolkitEditorUtility.IsApplicationFocused()) {
                    Debug.Log(TAG + "Unlock assembly reload");
                    blocked = false;
                    EditorApplication.UnlockReloadAssemblies();
                    if(!EditorApplication.isCompiling)
                        Recompile();
                }
            }
        }

        #endregion
    }
}
