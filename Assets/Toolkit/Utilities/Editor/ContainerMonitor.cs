using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    public class ContainerMonitor : EditorWindow
    {
        private const string MENU_PATH = "Toolkit/Monitor/Container";
        private const int MENU_PRIORITY = 100001;

        [MenuItem(MENU_PATH, priority = MENU_PRIORITY)]
        public static void OpenMonitor() {
            var w = GetWindow<ContainerMonitor>("Container Monitor", true);
            w.Show();
        }

        private void OnGUI() {
            var containers = Container.GetAllContainers();
            var area = new Rect(Vector2.zero, position.size).Shrink(5f);
            area.SplitVertical(out Rect header, out Rect body, 28f / area.height, 5f);
            DrawHeader(header, containers);
            DrawBody(body, containers);
        }



        void DrawHeader(Rect area, IEnumerable<Container> containers) {
            GUI.Box(area, "");
            GUILayout.BeginArea(area.Shrink(5f));
            EditorGUILayout.LabelField("Containers: " + containers.Count());
            GUILayout.EndArea();
        }

        void DrawBody(Rect area, IEnumerable<Container> containers) {
            GUI.Box(area, "");
            GUILayout.BeginArea(area.Shrink(5f));
            var ev = Event.current;
            foreach(var con in containers) {
                if(con == null) {
                    EditorGUILayout.LabelField($"destroyed container");
                }
                else {
                    EditorGUILayout.LabelField($"{con.ContainerName}");
                    var text = GUILayoutUtility.GetLastRect();
                    if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && text.Contains(ev.mousePosition)) {
                        Selection.activeObject = con;
                    }
                }
            }
            GUILayout.EndArea();
        }
    }
}
