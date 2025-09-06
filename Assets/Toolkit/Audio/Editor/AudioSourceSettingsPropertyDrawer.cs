using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Audio
{
    [CustomPropertyDrawer(typeof(AudioSourceSettings))]
    public class AudioSourceSettingsPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(!property.isExpanded)
                return EditorGUIUtility.singleLineHeight + 2f;
            if(property.FindPropertyRelative("volumeRolloff").intValue.ToEnum<AudioRolloffMode>() == AudioRolloffMode.Custom)
                return 310f + EditorGUIUtility.singleLineHeight;
            return 310f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.Box(position, "");
            position.ShrinkRef(1f);

            // Handle Foldout
            var foldoutArea = new Rect(position);
            foldoutArea.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldoutArea, property.isExpanded, label, true);

            if(!property.isExpanded)
                return;

            EditorGUI.indentLevel++;
            // Split
            position.y += EditorGUIUtility.singleLineHeight;
            position.height -= EditorGUIUtility.singleLineHeight;
            var eff = 3f;
            var mai = 7f;
            var _3d = property.FindPropertyRelative("volumeRolloff").intValue.ToEnum<AudioRolloffMode>() == AudioRolloffMode.Custom ? 6f : 5f;
            var total = eff + mai + _3d;

            position.SplitVertical(out Rect effectsArea, out Rect mainArea, out Rect _3dSettingsArea, eff / total, mai / total, 4f);
            GUI.Box(effectsArea, "");
            GUI.Box(mainArea, "");
            GUI.Box(_3dSettingsArea, "");

            // Output / Bypass
            effectsArea.SplitVertical(out Rect effectsHeaderArea, out Rect outputArea, out Rect bypassArea, 0.33f, 0.33f, 1f);
            EditorGUI.LabelField(effectsHeaderArea, "Effect", EditorStyles.boldLabel);
            EditorGUI.PropertyField(outputArea, property.FindPropertyRelative("output"));
            var bypassEffects = property.FindPropertyRelative("bypassEffects");
            var bypassListenerEffects = property.FindPropertyRelative("bypassListenerEffects");
            var bypassReverbZones = property.FindPropertyRelative("bypassReverbZones");
            bypassArea.SplitHorizontal(out Rect bypassHeaderArea, out Rect bypassToggles, 0.4f, 1f);
            EditorGUI.LabelField(bypassHeaderArea, "Bypass", EditorStyles.boldLabel);
            bypassToggles.SplitHorizontal(out Rect firstToggle, out Rect secondToggle, out Rect thirdToggle, 0.3f, 0.4f, 1f);
            Toggle(firstToggle, "Effects", bypassEffects);
            using(new EditorGUI.DisabledScope(property.FindPropertyRelative("output").objectReferenceValue != null))
                Toggle(secondToggle, "Listener Effect", bypassListenerEffects);
            Toggle(thirdToggle, "Reverb Zones", bypassReverbZones);

            // Main
            var mainAreaSplit = mainArea.SplitVertical(7, 0f);
            EditorGUI.LabelField(mainAreaSplit[0], "Main", EditorStyles.boldLabel);
            EditorGUI.PropertyField(mainAreaSplit[1], property.FindPropertyRelative("priority"));
            EditorGUI.PropertyField(mainAreaSplit[2], property.FindPropertyRelative("volume"));
            EditorGUI.PropertyField(mainAreaSplit[3], property.FindPropertyRelative("pitch"));
            EditorGUI.PropertyField(mainAreaSplit[4], property.FindPropertyRelative("stereoPan"));
            EditorGUI.PropertyField(mainAreaSplit[5], property.FindPropertyRelative("spatialBlend"));
            EditorGUI.PropertyField(mainAreaSplit[6], property.FindPropertyRelative("reverbZoneMix"));

            // 3D Settings
            if(property.FindPropertyRelative("volumeRolloff").intValue.ToEnum<AudioRolloffMode>() == AudioRolloffMode.Custom) {
                var _3dSettingsSplit = _3dSettingsArea.SplitVertical(6, 0f);
                EditorGUI.LabelField(_3dSettingsSplit[0], "3D Settings", EditorStyles.boldLabel);
                EditorGUI.PropertyField(_3dSettingsSplit[1], property.FindPropertyRelative("dopplerLevel"));
                EditorGUI.PropertyField(_3dSettingsSplit[2], property.FindPropertyRelative("spread"));
                EditorGUI.PropertyField(_3dSettingsSplit[3], property.FindPropertyRelative("volumeRolloff"));
                using(new EditorGUI.IndentLevelScope(1))
                    EditorGUI.PropertyField(_3dSettingsSplit[4], property.FindPropertyRelative("volumeRolloffCurve"));
                EditorGUI.PropertyField(_3dSettingsSplit[5], property.FindPropertyRelative("distance"));
            }
            else {
                var _3dSettingsSplit = _3dSettingsArea.SplitVertical(5, 0f);
                EditorGUI.LabelField(_3dSettingsSplit[0], "3D Settings", EditorStyles.boldLabel);
                EditorGUI.PropertyField(_3dSettingsSplit[1], property.FindPropertyRelative("dopplerLevel"));
                EditorGUI.PropertyField(_3dSettingsSplit[2], property.FindPropertyRelative("spread"));
                EditorGUI.PropertyField(_3dSettingsSplit[3], property.FindPropertyRelative("volumeRolloff"));
                EditorGUI.PropertyField(_3dSettingsSplit[4], property.FindPropertyRelative("distance"));
            }

            EditorGUI.indentLevel--;
        }

        private static void Toggle(Rect area, string content, SerializedProperty property) {
            EditorGUI.BeginChangeCheck();
            var val = EditorGUI.ToggleLeft(area, content, property.boolValue);
            if(EditorGUI.EndChangeCheck()) {
                property.boolValue = val;
            }
        }
    }
}
