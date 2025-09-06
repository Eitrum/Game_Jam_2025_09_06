using UnityEditor;
using UnityEngine;

namespace Toolkit.Rendering {
    [CustomPropertyDrawer(typeof(GridIterator))]
    public class GridIteratorPropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var expanded = property.isExpanded;
            var height = EditorGUIUtility.singleLineHeight;
            return expanded ? height * 3f : height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Position
            var x = property.FindPropertyRelative("x");
            var y = property.FindPropertyRelative("x");
            var z = property.FindPropertyRelative("x");

            // Size
            var w = property.FindPropertyRelative("width");
            var h = property.FindPropertyRelative("height");
            var d = property.FindPropertyRelative("depth");


            // Draw
            position.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(position, $"({x.intValue}, {y.intValue}, {z.intValue} | {w.intValue}, {h.intValue}, {d.intValue})        ", EditorStylesUtility.RightAlignedGrayMiniLabel);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

            if(property.isExpanded) {
                using var indent = new EditorGUI.IndentLevelScope(1);

                EditorGUI.BeginChangeCheck();
                var newPos = EditorGUI.Vector3IntField(position.NextLine(), "Offset", new Vector3Int(x.intValue, y.intValue, z.intValue));
                if(EditorGUI.EndChangeCheck()) {
                    x.intValue = newPos.x;
                    y.intValue = newPos.y;
                    z.intValue = newPos.z;
                }

                EditorGUI.BeginChangeCheck();
                var newSize = EditorGUI.Vector3IntField(position.NextLine(), "Size", new Vector3Int(w.intValue, h.intValue, d.intValue));
                if(EditorGUI.EndChangeCheck()) {
                    w.intValue = newSize.x;
                    h.intValue = newSize.y;
                    d.intValue = newSize.z;
                }
            }
        }
    }
}
