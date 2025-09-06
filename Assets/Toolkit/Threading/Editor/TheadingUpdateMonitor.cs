using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Threading
{
    public class TheadingUpdateMonitor : EditorWindow
    {
        [MenuItem("Toolkit/Monitor/Threading Update")]
        public static void ShowMonitor() {
            var w = GetWindow<TheadingUpdateMonitor>("Threading Monitor");
            w.Show();
        }

        private void OnGUI() {
            var count = ThreadedUpdate.RegisteredCount;
            var area = new Rect(0, 0, position.width, position.height);
            GUILayout.BeginArea(area);
            EditorGUILayout.LabelField("Total Threaded Update Registered: " + count, EditorStyles.boldLabel);
            using(new EditorGUILayout.HorizontalScope()) {
                using(new EditorGUILayout.VerticalScope("box", GUILayout.MinWidth(150f))) {
                    EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
                    for(int i = 0; i < count; i++) {
                        EditorGUILayout.LabelField(ThreadedUpdate.GetThreadedUpdate(i).Name);
                    }
                }
                using(new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(60f))) {
                    EditorGUILayout.LabelField("Running", EditorStyles.boldLabel);
                    for(int i = 0; i < count; i++) {
                        EditorGUILayout.LabelField($"{(ThreadedUpdate.GetThreadedUpdate(i).IsRunning ? "<color=green>True</color>" : "<color=red>False</color>")}", EditorStylesUtility.RichTextBoldLabel);
                    }
                }

                using(new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(60f))) {
                    EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
                    for(int i = 0; i < count; i++) {
                        EditorGUILayout.LabelField($"{ThreadedUpdate.GetThreadedUpdate(i).Count}");
                    }
                }
                using(new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(60f))) {
                    EditorGUILayout.LabelField("FPS", EditorStyles.boldLabel);
                    for(int i = 0; i < count; i++) {
                        EditorGUILayout.LabelField($"{ThreadedUpdate.GetThreadedUpdate(i).Fps: 0.0#######}", EditorStylesUtility.ItalicLabel);
                    }
                }
            }

            GUILayout.EndArea();
            Repaint();
        }
    }
}
