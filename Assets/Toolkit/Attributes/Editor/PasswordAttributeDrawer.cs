using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(PasswordAttribute), true)]
    class PasswordAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var pass = attribute as PasswordAttribute;
            if(EditorStylesUtility.IsHoldingAlt)
                EditorGUI.PropertyField(position, property, label, false);
            else
                property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
    }
}
