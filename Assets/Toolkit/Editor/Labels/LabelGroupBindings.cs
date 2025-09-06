using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public static class LabelGroupBindings {

        [System.Serializable]
        private class Storage { public List<string> bindings = new List<string>(); }

        #region Variables

        private const string FOLDER = "ProjectSettings/Labels";
        private const string FILE = "ProjectSettings/Labels/labelgroupbindings";
        private const string TAG = "[Toolkit.LabelGroupBindings] - ";
        [SerializeField] private static List<string> bindings = new List<string>();
        private static Dictionary<int, string> idToBinding = new Dictionary<int, string>();
        private static bool isDirty = false;
        public static string[] Labels;

        #endregion

        #region Properties

        public static IReadOnlyList<string> Bindings => bindings;
        public static IReadOnlyDictionary<int, string> IdToBindings => idToBinding;

        #endregion

        #region Init

        static LabelGroupBindings() {
            if(System.IO.File.Exists(FILE)) {
                var json = System.IO.File.ReadAllText(FILE);
                var storage = new Storage();
                EditorJsonUtility.FromJsonOverwrite(json, storage);
                bindings = storage.bindings;
            }

            foreach(var binding in bindings)
                idToBinding.Add(binding.GetHash32(), binding);

            Add("Type");
            Add("Category");
            Add("Descriptor");
            Add("Material");
            Add("Environment");
            Add("Action");
            Add("Unit");
            Add("Weather");
            Add("File Descriptor");

            Labels = Bindings.ToArray();
        }

        #endregion

        #region Save

        public static void Save() {
            if(isDirty)
                return;
            isDirty = true;
            EditorApplication.delayCall += Internal_Save;
        }

        private static void Internal_Save() {
            Labels = Bindings.ToArray();
            isDirty = false;
            try {
                if(!System.IO.Directory.Exists(FOLDER))
                    System.IO.Directory.CreateDirectory(FOLDER);
                var storage = new Storage();
                storage.bindings = bindings;
                var json = EditorJsonUtility.ToJson(storage, true);
                System.IO.File.WriteAllText(FILE, json);
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Add

        public static void Add(string group) {
            var id = group.GetHash32();
            if(idToBinding.ContainsKey(id))
                return;
            bindings.Add(group);
            idToBinding.Add(id, group);
            Save();
        }

        #endregion

        #region Remove

        public static void Remove(string group) {
            var id = group.GetHash32();
            bindings.Remove(group);
            idToBinding.Remove(id);
            Save();
        }

        public static void Remove(int id) {
            if(idToBinding.TryGetValue(id, out var ltb)) {
                bindings.Remove(ltb);
                idToBinding.Remove(id);
                Save();
            }
        }

        #endregion

        #region Util

        public static int GetId(string group) {
            return group.GetHash32();
        }

        public static bool Has(string group) {
            var id = group.GetHash32();
            return idToBinding.ContainsKey(id);
        }

        public static bool Has(int groupid) {
            return idToBinding.ContainsKey(groupid);
        }

        #endregion

        #region Get

        public static bool TryGet(int typeId, out string binding) {
            return idToBinding.TryGetValue(typeId, out binding);
        }

        #endregion

        #region Draw

        public static void DrawDropdownLayout(ref int typeId) {
            var selected = -1;
            for(int i = 0, length = bindings.Count; i < length; i++) {
                if(bindings[i].GetHash32() == typeId) {
                    selected = i;
                    break;
                }
            }
            if(selected == -1) {
                selected = 0;
                GUI.changed = true;
            }
            typeId = bindings[EditorGUILayout.Popup(selected, Labels)].GetHash32();
        }

        public static int DrawDropdownLayout(int typeId) {
            DrawDropdownLayout(ref typeId);
            return typeId;
        }

        public static void DrawDropdown(Rect area, ref int typeId) {
            var selected = -1;
            for(int i = 0, length = bindings.Count; i < length; i++) {
                if(bindings[i].GetHash32() == typeId) {
                    selected = i;
                    break;
                }
            }
            if(selected == -1) {
                selected = 0;
                GUI.changed = true;
            }
            typeId = bindings[EditorGUI.Popup(area, selected, Labels)].GetHash32();
        }

        public static int DrawDropdown(Rect area, int typeId) {
            DrawDropdown(area, ref typeId);
            return typeId;
        }

        public static void DrawLayout(string binding) {
            try {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("x", GUILayout.Width(16))) {
                    EditorApplication.delayCall += () => { Remove(binding); ToolkitEditorUtility.RepaintInspectors(); };
                }
                GUILayout.Label(binding);
                GUILayout.FlexibleSpace();
                GUILayout.Label(binding.GetHash32().ToString());
            }
            finally {
                GUILayout.EndHorizontal();
            }
        }

        public static void Draw(Rect area, string binding) {
            area.SplitHorizontal(out Rect destroyButton, out area, 16f / area.width);
            if(GUI.Button(destroyButton, "x")) {
                EditorApplication.delayCall += () => { Remove(binding); ToolkitEditorUtility.RepaintInspectors(); };
            }
            area.SplitHorizontal(out Rect left, out Rect right, 0.6f);
            GUI.Label(left, binding);
            GUI.Label(right, binding.GetHash32().ToString(), EditorStylesUtility.RightAlignedLabel);
        }

        #endregion
    }
}
