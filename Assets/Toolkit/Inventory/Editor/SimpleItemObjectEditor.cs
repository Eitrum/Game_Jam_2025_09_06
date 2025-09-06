using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(SimpleItemObject))]
    public class SimpleItemObjectEditor : Editor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty iconProperty;
        private SerializedProperty prefabProperty;
        private SerializedProperty itemTypeProperty;
        private SerializedProperty rarityProperty;
        private SerializedProperty weightProperty;

        private void OnEnable() {
            nameProperty = serializedObject.FindProperty("nameOverride");
            descriptionProperty = serializedObject.FindProperty("description");
            iconProperty = serializedObject.FindProperty("icon");
            prefabProperty = serializedObject.FindProperty("prefab");
            itemTypeProperty = serializedObject.FindProperty("itemType");
            rarityProperty = serializedObject.FindProperty("rarity");
            weightProperty = serializedObject.FindProperty("weight");
        }

        public override void OnInspectorGUI() {

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(nameProperty);
                EditorGUILayout.PropertyField(descriptionProperty);
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(itemTypeProperty);
                EditorGUILayout.PropertyField(rarityProperty);
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Weighted", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(weightProperty);
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(iconProperty);
                EditorGUILayout.PropertyField(prefabProperty);
            }
            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
