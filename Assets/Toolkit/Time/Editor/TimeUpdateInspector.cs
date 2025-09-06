using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.DayCycle
{
    [CustomEditor(typeof(TimeUpdate))]
    public class TimeUpdateInspector : Editor
    {
        private SerializedProperty timePerSecondProp;

        private void OnEnable() {
            timePerSecondProp = serializedObject.FindProperty("timePerSecondRealtime");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(timePerSecondProp);

            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button("Dawn", GUILayout.Width(80f))) {
                    TimeSystem.Set(TimeSystem.START_OF_DAWN);
                }
                if(GUILayout.Button("Day", GUILayout.Width(80f))) {
                    TimeSystem.Set(TimeSystem.START_OF_DAY);
                }
                if(GUILayout.Button("Mid Day", GUILayout.Width(80f))) {
                    TimeSystem.Set(TimeSystem.MID_DAY);
                }
                if(GUILayout.Button("Dusk", GUILayout.Width(80f))) {
                    TimeSystem.Set(TimeSystem.START_OF_DUSK);
                }
                if(GUILayout.Button("Night", GUILayout.Width(80f))) {
                    TimeSystem.Set(TimeSystem.START_OF_NIGHT);
                }
                if(GUILayout.Button("Midnight", GUILayout.Width(80f))) {
                    TimeSystem.Set(TimeSystem.MIDNIGHT);
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Time", TimeSystem.FormattedTime, EditorStylesUtility.BoldLabel);
                EditorGUILayout.LabelField("Time of Day", TimeSystem.TimeOfDay.GetName(), EditorStylesUtility.BoldLabel);
                EditorGUILayout.LabelField("Is Sun Up?", TimeSystem.IsSunUp ? "True" : "False", EditorStylesUtility.BoldLabel);
                EditorGUILayout.LabelField("Intensity", $"{TimeSystem.Intesity:0.00}", EditorStylesUtility.BoldLabel);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            Repaint();
        }
    }
}
