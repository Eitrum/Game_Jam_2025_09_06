using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(SimplePortrait))]
    public class SimplePortraitInspector : Editor
    {
        private Vector2 scroll;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            var sp = target as SimplePortrait;
            if(Application.isPlaying && sp.Portrait != null) {
                using(var scrollView = new EditorGUILayout.ScrollViewScope(scroll)) {
                    scroll = scrollView.scrollPosition;
                    var area = GUILayoutUtility.GetRect(sp.Width, sp.Height, GUILayout.Width(sp.Width), GUILayout.Height(sp.Height));
                    area.width = sp.Width;
                    area.height = sp.Height;
                    GUI.DrawTexture(area, sp.Portrait);
                }
            }
        }
    }
}
