using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class HierarchyToggleActiveDrawer : HierarchyDrawer.ICustomDrawer {

        private static class Style {
            public static readonly Texture HeaderIcon = EditorGUIUtility.TrIconContent("d_SceneViewVisibility On").image;
        }

        [InitializeOnLoadMethod]
        private static void Init() => HierarchyDrawer.RegisterCustomDrawer(new HierarchyToggleActiveDrawer());

        public float Width => 18;

        public string Header => "Toggle Active";

        public void DrawHeader(Rect area) {
            area.x += 2;
            area.width -= 4;
            GUI.DrawTexture(area, Style.HeaderIcon);
            //GUI.Label(area, "Active", EditorStylesUtility.CenterAlignedMiniLabel);
        }

        public void Draw(Rect area, HierarchyDrawer.Context context) {
            area.x += 2;
            area.width -= 4;
            EditorGUI.BeginChangeCheck();
            var setActive = GUI.Toggle(area, context.gameObject.activeSelf, GUIContent.none);
            if(EditorGUI.EndChangeCheck()) {
                context.gameObject.SetActive(setActive);
            }
        }
    }
}
