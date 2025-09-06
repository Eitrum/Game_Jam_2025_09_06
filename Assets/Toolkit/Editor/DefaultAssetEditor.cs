using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolkit {
    [CustomEditor(typeof(DefaultAsset), isFallback = false)]
    public class DefaultAssetEditor : Editor {
        private class Container {
            public string FileExtension;
            public Type Type;

            public MethodInfo Awake;
            public MethodInfo OnDestroy;

            public MethodInfo OnEnable;
            public MethodInfo OnDisable;

            public MethodInfo OnSceneGUI;
            public MethodInfo OnValidate;
            public MethodInfo OnHeaderGUI;
            public MethodInfo ShouldHideOpenButton;

            public Container(string fileExtension, Type type) {
                this.FileExtension = fileExtension;
                this.Type = type;
                Awake = type.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                OnDestroy = type.GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                OnEnable = type.GetMethod("OnEnable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                OnDisable = type.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                OnSceneGUI = type.GetMethod("OnSceneGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                OnValidate = type.GetMethod("OnValidate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                OnHeaderGUI = type.GetMethod("OnHeaderGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                ShouldHideOpenButton = type.GetMethod("ShouldHideOpenButton", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        private static Dictionary<string, Container> extensionToEditor = new Dictionary<string, Container>();

        public static void Register<T>(string fileExtension) where T : Editor {
            extensionToEditor.Add(fileExtension, new Container(fileExtension, typeof(T)));

            EditorApplication.delayCall += () => {
                var objs = Editor.FindObjectsByType<T>(FindObjectsSortMode.None);
                if(objs.Length > 0) {
                    Debug.Log($"Existing {fileExtension} editors: " + objs.Length);
                }
            };
        }

        public static void Register(string fileExtension, Type type) {
            extensionToEditor.Add(fileExtension, new Container(fileExtension, type));

            EditorApplication.delayCall += () => {
                var objs = Editor.FindObjectsByType(type, FindObjectsSortMode.None);
                if(objs.Length > 0) {
                    Debug.Log($"Existing {fileExtension} editors: " + objs.Length);
                }
            };
        }

        #region Local Variables

        [NonSerialized] private Editor currentEditor;
        private Container container;
        private Vector2 localScroll = Vector2.zero;

        #endregion

        #region Unity Methods

        private void Awake() {
            if(currentEditor == null) {
                var extension = System.IO.Path.GetExtension(AssetDatabase.GetAssetPath(target));
                if(extensionToEditor.TryGetValue(extension, out Container con)) {
                    this.container = con;
                    currentEditor = Editor.CreateEditor(target, con.Type);
                    container.Awake?.Invoke(currentEditor, null);
                }
            }
        }

        private void OnDestroy() {
            if(currentEditor != null) {
                container.OnDestroy?.Invoke(currentEditor, null);
                DestroyImmediate(currentEditor);
                currentEditor = null;
                container = null;
            }
        }

        private void OnEnable() {
            if(currentEditor == null) {
                var extension = System.IO.Path.GetExtension(AssetDatabase.GetAssetPath(target));
                if(extensionToEditor.TryGetValue(extension, out Container con)) {
                    this.container = con;
                    currentEditor = Editor.CreateEditor(target, con.Type);
                    container.Awake?.Invoke(currentEditor, null);
                }
            }
            if(currentEditor != null) {
                container.OnEnable?.Invoke(currentEditor, null);
            }
        }

        private void OnDisable() {
            if(currentEditor != null) {
                container.OnDisable?.Invoke(currentEditor, null);
            }
        }

        private void OnSceneGUI() {
            if(currentEditor != null) {
                container.OnSceneGUI?.Invoke(currentEditor, null);
            }
        }

        private void OnValidate() {
            if(currentEditor != null) {
                container.OnValidate?.Invoke(currentEditor, null);
            }
        }

        #endregion

        #region Editor Methods

        public override VisualElement CreateInspectorGUI() {
            if(currentEditor != null) {
                return currentEditor.CreateInspectorGUI();
            }
            return base.CreateInspectorGUI();
        }

        public override void DrawPreview(Rect previewArea) {
            if(currentEditor != null) {
                currentEditor.DrawPreview(previewArea);
            }
            else {
                base.DrawPreview(previewArea);
            }
        }

        public override bool Equals(object other) {
            if(currentEditor != null) {
                return currentEditor.Equals(other);
            }
            return base.Equals(other);
        }

        public override int GetHashCode() {
            if(currentEditor != null) {
                return currentEditor.GetHashCode();
            }
            return base.GetHashCode();
        }

        public override string ToString() {
            if(currentEditor != null) {
                return currentEditor.ToString();
            }
            return base.ToString();
        }

        public override string GetInfoString() {
            if(currentEditor != null) {
                return currentEditor.GetInfoString();
            }
            return base.GetInfoString();
        }

        public override GUIContent GetPreviewTitle() {
            if(currentEditor != null) {
                return currentEditor.GetPreviewTitle();
            }
            return base.GetPreviewTitle();
        }

        public override bool HasPreviewGUI() {
            if(currentEditor != null) {
                return currentEditor.HasPreviewGUI();
            }
            return base.HasPreviewGUI();
        }

        protected override void OnHeaderGUI() {
            if(currentEditor != null)
                container.OnHeaderGUI?.Invoke(currentEditor, null);
            else
                base.OnHeaderGUI();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            if(currentEditor != null)
                currentEditor.OnInteractivePreviewGUI(r, background);
            else
                base.OnInteractivePreviewGUI(r, background);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background) {
            if(currentEditor != null)
                currentEditor.OnPreviewGUI(r, background);
            else
                base.OnPreviewGUI(r, background);
        }

        public override void OnPreviewSettings() {
            if(currentEditor != null)
                currentEditor.OnPreviewSettings();
            else
                base.OnPreviewSettings();
        }

        public override void ReloadPreviewInstances() {
            if(currentEditor != null)
                currentEditor.ReloadPreviewInstances();
            else
                base.ReloadPreviewInstances();
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height) {
            if(currentEditor != null)
                return currentEditor.RenderStaticPreview(assetPath, subAssets, width, height);
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override bool RequiresConstantRepaint() {
            if(currentEditor != null)
                return currentEditor.RequiresConstantRepaint();
            return base.RequiresConstantRepaint();
        }

        protected override bool ShouldHideOpenButton() {
            if(currentEditor != null && container.ShouldHideOpenButton != null)
                return (bool)container.ShouldHideOpenButton.Invoke(currentEditor, null);
            return base.ShouldHideOpenButton();
        }

        public override bool UseDefaultMargins() {
            if(currentEditor != null)
                return currentEditor.UseDefaultMargins();
            return base.UseDefaultMargins();
        }

        public override void OnInspectorGUI() {
            GUI.enabled = true;
            if(currentEditor != null) {
                currentEditor.OnInspectorGUI();
            }
            else {
                var path = AssetDatabase.GetAssetPath(target);
                if(AssetDatabase.IsValidFolder(path)) {
                    var dirs = System.IO.Directory.GetDirectories(path);
                    var files = System.IO.Directory.GetFiles(path).Where(x => !x.EndsWith(".meta"));
                    var fileCount = files.Count();

                    using(new EditorGUILayout.VerticalScope("box")) {
                        EditorGUILayout.LabelField($"{dirs.Length} Folders", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"{fileCount} Assets", EditorStyles.boldLabel);
                    }

                    EditorGUILayout.Space(12f);

                    localScroll = EditorGUILayout.BeginScrollView(localScroll, "box");
                    var buttons = FolderEditor.Buttons;
                    if(buttons.Count > 0) {
                        EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
                        var totalWidth = Screen.width;
                        var width = 0f;

                        EditorGUILayout.BeginHorizontal();
                        foreach(var but in buttons) {
                            width += but.CalculateWidth() + 5f;
                            if(width > totalWidth) {
                                width = but.Width + 5f;
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space(2f);
                                EditorGUILayout.BeginHorizontal();
                            }
                            but.DrawButton(target);
                            EditorGUILayout.Space(5f);
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space(12f);

                    if(dirs.Length > 0) {
                        EditorGUILayout.LabelField("Folders", EditorStyles.boldLabel);
                        using(new EditorGUI.IndentLevelScope())
                            foreach(var dir in dirs)
                                EditorGUILayout.LabelField(new GUIContent(System.IO.Path.GetFileName(dir), AssetDatabase.GetCachedIcon(dir)));
                    }
                    if(fileCount > 0) {
                        EditorGUILayout.LabelField("Assets", EditorStyles.boldLabel);
                        using(new EditorGUI.IndentLevelScope())
                            foreach(var file in files)
                                EditorGUILayout.LabelField(new GUIContent(System.IO.Path.GetFileNameWithoutExtension(file), AssetDatabase.GetCachedIcon(file)));
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                    EditorGUILayout.LabelField("This file is not supported.");
            }
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultAssetEditorAttribute : Attribute {
        public string Extension { get; private set; }

        public DefaultAssetEditorAttribute(string extension) {
            this.Extension = extension;
        }

        [InitializeOnLoadMethod]
        private static void RegisterAll() {
            AssetDatabaseUtility.GetAllTypes(UnityEditor.Compilation.AssembliesType.Editor)
                .Where(x => x.GetCustomAttributes<DefaultAssetEditorAttribute>().Count() != 0)
                .Select(x => (x, x.GetCustomAttributes<DefaultAssetEditorAttribute>()))
                .Foreach(x => x.Item2.Foreach(y => DefaultAssetEditor.Register(y.Extension, x.x)));
        }
    }
}
