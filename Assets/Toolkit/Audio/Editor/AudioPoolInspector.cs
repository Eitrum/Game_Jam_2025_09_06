using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Audio
{
    [CustomEditor(typeof(AudioPool))]
    public class AudioPoolInspector : Editor
    {
        public override void OnInspectorGUI() {
            var ap = (AudioPool)target;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStylesUtility.BoldLabel);
                EditorGUILayout.LabelField("Mode", ap.Mode.ToString());
                EditorGUILayout.LabelField("Size", ap.Size.ToString());
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Information", EditorStylesUtility.BoldLabel);
                DrawLine();
                using(new EditorGUI.IndentLevelScope(1)) {
                    EditorGUILayout.LabelField("Free", EditorStylesUtility.BoldLabel);
                    var free = ap.FreeObjects;
                    while(free.MoveNext(out AudioPoolObject apo)) {
                        if(apo == null) {
                            EditorGUILayout.LabelField("Null");
                        }
                        else {
                            EditorGUILayout.LabelField($"{apo.name} - IsPlaying ({apo.IsPlaying}) - IsFollowing ({(apo.Target != null ? apo.Target.name : "no")})");
                        }
                    }
                }
                DrawLine();
                using(new EditorGUI.IndentLevelScope(1)) {
                    EditorGUILayout.LabelField("Active", EditorStylesUtility.BoldLabel);
                    var active = ap.ActiveObjects;
                    while(active.MoveNext(out AudioPoolObject apo)) {
                        if(apo == null) {
                            EditorGUILayout.LabelField("Null");
                        }
                        else {
                            EditorGUILayout.LabelField($"{apo.name} - IsPlaying ({apo.IsPlaying}) - IsFollowing ({(apo.Target != null ? apo.Target.name : "no")})");
                        }
                    }
                }
            }
        }

        private static void DrawLine() {
            EditorGUILayout.Space();
            var lineArea = GUILayoutUtility.GetLastRect();
            lineArea.height = 2f;
            EditorGUI.DrawRect(lineArea, Color.gray);
        }
    }
}
