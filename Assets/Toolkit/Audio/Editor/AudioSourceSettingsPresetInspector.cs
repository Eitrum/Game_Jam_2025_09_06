using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Audio
{
    [CustomEditor(typeof(AudioSourceSettingsPreset))]
    public class AudioSourceSettingsPresetInspector : Editor
    {
        #region Variables

        private SerializedProperty output;
        private SerializedProperty bypassEffects;
        private SerializedProperty bypassListenerEffects;
        private SerializedProperty bypassReverbZones;

        private SerializedProperty priority;
        private SerializedProperty volume;
        private SerializedProperty pitch;
        private SerializedProperty stereoPan;
        private SerializedProperty spatialBlend;
        private SerializedProperty reverbZoneMix;

        private SerializedProperty dopplerLevel;
        private SerializedProperty spread;
        private SerializedProperty volumeRolloff;
        private SerializedProperty distance;
        private SerializedProperty volumeRolloffCurve;

        #endregion

        #region Initialize

        private void OnEnable() {
            output = serializedObject.FindProperty("output");
            bypassEffects = serializedObject.FindProperty("bypassEffects");
            bypassListenerEffects = serializedObject.FindProperty("bypassListenerEffects");
            bypassReverbZones = serializedObject.FindProperty("bypassReverbZones");

            priority = serializedObject.FindProperty("priority");
            volume = serializedObject.FindProperty("volume");
            pitch = serializedObject.FindProperty("pitch");
            stereoPan = serializedObject.FindProperty("stereoPan");
            spatialBlend = serializedObject.FindProperty("spatialBlend");
            reverbZoneMix = serializedObject.FindProperty("reverbZoneMix");

            dopplerLevel = serializedObject.FindProperty("dopplerLevel");
            spread = serializedObject.FindProperty("spread");
            volumeRolloff = serializedObject.FindProperty("volumeRolloff");
            distance = serializedObject.FindProperty("distance");
            volumeRolloffCurve = serializedObject.FindProperty("volumeRolloffCurve");
        }

        #endregion

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Effect", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(output);
                using(new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField("Bypass", GUILayout.Width(80f));
                    GUILayout.FlexibleSpace();
                    bypassEffects.boolValue = EditorGUILayout.ToggleLeft("Effects", bypassEffects.boolValue, GUILayout.Width(100f));
                    using(new EditorGUI.DisabledScope(output.objectReferenceValue != null))
                        bypassListenerEffects.boolValue = EditorGUILayout.ToggleLeft("Listener", bypassListenerEffects.boolValue, GUILayout.Width(120f));
                    bypassReverbZones.boolValue = EditorGUILayout.ToggleLeft("Reverb Zones", bypassReverbZones.boolValue, GUILayout.Width(120f));
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Main", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(priority);
                EditorGUILayout.PropertyField(volume);
                EditorGUILayout.PropertyField(pitch);
                EditorGUILayout.PropertyField(stereoPan);
                EditorGUILayout.PropertyField(spatialBlend);
                EditorGUILayout.PropertyField(reverbZoneMix);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("3D Settings");
                EditorGUILayout.PropertyField(dopplerLevel);
                EditorGUILayout.PropertyField(spread);
                EditorGUILayout.PropertyField(volumeRolloff);
                if(volumeRolloff.intValue.ToEnum<AudioRolloffMode>() == AudioRolloffMode.Custom)
                    EditorGUILayout.PropertyField(volumeRolloffCurve);
                EditorGUILayout.PropertyField(distance);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
