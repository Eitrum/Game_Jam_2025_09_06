using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(SimpleCaptureAddon))]
    public class SimpleCaptureAddonInspector : Editor
    {
        #region Variables

        private SerializedProperty openFileLocation;
        private SerializedProperty directoryMode;
        private SerializedProperty customDirectory;

        private SerializedProperty captureAudioClip;
        private SerializedProperty audioSettings;

        #endregion

        #region Init

        private void OnEnable() {
            openFileLocation = serializedObject.FindProperty("openFileLocation");
            directoryMode = serializedObject.FindProperty("directoryMode");
            customDirectory = serializedObject.FindProperty("customDirectory");
            captureAudioClip = serializedObject.FindProperty("captureAudioClip");
            audioSettings = serializedObject.FindProperty("audioSettings");
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var sca = (SimpleCaptureAddon)target;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField($"File", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(openFileLocation);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(directoryMode);
                if(directoryMode.intValue.ToEnum<SimpleCaptureAddon.ScreenshotDirectoryMode>() == SimpleCaptureAddon.ScreenshotDirectoryMode.Custom)
                    using(new EditorGUI.IndentLevelScope(1))
                        EditorGUILayout.PropertyField(customDirectory);
                if(EditorGUI.EndChangeCheck()) {
                    sca.UpdateDirectory();
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField($"Audio", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(captureAudioClip);
                EditorGUILayout.PropertyField(audioSettings);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
