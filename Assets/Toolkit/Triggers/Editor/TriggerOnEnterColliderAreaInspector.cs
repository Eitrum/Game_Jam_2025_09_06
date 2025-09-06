using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(TriggerOnEnterColliderArea))]
    public class TriggerOnEnterColliderAreaInspector : Editor
    {
        #region Variables

        private SerializedProperty repeatable;
        private SerializedProperty requiresEntity;
        private SerializedProperty entityFilter;

        #endregion

        #region Init

        private void OnEnable() {
            repeatable = serializedObject.FindProperty("repeatable");
            requiresEntity = serializedObject.FindProperty("requiresEntity");
            entityFilter = serializedObject.FindProperty("entityFilter");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(repeatable);
                    EditorGUILayout.PropertyField(requiresEntity);
                    if(requiresEntity.boolValue)
                        using(new EditorGUI.IndentLevelScope(1))
                            EditorGUILayout.PropertyField(entityFilter);
                }

                TriggerEditorUtility.DrawEditorDebug(this, target);
            }
        }

        #endregion
    }
}
