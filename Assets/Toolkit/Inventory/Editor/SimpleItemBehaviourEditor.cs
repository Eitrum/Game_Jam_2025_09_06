using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(SimpleItemBehaviour))]
    public class SimpleItemBehaviourEditor : Editor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty iconProperty;
        private SerializedProperty itemTypeProperty;
        private SerializedProperty rarityProperty;

        private void OnEnable() {
            nameProperty = serializedObject.FindProperty("nameOverride");
            descriptionProperty = serializedObject.FindProperty("description");
            iconProperty = serializedObject.FindProperty("icon");
            itemTypeProperty = serializedObject.FindProperty("itemType");
            rarityProperty = serializedObject.FindProperty("rarity");
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
                EditorGUILayout.LabelField($"{(target as IItemWeight).Weight:0.##}kg");
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(iconProperty);
            }
            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
