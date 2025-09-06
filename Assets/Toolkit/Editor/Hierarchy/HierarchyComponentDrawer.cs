using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class HierarchyComponentDrawer : HierarchyDrawer.ICustomDrawer {

        [InitializeOnLoadMethod]
        private static void Init() => HierarchyDrawer.RegisterCustomDrawer(new HierarchyComponentDrawer());

        private static HashSet<Texture> drawComponentTextureCache = new HashSet<Texture>();

        public string Header => "Components";
        public float Width => 16 * 6;

        private Color disabledIconColor = new Color(1f, 1f, 1f, 0.4f);

        public void Draw(Rect area, HierarchyDrawer.Context context) {
            try {
                GUI.BeginGroup(area);
                var drawArea = new Rect(area.width - HierarchyDrawer.ITEM_HEIGHT, 0, HierarchyDrawer.ITEM_HEIGHT, HierarchyDrawer.ITEM_HEIGHT);
                drawComponentTextureCache.Clear();
                var ev = Event.current;

                foreach(var c in context.components) {
                    switch(c) {
                        case Transform t:
                            continue;
                        case HierarchyItem:
                            continue;
                    }
                    var tex = EditorGUIUtility.ObjectContent(null, c.GetType()).image;
                    if(drawComponentTextureCache.Contains(tex))
                        continue;
                    drawComponentTextureCache.Add(tex);

                    bool enabled = c.IsEnabled();
                    if(enabled)
                        GUI.DrawTexture(drawArea, tex, ScaleMode.ScaleToFit);
                    else
                        GUI.DrawTexture(drawArea, tex, ScaleMode.ScaleToFit, true, 1f, disabledIconColor, 0f, 0f);

                    if(ev.type == EventType.MouseDown && ev.button == 0 && ev.alt && drawArea.Contains(ev.mousePosition)) {
                        if(c is MonoBehaviour mb && !c.GetType().FullName.StartsWith("Unity"))
                            HierarchySearchUtility.SetSearch("t:MonoBehaviour");
                        else
                            HierarchySearchUtility.SetSearch(c.GetType());
                        ev.Use();
                    }

                    drawArea.x -= HierarchyDrawer.ITEM_HEIGHT;
                }
            }
            finally {
                GUI.EndGroup();
            }
        }
    }
}
