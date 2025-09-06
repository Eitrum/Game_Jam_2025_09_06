using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(BoolStack))]
    public class BoolStackPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.SplitHorizontal(out Rect labelArea, out Rect buttonArea, 1f - (120f / position.width));
            var t = property.FindPropertyRelative("value");
            EditorGUI.LabelField(labelArea, $"{label.text} -   ( {t.intValue > 0} )   [ {t.intValue} ]");

            buttonArea.SplitHorizontal(out Rect left, out Rect right, 0.5f);
            if(GUI.Button(left, "<-"))
                t.intValue--;
            if(GUI.Button(right, "->"))
                t.intValue++;
        }
    }
}
