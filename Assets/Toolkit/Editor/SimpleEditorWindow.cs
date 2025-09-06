using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    [SimpleEditorWindowBinding]
    public abstract class SimpleEditorWindow : EditorWindow {

        #region Attribute Hack

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        private class SimpleEditorWindowBindingAttribute : Attribute { }

        #endregion

        #region Init

        private static bool isLoading = false;

        [InitializeOnLoadMethod]
        private static void Init() {
            Load();
            Menu.OnMenuUpdated += Load;
        }

        private static async void Load() {
            if(isLoading)
                return;
            isLoading = true;
            await AttributeStorage.WaitUntilInitialized();
            if(!AttributeStorage.TryFind(typeof(SimpleEditorWindowBindingAttribute), out var storage)) {
                Debug.LogError(TAG + "Could not find " + nameof(SimpleEditorWindowBindingAttribute));
                return;
            }

            var baseType = typeof(SimpleEditorWindow);
            foreach(var c in storage.GetAllClasses(true)) {
                if(c.Class == baseType)
                    continue;
                try {
                    //Debug.Log(TAG + "FOUND: " + c.Class.FullName);
                    var temporaryObj = ScriptableObject.CreateInstance(c.Class) as SimpleEditorWindow;
                    if(temporaryObj.ADD_MENU_ITEM) {
                        var path = temporaryObj.MENU_ITEM_PATH;
                        if(Menu.MenuItemExists(path))
                            continue;
                        Menu.AddMenuItem(path, () => {
                            var w = GetWindow(c.Class, false, temporaryObj.TITLE);
                            w.Show();
                        });
                    }
                    DestroyImmediate(temporaryObj);
                }
                catch(Exception e) {
                    Debug.LogException(e);
                }
            }
            isLoading = false;
        }

        #endregion

        #region Variables

        private const string TAG = ColorTable.RichTextTags.CYAN + "[Toolkit.SimpleEditorWindow]</color> - ";

        protected virtual bool ADD_MENU_ITEM => true;
        protected virtual string MENU_ITEM_PATH => "Window/" + this.GetType().FullName
            .Replace(".", "/")
            .Replace("EditorWindow", "")
            .SplitPascalCase();
        protected virtual string TITLE => this.GetType().Name
            .Replace("EditorWindow", "")
            .SplitPascalCase();

        #endregion

        #region Draw

        private void OnGUI() {
            var area = new Rect(Vector2.zero, position.size);
            Draw(area);
            try {
                GUILayout.BeginArea(area);
                DrawLayout();
            }
            finally { GUILayout.EndArea(); }
        }

        protected virtual void Draw(Rect area) { }
        protected virtual void DrawLayout() { }

        #endregion
    }
}
