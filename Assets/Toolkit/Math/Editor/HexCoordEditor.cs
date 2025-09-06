using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Mathematics
{
    [CustomPropertyDrawer(typeof(HexCoord))]
    public class HexCoordEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var xProp = property.FindPropertyRelative("x");
            var yProp = property.FindPropertyRelative("y");
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.Vector2Field(position, label, new Vector2(xProp.floatValue, yProp.floatValue));

            if(EditorGUI.EndChangeCheck()) {
                xProp.floatValue = result.x;
                yProp.floatValue = result.y;
            }
        }
    }

    [CustomPropertyDrawer(typeof(HexCoordInt))]
    public class HexCoordIntEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var xProp = property.FindPropertyRelative("x");
            var yProp = property.FindPropertyRelative("y");
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.Vector2IntField(position, label, new Vector2Int(xProp.intValue, yProp.intValue));

            if(EditorGUI.EndChangeCheck()) {
                xProp.intValue = result.x;
                yProp.intValue = result.y;
            }
        }
    }
}
