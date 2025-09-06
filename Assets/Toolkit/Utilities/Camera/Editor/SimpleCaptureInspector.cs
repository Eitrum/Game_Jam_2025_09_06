using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(SimpleCapture))]
    public class SimpleCaptureInspector : Editor
    {
        #region Variables

        private SerializedProperty format;
        private SerializedProperty mode;
        private SerializedProperty width;
        private SerializedProperty height;
        private SerializedProperty upscale;
        private SerializedProperty captureKey;

        private Texture2D texture;

        private float editorWidth = 0f;

        #endregion

        #region Enable / Destroy

        private void OnEnable() {
            format = serializedObject.FindProperty("format");
            mode = serializedObject.FindProperty("mode");
            width = serializedObject.FindProperty("width");
            height = serializedObject.FindProperty("height");
            upscale = serializedObject.FindProperty("upscale");
            captureKey = serializedObject.FindProperty("captureKey");
        }

        private void OnDestroy() {
            if(texture)
                DestroyImmediate(texture);
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Config", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(format);
                EditorGUILayout.PropertyField(mode);
                var mValue = (SimpleCapture.Mode)mode.intValue;
                using(new EditorGUI.IndentLevelScope(1)) {
                    switch(mValue) {
                        case SimpleCapture.Mode.Upscale:
                            EditorGUILayout.PropertyField(upscale);
                            break;
                        case SimpleCapture.Mode.CustomValues:
                            using(new EditorGUILayout.HorizontalScope()) {
                                EditorGUILayout.LabelField("Screen");
                                GUILayout.FlexibleSpace();
                                width.intValue = Mathf.Max(1, EditorGUILayout.IntField(width.intValue));
                                GUILayout.Label("x");
                                height.intValue = Mathf.Max(1, EditorGUILayout.IntField(height.intValue));
                            }
                            break;
                        case SimpleCapture.Mode.Screen:
                            using(new EditorGUI.DisabledScope(true))
                                EditorGUILayout.LabelField($"Screen ({Screen.width} x {Screen.height})");
                            break;
                    }
                }
                EditorGUILayout.PropertyField(captureKey);
            }
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            var cap = target as SimpleCapture;

            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button("Capture", GUILayout.Width(100f))) {
                    if(texture) {
                        DestroyImmediate(texture);
                    }
                    texture = cap.Capture(cap.Camera.clearFlags == CameraClearFlags.Depth);
                    texture.Apply();
                }

                using(new EditorGUI.DisabledScope(texture == null)) {
                    if(GUILayout.Button("Save", GUILayout.Width(100f))) {
                        Screenshot.WriteToFile(texture, cap.Format);
                    }
                    if(GUILayout.Button("Clear", GUILayout.Width(100f))) {
                        DestroyImmediate(texture);
                    }
                }

                GUILayout.FlexibleSpace();
            }
            var tempEW = GUILayoutUtility.GetLastRect().width;
            if(!tempEW.Equals(1f, 0.1f))
                editorWidth = tempEW;

            if(texture) {
                var r = GUILayoutUtility.GetRect(editorWidth, editorWidth / cap.Ratio);
                EditorGUI.DrawRect(r, Color.black);
                GUI.DrawTexture(r.Shrink(2f), texture);
            }
        }

        #endregion
    }
}
