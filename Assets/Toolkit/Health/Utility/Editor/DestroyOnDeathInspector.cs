using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Health.Utility
{
    [CustomEditor(typeof(DestroyOnDeath))]
    public class DestroyOnDeathInspector : Editor
    {
        private SerializedProperty delay;

        private void OnEnable() {
            delay = serializedObject.FindProperty("delay");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            HealthEditorUtility.DrawHealthComponentReference(target as DestroyOnDeath);

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(delay);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
