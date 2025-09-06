using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public class ActiveWindowMonitor : EditorWindow
    {

        [MenuItem("Toolkit/Editor/Active Window Monitor")]
        public static void Open() {
            var w = GetWindow<ActiveWindowMonitor>("Window Monitor", true);
        }

        private Vector2 scroll;

        private void OnEnable() {
            EditorApplication.update -= Repaint;
            EditorApplication.update -= Repaint;
            EditorApplication.update += Repaint;
        }

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));

            var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            using(new EditorGUILayout.ScrollViewScope(scroll, "box")) {
                GUILayout.Label("Windows found: " + windows.Length);
                foreach(var w in windows) {
                    if(w == null)
                        continue;

                    EditorGUILayout.LabelField(w.titleContent);
                }
            }

            GUILayout.EndArea();
        }

    }
}
