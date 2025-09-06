using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Toolkit {
    public class RecentPrefabs : EditorWindow {

        #region Classes

        public class Styles {
            public static GUIContent Prefab = EditorGUIUtility.IconContent("d_Prefab Icon");
            public static GUIContent GameObject = EditorGUIUtility.IconContent("d_GameObject Icon");
            public static GUIContent Favorite = EditorGUIUtility.IconContent("d_Favorite Icon");
            public static GUIContent Delete = EditorGUIUtility.IconContent("d_Clear");
        }

        public class Data {
            public List<GameObject> favorites = new List<GameObject>();
            public List<GameObject> recents = new List<GameObject>();
        }

        #endregion

        #region Menu Item

        [MenuItem("Window/Toolkit/Recent Prefabs")]
        private static void ShowRecentPrefabs() {
            var w = GetWindow<RecentPrefabs>();
            w.Show();
            w.titleContent = new GUIContent("Recent Prefabs", Styles.Prefab.image);
        }

        #endregion

        #region Variables

        private const string TAG = "<color=#00FFFF>[Recent Prefabs]</color> - ";
        private const string FILE_PATH = "Library/Toolkit/recentPrefabs.sav";
        private const int MAX_RECENTS = 50;

        private const float HEIGHT = 18;

        [SerializeField] private Vector2 scroll;

        private static Data data = new Data();
        private static List<GameObject> favorites => data.favorites;
        private static List<GameObject> recents => data.recents;
        private static System.Action OnUpdated;

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Init() {
            PrefabStage.prefabStageOpened += OnPrefabOpened;
        }

        private void OnEnable() {
            Load();
            OnUpdated += Repaint;
        }

        private void OnDisable() {
            OnUpdated -= Repaint;
        }

        private static void OnPrefabOpened(PrefabStage stage) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(stage.assetPath);
            if(favorites.Contains(prefab))
                return;
            if(recents.Count > 0 && recents[recents.Count - 1] == prefab)
                return;
            recents.Remove(prefab);
            recents.Add(prefab);

            while(recents.Count > MAX_RECENTS)
                recents.RemoveAt(0);

            Save();
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

        private void DrawEntry(GameObject gameObject, bool favorited) {
            using(var scope = new EditorGUILayout.HorizontalScope("box")) {
                EditorGUILayout.LabelField(Styles.GameObject, GUILayout.Width(HEIGHT), GUILayout.Height(HEIGHT));
                var iconArea = GUILayoutUtility.GetLastRect();
                EditorGUILayout.LabelField(gameObject.name, GUILayout.Height(HEIGHT), GUILayout.MaxWidth(200f));
                var labelRect = GUILayoutUtility.GetLastRect();
                GUILayout.FlexibleSpace();

                var ev = Event.current;
                bool isHoveringAButton = false;

                using(new ColorScope(favorited ? Color.yellow : Color.grey))
                    EditorGUILayout.LabelField(Styles.Favorite, GUILayout.Width(HEIGHT), GUILayout.Height(HEIGHT));

                var favoriteArea = GUILayoutUtility.GetLastRect();
                if(ev.type == EventType.MouseDown && ev.button == 0 && favoriteArea.Contains(ev.mousePosition)) {
                    EditorApplication.delayCall += () => SetFavorite(gameObject, !favorited);
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
                    EditorApplication.delayCall += () => Delete(gameObject);
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
                        AssetDatabase.OpenAsset(gameObject);
                    else if(ev.button == 1)
                        EditorGUIUtility.PingObject(gameObject);
                    ev.Use();
                }
                else if(!isHoveringAButton && scope.rect.Contains(ev.mousePosition)) {
                    EditorGUI.LabelField(labelRect, gameObject.name);
                    using(new ColorScope(Color.black))
                        EditorGUI.LabelField(iconArea, Styles.GameObject);
                    EditorGUI.LabelField(iconArea, Styles.Prefab);
                }
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

        public static void SetFavorite(GameObject prefab, bool isFavorite) {
            if(isFavorite) {
                recents.Remove(prefab);
                if(!favorites.Contains(prefab))
                    favorites.Add(prefab);
            }
            else {
                favorites.Remove(prefab);
                recents.Remove(prefab);
                recents.Add(prefab);
            }
            Save();
        }

        public static void Delete(GameObject prefab) {
            favorites.Remove(prefab);
            recents.Remove(prefab);
            Save();
        }

        #endregion
    }
}
