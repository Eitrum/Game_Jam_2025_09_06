using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Unit
{
    [CustomEditor(typeof(ExperienceTableFunction))]
    public class ExperienceTableFunctionInspector : Editor
    {
        private SerializedProperty expression;
        private SerializedProperty levels;
        private int startIndex = 0;

        private void OnEnable() {
            expression = serializedObject.FindProperty("expression");
            levels = serializedObject.FindProperty("levels");
        }

        public override void OnInspectorGUI() {
            using(var s = new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.DelayedTextField(expression);
                EditorGUILayout.PropertyField(levels);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Table", EditorStylesUtility.BoldLabel);
                startIndex = EditorGUILayout.IntSlider("View Index", startIndex, 0, Mathf.Max(0, levels.intValue - 99));
                var t = (ExperienceTableFunction)target;
                var d = t.Difference;
                var tot = t.TotalExperience;
                var len = Mathf.Min(d?.Count ?? 0, tot?.Count ?? 0);
                var range = new MinMaxInt(startIndex, Mathf.Min(len, startIndex + 100));

                var line = GUILayoutUtility.GetRect(100, 16);
                DrawHeader(line);

                for(int i = range.min; i < range.max; i++) {
                    var r = GUILayoutUtility.GetRect(100, 14);
                    if(i % 2 == 0)
                        EditorGUI.DrawRect(r, new Color(0, 0, 0, 0.1f));
                    DrawElement(r, t, i);
                }
            }
        }

        private void DrawHeader(Rect area) {
            area.width = 100;
            EditorGUI.LabelField(area, "Level", EditorStylesUtility.RightAlignedLabel);
            area.x += 100;
            EditorGUI.LabelField(area, "Difference", EditorStylesUtility.RightAlignedLabel);
            area.x += 150;
            EditorGUI.LabelField(area, "Total", EditorStylesUtility.RightAlignedLabel);
        }

        private void DrawElement(Rect area, IExperienceTable table, int index) {
            area.width = 100;
            EditorGUI.LabelField(area, $"{index}", EditorStylesUtility.RightAlignedLabel);
            area.x += 100;
            EditorGUI.LabelField(area, $"{table.Difference[index]:N0}", EditorStylesUtility.RightAlignedLabel);
            area.x += 150;
            EditorGUI.LabelField(area, $"{table.TotalExperience[index]:N0}", EditorStylesUtility.RightAlignedLabel);
        }
    }
}
