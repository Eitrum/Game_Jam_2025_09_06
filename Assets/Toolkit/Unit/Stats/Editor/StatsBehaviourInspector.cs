using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Unit {
    [CustomEditor(typeof(StatsBehaviour))]
    public class StatsBehaviourInspector : Editor {

        private StatType type;
        private Stat stat;

        public override void OnInspectorGUI() {
            if(!Application.isPlaying) {
                EditorGUILayout.HelpBox("Only editable in runtime", MessageType.Info);
                return;
            }
            var statsBehaviour = (StatsBehaviour)target;
            var stats = statsBehaviour.Stats;

            if(stats.Count == 0)
                EditorGUILayout.HelpBox("No stats assigned", MessageType.Info);

            foreach(var s in stats)
                EditorGUILayout.LabelField($"{StatsUtility.GetName(s.Key)}", $"{s.Value}");

            EditorGUILayout.Space();

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Debug", EditorStylesUtility.BoldLabel);
                EditorGUI.BeginChangeCheck();
                type = (StatType)EditorGUILayout.EnumPopup("StatType", type);
                if(EditorGUI.EndChangeCheck() && stats.TryGetValue(type, out var v)) {
                    stat = v;
                }
                stat = StatPropertyDrawer.DrawLayout(stat);

                if(stats.ContainsKey(type)) {
                    using(new EditorGUILayout.HorizontalScope()) {
                        if(GUILayout.Button("Modify", GUILayout.Width(60f))) {
                            statsBehaviour.SetStat(type, stat);
                        }
                        if(GUILayout.Button("Remove", GUILayout.Width(60f))) {
                            statsBehaviour.Remove(type);
                        }
                    }
                }
                else if(GUILayout.Button("Add", GUILayout.Width(60f))) {
                    statsBehaviour.SetStat(type, stat);
                }
            }
        }
    }
}
