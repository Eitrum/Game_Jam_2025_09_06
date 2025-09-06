using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.DayCycle
{
    [CustomEditor(typeof(TimeFog))]
    public class TimeFogInspector : Editor
    {
        private SerializedProperty fogColorProp;
        private SerializedProperty modeProp;
        private SerializedProperty fogStartDistanceProp;
        private SerializedProperty fogEndDistanceProp;
        private SerializedProperty fogDensityProp;

        private void OnEnable() {
            fogColorProp = serializedObject.FindProperty("fogColor");
            modeProp = serializedObject.FindProperty("mode");
            fogStartDistanceProp = serializedObject.FindProperty("fogStartDistance");
            fogEndDistanceProp = serializedObject.FindProperty("fogEndDistance");
            fogDensityProp = serializedObject.FindProperty("fogDensity");
        }


        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                var tf = target as TimeFog;
                EditorGUILayout.PropertyField(fogColorProp);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(modeProp);
                if(EditorGUI.EndChangeCheck()) {
                    tf.UpdateFogMode(modeProp.intValue.ToEnum<FogMode>());
                }
                using(new EditorGUI.IndentLevelScope(1)) {
                    var mode = modeProp.intValue.ToEnum<FogMode>();
                    switch(mode) {
                        case FogMode.Exponential:
                        case FogMode.ExponentialSquared:
                            EditorGUILayout.PropertyField(fogDensityProp);
                            break;
                        case FogMode.Linear:
                            EditorGUILayout.PropertyField(fogStartDistanceProp);
                            EditorGUILayout.PropertyField(fogEndDistanceProp);
                            break;
                    }
                }
            }

            EditorGUILayout.Space();
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.DisabledScope(true)) {
                    var tf = target as TimeFog;
                    EditorGUILayout.LabelField("Info", EditorStylesUtility.BoldLabel);
                    EditorGUILayout.ColorField("Current Fog Color", tf.CurrentFogColor);
                    switch(tf.Mode) {
                        case FogMode.Exponential:
                        case FogMode.ExponentialSquared:
                            EditorGUILayout.LabelField("Density", $"{tf.CurrentFogDensity:0.00}");
                            break;
                        case FogMode.Linear:
                            var range = tf.CurrentFogRange;
                            EditorGUILayout.LabelField("Fog Range", $"{range.min:0.00} -> {range.max:0.00}");
                            break;
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
