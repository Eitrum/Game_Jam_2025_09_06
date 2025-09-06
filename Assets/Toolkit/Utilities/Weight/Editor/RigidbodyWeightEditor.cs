using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit{
    [CustomEditor(typeof(RigidbodyWeight))]
    public class RigidbodyWeightEditor : Editor{
        public override void OnInspectorGUI() {
            var weight = serializedObject.FindProperty("weight");
            var scale= serializedObject.FindProperty("scaleWeight");
            EditorGUILayout.PropertyField(weight);
            EditorGUILayout.PropertyField(scale);
            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
