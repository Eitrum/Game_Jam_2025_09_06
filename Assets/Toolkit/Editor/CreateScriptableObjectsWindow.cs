using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Internal {
    public class CreateScriptableObjectsWindow : EditorWindow {

        #region Variables

        private static bool isInitialized = false;
        private static IEnumerable<System.Type> scriptableObjects;
        private string filter = "";
        private System.Type currentlySelected;
        private Vector2 scroll;

        #endregion

        #region Open

        [MenuItem("Window/Toolkit/Internal/Create Scriptable Objects")]
        private static void OpenWindow() {
            var w = GetWindow<CreateScriptableObjectsWindow>();
            w.Show();
        }

        #endregion

        #region Init

        private void OnEnable() {
            Initialize();
        }

        private static void Initialize() {
            if(isInitialized)
                return;
            isInitialized = true;
            var monoScript = MonoImporter.GetAllRuntimeMonoScripts();

            scriptableObjects = monoScript
                .Where(x => x != null)
                .Select(x => x.GetClass())
                .Where(x => x != null)
                .Where(x => x.IsSubclassOf(typeof(ScriptableObject)));
        }

        #endregion

        private void OnGUI() {
            var area = new Rect(Vector2.zero, position.size);
            try {
                GUILayout.BeginArea(area);
                DrawHeader();
                using(var ss = new EditorGUILayout.ScrollViewScope(scroll)) {
                    ss.handleScrollWheel = true;
                    DrawList();
                    scroll = ss.scrollPosition;
                }
            }
            finally {
                GUILayout.EndArea();
            }
        }

        private void DrawHeader() {
            using(new EditorGUILayout.HorizontalScope("box")) {
                filter = GUILayout.TextField(filter);

                using(new EditorGUI.DisabledScope(currentlySelected == null)) {
                    if(GUILayout.Button("Create", GUILayout.Width(90))) {
                        var so = ScriptableObject.CreateInstance(currentlySelected);
                        ProjectWindowUtil.CreateAsset(so, UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/new {currentlySelected.Name}.asset"));
                    }
                }
            }
        }

        private void DrawList() {
            var filtered = scriptableObjects
                .Where(x=>x.FullName.StartsWith(filter));
            int len = 0;
            foreach(var so in filtered) {
                DrawEntry(so);
                len++;
            }
            currentlySelected = filtered.FirstOrDefault();
            if(currentlySelected != null && currentlySelected.FullName != filter) {
                currentlySelected = null;
            }
        }

        private void DrawEntry(Type so) {
            var ev = Event.current;
            GUILayout.Label($"{so.FullName}");

            if(ev.type == EventType.MouseDown && ev.button == 0) {
                var area = GUILayoutUtility.GetLastRect();
                if(area.Contains(ev.mousePosition)) {
                    filter = so.FullName;
                    ev.Use();
                }
            }
        }
    }
}
