using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Audio
{
    [CustomEditor(typeof(PlaySoundAwake))]
    public class PlaySoundAwakeInspector : Editor
    {
        #region Variables

        private SerializedProperty file;
        private SerializedProperty volumeMultiplier;
        private SerializedProperty delay;

        private SerializedProperty follow;
        private SerializedProperty usePreset;
        private SerializedProperty preset;
        private SerializedProperty createSeperateAudioSource;

        #endregion

        #region Initialize

        private void OnEnable() {
            file = serializedObject.FindProperty("file");
            volumeMultiplier = serializedObject.FindProperty("volumeMultiplier");
            delay = serializedObject.FindProperty("delay");
            follow = serializedObject.FindProperty("follow");
            usePreset = serializedObject.FindProperty("usePreset");
            preset = serializedObject.FindProperty("preset");
            createSeperateAudioSource = serializedObject.FindProperty("createSeperateAudioSource");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            serializedObject.Update();

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(file);
                if(file.FindPropertyRelative("reference").objectReferenceValue is AudioPlayer) {
                    using(new EditorGUI.IndentLevelScope(1))
                        EditorGUILayout.PropertyField(follow);
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(volumeMultiplier);
                EditorGUILayout.PropertyField(delay);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUILayout.HorizontalScope()) {
                    usePreset.boolValue = GUILayout.Toggle(usePreset.boolValue, GUIContent.none, GUILayout.Width(20));
                    using(new EditorGUI.DisabledScope(!usePreset.boolValue)) {
                        EditorGUILayout.PropertyField(preset, new GUIContent("Audio Source Settings"));
                    }
                }
                if(usePreset.boolValue) {
                    using(new EditorGUI.IndentLevelScope(1)) {
                        EditorGUILayout.PropertyField(createSeperateAudioSource);
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
