using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class HierarchyBrokenComponentDrawer : HierarchyDrawer.ICustomDrawer {

        [InitializeOnLoadMethod]
        private static void Init() => HierarchyDrawer.RegisterCustomDrawer(new HierarchyBrokenComponentDrawer());

        public float Width => 60;

        public string Header => "Broken Components";

        public void DrawHeader(Rect area) {
            GUI.Label(area, "Broken", EditorStylesUtility.CenterAlignedMiniLabel);
        }

        public void Draw(Rect area, HierarchyDrawer.Context context) {
            var comps = context.components;
            bool isBroken = false;
            foreach(var comp in comps) {
                if(comp == null) {
                    isBroken = true;
                    break;
                }
            }
            if(!isBroken) {
                return;
            }

            EditorGUI.DrawRect(area, new Color32(205, 92, 92, 255));
            EditorGUI.LabelField(area, "Broken", EditorStylesUtility.CenterAlignedBoldLabel);
        }
    }
}
