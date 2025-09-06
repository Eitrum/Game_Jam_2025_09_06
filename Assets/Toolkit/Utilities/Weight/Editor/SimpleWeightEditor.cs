using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    [CustomEditor(typeof(SimpleWeight))]
    internal class SimpleWeightEditor : Editor {
        public override void OnInspectorGUI() {
            var weight = serializedObject.FindProperty("weight");
            var scale = serializedObject.FindProperty("scaleWeight");
            EditorGUILayout.PropertyField(weight);
            EditorGUILayout.PropertyField(scale);
            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
