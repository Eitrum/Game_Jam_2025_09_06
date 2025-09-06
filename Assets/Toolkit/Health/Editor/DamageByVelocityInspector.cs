using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Health
{
    [CustomEditor(typeof(DamageByVelocity))]
    public class DamageByVelocityInspector : Editor
    {
        #region Variables

        private SerializedProperty mode = null;
        private SerializedProperty useImpulseVelocity = null;
        private SerializedProperty damageByVelocity = null;
        private SerializedProperty damageType = null;
        private SerializedProperty destroyOnHit = null;

        #endregion

        #region Init

        private void OnEnable() {
            mode = serializedObject.FindProperty("mode");
            useImpulseVelocity = serializedObject.FindProperty("useImpulseVelocity");
            damageByVelocity = serializedObject.FindProperty("damageByVelocity");
            damageType = serializedObject.FindProperty("damageType");
            destroyOnHit = serializedObject.FindProperty("destroyOnHit");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            serializedObject.Update();

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(mode);
                if(mode.intValue == 3) {
                    using(new EditorGUI.IndentLevelScope(1))
                        EditorGUILayout.PropertyField(useImpulseVelocity);
                }
                EditorGUILayout.PropertyField(destroyOnHit);
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Damage", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(damageByVelocity);
                EditorGUILayout.PropertyField(damageType);
            }


            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
