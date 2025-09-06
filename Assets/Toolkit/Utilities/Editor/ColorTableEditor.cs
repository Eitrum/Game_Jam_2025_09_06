using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(ColorTableType))]
    public class ColorTableEditor : PropertyDrawer
    {
        private static GUIContent[] labels;
        private static Color32[] colors;

        private static void Initialize() {
            if(labels != null)
                return;
            var type = typeof(ColorTableType);
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            labels = new GUIContent[fields.Length];
            colors = new Color32[fields.Length];

            for(int i = 0, length = fields.Length; i < length; i++) {
                var f = fields[i];
                var atts = f.GetCustomAttributes(typeof(InspectorNameAttribute), true);
                var col = ColorTable.GetColor((ColorTableType)i);
                labels[i] = new GUIContent(atts.Length > 0 ? (atts[0] as InspectorNameAttribute).displayName : f.Name.SplitPascalCase(), $"Hex ({col.ToHex24()}) - RGB ({col.r}, {col.g}, {col.b})");
                colors[i] = col;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Initialize();
            position.SplitHorizontal(out Rect dropdownArea, out Rect colorArea, 1f - 20f / position.width, 2f);
            EditorGUI.BeginChangeCheck();
            var index = EditorGUI.Popup(dropdownArea, label, property.intValue, labels);
            if(EditorGUI.EndChangeCheck())
                property.intValue = index;
            EditorGUI.DrawRect(colorArea, colors[index]);
        }
    }
}
