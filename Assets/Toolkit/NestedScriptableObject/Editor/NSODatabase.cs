using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public static class NSODatabase {
        public struct Data {
            public string Name;
            public System.Type Type;
            public int Priority;
            public MonoScript Script;
        }

        #region Variables

        private static Dictionary<System.Type, List<Data>> scriptableObjectLookup = new Dictionary<System.Type, List<Data>>();
        private static Dictionary<System.Type, string[]> popupLookup = new Dictionary<System.Type, string[]>();
        private static string[] empty = new string[] { "none" };
        private static bool isInitialized = false;

        #endregion

        #region Initialize / Reload

        public static void Initialize() {
            if(isInitialized)
                return;
            isInitialized = true;
            Reload();
        }

        public static void Reload() {
            scriptableObjectLookup.Clear();
            popupLookup.Clear();
            var fact = typeof(NSOFileAttribute);
            var scriptable = typeof(ScriptableObject);
            var factoryBlocks = MonoImporter.GetAllRuntimeMonoScripts().Where(x => (x.GetClass()?.GetCustomAttributes(fact, false)?.Length > 0));
            foreach(var b in factoryBlocks) {
                var attributes = b.GetClass().GetCustomAttributes(fact, true);
                foreach(var att in attributes) {
                    if(att is NSOFileAttribute t && b.GetClass().IsSubclassOf(scriptable)) {
                        var data = new Data() {
                            Name = string.IsNullOrEmpty(t.Name) ? b.GetClass().FullName.Replace('.', '/') : t.Name,
                            Type = b.GetClass(),
                            Priority = t.Priority,
                            Script = b,
                        };
                        if(scriptableObjectLookup.TryGetValue(t.Type, out List<Data> types))
                            types.Add(data);
                        else
                            scriptableObjectLookup.Add(t.Type, new List<Data>() { data });
                    }
                }
            }
            foreach(var list in scriptableObjectLookup) {
                Sort.Merge(list.Value, (a, b) => -a.Priority.CompareTo(b.Priority));
                popupLookup.Add(list.Key, list.Value.Select(x => x.Name).Insert(0, "none").ToArray());
            }
        }

        #endregion

        #region Find

        public static IReadOnlyList<Data> Find<T>() => Find(typeof(T));

        public static IReadOnlyList<Data> Find(System.Type type) {
            Initialize();
            if(type == null)
                return null;
            return scriptableObjectLookup.TryGetValue(type, out List<Data> list) ? list : null;
        }

        public static string[] GetPopupList(Type t) {
            Initialize();
            if(t == null)
                return empty;
            return popupLookup.TryGetValue(t, out string[] values) ? values : empty;
        }

        #endregion

        #region Draw Popup

        public static bool DrawLayoutPopup<T>(out Data data) {
            return DrawLayoutPopup(typeof(T), out data);
        }

        public static bool DrawLayoutPopup(Type type, out Data data) {
            Initialize();
            int index = 0;
            if(popupLookup.TryGetValue(type, out string[] labels)) {
                index = EditorGUILayout.Popup(index, labels);
                if(index > 0) {
                    scriptableObjectLookup.TryGetValue(type, out List<Data> list);
                    data = list[index - 1];
                    return true;
                }
            }
            else {
                EditorGUILayout.Popup(index, new string[1] { "missing" });
            }
            data = default;
            return false;
        }

        public static bool DrawLayoutPopup<T>(string label, out Data data) {
            return DrawLayoutPopup(label, typeof(T), out data);
        }

        public static bool DrawLayoutPopup(string label, Type type, out Data data) {
            Initialize();
            int index = 0;
            if(popupLookup.TryGetValue(type, out string[] labels)) {
                index = EditorGUILayout.Popup(label, index, labels);
                if(index > 0) {
                    scriptableObjectLookup.TryGetValue(type, out List<Data> list);
                    data = list[index - 1];
                    return true;
                }
            }
            else {
                EditorGUILayout.Popup(label, index, new string[1] { "missing" });
            }
            data = default;
            return false;
        }

        #endregion
    }
}
