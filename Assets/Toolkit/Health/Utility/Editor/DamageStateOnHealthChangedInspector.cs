using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Health.Utility
{
    [CustomEditor(typeof(DamageStateOnHealthChanged))]
    public class DamageStateOnHealthChangedInspector : Editor
    {
        #region Variables

        private SerializedProperty usePercentageHealth;
        private SerializedProperty data;
        private static List<Rect> containers = new List<Rect>();

        #endregion

        #region Init

        private void OnEnable() {
            usePercentageHealth = serializedObject.FindProperty("usePercentageHealth");
            data = serializedObject.FindProperty("data");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(var s = new ToolkitEditorUtility.InspectorScope(this, true)) {
                EditorGUILayout.PropertyField(usePercentageHealth);
                var usesPercen = usePercentageHealth.boolValue;

                var header = GUILayoutUtility.GetRect(20, 20);
                CacheList(header, containers, data);
                for(int i = 0; i < containers.Count; i++) {
                    EditorGUI.DrawRect(containers[i], ToolkitEditorUtility.GetColor(i));
                    GUI.Label(containers[i], usesPercen ? $" {(GetThreshold(data, i)):P0}" : $" {(GetThreshold(data, i)):0.0}", EditorStylesUtility.ItalicLabel);
                }
                EditorGUILayout.PropertyField(data);
            }
        }

        #endregion

        #region Utility

        private static float GetThreshold(SerializedProperty data, int index) {
            return data.GetArrayElementAtIndex(index).FindPropertyRelative("threshold").floatValue;
        }

        private static void CacheList(Rect c, List<Rect> rects, SerializedProperty data) {
            rects.Clear();
            var len = data.arraySize;
            rects.Fill(len);

            var prev = 1f;
            for(int i = len - 1; i >= 0; i--) {
                var t = 1f - data.GetArrayElementAtIndex(i).FindPropertyRelative("threshold").floatValue;
                var ep = prev;
                var r = new Rect(c.x + t * c.width, c.y, c.width * (ep - t), c.height);
                rects[i] = r;
                prev = t;
            }
        }

        #endregion
    }
}
