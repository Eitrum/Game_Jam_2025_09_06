using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.QuickInspect {
    public static class QuickInspectSettings {
        #region Variables


        private const string ENABLED_PATH = "Tookit.QuickInspect.Enabled";
        private const string RECURSON_LIMIT_PATH = "Tookit.QuickInspect.RecursionLimit";
        private const string ALLOW_EDIT_PATH = "Tookit.QuickInspect.AllowEdit";
        private const string RENDER_COLORS_PATH = "Tookit.QuickInspect.Color";
        private const string RENDER_SHADOW_PATH = "Tookit.QuickInspect.Shadow";
        private const string DIALOGUE_CREATE_ASSET_PATH = "Tookit.QuickInspect.Dialogue.CreateAsset";
        private const string DIALOGUE_DELETE_COMPONENT_PATH = "Tookit.QuickInspect.Dialogue.DeleteComponent";

        private static bool enabled = false;
        private static int recursionLimit = 4;
        private static bool allowEdit = false;
        private static bool renderColor = false;
        private static bool renderShadow = false;
        private static bool dialogueCreateAsset = true;
        private static bool dialogueDeleteComponent = true;

        #endregion

        #region Properties

        public static bool Enabled {
            get => enabled;
            set {
                if(enabled == value) return;

                enabled = value;
                EditorPrefs.SetBool(ENABLED_PATH, value);
            }
        }

        public static int RecursionLimit {
            get => recursionLimit;
            set {
                value = Mathf.Clamp(value, 0, 12);
                if(recursionLimit == value) return;

                recursionLimit = value;
                EditorPrefs.SetInt(RECURSON_LIMIT_PATH, recursionLimit);
            }
        }

        public static bool AllowEdit {
            get => allowEdit;
            set {
                if(allowEdit == value) return;
                allowEdit = value;
                EditorPrefs.SetBool(ALLOW_EDIT_PATH, value);
            }
        }

        public static bool RenderColor {
            get => renderColor;
            set {
                if(renderColor == value) return;
                renderColor = value;
                EditorPrefs.SetBool(RENDER_COLORS_PATH, value);
            }
        }

        public static bool RenderShadow {
            get => renderShadow;
            set {
                if(renderShadow == value) return;
                renderShadow = value;
                EditorPrefs.SetBool(RENDER_SHADOW_PATH, value);
            }
        }

        public static bool DialogueCreateAsset {
            get => dialogueCreateAsset;
            set {
                if(dialogueCreateAsset == value) return;
                dialogueCreateAsset = value;
                EditorPrefs.SetBool(DIALOGUE_CREATE_ASSET_PATH, value);
            }
        }

        public static bool DialogueDeleteComponent {
            get => dialogueDeleteComponent;
            set {
                if(dialogueDeleteComponent == value) return;
                dialogueDeleteComponent = value;
                EditorPrefs.SetBool(DIALOGUE_DELETE_COMPONENT_PATH, value);
            }
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Init() {
            enabled = EditorPrefs.GetBool(ENABLED_PATH, false);
            recursionLimit = EditorPrefs.GetInt(RECURSON_LIMIT_PATH, 4);
            allowEdit = EditorPrefs.GetBool(ALLOW_EDIT_PATH, false);
            renderColor = EditorPrefs.GetBool(RENDER_COLORS_PATH, true);
            renderShadow = EditorPrefs.GetBool(RENDER_SHADOW_PATH, true);
            dialogueCreateAsset = EditorPrefs.GetBool(DIALOGUE_CREATE_ASSET_PATH, true);
            dialogueDeleteComponent = EditorPrefs.GetBool(DIALOGUE_DELETE_COMPONENT_PATH, true);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Editor/Quick Inspect", SettingsScope.Project) {
                guiHandler = OnSettingsGUI
            };
        }

        #endregion

        #region Draw

        private static void OnSettingsGUI(string obj) {
            using(new EditorGUILayout.VerticalScope("box")) {
                Enabled = EditorGUILayout.Toggle("Enabled", Enabled);
                using(new EditorGUI.DisabledScope(!enabled)) {
                    RecursionLimit = EditorGUILayout.IntSlider("Recursion Limit", RecursionLimit, 0, 12);
                    AllowEdit = EditorGUILayout.Toggle("Allow Edit", AllowEdit);
                    EditorGUILayout.Space();
                    RenderColor = EditorGUILayout.Toggle("Render Color", RenderColor);
                    RenderShadow = EditorGUILayout.Toggle("Render Shadow", RenderShadow);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Dialogue Confirmation", EditorStyles.boldLabel);
                    DialogueCreateAsset = EditorGUILayout.Toggle("Create Asset", DialogueCreateAsset);
                    DialogueDeleteComponent = EditorGUILayout.Toggle("Delete Component", DialogueDeleteComponent);
                }
            }
        }

        #endregion
    }
}
