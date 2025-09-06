using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit.CodeAnalysis
{
    public class CodeStatisticsWindow : EditorWindow
    {

        private const string MENU_PATH = "Toolkit/Code Analysis/Code Statistics";
        private const int MENU_PRIORITY = 2000;

        [MenuItem(MENU_PATH, priority = MENU_PRIORITY)]
        public static void ShowCodeStatistics() {
            var w = GetWindow<CodeStatisticsWindow>("Code Statistics", true);
            w.Show();
        }

        private void OnGUI() {
            var area = new Rect(Vector2.zero, position.size);
            area.ShrinkRef(5f);
            if(!CodeStatistics.HasLoaded) {
                return;
            }

            CodeStatistics.DrawStatistics(area);
            Repaint();
        }
    }
}
