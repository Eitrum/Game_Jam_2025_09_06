using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    internal static class TodoCSharp {
        private static Dictionary<MonoScript, Container> todos = new Dictionary<MonoScript, Container>();

        public static IReadOnlyDictionary<MonoScript, Container> Todos => todos;

        public static void LoadAll() {
            var assets = AssetDatabase.FindAssets("t:MonoScript");

            foreach (var asset in assets) {
                var p = AssetDatabase.GUIDToAssetPath(asset);
                if (!p.StartsWith("Assets"))
                    continue;
                var ms = AssetDatabase.LoadAssetAtPath<MonoScript>(p);
                if (todos.ContainsKey(ms))
                    continue;
                if (ms.name.Equals("TodoCSharpViewWindow"))
                    continue;
                if (ms.text.Contains("TODO:")) {
                    todos.TryAdd(ms, new Container(ms));
                }
            }
        }

        public class Container {
            public MonoScript MonoScript { get; private set; }
            public List<Data> Lines = new List<Data>();

            public Container() { }
            public Container(MonoScript monoScript) {
                this.MonoScript = monoScript;
                var split = monoScript.text.Split('\n');
                for (int i = 0, length = split.Length; i < length; i++) {
                    var line = split[i];
                    if (line.Contains("TODO:")) {
                        Lines.Add(new Data() {
                            Description = line.Trim(),
                            LineNumber = i,
                        });
                    }
                }
            }

            public struct Data {
                public string Description;
                public int LineNumber;
            }
        }
    }

    internal class TodoCSharpViewWindow : EditorWindow {
        #region Variables

        private static readonly GUIContent Title = new GUIContent("Todo C#");
        private Vector2 scroll;
        private GUILayoutOption textWidth;

        public class Styles {
            public static readonly GUIStyle DescriptionStyle = new GUIStyle(EditorStyles.label) { wordWrap = false, richText = true };
        }

        #endregion

        #region Menu

        [MenuItem("Window/Toolkit/Todo C# View")]
        public static void Open() {
            var w = GetWindow<TodoCSharpViewWindow>();
            w.titleContent = Title;
            w.Show();
        }

        #endregion

        #region init

        private void OnEnable() {
            TodoCSharp.LoadAll();
        }

        #endregion

        #region Draw

        private void OnGUI() {
            var area = new Rect(Vector2.zero, position.size);
            textWidth = GUILayout.Width(area.width - 80f);
            EditorGUI.DrawRect(new Rect(area.width - 80f, 0, 2f, area.height), new Color(0.3f, 0.3f, 0.3f, 0.3f));
            var scipts = TodoCSharp.Todos;
            using (new GUILayout.AreaScope(area)) {
                using (var s = new GUILayout.ScrollViewScope(scroll)) {
                    scroll = s.scrollPosition;
                    foreach (var t in scipts)
                        Draw(t.Value);
                }
            }
        }

        private void Draw(TodoCSharp.Container container) {
            if (container.Lines.Count > 1) {
                EditorGUILayout.LabelField($"{container.MonoScript.name}.cs", EditorStyles.boldLabel, textWidth);
                using (new EditorGUI.IndentLevelScope(1)) {
                    foreach (var l in container.Lines) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField($"{l.Description}", Styles.DescriptionStyle, textWidth);
                            GUILayout.Label($"@{l.LineNumber}");
                        }
                    }
                }
            }
            else {
                var l = container.Lines[0];
                using (new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Label($"<b>{container.MonoScript.name}.cs - </b>{l.Description}", Styles.DescriptionStyle, textWidth);
                    GUILayout.Label($"@{l.LineNumber}");
                }
            }
        }

        #endregion
    }
}
