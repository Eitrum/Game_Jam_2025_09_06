using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEditorInternal;
using System;
using Object = UnityEngine.Object;

namespace Toolkit.SceneManagement {
    public static class QuickSceneProjectSettings {

        public class Styles {
            public static GUIStyle FilePathLabel = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic };
            public static GUIStyle DescriptionLabel = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic, alignment = TextAnchor.UpperLeft };
        }

        #region Variables

        private const string TAG = "<color=#008080>[Toolkit.SceneManagement.QuickSceneProjectSettings]</color> - ";

        public const string SHORTCUT_ID = "openQuickScene";
        private const string LOADMODE_PATH = "Tookit.QuickScene.LoadMode";
        private const string LOADMODE_ALT_PATH = "Tookit.QuickScene.LoadModeAlt";

        private static OpenSceneMode defaultLoadMode = OpenSceneMode.Additive;
        private static OpenSceneMode alternativeLoadMode = OpenSceneMode.Single;

        private static QuickSceneData shared = new QuickSceneData(QuickSceneData.Mode.Shared);
        private static QuickSceneData local = new QuickSceneData(QuickSceneData.Mode.Local);

        private static ListDrawer sharedDrawer;
        private static ListDrawer localDrawer;

        #endregion

        #region Properties

        public static ShortcutBinding Shortcut {
            get {
                return ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID);
            }
            set {
                if(value.ToString().Equals(Shortcut.ToString()))
                    return;
                ShortcutManager.instance.RebindShortcut(SHORTCUT_ID, value);
            }
        }

        public static OpenSceneMode DefaultLoadMode {
            get => defaultLoadMode;
            set {
                if(defaultLoadMode == value)
                    return;
                defaultLoadMode = value;
                EditorPrefs.SetInt(LOADMODE_PATH, (int)defaultLoadMode);
            }
        }

        public static OpenSceneMode AlternativeLoadMode {
            get => alternativeLoadMode;
            set {
                if(alternativeLoadMode == value)
                    return;
                alternativeLoadMode = value;
                EditorPrefs.SetInt(LOADMODE_ALT_PATH, (int)alternativeLoadMode);
            }
        }

        public static IReadOnlyList<QuickSceneData.SceneData> AllScenes {
            get {
                var scenes = new List<QuickSceneData.SceneData>();
                scenes.AddRange(shared.scenes);
                scenes.AddRange(local.scenes);
                return scenes;
            }
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Init() {
            defaultLoadMode = (OpenSceneMode)EditorPrefs.GetInt(LOADMODE_PATH, 1);
            alternativeLoadMode = (OpenSceneMode)EditorPrefs.GetInt(LOADMODE_ALT_PATH, 0);

            Reload();
            EditorApplication.playModeStateChanged += (state) => { if(state == PlayModeStateChange.EnteredEditMode) Reload(); };

            sharedDrawer = new ListDrawer(shared);
            localDrawer = new ListDrawer(local);
        }

        private static void Reload() {
            shared.Load();
            local.Load();
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Editor/Quick Scene", SettingsScope.Project) {
                guiHandler = OnSettingsGUI
            };
        }

        #endregion

        #region Drawing

        private static void OnSettingsGUI(string obj) {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                using(new EditorGUILayout.HorizontalScope()) {
                    var keybind = UnityEditor.ShortcutManagement.ShortcutManager.instance.GetShortcutBinding(SHORTCUT_ID);
                    EditorGUILayout.LabelField($"Shortcut - {string.Join("+", keybind.keyCombinationSequence)}");
                    if(GUILayout.Button("Edit", GUILayout.Width(80))) {
                        OpenShortcutManager();
                    }
                }

                EditorGUI.BeginChangeCheck();

                defaultLoadMode = (OpenSceneMode)EditorGUILayout.EnumPopup("Load Mode (Default)", defaultLoadMode);
                alternativeLoadMode = (OpenSceneMode)EditorGUILayout.EnumPopup("Load Mode (CTRL)", alternativeLoadMode);

                if(EditorGUI.EndChangeCheck()) {
                    EditorPrefs.SetInt(LOADMODE_PATH, (int)defaultLoadMode);
                    EditorPrefs.SetInt(LOADMODE_ALT_PATH, (int)alternativeLoadMode);
                }
            }
            EditorGUILayout.Space();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Shared [ProjectSettings/]", EditorStyles.boldLabel);
                sharedDrawer.Draw();
            }
            EditorGUILayout.Space();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Local [UserSettings/]", EditorStyles.boldLabel);
                localDrawer.Draw();
            }
        }

        #endregion

        #region Open Shortcut Manager

        public static void OpenShortcutManager() {
            try {
                EditorApplication.ExecuteMenuItem("Edit/Shortcuts...");

                var shortcutWindowType = typeof(Editor).Assembly.GetType("UnityEditor.ShortcutManagement.ShortcutManagerWindow");
                var window = EditorWindow.GetWindow(shortcutWindowType);
                var viewController = shortcutWindowType
                        .GetField("m_ViewController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                        .GetValue(window);

                var allEntries = viewController
                        .GetType()
                        .GetField("m_AllEntries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(viewController) as System.Collections.IList;

                var displayNameFieldInfo = allEntries[0].GetType().GetField("m_DisplayName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                foreach(var entry in allEntries) {
                    var displayName = displayNameFieldInfo.GetValue(entry);
                    if(displayName.Equals("Toolkit/Open Quick Scene")) {
                        viewController
                            .GetType()
                            .GetMethod("NavigateTo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                            .Invoke(viewController, new object[] { entry });
                        break;
                    }
                }
            }
            catch {
                EditorUtility.DisplayDialog("Error", "Failed to open the shortcut manager properly!\nNavigate to \"Edit/Shortcuts...\"\nSelect category \"Toolkit\" or search for \"Open Quick Scene\"", "Ok");
            }
        }

        #endregion

        #region Screenshot 

        private static Texture2D CaptureSceneViewCamera(int width, int height) {
            var cam = UnityEditor.SceneView.lastActiveSceneView.camera;
            if(!cam) {
                Debug.LogError("Camera does not exists!");
                return null;
            }
            return Capture(cam, width, height);
        }

        private static Texture2D Capture(Camera camera, int width, int height) {
            // Setup
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            var tempRT = new RenderTexture(width, height, 24);

            // Draw
            camera.targetTexture = tempRT;
            camera.Render();

            // Copy
            RenderTexture.active = tempRT;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // Restore
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(tempRT);
            return texture;
        }

        #endregion

        private class ListDrawer {
            private QuickSceneData data;
            private ReorderableList sceneList;
            public ListDrawer(QuickSceneData data) {
                this.data = data;
                sceneList = new ReorderableList(data.scenes, typeof(QuickSceneData.SceneData));
                sceneList.drawElementCallback += OnDrawScene;
                sceneList.elementHeight = EditorGUIUtility.singleLineHeight * 4f + 8f;
                sceneList.headerHeight = 0f;
                sceneList.onReorderCallback += OnReorder;
            }

            public void Draw() {
                sceneList.DoLayoutList();
            }

            private void OnReorder(ReorderableList list) {
                data.Save();
            }

            private void OnDrawScene(Rect rect, int index, bool isActive, bool isFocused) {
                var dat = data.scenes[index];
                // Area setup
                rect = new Rect(rect.x, rect.y + 2, rect.width - 2, rect.height - 4);
                var previewWidth = EditorGUIUtility.singleLineHeight * 4f;
                var preview = new Rect(rect.x, rect.y, previewWidth, rect.height);
                var body = new Rect(rect.x + previewWidth + 2, rect.y, rect.width - previewWidth - 2, rect.height);
                var top = new Rect(body.x, body.y, body.width, EditorGUIUtility.singleLineHeight);
                var mid = new Rect(body.x, body.y + EditorGUIUtility.singleLineHeight + 1, body.width, EditorGUIUtility.singleLineHeight);
                var bot = new Rect(body.x, body.y + EditorGUIUtility.singleLineHeight * 2 + 4, body.width, EditorGUIUtility.singleLineHeight * 2);
                EditorGUI.DrawRect(preview, Color.black);

                if(dat.scenePreview)
                    GUI.DrawTexture(preview, dat.scenePreview);

                bool enabled = dat.sceneAsset != null && EditorSceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(dat.sceneAsset)).isLoaded;
                if(enabled) {
                    var iconArea = new Rect(preview.x + preview.width - 20f, preview.y + 4, 16, 16f);
                    GUI.DrawTexture(iconArea, EditorGUIUtility.IconContent("d_rotateTool On").image);
                    var ev = Event.current;
                    if(enabled && ev != null && ev.type == EventType.MouseDown && ev.button == 0 && iconArea.Contains(ev.mousePosition)) {
                        try {
                            if(dat.scenePreview != null) {
                                Object.DestroyImmediate(dat.scenePreview);
                            }
                            dat.scenePreview = CaptureSceneViewCamera(256, 256);
                            dat.scenePreview.Apply();
                            dat.scenePreview.hideFlags = HideFlags.HideAndDontSave;
                            dat.textureFilePath = string.IsNullOrEmpty(dat.textureFilePath) ? ($"{data.GetTextureFolderPath()}/{dat.sceneAsset.name}.png") : dat.textureFilePath;
                            System.IO.File.WriteAllBytes(dat.textureFilePath, dat.scenePreview.EncodeToPNG());
                            data.Save();
                        }
                        catch(Exception e) {
                            Debug.LogException(e);
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                dat.sceneAsset = EditorGUI.ObjectField(top, "Scene", dat.sceneAsset, typeof(SceneAsset), false) as SceneAsset;
                dat.description = EditorGUI.TextArea(bot, dat.description);
                if(string.IsNullOrEmpty(dat.description))
                    EditorGUI.LabelField(bot, "description...", Styles.DescriptionLabel);

                if(EditorGUI.EndChangeCheck()) {
                    data.Save();
                }
                EditorGUI.LabelField(mid, dat.textureFilePath, Styles.FilePathLabel);
            }
        }
    }
}
