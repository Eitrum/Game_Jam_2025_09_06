using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(StackableItemObject))]
    public class StackableItemObjectEditor : Editor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty iconProperty;
        private SerializedProperty prefabProperty;
        private SerializedProperty itemTypeProperty;
        private SerializedProperty rarityProperty;
        private SerializedProperty maxStackProperty;
        private SerializedProperty weightPerStackProperty;

        private void OnEnable() {
            nameProperty = serializedObject.FindProperty("nameOverride");
            descriptionProperty = serializedObject.FindProperty("description");
            iconProperty = serializedObject.FindProperty("icon");
            prefabProperty = serializedObject.FindProperty("prefab");
            itemTypeProperty = serializedObject.FindProperty("itemType");
            rarityProperty = serializedObject.FindProperty("rarity");
            maxStackProperty = serializedObject.FindProperty("maxStack");
            weightPerStackProperty = serializedObject.FindProperty("weightPerStack");
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
                EditorGUILayout.LabelField("Stackable", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(maxStackProperty);
                using(new EditorGUI.DisabledScope(true)) {
                    var stackable = (target as StackableItemObject);
                    EditorGUILayout.LabelField("Current Stack", $"({stackable.CurrentStackSize}/{stackable.MaxStackSize})");
                }
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Weighted", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(weightPerStackProperty);
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
