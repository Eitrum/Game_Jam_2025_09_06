using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public static class ToolkitProjectSettings {
        #region IProject Settings

        public interface IProjectSettings {
            string Name { get; }
            int Order { get; }
            event Action<string> OnRender;
            void Render(string search);
        }

        public class ProjectSettings : IProjectSettings {
            public string Name { get; private set; } = "null";
            public int Order { get; private set; } = 0;

            private Action<string> onRender;
            public event Action<string> OnRender {
                add => onRender += value;
                remove => onRender -= value;
            }

            public void Render(string search) {
                onRender?.Invoke(search);
            }

            public ProjectSettings(string name, int order, Action<string> onRender) {
                this.Name = name;
                this.Order = order;
                this.onRender = onRender;
            }
        }

        #endregion

        #region Variables

        private static Texture2D toolkitBanner;
        private static Vector2 scroll;

        private static List<IProjectSettings> projectSettings = new List<IProjectSettings>();
        private static List<IProjectSettings> editorProjectSettings = new List<IProjectSettings>();

        private static GUIStyle RichLabel => EditorStylesUtility.RichTextLabel;

        #endregion

        #region Initialize

        [InitializeOnLoadMethod]
        private static void LoadTextures() {
            toolkitBanner = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Editor/Content/toolkitbanner.png");
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit", SettingsScope.Project) {
                guiHandler = OnGUIToolkit
            };
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProviderEditor() {
            return new SettingsProvider("Project/Toolkit/Editor", SettingsScope.Project) {
                guiHandler = OnGUIToolkitEditor
            };
        }

        #endregion

        #region Register

        public static void RegisterEditor(string name, int order, Action<string> onRender) {
            for (int i = editorProjectSettings.Count - 1; i >= 0; i--) {
                if (editorProjectSettings[i].Name == name) {
                    editorProjectSettings[i].OnRender += onRender;
                    return;
                }
            }
            editorProjectSettings.Add(new ProjectSettings(name, order, onRender));
            editorProjectSettings.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        public static void RegisterEditor(IProjectSettings projectSettings) {
            editorProjectSettings.Add(projectSettings);
            editorProjectSettings.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        #endregion

        #region Draw Main Toolkit

        private static void OnGUIToolkit(string obj) {
            DrawBanner();
            GUILayout.Space(12f);
            using (var s = new GUILayout.ScrollViewScope(scroll)) {
                DrawColorInformation();
                scroll = s.scrollPosition;
            }
        }

        private static void DrawBanner() {
            var area = GUILayoutUtility.GetRect(256, 64);
            area.width = 256;
            area.x += 30;
            GUI.DrawTexture(area, toolkitBanner, ScaleMode.ScaleToFit);
        }

        private static void DrawColorInformation() {
            using (new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Console Color Coding", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.CYAN + "[Cyan]</color>\t\t - Editor logs", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.MAGENTA + "[Magenta]</color>\t\t - Recompile", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.LIME + "[Lime</color>/<color=red>Red]</color>\t\t - Reserved <color=grey>(Enabled/Disabled) (Positive/Negative)</color>", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.ORANGE + "[Orange]</color>\t\t - Networking", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.YELLOW + "[Yellow]</color>\t\t - Database", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.PURPLE + "[Purple]</color>\t\t - Turn Manager", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.GREY + "[Grey]</color>\t\t - Command Line Arguments", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.BLUE + "[Blue]</color>\t\t\t - Singletons", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.OLIVE + "[Olive]</color>\t\t - IO <color=grey>(Buffers, Toolkit Markdown Language, etc)</color>", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.TEAL + "[Teal]</color>\t\t\t - Scene management", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.NAVY + "[Navy]</color>\t\t - Version Control", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.BROWN + "[Brown]</color>\t\t - Toolkit.UI", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.SILVER + "[Silver]</color>\t\t - Default", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.BLACK + "[Black]</color>\t\t - ", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.GREEN + "[Green]</color>\t\t - Blocked", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.MAROON + "[Maroon]</color>\t\t - Blocked", RichLabel);
                EditorGUILayout.LabelField(ColorTable.RichTextTags.WHITE + "[White]</color>\t\t - Others", RichLabel);
            }
        }

        #endregion

        #region Draw Toolkit Editor

        private static void OnGUIToolkitEditor(string search) {
            for (int i = 0, length = editorProjectSettings.Count; i < length; i++) {
                using (new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField(editorProjectSettings[i].Name, EditorStylesUtility.RichTextBoldLabel);
                    editorProjectSettings[i].Render(search);
                }
            }
        }

        #endregion
    }
}
