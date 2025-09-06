using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit {
    public class UnityEditorInternalIcons : EditorWindow {
        #region Assets

        private static Texture2D[] internalTextures;

        private static IEnumerable<Texture2D> LoadTextures() {
            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach(Texture2D x in textures) {
                if((x.name.Length == 0) ||
                    (x.hideFlags != HideFlags.HideAndDontSave && x.hideFlags != (HideFlags.HideInInspector | HideFlags.HideAndDontSave)) ||
                    (!EditorUtility.IsPersistent(x)))
                    continue;
                Debug.unityLogger.logEnabled = false;
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                Debug.unityLogger.logEnabled = true;
                if(gc == null || gc.image == null)
                    continue;
                yield return (Texture2D)gc.image;
            }
            Resources.UnloadUnusedAssets();
        }

        public static void LoadAssets() {
            if(internalTextures == null) {
                internalTextures = LoadTextures().ToArray();
                Sort.Quick(internalTextures, (a, b) => a.name.CompareTo(b.name));
            }
        }

        #endregion

        private static Color backgroundColor;

        [MenuItem("Toolkit/Editor/Unity Internal Icons")]
        public static void Open() {
            var content = new GUIContent("Internal Icons", EditorGUIUtility.IconContent("d_SceneAsset Icon").image);
            var w = GetWindow<UnityEditorInternalIcons>(content.text, true);
            w.titleContent = content;
        }

        private int selected = -1;
        private Vector2 scroll;
        private static string searchFilter;

        private void OnGUI() {
            LoadAssets();
            var area = new Rect(Vector2.zero, position.size);
            float iconScale = 24f;
            area.SplitVertical(out Rect search, out Rect body, 20f / area.height);
            searchFilter = EditorGUI.TextField(search, searchFilter);
            body.SplitVertical(out Rect header, out body, 40f / area.height, 4f);

            if(selected >= 0) {
                header.PadRef(10f, 0f, 4f, 4f);
                header.SplitVertical(out Rect nameArea, out Rect iconCodePath, 0.5f);
                EditorGUI.LabelField(nameArea, internalTextures[selected].name);
            }

            EditorGUI.DrawRect(body, backgroundColor.MultiplyRGB(0.3f));
            EditorGUI.DrawRect(body.Shrink(4f), Color.white);
            EditorGUI.DrawRect(body.Shrink(8f), backgroundColor);
            DrawIconsGrid(body.Shrink(8f), ref scroll, iconScale, ref selected);
        }

        public static void DrawIconsGrid(Rect area, ref Vector2 scroll, float iconScale, ref int selected) {
            LoadAssets();
            if(backgroundColor == default) {
                backgroundColor = EditorGUIUtility.isProSkin ? (Color)new Color32(56, 56, 56, 255) : (Color)new Color32(194, 194, 194, 255);
            }
            iconScale += 4f;

            var width = (int)((area.width - 12) / iconScale);
            var neededHeight = internalTextures.Length / width + 1;
            var drawHeight = neededHeight * iconScale;
            EditorGUI.DrawRect(area.Pad(0, 12f, 0, 0), Color.white);

            var drawArea = new Rect(0, 0, width * iconScale, drawHeight);
            scroll = GUI.BeginScrollView(area, scroll, drawArea, false, true);

            int index = 0;
            int widthPos = 0;
            int height = 0;

            if(selected >= 0) {
                int selectedXPos = selected % width;
                int selectedYPos = selected / width;
                var verticalLine = new Rect(selectedXPos * iconScale - 1, 0, iconScale + 2, drawArea.height);
                var horizontalLine = new Rect(0, selectedYPos * iconScale - 1, drawArea.width, iconScale + 2);
                EditorGUI.DrawRect(verticalLine, Color.magenta);
                EditorGUI.DrawRect(horizontalLine, Color.magenta);
            }

            Event ev = Event.current;

            foreach(var icon in internalTextures) {
                if(!string.IsNullOrEmpty(searchFilter) && !icon.name.Contains(searchFilter)) {
                    index++;
                    continue;
                }
                var iconArea = new Rect(widthPos * iconScale, height * iconScale, iconScale, iconScale);
                if(selected == index) {
                    EditorGUI.DrawRect(iconArea.Shrink(-1), Color.cyan);
                }
                EditorGUI.DrawRect(iconArea.Shrink(1f), backgroundColor);
                GUI.DrawTexture(iconArea.Shrink(2f), icon);
                widthPos++;
                if(widthPos >= width) {
                    height++;
                    widthPos = 0;
                }
                if(ev.type == EventType.MouseDown && ev.button == 0 && iconArea.Contains(ev.mousePosition)) {
                    selected = index == selected ? -1 : index;
                    ev.Use();
                }
                index++;
            }
            GUI.EndScrollView(true);
        }
    }
}
