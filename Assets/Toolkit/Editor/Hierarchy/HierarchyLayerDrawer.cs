using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class HierarchyLayerDrawer : HierarchyDrawer.ICustomDrawer {

        [InitializeOnLoadMethod]
        private static void Init() => HierarchyDrawer.RegisterCustomDrawer(new HierarchyLayerDrawer());

        public class Styles {
            public static readonly Texture Background = Resources.Load<Texture2D>("hierarchy_tag_background");
            public static readonly GUIStyle Label = new GUIStyle(EditorStylesUtility.CenterAlignedMiniLabel) { normal = new GUIStyleState(){ textColor = Color.black }, contentOffset = new Vector2(0,-1) };
        }

        public string Header => "Layers";
        public float Width => 96;

        public void Draw(Rect area, HierarchyDrawer.Context context) {
            var color = GetColor(context.gameObject.layer);
            GUI.DrawTexture(area, Styles.Background, ScaleMode.StretchToFill, true, 4f, color, -4, 0);
            GUI.Label(area.Shrink(3f), LayerMask.LayerToName(context.gameObject.layer), Styles.Label);
        }

        #region Color

        private static int GetHash32(string str) {
            unchecked {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        private static int Bit(int a, int b) => (a & (1 << b)) >> b;
        public static Color GetColor(int i, float alpha = 0.5f) {
            if(i == 0)
                return new Color(0, 0.75f, 1.0f, 0.5f);
            int r = (Bit(i, 4) + Bit(i, 1) * 2 + 1) * 63;
            int g = (Bit(i, 3) + Bit(i, 2) * 2 + 1) * 63;
            int b = (Bit(i, 5) + Bit(i, 0) * 2 + 1) * 63;
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, alpha);
        }

        #endregion
    }
}
