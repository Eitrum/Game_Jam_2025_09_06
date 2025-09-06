using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;

namespace Toolkit {
    public static class ToolkitEditorUtility {
        #region Variables

        public const string TAG = "<color=cyan>[Toolkit]</color> - ";

        #endregion

        #region Properties

        public static bool IsHoldingAlt => EditorStylesUtility.IsHoldingAlt;
        public static bool IsHoldingShift => EditorStylesUtility.IsHoldingShift;
        public static bool IsHoldingCtrl => EditorStylesUtility.IsHoldingCtrl;

        #endregion

        #region Persistent Data Path

        [MenuItem("Toolkit/Persistent Data Path/Open", priority = 100000)]
        public static void OpenPersistentDataPath() {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }

        [MenuItem("Toolkit/Persistent Data Path/Log", priority = 100001)]
        public static void LogPersistentDataPath() {
            Debug.Log(TAG + $"Persistent Data Path: '{Application.persistentDataPath}'");
        }

        #endregion

        #region Data Path

        [MenuItem("Toolkit/Data Path/Open", priority = 100005)]
        public static void OpenDataPath() {
            EditorUtility.OpenWithDefaultApp(Application.dataPath);
        }

        [MenuItem("Toolkit/Data Path/Log", priority = 100006)]
        public static void LogDataPath() {
            Debug.Log(TAG + $"Data Path: '{Application.dataPath}'");
        }

        #endregion

        #region Is Application Focused

#if UNITY_EDITOR_WIN

        public static bool IsApplicationFocused() {
            var activatedHandle = GetForegroundWindow();
            if(activatedHandle == IntPtr.Zero) {
                return false;
            }

            var procId = System.Diagnostics.Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

#else
        public static bool IsApplicationFocused() {
            return true;
        }
#endif

        #endregion

        #region Color

        private static int Bit(int a, int b) => (a & (1 << b)) >> b;

        public static Color GetColor(int i)
            => GetColor(i, 1f);

        public static Color GetColor(int i, float alpha) {
            if(i == 0)
                return new Color(0, 0.75f, 1.0f, 0.5f);
            int r = (Bit(i, 4) + Bit(i, 1) * 2 + 1) * 63;
            int g = (Bit(i, 3) + Bit(i, 2) * 2 + 1) * 63;
            int b = (Bit(i, 5) + Bit(i, 0) * 2 + 1) * 63;
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, alpha);
        }

        #endregion

        #region Repaint

        public static void RepaintInspectors() {
            var windows = EditorWindow.FindObjectsByType(typeof(EditorApplication).Assembly.GetType("UnityEditor.InspectorWindow"), FindObjectsSortMode.None);
            foreach(var w in windows)
                if(w is EditorWindow ew)
                    ew.Repaint();
        }

        public static void RepaintInspectorsDelayed() {
            EditorApplication.delayCall += RepaintInspectors;
        }

        #endregion

        public class InspectorScope : IDisposable {
            #region Variables

            public bool IsLocked => false; // Implement file lock support
            public bool ApplyChanges { get; set; } = true;

            public Editor Editor { get; private set; }
            public SerializedObject SerializedObject { get; private set; }

            public bool IsBox { get; private set; }
            public Rect VerticalLayoutArea { get; private set; }

            #endregion

            #region Properties

            public UnityEngine.Object Target => SerializedObject.targetObject;
            public UnityEngine.Object[] Targets => SerializedObject.targetObjects;

            #endregion

            #region Constructor

            public InspectorScope(Editor editor) : this(editor, false) { }

            public InspectorScope(SerializedObject serializedObject) : this(serializedObject, false) { }

            public InspectorScope(Editor editor, bool box) {
                this.Editor = editor;
                this.SerializedObject = editor.serializedObject;
                if(this.SerializedObject != null)
                    this.SerializedObject.UpdateIfRequiredOrScript();

                this.IsBox = box;
                VerticalLayoutArea = box ? EditorGUILayout.BeginVertical("box") : EditorGUILayout.BeginVertical();
            }

            public InspectorScope(SerializedObject serializedObject, bool box) {
                this.SerializedObject = serializedObject;
                if(this.SerializedObject != null)
                    this.SerializedObject.UpdateIfRequiredOrScript();

                this.IsBox = box;
                VerticalLayoutArea = box ? EditorGUILayout.BeginVertical("box") : EditorGUILayout.BeginVertical();
            }

            #endregion

            #region Draw

            public void DrawAll() {
                DrawDefault();
                DrawExtras();
            }

            public void DrawDefault() {
                DefaultInspectorUtility.DrawInspectorWithUserSettings(SerializedObject);
            }

            public void DrawExtras() {
                ButtonDrawer.DrawLayoutFromCache(SerializedObject);
                DebugViewDrawer.DrawLayoutFromCache(SerializedObject);
            }

            #endregion

            #region Dispose Impl

            public void Dispose() {
                if(ApplyChanges && SerializedObject != null && SerializedObject.hasModifiedProperties) {
                    SerializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndVertical();
            }

            #endregion

            #region Debugging

            public void PrintAllProperties() {
                StringBuilder sb = new StringBuilder();
                var iterator = this.SerializedObject.GetIterator();
                Debug.Log("Properties Of: " + Target.name);
                iterator.NextVisible(true);
                while(iterator.NextVisible(false)) {
                    iterator.FullLog();
                }
                Debug.Log("End: " + Target.name);

            }

            #endregion
        }

        public class DebugScope : IDisposable {
            #region Variables

            public Rect Area { get; private set; }

            #endregion

            #region Constructor

            public DebugScope() : this("Debug") { }

            public DebugScope(string header) {
                EditorGUILayout.Space();
                Area = EditorGUILayout.BeginVertical("box");
                if(!string.IsNullOrEmpty(header))
                    EditorGUILayout.LabelField(header, EditorStylesUtility.BoldLabel);
                EditorGUI.BeginDisabledGroup(true);
            }

            #endregion

            #region Dispose Impl

            public void Dispose() {
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }

            #endregion
        }

        public class LabelWidthScope : IDisposable {
            #region Variables

            public float PreviousWidth { get; private set; }
            public float Width { get; private set; }

            #endregion

            #region Constructor

            public LabelWidthScope(float value) {
                PreviousWidth = EditorGUIUtility.labelWidth;
                Width = value;
                EditorGUIUtility.labelWidth = Width;
            }

            #endregion

            #region Dispose Impl

            public void Dispose() {
                EditorGUIUtility.labelWidth = PreviousWidth;
            }

            #endregion
        }

        public class GUILayout {

            #region Matrix

            #region Content Row

            private static GUIContent[] row0 = new GUIContent[] { new GUIContent("m00"), new GUIContent("m01"),new GUIContent("m02"), new GUIContent("m03") };
            private static GUIContent[] row1 = new GUIContent[] { new GUIContent("m10"), new GUIContent("m11"),new GUIContent("m12"), new GUIContent("m13") };
            private static GUIContent[] row2 = new GUIContent[] { new GUIContent("m20"), new GUIContent("m21"),new GUIContent("m22"), new GUIContent("m23") };
            private static GUIContent[] row3 = new GUIContent[] { new GUIContent("m30"), new GUIContent("m31"),new GUIContent("m32"), new GUIContent("m33") };
            private static GUIContent[] GetContentRow(int index) {
                switch(index) {
                    case 0: return row0;
                    case 1: return row1;
                    case 2: return row2;
                    case 3: return row3;
                }
                throw new IndexOutOfRangeException();
            }

            #endregion

            public static Matrix4x4 DrawMatrix(string label, Matrix4x4 matrix) {
                EditorGUILayout.LabelField(label);
                using(new EditorGUI.IndentLevelScope(1)) {
                    matrix.SetRow(0, Internal_DrawRow(0, matrix.GetRow(0)));
                    matrix.SetRow(1, Internal_DrawRow(1, matrix.GetRow(1)));
                    matrix.SetRow(2, Internal_DrawRow(2, matrix.GetRow(2)));
                    matrix.SetRow(3, Internal_DrawRow(3, matrix.GetRow(3)));
                }
                return matrix;
            }

            private static float[] rowCache = new float[4];
            private static Vector4 Internal_DrawRow(int rowindex, Vector4 row) {
                rowCache[0] = row.x;
                rowCache[1] = row.y;
                rowCache[2] = row.z;
                rowCache[3] = row.w;
                var area = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                area = EditorGUI.IndentedRect(area);
                EditorGUI.MultiFloatField(area, row0, rowCache);
                row.x = rowCache[0];
                row.y = rowCache[1];
                row.z = rowCache[2];
                row.w = rowCache[3];
                return row;
            }

            #endregion
        }
    }
}
