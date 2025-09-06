using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public static class LabelTypeBindings {

        [System.Serializable]
        private class Storage { public List<LabelTypeBinding> bindings = new List<LabelTypeBinding>(); }

        #region Variables

        private const string FOLDER = "ProjectSettings/Labels";
        private const string FILE = "ProjectSettings/Labels/labeltypebindings";
        private const string TAG = "[Toolkit.LabelTypeBindings] - ";
        private static List<LabelTypeBinding> bindings = new List<LabelTypeBinding>();
        private static Dictionary<int, LabelTypeBinding> idToBinding = new Dictionary<int, LabelTypeBinding>();
        private static bool isDirty = false;
        public static string[] Labels;

        #endregion

        #region Properties

        public static IReadOnlyList<LabelTypeBinding> Bindings => bindings;
        public static IReadOnlyDictionary<int, LabelTypeBinding> IdToBindings => idToBinding;

        #endregion

        #region Init

        static LabelTypeBindings() {
            if(System.IO.File.Exists(FILE)) {
                var json = System.IO.File.ReadAllText(FILE);
                var storage = new Storage();
                EditorJsonUtility.FromJsonOverwrite(json, storage);
                bindings = storage.bindings;
            }

            foreach(var binding in bindings)
                idToBinding.Add(binding.TypeId, binding);

            Add<UnityEngine.Texture2D>();
            Add<UnityEngine.AudioClip>();
            Add<UnityEngine.Mesh>();
            Add<UnityEngine.Material>();
            Add<UnityEngine.AnimationClip>();
            Add<UnityEngine.Texture>();
            Add<UnityEngine.GameObject>();

            Labels = Bindings.Select(x => x.TypeName).ToArray();
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
            Labels = Bindings.Select(x => x.TypeName).ToArray();
            isDirty = false;
            try {
                var storage = new Storage(){ bindings = bindings };
                if(!System.IO.Directory.Exists(FOLDER))
                    System.IO.Directory.CreateDirectory(FOLDER);
                var json = EditorJsonUtility.ToJson(storage, true);
                System.IO.File.WriteAllText(FILE, json);
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Add

        public static void Add<T>() {
            var ltb = new LabelTypeBinding(typeof(T));
            if(idToBinding.ContainsKey(ltb.TypeId))
                return;
            bindings.Add(ltb);
            idToBinding.Add(ltb.TypeId, ltb);
            Save();
        }

        public static void Add(System.Type type) {
            var ltb = new LabelTypeBinding(type);
            if(idToBinding.ContainsKey(ltb.TypeId))
                return;
            bindings.Add(ltb);
            idToBinding.Add(ltb.TypeId, ltb);
            Save();
        }

        #endregion

        #region Remove

        public static void Remove<T>() {
            var id = typeof(T).FullName.GetHash32();
            if(idToBinding.TryGetValue(id, out var ltb)) {
                bindings.Remove(ltb);
                idToBinding.Remove(id);
                Save();
            }
        }

        public static void Remove(LabelTypeBinding binding) {
            bindings.Remove(binding);
            idToBinding.Remove(binding.TypeId);
            Save();
        }

        #endregion

        #region Util

        public static bool Has<T>() {
            var id = typeof(T).FullName.GetHash32();
            return idToBinding.ContainsKey(id);
        }

        #endregion

        #region Get

        public static bool TryGet(int typeId, out LabelTypeBinding binding) {
            return idToBinding.TryGetValue(typeId, out binding);
        }

        public static bool TryGet<T>(out LabelTypeBinding binding) {
            var id = typeof(T).FullName.GetHash32();
            return idToBinding.TryGetValue(id, out binding);
        }

        public static bool TryGet(System.Type type, out LabelTypeBinding binding) {
            var id = type.FullName.GetHash32();
            return idToBinding.TryGetValue(id, out binding);
        }

        #endregion

        #region Drawing

        public static void DrawDropdownLayout(ref int typeId) {
            var selected = -1;
            for(int i = 0, length = bindings.Count; i < length; i++) {
                if(bindings[i].TypeId == typeId) {
                    selected = i;
                    break;
                }
            }
            if(selected == -1) {
                selected = 0;
                GUI.changed = true;
            }
            typeId = bindings[EditorGUILayout.Popup(selected, Labels)].TypeId;
        }

        public static int DrawDropdownLayout(int typeId) {
            DrawDropdownLayout(ref typeId);
            return typeId;
        }

        public static void DrawDropdown(Rect area, ref int typeId) {
            var selected = -1;
            for(int i = 0, length = bindings.Count; i < length; i++) {
                if(bindings[i].TypeId == typeId) {
                    selected = i;
                    break;
                }
            }
            if(selected == -1) {
                selected = 0;
                GUI.changed = true;
            }
            typeId = bindings[EditorGUI.Popup(area, selected, Labels)].TypeId;
        }

        public static int DrawDropdown(Rect area, int typeId) {
            DrawDropdown(area, ref typeId);
            return typeId;
        }

        public static void DrawLayout(LabelTypeBinding binding) {
            try {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("x", GUILayout.Width(16))) {
                    EditorApplication.delayCall += () => { Remove(binding); ToolkitEditorUtility.RepaintInspectors(); };
                }
                GUILayout.Label(binding.TypeName);
                GUILayout.Space(5);
                GUILayout.Label(binding.TypeId.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.Label(binding.Assembly);
            }
            finally {
                GUILayout.EndHorizontal();
            }
        }

        public static void Draw(Rect area, LabelTypeBinding binding) {
            area.SplitHorizontal(out Rect destroyButton, out area, 16f / area.width);
            if(GUI.Button(destroyButton, "x")) {
                EditorApplication.delayCall += () => { Remove(binding); ToolkitEditorUtility.RepaintInspectors(); };
            }
            area.SplitHorizontal(out Rect left, out Rect right, 0.6f);
            GUI.Label(left, $"{binding.TypeName}\t\t  {binding.TypeId}");
            GUI.Label(right, binding.Assembly, EditorStylesUtility.RightAlignedLabel);
        }

        #endregion
    }
}
