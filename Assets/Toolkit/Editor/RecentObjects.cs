using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Toolkit {
    public class RecentObjects : EditorWindow {

        #region Classes

        public class Styles {
            public static GUIContent Prefab = EditorGUIUtility.IconContent("d_Prefab Icon");
            public static GUIContent GameObject = EditorGUIUtility.IconContent("d_GameObject Icon");
            public static GUIContent Favorite = EditorGUIUtility.IconContent("d_Favorite Icon");
            public static GUIContent Delete = EditorGUIUtility.IconContent("d_Clear");
        }

        public class Data {
            public List<UnityEngine.Object> favorites = new List<UnityEngine.Object>();
            public List<UnityEngine.Object> recents = new List<UnityEngine.Object>();
        }

        #endregion

        #region Menu Item

        [MenuItem("Window/Toolkit/Recent Objects")]
        private static void ShowRecentPrefabs() {
            var w = GetWindow<RecentObjects>();
            w.Show();
            w.titleContent = new GUIContent("Recent Objects", Styles.Prefab.image);
        }

        #endregion

        #region Variables

        private const string TAG = "<color=#00FFFF>[Recent Objects]</color> - ";
        private const string FILE_PATH = "Library/Toolkit/recentObjects.sav";
        private const int MAX_RECENTS = 50;

        private const float HEIGHT = 18;

        [SerializeField] private Vector2 scroll;

        private static Data data = new Data();
        private static List<UnityEngine.Object> favorites => data.favorites;
        private static List<UnityEngine.Object> recents => data.recents;
        private static System.Action OnUpdated;

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Init() {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged() {
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            if(string.IsNullOrEmpty(path))
                return;
            if(favorites.Contains(obj))
                return;
            if(recents.Count > 0 && recents[recents.Count - 1] == obj)
                return;
            recents.Remove(obj);
            recents.Add(obj);

            while(recents.Count > MAX_RECENTS)
                recents.RemoveAt(0);

            Save();
        }

        private void OnEnable() {
            Load();
            OnUpdated += Repaint;
        }

        private void OnDisable() {
            OnUpdated -= Repaint;
        }

        #endregion

        #region Save / Load

        private static void Load() {
            try {
                var dir = System.IO.Path.GetDirectoryName(FILE_PATH);
                if(!System.IO.Directory.Exists(dir))
                    return;
                if(!System.IO.File.Exists(FILE_PATH))
                    return;
                var json = System.IO.File.ReadAllText(FILE_PATH);
                EditorJsonUtility.FromJsonOverwrite(json, data);
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        private static void Save() {
            try {
                var dir = System.IO.Path.GetDirectoryName(FILE_PATH);
                if(!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                var json = EditorJsonUtility.ToJson(data, true);
                System.IO.File.WriteAllText(FILE_PATH, json);
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
            OnUpdated?.Invoke();
        }

        #endregion

        #region Draw

        private void OnGUI() {
            try {
                GUILayout.BeginArea(new Rect(Vector2.zero, position.size));
                using(var s = new EditorGUILayout.ScrollViewScope(scroll)) {
                    scroll = s.scrollPosition;
                    using(new EditorGUILayout.VerticalScope("box")) {
                        for(int i = 0; i < favorites.Count; i++) {
                            DrawEntry(favorites[i], true);
                        }
                    }
                    var line = GUILayoutUtility.GetRect(position.width, 2);
                    EditorGUI.DrawRect(new Rect(line.x + 4, line.y, line.width - 8, line.height), Color.gray);

                    using(new EditorGUILayout.VerticalScope("box")) {
                        for(int i = recents.Count - 1; i >= 0; i--) {
                            if(recents[i] == null) {
                                recents.RemoveAt(i);
                                continue;
                            }
                            DrawEntry(recents[i], false);
                        }
                    }
                }
            }
            finally {
                GUILayout.EndArea();
            }
        }

        private void DrawEntry(UnityEngine.Object obj, bool favorited) {
            using(var scope = new EditorGUILayout.HorizontalScope("box")) {
                var iconRect = GUILayoutUtility.GetRect(HEIGHT, HEIGHT);
                var nameOfObject = ProjectWindowUtil.IsFolder(obj.GetInstanceID()) ? AssetDatabase.GetAssetPath(obj) : obj.name;
                GUI.DrawTexture(iconRect, EditorGUIUtility.ObjectContent(obj, typeof(UnityEngine.Object)).image);
                EditorGUILayout.LabelField(nameOfObject, GUILayout.Height(HEIGHT), GUILayout.MaxWidth(200f));
                var labelRect = GUILayoutUtility.GetLastRect();
                GUILayout.FlexibleSpace();

                var ev = Event.current;
                bool isHoveringAButton = false;

                using(new ColorScope(favorited ? Color.yellow : Color.grey))
                    EditorGUILayout.LabelField(Styles.Favorite, GUILayout.Width(HEIGHT), GUILayout.Height(HEIGHT));

                var favoriteArea = GUILayoutUtility.GetLastRect();
                if(ev.type == EventType.MouseDown && ev.button == 0 && favoriteArea.Contains(ev.mousePosition)) {
                    EditorApplication.delayCall += () => SetFavorite(obj, !favorited);
                    ev.Use();
                    Repaint();
                }
                else if(favoriteArea.Contains(ev.mousePosition)) {
                    isHoveringAButton = true;
                    using(new ColorScope(favorited ? Color.white : Color.black))
                        EditorGUI.LabelField(new Rect(favoriteArea.x - 2, favoriteArea.y - 2, favoriteArea.width + 4, favoriteArea.height + 4), Styles.Favorite);

                    using(new ColorScope(favorited ? Color.grey : Color.yellow))
                        EditorGUI.LabelField(favoriteArea, Styles.Favorite);
                }

                using(new ColorScope(Color.gray))
                    EditorGUILayout.LabelField(Styles.Delete, GUILayout.Width(HEIGHT), GUILayout.Height(HEIGHT));
                var deleteArea = GUILayoutUtility.GetLastRect();
                if(ev.type == EventType.MouseDown && ev.button == 0 && deleteArea.Contains(ev.mousePosition)) {
                    EditorApplication.delayCall += () => Delete(obj);
                    ev.Use();
                    Repaint();
                }
                else if(deleteArea.Contains(ev.mousePosition)) {
                    isHoveringAButton = true;
                    using(new ColorScope(Color.red))
                        EditorGUI.LabelField(deleteArea, Styles.Delete);
                }

                if(ev.type == EventType.MouseDown && scope.rect.Contains(ev.mousePosition)) {
                    if(ev.button == 0)
                        Activate(obj);
                    else if(ev.button == 1)
                        EditorGUIUtility.PingObject(obj);
                    ev.Use();
                }
                else if(!isHoveringAButton && scope.rect.Contains(ev.mousePosition)) {
                    EditorGUI.LabelField(labelRect, nameOfObject);
                }
            }
        }

        private static void Activate(UnityEngine.Object obj) {
            switch(obj) {
                case MonoScript:
                case GameObject:
                    AssetDatabase.OpenAsset(obj);
                    break;
                default:
                    Selection.SetActiveObjectWithContext(obj, null);
                    if(ProjectWindowUtil.IsFolder(obj.GetInstanceID())) {
                        var projectWindow = UnityInternalUtility.EditorWindow.GetProject();
                        if(UnityInternalUtility.TryGetMethod(projectWindow.GetType(), "OpenSelectedFolders", out var mi)) {
                            mi.Invoke(null, null);
                            EditorApplication.delayCall += () => mi?.Invoke(null, null);
                        }
                    }
                    else {
                        EditorGUIUtility.PingObject(obj);
                    }
                    break;
            }
        }

        public class ColorScope : IDisposable {

            private Color previousColor;
            private bool isDisposed;

            public ColorScope(Color color) {
                previousColor = GUI.color;
                GUI.color = color;
            }

            public void Dispose() {
                if(isDisposed)
                    return;
                isDisposed = true;
                GUI.color = previousColor;
            }
        }

        #endregion

        #region Util

        public static void SetFavorite(UnityEngine.Object obj, bool isFavorite) {
            if(isFavorite) {
                recents.Remove(obj);
                if(!favorites.Contains(obj))
                    favorites.Add(obj);
            }
            else {
                favorites.Remove(obj);
                recents.Remove(obj);
                recents.Add(obj);
            }
            Save();
        }

        public static void Delete(UnityEngine.Object obj) {
            favorites.Remove(obj);
            recents.Remove(obj);
            Save();
        }

        #endregion
    }
}
