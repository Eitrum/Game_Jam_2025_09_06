using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.DayCycle
{
    [CustomEditor(typeof(TimeLightColor))]
    public class TimeLightColorInspector : Editor
    {
        private SerializedProperty colorProp;
        private SerializedProperty intensityProp;

        private void OnEnable() {
            colorProp = serializedObject.FindProperty("color");
            intensityProp = serializedObject.FindProperty("intensity");
        }


        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(colorProp);
                EditorGUILayout.PropertyField(intensityProp);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
