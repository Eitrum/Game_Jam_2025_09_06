using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(InstantiateSettings), true)]
    public class InstantiateSettingsEditor : PropertyDrawer {

        private static bool isExpanded = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * (isExpanded ? 5 : 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var localPosition = property.FindPropertyRelative("localPosition");
            var localRotation = property.FindPropertyRelative("localRotation");
            var localScale = property.FindPropertyRelative("localScale");
            var parent = property.FindPropertyRelative("parent");

            position.height = EditorGUIUtility.singleLineHeight;
            isExpanded = EditorGUI.Foldout(position, isExpanded, label.text, true);
            if(isExpanded) {
                EditorGUI.indentLevel++;
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, localPosition, false);
                position.y += EditorGUIUtility.singleLineHeight;
                localRotation.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(position, localRotation.displayName, localRotation.quaternionValue.eulerAngles));
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, localScale, false);
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, parent, false);
                EditorGUI.indentLevel--;
            }
        }
    }
}
