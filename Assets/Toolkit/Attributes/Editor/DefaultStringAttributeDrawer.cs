using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(DefaultStringAttribute), true)]
    public class DefaultStringAttributeDrawer : PropertyDrawer {

        private static class Styles {
            public static GUIStyle InactiveDefaultString;

            static Styles() {
                InactiveDefaultString = new GUIStyle(EditorStyles.textField);
                InactiveDefaultString.fontStyle = FontStyle.Italic;
                InactiveDefaultString.normal.textColor = Color.gray;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
            var att = attribute as DefaultStringAttribute;
            var stringValue = property.stringValue;
            var isEditing = EditorGUIUtility.editingTextField;

            if(!isEditing && stringValue == att.DefaultString) {
                EditorGUI.TextField(position, label, stringValue, Styles.InactiveDefaultString);
            }
            else {
                EditorGUI.BeginChangeCheck();
                stringValue = EditorGUI.TextField(position, label, stringValue);
                if(EditorGUI.EndChangeCheck()) {
                    property.stringValue = stringValue;
                }

                if(att.AssignToVariable && !isEditing && string.IsNullOrEmpty(stringValue)) {
                    stringValue = att.DefaultString;
                    property.stringValue = att.DefaultString;
                }

                if(string.IsNullOrEmpty(stringValue)) {
                    if(label == GUIContent.none) {
                        Rect valueRect = new Rect(position.x + 4f, position.y, position.width - 4f, position.height);
                        GUI.Label(valueRect, att.DefaultString, EditorStylesUtility.GrayItalicLabel);
                    }
                    else {
                        Rect valueRect = new Rect(position.x + EditorGUIUtility.labelWidth + 4f, position.y, position.width - EditorGUIUtility.labelWidth - 4f, position.height);
                        GUI.Label(valueRect, att.DefaultString, EditorStylesUtility.GrayItalicLabel);
                    }
                }
                else if(att.AlwaysShow) {
                    GUI.Label(position, $"({att.DefaultString})", EditorStylesUtility.RightAlignedGrayMiniLabel);
                }
            }
        }
    }
}
