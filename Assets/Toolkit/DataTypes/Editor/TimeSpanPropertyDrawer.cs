using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(Toolkit.TKTimeSpan))]
    public class TimeSpanPropertyDrawer : PropertyDrawer
    {
        private static GUIContent DaysContent = new GUIContent("Days");
        private static GUIContent HoursContent = new GUIContent("Hours");
        private static GUIContent MinutesContent = new GUIContent("Minutes");
        private static GUIContent SecondsContent = new GUIContent("Seconds");
        private static GUIContent MillisecondsContent = new GUIContent("Milliseconds");
        private static GUIContent TicksContent = new GUIContent("Ticks");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.isExpanded)
                return EditorGUIUtility.singleLineHeight * 7;
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var longProp = property.FindPropertyRelative("ticks");
            var ts = new TKTimeSpan(longProp.longValue);
            position.height = EditorGUIUtility.singleLineHeight;
            var temp = new GUIContent(label);

            ts.Ticks = EditorGUI.LongField(position, " ", ts.Ticks);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, temp, false);

            if(property.isExpanded) {
                using(new EditorGUI.IndentLevelScope()) {
                    ts.Extract(out int days, out int hours, out int min, out int sec, out int milliseconds, out long remainingTicks);
                    position.y += EditorGUIUtility.singleLineHeight;
                    days = EditorGUI.IntField(position, DaysContent, days);
                    position.y += EditorGUIUtility.singleLineHeight;
                    hours = EditorGUI.IntField(position, HoursContent, hours);
                    position.y += EditorGUIUtility.singleLineHeight;
                    min = EditorGUI.IntField(position, MinutesContent, min);
                    position.y += EditorGUIUtility.singleLineHeight;
                    sec = EditorGUI.IntField(position, SecondsContent, sec);
                    position.y += EditorGUIUtility.singleLineHeight;
                    milliseconds = EditorGUI.IntField(position, MillisecondsContent, milliseconds);
                    position.y += EditorGUIUtility.singleLineHeight;
                    remainingTicks = EditorGUI.LongField(position, TicksContent, ts.Ticks % TKTimeSpan.TicksPerMillisecond);
                    ts.Set(days, hours, min, sec, milliseconds, remainingTicks);
                }
            }
            longProp.longValue = ts.Ticks;
        }
    }
}
