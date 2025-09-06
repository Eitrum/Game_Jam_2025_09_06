using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit {
    [DefaultAssetEditor(".rsp")]
    public class RSPEditor : Editor {
        private List<string> lines = new List<string>();
        private UnityEditorInternal.ReorderableList reorderableList;

        private void Awake() {
            reorderableList = new UnityEditorInternal.ReorderableList(lines, typeof(string), true, true, true, true);
            reorderableList.drawHeaderCallback += OnDrawHeader;
            reorderableList.drawElementCallback += OnDrawElement;
            reorderableList.onAddCallback += OnAddElement;
        }

        private void OnAddElement(ReorderableList list) {
            lines.Add("");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect lineArea, 20f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString());
            lines[index] = EditorGUI.TextField(lineArea, lines[index]);
        }

        private void OnDrawHeader(Rect rect) {
            rect.width -= 16;
            rect.x += 16;
            rect.SplitHorizontal(out Rect indexArea, out Rect lineArea, 20f / rect.width);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(lineArea, "Entry", EditorStyles.boldLabel);
        }

        private void OnEnable() {
            if(target == null)
                return;
            var path = AssetDatabase.GetAssetPath(target);
            if(!System.IO.File.Exists(path))
                return;

            lines.Clear();
            lines.AddRange(System.IO.File.ReadAllLines(path));
        }

        private void Save() {
            var path = AssetDatabase.GetAssetPath(target);
            System.IO.File.WriteAllLines(path, lines);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public override void OnInspectorGUI() {
            reorderableList.DoLayoutList();
            if(GUILayout.Button("Save", GUILayout.Width(80f))) {
                Save();
            }
        }
    }
}
