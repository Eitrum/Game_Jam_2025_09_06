using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.DayCycle
{
    [CustomEditor(typeof(TimeEnvironmentLight))]
    public class TimeEnvironmentLightInspector : Editor
    {
        private SerializedProperty ambientModeProp;
        private SerializedProperty skyColorProp;
        private SerializedProperty equatorColorProp;
        private SerializedProperty groundColorProp;
        private SerializedProperty skyboxStrengthProp;

        private static GUIContent alternativeName = new GUIContent("Ambient Color", "A global ambient color");

        private void OnEnable() {
            ambientModeProp = serializedObject.FindProperty("ambientMode");
            skyColorProp = serializedObject.FindProperty("skyColor");
            equatorColorProp = serializedObject.FindProperty("equatorColor");
            groundColorProp = serializedObject.FindProperty("groundColor");
            skyboxStrengthProp = serializedObject.FindProperty("skyboxIntensityModifier");
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                var tel = target as TimeEnvironmentLight;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(ambientModeProp);
                if(EditorGUI.EndChangeCheck()) {
                    tel.UpdateAmbientMode(ambientModeProp.intValue.ToEnum<EnvironmentLightSource>());
                }
                var mode = tel.AmbientMode;

                switch(mode) {
                    case EnvironmentLightSource.Color:
                        EditorGUILayout.PropertyField(skyColorProp, alternativeName);
                        break;
                    case EnvironmentLightSource.Gradient:
                        EditorGUILayout.PropertyField(skyColorProp);
                        EditorGUILayout.PropertyField(equatorColorProp);
                        EditorGUILayout.PropertyField(groundColorProp);
                        break;
                    case EnvironmentLightSource.Skybox:
                        EditorGUILayout.PropertyField(skyboxStrengthProp);
                        break;
                }
            }
            EditorGUILayout.Space();
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.DisabledScope(true)) {
                    EditorGUILayout.LabelField("Info", EditorStylesUtility.BoldLabel);
                    var tel = target as TimeEnvironmentLight;
                    var mode = tel.AmbientMode;
                    switch(mode) {
                        case EnvironmentLightSource.Color:
                            EditorGUILayout.ColorField("Ambient Color", tel.CurrentSkyColor);
                            break;
                        case EnvironmentLightSource.Gradient:
                            EditorGUILayout.ColorField("Sky Color", tel.CurrentSkyColor);
                            EditorGUILayout.ColorField("Equator Color", tel.CurrentEquatorColor);
                            EditorGUILayout.ColorField("Ground Color", tel.CurrentGroundColor);
                            break;
                        case EnvironmentLightSource.Skybox:
                            EditorGUILayout.LabelField("Skybox Intensity Modifier", $"{tel.CurrentSkyboxIntensityModifier:0.0}", EditorStylesUtility.BoldLabel);
                            break;
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
