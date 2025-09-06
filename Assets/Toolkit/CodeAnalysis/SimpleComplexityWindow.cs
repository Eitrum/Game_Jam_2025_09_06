using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.CodeAnalysis {
    public class SimpleComplexityWindow : EditorWindow {
        #region Menu Item

        [MenuItem("Toolkit/Code Analysis/Simple Complexity Viewer")]
        public static void Open() {
            var w = GetWindow<SimpleComplexityWindow>("Simple Complexity Viewer");
            w?.Close();
            w = GetWindow<SimpleComplexityWindow>("Simple Complexity Viewer");
            w.Show();
            w.minSize = new Vector2(300, 200);
            w.position = new Rect(50, 50, 300, 200);
        }

        #endregion

        #region Variables

        struct Data {
            public FileComplexity file;
            public float result;
            public string resultText;

            public Data(MonoScript script) {
                file = ComplexityUtility.Calculate(script);
                result = file.CalculatedComplexity;
                resultText = $"Calculated [{result:0.00}]";
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
                    GUILayout.Label($"{i:000} - {data[i].file.Name}");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"Field [{data[i].file.FieldsComplexity:0.00}]", GUILayout.Width(120));
                    GUILayout.Label($"Property [{data[i].file.PropertiesComplexity:0.00}]", GUILayout.Width(120));
                    GUILayout.Label($"Method [{data[i].file.MethodsComplexity:0.00}]", GUILayout.Width(120));
                    GUILayout.Label($"LoC [{data[i].file.LoCComplexity:0.00}]", GUILayout.Width(120));
                    GUILayout.Label($"CC [{data[i].file.CCComplexity:0.00}]", GUILayout.Width(120));
                    GUILayout.Label(data[i].resultText, GUILayout.Width(120));
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
