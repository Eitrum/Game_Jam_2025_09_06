using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolkit.SceneManagement {
    public class QuickScene : EditorWindow {

        private class Styles {
            public static GUIStyle RichTextCenterAlignedBold = new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            public static GUIStyle RichTextCenterAligned = new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true, alignment = TextAnchor.MiddleCenter };
        }

        #region Variables

        public const string TAG = "<color=#008080>[Toolkit.SceneManagement.QuickScene]</color> - ";
        public Texture2D texture;
        private Texture2D background;
        private double openTime = 0d;
        private IReadOnlyList<QuickSceneData.SceneData> scenes;

        #endregion

        #region Init

        [UnityEditor.ShortcutManagement.Shortcut(QuickSceneProjectSettings.SHORTCUT_ID, KeyCode.S, UnityEditor.ShortcutManagement.ShortcutModifiers.Control | UnityEditor.ShortcutManagement.ShortcutModifiers.Alt, displayName = "Toolkit/Open Quick Scene")]
        private static void OpenQuickScene() {
            var pos = (Event.current.mousePosition);
            var relativePos = EditorGUIUtility.GUIToScreenPoint(pos);
            var area = new Rect(relativePos.x - 300, relativePos.y - 300, 600, 600);

            var w = CreateInstance<QuickScene>();
            w.position = area;
            w.scenes = QuickSceneProjectSettings.AllScenes;
            w.ShowPopup();
        }

        private void OnEnable() {
            background = Resources.Load<Texture2D>("QuickSceneBackground");
            openTime = EditorApplication.timeSinceStartup;
        }

        private void OnLostFocus() {
            if(texture)
                DestroyImmediate(texture);
            Close();
        }

        private void LoadTexture() {
            var area = position;
            var color = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(area.position, (int)area.width, (int)area.height);
            texture = new Texture2D((int)area.width, (int)area.height);
            texture.SetPixels(color);
            texture.Apply();
        }

        #endregion

        #region Draw

        private void OnGUI() {
            if(scenes == null)
                return;
            if(texture == null)
                LoadTexture();

            var area = new Rect(Vector2.zero, position.size);
            var center = area.center;

            float time = (float)(EditorApplication.timeSinceStartup - openTime);

            EditorGUI.DrawRect(area, Color.black);
            if(texture)
                GUI.DrawTexture(area, texture);

            var ev = Event.current;
            var mousePos = ev.mousePosition;

            static float EaseOut(float time) {
                return -time * (time - 2f);
            }

            var transition = EaseOut(Mathf.Clamp01(time * 4f));
            int hoverIndex = -1;
            float count = scenes.Count;
            float dist = Mathf.Lerp(150f, 300f, (count - 12f) / 24f);
            var mtx = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(center, Quaternion.identity, new Vector3(transition, transition, transition));

            mousePos -= center;

            if(background) {
                GUI.DrawTexture(new Rect(-new Vector2(dist + 50, dist + 50), new Vector2(dist * 2f + 100, dist * 2f + 100)), background);
            }

            for(int i = 0; i < count; i++) {
                var s = scenes[i];
                var r = Mathf.Deg2Rad * (360f / count) * (float)i;
                var dir = new Vector2(Mathf.Sin(r), -Mathf.Cos(r));
                var pos = dir * dist;
                var objDist = Vector2.Distance(mousePos, pos);
                var scale = (1f + Mathf.Pow(Mathf.Clamp01((objDist - 200f) / (50f - 200f)), 4f) / 4f);

                var size = new Vector2(50f * scale, 50f * scale);
                var texArea = new Rect(pos - size / 2f, size);
                var borderColor = Color.black;
                var isLoaded = false;

                if(s.IsValid) {
                    var scene = EditorSceneManager.GetSceneByName(s.sceneAsset.name);
                    if(scene.isLoaded) {
                        isLoaded = true;
                        borderColor = new Color32(218, 165, 32, 255); // GoldenRod
                    }
                }

                EditorGUI.DrawRect(new Rect(texArea.x - 1, texArea.y - 1, texArea.width + 2, texArea.height + 2), borderColor);
                EditorGUI.DrawRect(texArea, Color.gray);

                if(s.scenePreview)
                    GUI.DrawTexture(texArea, s.scenePreview);

                if(s.sceneAsset)
                    EditorGUI.DropShadowLabel(new Rect(pos - new Vector2(100f * scale, 10f * scale), new Vector2(200f * scale, 20f * scale)), s.sceneAsset.name, Styles.RichTextCenterAlignedBold);

                if(texArea.Contains(mousePos))
                    hoverIndex = i;

                if(ev.type == EventType.MouseDown && ev.button == 0 && s.sceneAsset && texArea.Contains(ev.mousePosition)) {
                    var mode = (ev.control || ev.alt) ? QuickSceneProjectSettings.AlternativeLoadMode : QuickSceneProjectSettings.DefaultLoadMode;
                    if(isLoaded && mode != OpenSceneMode.Single) { // Unload
                        var sceneToUnload = EditorSceneManager.GetSceneByName(s.sceneAsset.name);
                        var result = EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { sceneToUnload });
                        if(result)
                            EditorSceneManager.UnloadSceneAsync(sceneToUnload, UnloadSceneOptions.None);
                    }
                    else {
                        if(mode == OpenSceneMode.Single) {
                            if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                break;
                        }
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(s.sceneAsset), mode);
                    }
                    break;
                }
            }

            if(hoverIndex >= 0) {
                EditorGUI.DrawRect(new Rect(-new Vector2(75, 75), new Vector2(150, 150)), Color.gray);
                if(scenes[hoverIndex].scenePreview)
                    GUI.DrawTexture(new Rect(-new Vector2(75, 75), new Vector2(150, 150)), scenes[hoverIndex].scenePreview);
                EditorGUI.DrawRect(new Rect(-new Vector2(75, 10), new Vector2(150, 85)), new Color(0.1f, 0.1f, 0.1f, 0.5f));
                if(scenes[hoverIndex].sceneAsset) {
                    EditorGUI.LabelField(new Rect(-new Vector2(150, 0), new Vector2(300, 75)), $"<b>{scenes[hoverIndex].sceneAsset.name}</b>\n{scenes[hoverIndex].description}", Styles.RichTextCenterAligned);
                }
            }
            GUI.matrix = mtx;

            if(ev.type == EventType.MouseDown && ev.button == 0) {
                Close();
            }

            Repaint();
        }

        #endregion
    }
}
