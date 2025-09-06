using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public static class HierarchySearchUtility {

        #region Variables

        private static MethodInfo setSearch;
        private static FieldInfo getSearch;

        #endregion

        static HierarchySearchUtility() {
            getSearch = typeof(SearchableEditorWindow).GetField("m_SearchFilter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private static string GetTypeSearch(Type type) {
            var res = type.FullName;
            if(res.StartsWith("UnityEngine"))
                res.SplitAt(res.LastIndexOf('.'), out string dump, out res);
            return $"t:{res}";
        }

        public static void AddSearch<T>() where T : Component {
            AddSearch(typeof(T));
        }

        public static void AddSearch(Type component) {
            var toAdd = GetTypeSearch(component);
            TryGetCurrent(out string current);
            if(string.IsNullOrEmpty(current)) {
                current = toAdd;
            }
            else {
                current += " " + toAdd;
            }
            SetSearch(current);
        }

        public static void SetSearch<T>() where T : Component {
            SetSearch(typeof(T));
        }

        public static void SetSearch(Type type) {
            SetSearch(GetTypeSearch(type));
        }

        public static void SetSearch(string searchQuery) {
            try {

                EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
                var sew = Resources.FindObjectsOfTypeAll<SearchableEditorWindow>();
                SearchableEditorWindow hierarchyWindow = sew.FirstOrDefault(x=>x.titleContent.text == "Hierarchy");
                if(hierarchyWindow != null) {
                    if(setSearch == null) {
                        Debug.Log(hierarchyWindow.GetType().FullName);
                        setSearch = hierarchyWindow.GetType().GetMethod("SetSearchFilter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    }
                    setSearch.Invoke(hierarchyWindow, new object[] { searchQuery, SearchableEditorWindow.SearchMode.All, false, false });
                }
            }
            catch {

            }
        }


        public static bool TryGetCurrent(out string searchQuery) {
            searchQuery = string.Empty;
            try {
                var sew = Resources.FindObjectsOfTypeAll<SearchableEditorWindow>();
                SearchableEditorWindow hierarchyWindow = sew.FirstOrDefault(x=>x.titleContent.text == "Hierarchy");
                if(hierarchyWindow != null) {
                    var value =   getSearch.GetValue(hierarchyWindow);
                    searchQuery = (string)value;
                    return !string.IsNullOrEmpty(searchQuery);
                }
                return false;
            }
            catch {
                return false;
            }
        }
    }
}
