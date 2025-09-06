using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(UseObjectNameAttribute), true)]
    public class UseObjectNameAttributeEditor : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            if(property.propertyType == SerializedPropertyType.String && string.IsNullOrEmpty(property.stringValue) && property.serializedObject != null && property.serializedObject.targetObject) {
                if(label == GUIContent.none) {
                    Rect valueRect = new Rect(position.x + 4f, position.y, position.width - 4f, position.height);
                    GUI.Label(valueRect, property.serializedObject.targetObject.name, EditorStylesUtility.GrayItalicLabel);
                }
                else {
                    Rect valueRect = new Rect(position.x + EditorGUIUtility.labelWidth + 4f, position.y, position.width - EditorGUIUtility.labelWidth - 4f, position.height);
                    GUI.Label(valueRect, property.serializedObject.targetObject.name, EditorStylesUtility.GrayItalicLabel);
                }
            }
        }
    }
}
