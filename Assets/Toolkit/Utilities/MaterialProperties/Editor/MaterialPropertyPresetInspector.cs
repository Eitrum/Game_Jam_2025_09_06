using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(MaterialPropertyPreset))]
    public class MaterialPropertyPresetInspector : Editor
    {
        private SerializedProperty properties;
        private ReorderableList list;

        private void OnEnable() {
            properties = serializedObject.FindProperty("properties");
            list = new ReorderableList(serializedObject, properties);
            list.elementHeightCallback += (index) => EditorGUI.GetPropertyHeight(properties.GetArrayElementAtIndex(index)) + 4;
            list.drawElementCallback += DrawProperty;
            list.headerHeight = 0;
        }

        private void DrawProperty(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect.Pad(12, 0, 2, 2), properties.GetArrayElementAtIndex(index));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.Space();
            list.DoLayoutList();
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
