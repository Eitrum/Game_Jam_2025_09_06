using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    [DefaultAssetEditor(".bundlemap")]
    public class BundleMapEditor : Editor
    {
        private BundleMapObject bundleMap;
        private List<string> added = new List<string>();
        private List<string> removed = new List<string>();
        private Vector2 scroll = Vector2.zero;

        void OnEnable() {
            if(target == null)
                return;
            bundleMap = new BundleMapObject(target);
            var files = bundleMap.Files;
            var currentFiles = AssetDatabase.FindAssets("", new string[] { bundleMap.Root }).Select(x => AssetDatabase.GUIDToAssetPath(x));

            added.Clear();
            removed.Clear();

            foreach(var f in files) {
                if(!currentFiles.Contains(f)) {
                    removed.Add(f);
                }
            }
            foreach(var f in currentFiles) {
                if(!files.Contains(f)) {
                    added.Add(f);
                }
            }
        }

        public override void OnInspectorGUI() {

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Bundle Map Object", EditorStyles.boldLabel);
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUILayout.LabelField("Root", EditorStyles.boldLabel);
            using(new EditorGUI.IndentLevelScope()) {
                var rootObject = AssetDatabase.LoadAssetAtPath<Object>(bundleMap.Root);
                var newRoot = EditorGUILayout.ObjectField(bundleMap.Root, rootObject, typeof(DefaultAsset), false);
                if(newRoot != rootObject) {
                    bundleMap.Root = AssetDatabase.GetAssetPath(newRoot);
                    bundleMap.Update();
                    bundleMap.Save();
                }
            }

            GUILayout.Space(8f);
            var line = GUILayoutUtility.GetRect(10f, 2f);
            EditorGUI.DrawRect(line, Color.green);
            using(new EditorGUILayout.HorizontalScope("box")) {
                EditorGUILayout.LabelField("Local Extra Files", EditorStyles.boldLabel);
                if(GUILayout.Button("Append All", GUILayout.Width(80f))) {
                    bundleMap.Add(added);
                    bundleMap.Save();
                    added.Clear();
                }
                if(GUILayout.Button("Remove All", GUILayout.Width(80f))) {
#if UNITY_2020_OR_NEWER
                    AssetDatabase.DeleteAssets(added.ToArray(), added);
#endif
                }
            }
            using(new EditorGUI.IndentLevelScope()) {
                for(int i = 0; i < added.Count; i++) {
                    var add = added[i];
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField($"+ {add}");
                        if(GUILayout.Button("Append", GUILayout.Width(80f))) {
                            bundleMap.Add(add);
                            bundleMap.Save();
                            added.RemoveAt(i);
                            i--;
                        }
                        if(GUILayout.Button("Remove", GUILayout.Width(80f))) {
                            AssetDatabase.DeleteAsset(add);
                            added.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            GUILayout.Space(8f);
            var line2 = GUILayoutUtility.GetRect(10f, 2f);
            EditorGUI.DrawRect(line2, Color.red);

            using(new EditorGUILayout.HorizontalScope("box")) {
                EditorGUILayout.LabelField("Missing Files", EditorStyles.boldLabel);
                if(GUILayout.Button("Clear", GUILayout.Width(80f))) {
                    bundleMap.Remove(removed);
                    bundleMap.Save();
                    removed.Clear();
                }
            }

            using(new EditorGUI.IndentLevelScope()) {
                foreach(var rem in removed) {
                    EditorGUILayout.LabelField($"- {rem}");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        public static void UpdateAllBundleMapsIn(string path) {
            var bundles = GetAllBundleMaps();
            foreach(var bundle in bundles) {
                if(bundle.Root.StartsWith(path)) {
                    bundle.Update();
                    bundle.Save();
                }
            }
        }

        public static void UpdateAllBundleMaps() {
            var bundles = GetAllBundleMaps();
            foreach(var bundle in bundles) {
                bundle.Update();
                bundle.Save();
            }
        }

        public static IEnumerable<BundleMapObject> GetAllBundleMaps() {
            return AssetDatabase.FindAssets("t:DefaultAsset").Select(x => AssetDatabase.GUIDToAssetPath(x)).Where(x => x.EndsWith(".bundlemap")).Select(x => new BundleMapObject(x));
        }
    }
}
