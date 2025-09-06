using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit.Audio {
    [CustomEditor(typeof(AudioVariation))]
    public class AudioVariationInspector : Editor {
        private SerializedProperty path;
        private SerializedProperty clips;
        private ReorderableList list;

        private void OnEnable() {
            path = serializedObject.FindProperty("path");
            clips = serializedObject.FindProperty("clips");

            list = new ReorderableList(serializedObject, clips);
            list.drawHeaderCallback += DrawHeader;
            list.drawElementCallback += DrawElement;
            list.onAddCallback += AddElement;
        }

        private void AddElement(ReorderableList list) {
            var index = clips.arraySize++;
            var element = clips.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("weight").floatValue = 1f;
        }

        private void DrawHeader(Rect rect) {
            rect.PadRef(12, 0, 0, 0);
            rect.SplitHorizontal(out Rect index, out Rect clip, out Rect weight, 20f / rect.width, 1f - (120f / rect.width), 2f);
            EditorGUI.LabelField(index, "#");
            EditorGUI.LabelField(clip, "Clip");
            EditorGUI.LabelField(weight, "Weight");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.Pad(0, 0, 1, 1).SplitHorizontal(out Rect indexArea, out Rect clipArea, out Rect weightArea, 20f / rect.width, 1f - (120f / rect.width), 2f);
            weightArea.height = EditorGUIUtility.singleLineHeight;
            indexArea.height = EditorGUIUtility.singleLineHeight;

            var element = clips.GetArrayElementAtIndex(index);
            EditorGUI.LabelField(indexArea, $"{index}");
            EditorGUI.ObjectField(clipArea, element.FindPropertyRelative("clip"), GUIContent.none);
            //NSOEditor.DrawProperty(clipArea, element.FindPropertyRelative("clip"), GUIContent.none, typeof(IAudioClipPlayer));
            var weight = element.FindPropertyRelative("weight");
            weight.floatValue = EditorGUI.FloatField(weightArea, weight.floatValue);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            path.stringValue = EditorGUILayout.TextField(path.stringValue);
            if(string.IsNullOrEmpty(path.stringValue)) {
                var textArea = GUILayoutUtility.GetLastRect();
                EditorGUI.LabelField(textArea, $"{target.name}", EditorStylesUtility.GrayItalicLabel);
            }

            list.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
