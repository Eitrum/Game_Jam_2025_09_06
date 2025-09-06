using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Procedural.Items
{
    [CustomPropertyDrawer(typeof(PartConnection))]
    public class PartConnectionEditor : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.Box(position, "");
            var partProperty = property.FindPropertyRelative("otherPart");
            var offsetProperty = property.FindPropertyRelative("anchor");
            var offsetPositionProperty = offsetProperty.FindPropertyRelative("position");
            var offsetRotationProperty = offsetProperty.FindPropertyRelative("rotation");
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, partProperty.enumDisplayNames[partProperty.enumValueIndex], true);
            if(property.isExpanded) {
                using(new EditorGUI.IndentLevelScope(1)) {
                    position.y += position.height;
                    EditorGUI.PropertyField(position, partProperty);
                    position.y += position.height;
                    EditorGUI.PropertyField(position, offsetPositionProperty);
                    position.y += position.height;
                    offsetRotationProperty.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(position, "Rotation", offsetRotationProperty.quaternionValue.eulerAngles));
                }
            }
        }

    }
}
