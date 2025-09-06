using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Mathematics
{
    [CustomPropertyDrawer(typeof(Bezier))]
    public class BezierInspector : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 6f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if(property.isExpanded) {
                EditorGUI.indentLevel++;
                position.y += EditorGUIUtility.singleLineHeight;
                position.height = EditorGUIUtility.singleLineHeight * 5f;

                position.PadRef(0, 0, 4f, 4f);
                position.SplitVertical(out Rect startArea, out Rect endArea, 0.5f, 10f);

                startArea.SplitVertical(out Rect sArea, out Rect shArea, 0.5f);
                GUI.Box(startArea.Shrink(-2f), "");
                EditorGUI.PropertyField(sArea, property.FindPropertyRelative("startPoint"));
                EditorGUI.PropertyField(shArea, property.FindPropertyRelative("startHandle"));

                endArea.SplitVertical(out Rect eArea, out Rect ehArea, 0.5f);
                GUI.Box(endArea.Shrink(-2f), "");
                EditorGUI.PropertyField(eArea, property.FindPropertyRelative("endPoint"));
                EditorGUI.PropertyField(ehArea, property.FindPropertyRelative("endHandle"));
                EditorGUI.indentLevel--;
            }
        }

        public enum Mode
        {
            Position,
            Relative
        }
    }
}
