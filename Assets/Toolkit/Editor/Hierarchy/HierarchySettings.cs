using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class HierarchySettings : ScriptableObject {

        public enum BackgroundMode {
            None,
            Small,
            Medium,
            Full,
        }

        #region Variables

        private const string TAG = "[Toolkit.HierarchySettings] - ";
        private const string SAVE_PATH = "UserSettings/HierarchySettings.asset";

        private static HierarchySettings settings;
        private static SerializedObject serializedObject;

        [SerializeField] private bool stripes = false;
        [SerializeField] private BackgroundMode backgroundMode = BackgroundMode.None;
        [SerializeField, Range(0f, 1f)] private float childColorMultiplier = 0.5f;
        [SerializeField, HierarchyCustomDrawerSelector] private string[] drawers = { };

        #endregion

        #region Properties

        public static bool Stripes => settings.stripes;
        public static BackgroundMode Background => settings.backgroundMode;
        public static float ChildColorMultiplier => settings.childColorMultiplier;
        public static IReadOnlyList<string> Drawers => settings.drawers;

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Initialize() {
            var objs = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(SAVE_PATH);
            if(objs == null || objs.Length == 0)
                settings = ScriptableObject.CreateInstance<HierarchySettings>();
            if(objs.Length > 0 && objs[0] is HierarchySettings hs) {
                settings = hs;
            }
            settings.hideFlags = HideFlags.DontUnloadUnusedAsset | HideFlags.DontSaveInEditor;
            serializedObject = new SerializedObject(settings);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Editor/Hierarchy", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        #endregion

        #region OnGUI

        private static void OnGUI(string filter) {
            if(serializedObject == null)
                serializedObject = new SerializedObject(settings);
            serializedObject.Update();
            var entry = serializedObject.FindScriptableObjectEntry();
            do {
                EditorGUILayout.PropertyField(entry, true);
            } while(entry.NextVisible(false));

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { settings }, SAVE_PATH, true);
                HierarchyDrawer.Rebuild();
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        #endregion
    }
}
