using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public class BundleMapObject
    {
        #region Variables

        public const string TAG = "<color=cyan>[Bundle Map]</color> - ";

        private Object reference = null;
        private List<string> files = new List<string>();
        private string root = "";

        #endregion

        #region Properties

        public string Root {
            get => root;
            set {
                if(AssetDatabase.IsValidFolder(value) || string.IsNullOrEmpty(value))
                    root = value;
            }
        }
        public string Path => reference == null ? "" : AssetDatabase.GetAssetPath(reference);
        public IReadOnlyList<string> Files => files;

        #endregion

        #region Constructor

        public BundleMapObject(string path) {
            this.reference = AssetDatabase.LoadAssetAtPath<Object>(path);
            Load();
        }

        public BundleMapObject(Object reference) {
            this.reference = reference;
            Load();
        }

        #endregion

        #region Update

        public void Update() {
            files.Clear();
            files.AddRange(AssetDatabase.FindAssets("", new string[] { root }).Select(x => AssetDatabase.GUIDToAssetPath(x)));
        }

        #endregion

        #region Add

        public void Add(string path) {
            if(!files.Contains(path)) {
                files.Add(path);
            }
        }

        public void Add(IReadOnlyList<string> paths) {
            foreach(var p in paths)
                Add(p);
        }

        public void Add(IEnumerable<string> paths) {
            foreach(var p in paths)
                Add(p);
        }

        #endregion

        #region Remove

        public void Remove(string path) {
            files.Remove(path);
        }

        public void Remove(IReadOnlyList<string> paths) {
            foreach(var p in paths)
                Remove(p);
        }

        public void Remove(IEnumerable<string> paths) {
            foreach(var p in paths)
                Remove(p);
        }

        #endregion

        #region Save / Load To file

        public void Save() {
            files.Insert(0, root);
            System.IO.File.WriteAllLines(Path, files);
            files.RemoveAt(0);
        }

        public void Load() {
            if(reference == null || !System.IO.File.Exists(Path)) {
                Debug.LogError(TAG + "reference is null");
                files.Clear();
                return;
            }
            if(System.IO.Path.GetExtension(Path) != ".bundlemap") {
                Debug.LogError(TAG + "reference is not of .bundlemap type");
                files.Clear();
                return;
            }

            files.Clear();
            files.AddRange(System.IO.File.ReadAllLines(Path));
            if(files.Count > 0) {
                root = files[0];
                files.RemoveAt(0);
            }
        }

        #endregion
    }
}
