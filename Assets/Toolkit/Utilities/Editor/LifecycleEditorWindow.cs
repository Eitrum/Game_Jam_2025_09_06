using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit {
    public class LifecycleEditorWindow : EditorWindow {

        [MenuItem("Window/Toolkit/Lifecycle")]
        public static void OpenWindow() {
            GetWindow<LifecycleEditorWindow>().Show();
        }

        private Vector2 scroll;

        private void OnGUI() {
            try {
                GUILayout.BeginArea(new Rect(Vector2.zero, position.size));
                using(var s = new GUILayout.ScrollViewScope(scroll)) {
                    scroll = s.scrollPosition;
                    DrawObjects();
                    DrawDisposables();
                }
            }
            finally {
                GUILayout.EndArea();
            }
        }

        private void DrawDisposables() {
            GUILayout.Label("--- DISPOSABLES ---", EditorStylesUtility.BoldLabel);
            var objs = Lifecycle.Disposables;
            for(int i = 0, len = objs.Count; i < len; i++)
                DrawDisposable(i, objs[i]);
        }

        private void DrawDisposable(int index, IDisposable disposable) {
            if(disposable == null) {
                GUILayout.Label($"{index:000}. null");
                return;
            }
            GUILayout.Label($"{index:000}. {disposable.GetType().FullName}");
        }

        #region Draw Objects

        private void DrawObjects() {
            GUILayout.Label("--- OBJECTS ---", EditorStylesUtility.BoldLabel);
            var objs = Lifecycle.Objects;
            for(int i = 0, len = objs.Count; i < len; i++)
                DrawObject(i, objs[i]);
        }

        private void DrawObject(int index, UnityEngine.Object obj) {
            if(obj == null) {
                GUILayout.Label($"{index:000}. null");
                return;
            }
            GUILayout.Label($"{index:000}. {obj.name} ({obj.GetType().FullName})");
        }

        #endregion
    }
}
