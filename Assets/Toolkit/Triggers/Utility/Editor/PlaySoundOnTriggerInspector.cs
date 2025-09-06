using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit.Audio;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(PlaySoundOnTrigger))]
    public class PlaySoundOnTriggerInspector : Editor
    {
        #region Variables
        
        private SerializedProperty optionalSources;
        private SerializedProperty file;
        private SerializedProperty volumeMultiplier;
        private SerializedProperty delay;
        private SerializedProperty cancelIfDestroyed;

        private SerializedProperty followSource;
        private SerializedProperty sourceType;
        private SerializedProperty usePreset;
        private SerializedProperty preset;
        private SerializedProperty createSeperateAudioSource;

        #endregion

        #region Initialize

        private void OnEnable() {
            optionalSources = serializedObject.FindProperty("optionalSources");
            file = serializedObject.FindProperty("file");
            volumeMultiplier = serializedObject.FindProperty("volumeMultiplier");
            delay = serializedObject.FindProperty("delay");
            cancelIfDestroyed = serializedObject.FindProperty("cancelIfDestroyed");

            followSource = serializedObject.FindProperty("followSource");
            sourceType = serializedObject.FindProperty("sourceType");
            usePreset = serializedObject.FindProperty("usePreset");
            preset = serializedObject.FindProperty("preset");
            createSeperateAudioSource = serializedObject.FindProperty("createSeperateAudioSource");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                TriggerEditorUtility.CheckForTriggerWithOptionalSources(target, optionalSources);
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(file);
                    if(file.FindPropertyRelative("reference").objectReferenceValue is AudioPlayer) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            EditorGUILayout.PropertyField(followSource);
                            EditorGUILayout.PropertyField(sourceType);
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(volumeMultiplier);
                    EditorGUILayout.PropertyField(delay);
                    if(delay.floatValue > Mathf.Epsilon)
                        using(new EditorGUI.IndentLevelScope(1))
                            EditorGUILayout.PropertyField(cancelIfDestroyed);
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
            }
        }

        #endregion
    }
}
