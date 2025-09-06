using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(Stat))]
    public class StatPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.isExpanded) {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else {
                var s = $"{label}";
                var value = property.FindPropertyRelative("value").floatValue;
                var increased = property.FindPropertyRelative("increased").floatValue;
                var more = property.FindPropertyRelative("more").floatValue;
                var total = value * (1f + increased) * (1f + more);
                EditorGUI.LabelField(position, " ", $"value: ({value:0.##})  increased: ({increased:P2})  more: ({more:P2})  total: ({total:0.##})");
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, s, true);
            }
        }

        public static Stat DrawLayout(Stat stat) {
            using(new EditorGUI.IndentLevelScope(1)) {
                var v = EditorGUILayout.FloatField("Value", stat.Value);
                var i = EditorGUILayout.FloatField("Increased", stat.Increased);
                var m = EditorGUILayout.FloatField("More", stat.More);
                return new Stat(v, i, m);
            }
        }
    }
}
