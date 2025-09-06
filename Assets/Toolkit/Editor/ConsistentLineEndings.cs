using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit
{
    internal class ConsistentLineEndings
    {
        private const string TAG = "<color=#00FFFF>[Consistent Line Endings]</color> - ";
        private const string CLE_PREFS_PATH = "Toolkit.Consistent_Line_Endings";

        private static bool hasStartedCompilation = false;

        public static bool Enabled {
            get => EditorPrefs.GetBool(CLE_PREFS_PATH, false);
            set {
                if(value != Enabled) {
                    EditorPrefs.SetBool(CLE_PREFS_PATH, value);
                }
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize() {
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload += BeforeReload;
            ToolkitProjectSettings.RegisterEditor("Recompile", 1, ProjectSettingsGUI);
        }

        private static void ProjectSettingsGUI(string obj) {
            Enabled = EditorGUILayout.Toggle("Consistent Line Endings", Enabled);
        }

        private static void BeforeReload() {
            if(!Enabled || hasStartedCompilation) {
                return;
            }
            hasStartedCompilation = true;
            Debug.Log(TAG + "Making consistent line endings!");
            var scripts = AssetDatabaseUtility.LoadAssets<MonoScript>("Assets");
            foreach(var ms in scripts) {
                if(!AssetDatabase.GetAssetPath(ms).EndsWith(".cs"))
                    continue;
                var text = ms.text;
                if(text.Contains("\r\n")) // Replace line endings
                    text = text.Replace("\r\n", "\n");
                if(!text.EndsWith("\n")) // Add line at end of document
                    text += "\n";
                else if(text.EndsWith("\n\n")) // Remove extra lines from end of document
                    text = text.Remove(text.Length - 1, 1);
                if(ms.text != text) {
                    var path = AssetDatabase.GetAssetPath(ms);
                    System.IO.File.WriteAllText(path, text);
                    Debug.Log(TAG + $"Found inconsistent file @ {path}");
                }
            }
        }
    }
}
