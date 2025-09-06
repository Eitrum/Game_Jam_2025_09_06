using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

namespace Toolkit.Utility {
    [InitializeOnLoad]
    public static class AutoSave {

        #region Variables

        private const string TAG = ColorTable.RichTextTags.CYAN+"[Autosave]</color> - ";
        private const string AUTO_SAVE_PREFS_PATH = "Toolkit.AutoSave";

        private static bool hasSavedLostFocus = false;

        public static bool Enabled {
            get => EditorPrefs.GetBool(AUTO_SAVE_PREFS_PATH, false);
            set {
                if(value != Enabled) {
                    EditorPrefs.SetBool(AUTO_SAVE_PREFS_PATH, value);
                }
            }
        }

        #endregion

        #region Initialize

        static AutoSave() {
            ToolkitProjectSettings.RegisterEditor("Auto Save", 0, ProjectSettingGUI);
            EditorApplication.update += Update;
        }

        #endregion

        #region Methods

        private static void ProjectSettingGUI(string obj) {
            Enabled = EditorGUILayout.Toggle("On Lost Focus", Enabled);
        }

        static void Update() {
            if(Application.isPlaying) {
                return;
            }

            if(Enabled) {
                var hasFocus = ToolkitEditorUtility.IsApplicationFocused();
                if(!hasSavedLostFocus && !hasFocus) {
                    hasSavedLostFocus = true;
                    Save();
                }
                if(hasFocus) {
                    hasSavedLostFocus = false;
                }
            }
        }

        private static void Save() {
            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log(TAG + "Saving...");
        }

        #endregion
    }
}
