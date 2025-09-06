using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit.Audio {
    [CustomEditor(typeof(AudioPlayer))]
    public class AudioPlayerInspector : Editor {
        #region Variables

        private SerializedProperty settingsPreset;
        private SerializedProperty settings;

        //private SerializedProperty variationPreset;
        private SerializedProperty clips;
        //private ReorderableList clipsList;

        private SerializedProperty mode;
        private SerializedProperty poolingCount;

        #endregion

        #region Init

        private void OnEnable() {
            settingsPreset = serializedObject.FindProperty("settingsPreset");
            settings = serializedObject.FindProperty("settings");

            //variationPreset = serializedObject.FindProperty("variationPreset");
            clips = serializedObject.FindProperty("clips");
            //clipsList = new ReorderableList(serializedObject, clips);
            //clipsList.drawHeaderCallback += DrawClipHeader;
            //clipsList.drawElementCallback += DrawClipElement;

            mode = serializedObject.FindProperty("poolingMode");
            poolingCount = serializedObject.FindProperty("poolingCount");
        }

        #endregion

        #region Draw

        private void DrawClipHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Clips", EditorStyles.boldLabel);
        }

        private void DrawClipElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect clipArea, 20f / rect.width, 2f);
            EditorGUI.LabelField(indexArea, $"{index}");
            EditorGUI.PropertyField(clipArea, clips.GetArrayElementAtIndex(index), GUIContent.none);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(settingsPreset);
                if(settingsPreset.objectReferenceValue == null)
                    EditorGUILayout.PropertyField(settings);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(mode);
                EditorGUILayout.PropertyField(poolingCount);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Clips", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(clips);
                //EditorGUILayout.PropertyField(variationPreset);
                //if(variationPreset.objectReferenceValue == null)
                //    clipsList.DoLayoutList();
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
