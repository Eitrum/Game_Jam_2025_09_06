using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.DayCycle
{
    [CustomEditor(typeof(TimeSet))]
    public class TimeSetInspector : Editor
    {
        private SerializedProperty timeProp;

        private void OnEnable() {
            timeProp = serializedObject.FindProperty("time");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(timeProp);
            if(GUILayout.Button("Apply", GUILayout.Width(80f))) {
                TimeSystem.Set(timeProp.floatValue);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
