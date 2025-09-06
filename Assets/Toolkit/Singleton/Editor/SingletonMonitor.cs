using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public class SingletonMonitor : EditorWindow {

        private const string MENU_PATH = "Toolkit/Monitor/Singleton";
        private const int MENU_PRIORITY = 100000;

        private GUIStyle richText;

        [MenuItem(MENU_PATH, priority = MENU_PRIORITY)]
        public static void OpenMonitor() {
            var w = GetWindow<SingletonMonitor>("Singleton Monitor", true);
            w.Show();
        }

        private void ValidateGUIStyle() {
            if(richText == null) {
                richText = new GUIStyle(GUI.skin.label);
                richText.richText = true;
            }
        }

        private void OnGUI() {
            ValidateGUIStyle();
            var tracked = SingletonUtility.trackedSingletons;
            var area = new Rect(Vector2.zero, position.size).Shrink(5f);
            area.SplitVertical(out Rect header, out Rect body, 28f / area.height, 5f);
            DrawHeader(header, tracked);
            DrawBody(body, tracked);
        }



        void DrawHeader(Rect area, IReadOnlyList<SingletonUtility.SingletonRef> singletons) {
            GUI.Box(area, "");
            GUILayout.BeginArea(area.Shrink(5f));
            EditorGUILayout.LabelField("Singletons: " + singletons.Count);
            GUILayout.EndArea();
        }

        void DrawBody(Rect area, IReadOnlyList<SingletonUtility.SingletonRef> singletons) {
            GUI.Box(area, "");
            GUILayout.BeginArea(area.Shrink(5f));
            for(int i = 0, length = singletons.Count; i < length; i++) {
                var s = singletons[i];
                bool isActive = ((s.reference is UnityEngine.Object uObj) ? uObj != null : s.reference != null);
                EditorGUILayout.LabelField($"{s.name} is {(isActive ? "<color=green>active</color>" : "<color=red>inactive</color>")}", richText);
            }
            GUILayout.EndArea();
        }
    }
}
