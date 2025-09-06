using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [CustomEditor(typeof(TimeSun))]
    public class TimeSunInspector : Editor
    {
        private SerializedProperty applySunColorProp;
        private SerializedProperty colorProp;
        private SerializedProperty intensityProp;
        private SerializedProperty isMainSunProp;
        private SerializedProperty invertProp;

        private void OnEnable() {
            invertProp = serializedObject.FindProperty("invert");
            applySunColorProp = serializedObject.FindProperty("applySunColor");
            colorProp = serializedObject.FindProperty("color");
            isMainSunProp = serializedObject.FindProperty("isMainSun");
            intensityProp = serializedObject.FindProperty("intensityModifier");
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(applySunColorProp);
                EditorGUILayout.PropertyField(colorProp);
                intensityProp.floatValue = EditorGUILayout.Slider(intensityProp.displayName, intensityProp.floatValue, 0f, 10f);
                EditorGUILayout.PropertyField(isMainSunProp);
                EditorGUILayout.PropertyField(invertProp);
            }

            EditorGUILayout.Space();
            var ts = target as TimeSun;
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Info", EditorStylesUtility.BoldLabel);
                EditorGUILayout.LabelField("Is Sun Up", ts.IsSunUp ? "True" : "False", EditorStylesUtility.BoldLabel);
                EditorGUILayout.LabelField("Sun Rotation", $"{ts.SunRotation:0.0}", EditorStylesUtility.BoldLabel);
            }
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
