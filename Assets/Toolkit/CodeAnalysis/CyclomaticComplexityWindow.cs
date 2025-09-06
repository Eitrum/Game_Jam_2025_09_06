using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.CodeAnalysis {
    internal class CyclomaticComplexityWindow : EditorWindow {

        #region Menu Item

        [MenuItem("Toolkit/Code Analysis/Cyclomatic Complexity Viewer")]
        public static void Open() {
            var w = GetWindow<CyclomaticComplexityWindow>("Cyclomatic Complexity Viewer");
            w?.Close();
            w = GetWindow<CyclomaticComplexityWindow>("Cyclomatic Complexity Viewer");
            w.Show();
            w.minSize = new Vector2(300, 200);
            w.position = new Rect(50, 50, 300, 200);
        }

        #endregion

        #region Variables

        struct Data {
            public MonoScript script;
            public int ccTotal;
            public int classes;
            public int methods;
            public float result;
            public string resultText;

            public Data(MonoScript script) {
                this.script = script;
                CyclomaticComplexityUtility.Calculate(script, out ccTotal, out classes, out methods);
                result = (float)Mathf.Max(1, ccTotal) / Mathf.Max(1, methods);
                resultText = $"{result:0.000} [{ccTotal:000}]";
            }
        }

        private string path = "Assets/";
        private Vector2 scroll;
        private Data[] data;
        private string totalAverage = "";

        #endregion

        #region Styles

        private GUIStyle normal;
        private Texture2D backgroundTexture;

        private void OnEnable() {
            backgroundTexture = new Texture2D(2, 2);
            Color32[] colors = new Color32[4];
            for(int i = 0; i < colors.Length; i++) {
                colors[i] = new Color32(0, 0, 0, 15);
            }
            backgroundTexture.SetPixels32(colors);
            backgroundTexture.Apply();
            normal = new GUIStyle();
            normal.normal.background = backgroundTexture;
        }

        private void OnDisable() {
            DestroyImmediate(backgroundTexture);
        }

        #endregion

        #region Drawing

        private void OnGUI() {
            CalculateSize();
            DrawHeader();
            DrawBody();
        }

        private void DrawHeader() {
            GUI.Box(Header, "");
            GUILayout.BeginArea(GetMinus5(Header));
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Refresh", GUILayout.Width(65))) {
                Calculate();
            }
            path = EditorGUILayout.TextField(path, GUILayout.Width(160));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Total Average: " + (data == null ? "Undefined" : totalAverage));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawBody() {
            GUI.Box(Body, "");
            GUILayout.BeginArea(Body);
            scroll = GUILayout.BeginScrollView(scroll);
            if(data != null) {
                for(int i = 0; i < data.Length; i++) {
                    if(i % 2 == 0)
                        GUILayout.BeginHorizontal();
                    else
                        GUILayout.BeginHorizontal(normal);
                    GUILayout.Label($"{i:000} - {data[i].script.name}");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(data[i].resultText);
                    GUILayout.EndHorizontal();
                }
            }
            GUI.backgroundColor = Color.clear;
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region Calculate

        private void Calculate() {
            var assets = AssetDatabase.FindAssets("t:MonoScript");
            List<Data> scripts = new List<Data>();
            for(int i = 0; i < assets.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                if(path.StartsWith(this.path))
                    scripts.Add(new Data(AssetDatabase.LoadAssetAtPath<MonoScript>(path)));
            }
            scripts.Sort((a, b) => b.result.CompareTo(a.result));
            if(scripts.Count > 0) {
                totalAverage = ((float)scripts.Sum(x => x.result) / scripts.Count).ToString();
            }
            data = scripts.ToArray();
        }

        #endregion

        #region Size Calculation

        private void OnSizeChange(int width, int height) {

        }

        private int width = 0;
        private int height = 0;
        private int headerHeight = 26;

        private Vector2 Size => new Vector2(width, height);

        private Rect Header => new Rect(5, 5, width, headerHeight);

        private Rect Body => new Rect(5, headerHeight + 10, width, height - (headerHeight + 5));

        private Rect CodeBody => new Rect(10, headerHeight + 15, width / 2 - 7, height - (headerHeight + 15));

        private Rect SyntaxBody => new Rect(width / 2 + 8, headerHeight + 15, width / 2 - 7, height - (headerHeight + 15));

        private Rect GetMinus5(Rect rect) {
            rect.x += 5;
            rect.y += 5;
            rect.width -= 10;
            rect.height -= 10;
            return rect;
        }

        private void CalculateSize() {
            var newSize = position.size - new Vector2(10, 10);
            if(width != (int)(newSize.x) || height != (int)(newSize.y)) {
                width = (int)newSize.x;
                height = (int)newSize.y;
                OnSizeChange(width, height);
            }
        }

        #endregion
    }
}
