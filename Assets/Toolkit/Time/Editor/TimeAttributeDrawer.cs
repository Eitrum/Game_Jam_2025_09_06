using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.DayCycle
{
    [CustomPropertyDrawer(typeof(TimeAttribute))]
    public class TimeAttributeDrawer : PropertyDrawer
    {
        private static GUIContent hourContent = new GUIContent("h", "Hour");
        private static GUIContent minuteContent = new GUIContent("m", "Minute");
        private static GUIContent secondContent = new GUIContent("s", "Second");

        private static GUIContent[] subLabels = new GUIContent[] { hourContent, minuteContent, secondContent };

        private static int[] intValues = new int[3];

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            switch(property.propertyType) {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Float:
                    EditorGUI.BeginChangeCheck();
                    var val = (int)property.floatValue;
                    intValues[0] = val / 3600;
                    intValues[1] = (val / 60) % 60;
                    intValues[2] = val % 60;
                    position.SplitHorizontal(out Rect labelArea, out Rect valueArea, 0.35f, 2f);
                    EditorGUI.LabelField(labelArea, label);
                    EditorGUI.MultiIntField(valueArea, subLabels, intValues);
                    if(EditorGUI.EndChangeCheck()) {
                        intValues[0] = Mathf.Clamp(intValues[0], 0, 23);
                        intValues[1] = Mathf.Clamp(intValues[1], 0, 59);
                        intValues[2] = Mathf.Clamp(intValues[2], 0, 59);
                        property.floatValue = intValues[0] * 3600f + intValues[1] * 60f + intValues[2];
                    }
                    break;
                default:
                    EditorGUI.LabelField(position, label, "Time Attribute only works on float and integer");
                    break;
            }
        }
    }
}
