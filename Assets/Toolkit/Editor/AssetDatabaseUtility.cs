using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public static class AssetDatabaseUtility
    {
        #region Variables

        private const string TAG = "<color=aqua>[AssetDatabaseUtility]</color> - ";

        #endregion

        #region Asset Finding

        /// <summary>
        /// Returns a unity search string by type
        /// Example "t:Material" or "t:MyNamespace.CustomDataType"
        /// </summary>
        public static string GetFilterByType<T>() {
            return GetFilterByType(typeof(T));
        }

        /// <summary>
        /// Returns a unity search string by type
        /// Example "t:Material" or "t:MyNamespace.CustomDataType"
        /// </summary>
        public static string GetFilterByType(Type type) {
            if(type.IsInterface) {
                throw new Exception(TAG + "Unable to filter type by interface");
            }
            var filter = type.FullName;
            if(filter.StartsWith("UnityEngine.")) {
                filter = type.Name;
            }
            else if(filter.StartsWith("UnityEditor.")) {
                filter = type.Name;
            }

            return $"t:{filter}";
        }

        private static string Internal_GetFilterByType(Type type) {
            if(type.IsInterface) {
                return "Object";
            }
            var filter = type.FullName;
            if(filter.StartsWith("UnityEngine.")) {
                filter = type.Name;
            }
            else if(filter.StartsWith("UnityEditor.")) {
                filter = type.Name;
            }

            return $"t:{filter}";
        }

        public static string[] FindAssets(Type type) => AssetDatabase.FindAssets(GetFilterByType(type));
        public static string[] FindAssets(Type type, string path) => FindAssets(type, new string[] { path });
        public static string[] FindAssets(Type type, string[] paths) => AssetDatabase.FindAssets(GetFilterByType(type), paths);
        public static string[] FindAssets(Type type, IReadOnlyList<string> paths) => AssetDatabase.FindAssets(GetFilterByType(type), paths.ToArray());
        public static string[] FindAssets(Type type, IEnumerable<string> paths) => AssetDatabase.FindAssets(GetFilterByType(type), paths.ToArray());

        public static string[] FindAssets<T>() => AssetDatabase.FindAssets(GetFilterByType<T>());
        public static string[] FindAssets<T>(string path) => FindAssets<T>(new string[] { path });
        public static string[] FindAssets<T>(string[] paths) => AssetDatabase.FindAssets(GetFilterByType<T>(), paths);
        public static string[] FindAssets<T>(IReadOnlyList<string> paths) => AssetDatabase.FindAssets(GetFilterByType<T>(), paths.ToArray());
        public static string[] FindAssets<T>(IEnumerable<string> paths) => AssetDatabase.FindAssets(GetFilterByType<T>(), paths.ToArray());

        public static string[] FindAssetsByLabel(string label) => AssetDatabase.FindAssets("l:" + label);
        public static string[] FindAssetsByLabel(string label, string path) => AssetDatabase.FindAssets("l:" + label, new string[] { path });
        public static string[] FindAssetsByLabel(string label, string[] paths) => AssetDatabase.FindAssets("l:" + label, paths);
        public static string[] FindAssetsByLabel(string label, IReadOnlyList<string> paths) => AssetDatabase.FindAssets("l:" + label, paths.ToArray());
        public static string[] FindAssetsByLabel(string label, IEnumerable<string> paths) => AssetDatabase.FindAssets("l:" + label, paths.ToArray());

        public static string FindAssetByName(string name) => FindAssetByName(name, new string[] { "Assets/" });
        public static string FindAssetByName(string name, string path) => FindAssetByName(name, new string[] { path });

        public static string FindAssetByName(string name, string[] paths) {
            var result = AssetDatabase.FindAssets($"{name}", paths);
            if(result == null || result.Length == 0) {
                return "";
            }
            return result[0];
        }

        public static string FindAssetByName(string name, IReadOnlyList<string> paths) {
            var result = AssetDatabase.FindAssets($"{name}", paths.ToArray());
            if(result == null || result.Length == 0) {
                return "";
            }
            return result[0];
        }

        public static string FindAssetByName(string name, IEnumerable<string> paths) {
            var result = AssetDatabase.FindAssets($"{name}", paths.ToArray());
            if(result == null || result.Length == 0) {
                return "";
            }
            return result[0];
        }

        public static string[] FindAssetsByName(string name) => FindAssetsByName(name, new string[] { "Assets/" });
        public static string[] FindAssetsByName(string name, string path) => FindAssetsByName(name, new string[] { path });
        public static string[] FindAssetsByName(string name, string[] paths) => AssetDatabase.FindAssets($"{name}", paths);
        public static string[] FindAssetsByName(string name, IReadOnlyList<string> paths) => AssetDatabase.FindAssets($"{name}", paths.ToArray());
        public static string[] FindAssetsByName(string name, IEnumerable<string> paths) => AssetDatabase.FindAssets($"{name}", paths.ToArray());

        #endregion

        #region Asset loading

        public static T[] LoadAssets<T>() where T : UnityEngine.Object => Load<T>(FindAssets<T>());
        public static T[] LoadAssets<T>(string path) where T : UnityEngine.Object => LoadAssets<T>(new string[] { path });
        public static T[] LoadAssets<T>(string[] paths) where T : UnityEngine.Object => Load<T>(FindAssets<T>(paths));
        public static T[] LoadAssets<T>(IReadOnlyList<string> paths) where T : UnityEngine.Object => Load<T>(FindAssets<T>(paths));
        public static T[] LoadAssets<T>(IEnumerable<string> paths) where T : UnityEngine.Object => Load<T>(FindAssets<T>(paths));

        public static UnityEngine.Object[] LoadAssetsAsObject<T>() => LoadAsObject<T>(FindAssets<T>());
        public static UnityEngine.Object[] LoadAssetsAsObject<T>(string path) => LoadAsObject<T>(FindAssets<T>(new string[] { path }));
        public static UnityEngine.Object[] LoadAssetsAsObject<T>(string[] paths) => LoadAsObject<T>(FindAssets<T>(paths));
        public static UnityEngine.Object[] LoadAssetsAsObject<T>(IReadOnlyList<string> paths) => LoadAsObject<T>(FindAssets<T>(paths));
        public static UnityEngine.Object[] LoadAssetsAsObject<T>(IEnumerable<string> paths) => LoadAsObject<T>(FindAssets<T>(paths));

        public static UnityEngine.Object[] LoadAssets(Type type) => Load<UnityEngine.Object>(FindAssets(type));
        public static UnityEngine.Object[] LoadAssets(Type type, string path) => Load<UnityEngine.Object>(FindAssets(type, new string[] { path }));
        public static UnityEngine.Object[] LoadAssets(Type type, string[] paths) => Load<UnityEngine.Object>(FindAssets(type, paths));
        public static UnityEngine.Object[] LoadAssets(Type type, IReadOnlyList<string> paths) => Load<UnityEngine.Object>(FindAssets(type, paths));
        public static UnityEngine.Object[] LoadAssets(Type type, IEnumerable<string> paths) => Load<UnityEngine.Object>(FindAssets(type, paths));

        public static UnityEngine.Object[] LoadAssetsByLabel(string label) => Load<UnityEngine.Object>(FindAssetsByLabel(label));
        public static UnityEngine.Object[] LoadAssetsByLabel(string label, string path) => Load<UnityEngine.Object>(FindAssetsByLabel(label, new string[] { path }));
        public static UnityEngine.Object[] LoadAssetsByLabel(string label, string[] paths) => Load<UnityEngine.Object>(FindAssetsByLabel(label, paths));
        public static UnityEngine.Object[] LoadAssetsByLabel(string label, IReadOnlyList<string> paths) => Load<UnityEngine.Object>(FindAssetsByLabel(label, paths));
        public static UnityEngine.Object[] LoadAssetsByLabel(string label, IEnumerable<string> paths) => Load<UnityEngine.Object>(FindAssetsByLabel(label, paths));

        public static T LoadAssetByName<T>(string name) where T : UnityEngine.Object => LoadAssetByName<T>(name, new string[] { "Assets/" });
        public static T LoadAssetByName<T>(string name, string path) where T : UnityEngine.Object => LoadAssetByName<T>(name, new string[] { path });
        public static T LoadAssetByName<T>(string name, string[] paths) where T : UnityEngine.Object {
            var result = FindAssetByName(name, paths);
            if(string.IsNullOrEmpty(result)) {
                return default;
            }
            return Load<T>(result);
        }
        public static T LoadAssetByName<T>(string name, IReadOnlyList<string> paths) where T : UnityEngine.Object {
            var result = FindAssetByName(name, paths);
            if(string.IsNullOrEmpty(result)) {
                return default;
            }
            return Load<T>(result);
        }
        public static T LoadAssetByName<T>(string name, IEnumerable<string> paths) where T : UnityEngine.Object {
            var result = FindAssetByName(name, paths);
            if(string.IsNullOrEmpty(result)) {
                return default;
            }
            return Load<T>(result);
        }

        public static T[] LoadAssetsByName<T>(string name) where T : UnityEngine.Object => LoadAssetsByName<T>(name, new string[] { "Assets/" });
        public static T[] LoadAssetsByName<T>(string name, string path) where T : UnityEngine.Object => LoadAssetsByName<T>(name, new string[] { path });
        public static T[] LoadAssetsByName<T>(string name, string[] paths) where T : UnityEngine.Object => Load<T>(FindAssetsByName(name, paths));
        public static T[] LoadAssetsByName<T>(string name, IReadOnlyList<string> paths) where T : UnityEngine.Object => Load<T>(FindAssetsByName(name, paths));
        public static T[] LoadAssetsByName<T>(string name, IEnumerable<string> paths) where T : UnityEngine.Object => Load<T>(FindAssetsByName(name, paths));


        public static UnityEngine.Object LoadAssetByNameAsObject<T>(string name) => LoadAssetByNameAsObject<T>(name, new string[] { "Assets/" });
        public static UnityEngine.Object LoadAssetByNameAsObject<T>(string name, string path) => LoadAssetByNameAsObject<T>(name, new string[] { path });
        public static UnityEngine.Object LoadAssetByNameAsObject<T>(string name, string[] paths) {
            var result = FindAssetByName(name, paths);
            if(string.IsNullOrEmpty(result)) {
                return default;
            }
            return LoadAsObject<T>(result);
        }
        public static UnityEngine.Object LoadAssetByNameAsObject<T>(string name, IReadOnlyList<string> paths) {
            var result = FindAssetByName(name, paths);
            if(string.IsNullOrEmpty(result)) {
                return default;
            }
            return LoadAsObject<T>(result);
        }
        public static UnityEngine.Object LoadAssetByNameAsObject<T>(string name, IEnumerable<string> paths) {
            var result = FindAssetByName(name, paths);
            if(string.IsNullOrEmpty(result)) {
                return default;
            }
            return LoadAsObject<T>(result);
        }

        public static UnityEngine.Object[] LoadAssetsByNameAsObject<T>(string name) => LoadAssetsByNameAsObject<T>(name, new string[] { "Assets/" });
        public static UnityEngine.Object[] LoadAssetsByNameAsObject<T>(string name, string path) => LoadAssetsByNameAsObject<T>(name, new string[] { path });
        public static UnityEngine.Object[] LoadAssetsByNameAsObject<T>(string name, string[] paths) => LoadAsObject<T>(FindAssetsByName(name, paths));
        public static UnityEngine.Object[] LoadAssetsByNameAsObject<T>(string name, IReadOnlyList<string> paths) => LoadAsObject<T>(FindAssetsByName(name, paths));
        public static UnityEngine.Object[] LoadAssetsByNameAsObject<T>(string name, IEnumerable<string> paths) => LoadAsObject<T>(FindAssetsByName(name, paths));



        #endregion

        #region Load Utility

        public static T Load<T>(string guid) where T : UnityEngine.Object {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if(string.IsNullOrEmpty(path)) {
                Debug.LogError(TAG + $"File path not found on file with GUID: {guid}");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static UnityEngine.Object LoadAsObject<T>(string guid) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if(string.IsNullOrEmpty(path)) {
                Debug.LogError(TAG + $"File path not found on file with GUID: {guid}");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath(path, typeof(T));
        }

        public static T[] Load<T>(string[] guids) where T : UnityEngine.Object {
            var length = guids.Length;
            T[] files = new T[length];
            for(int i = 0; i < length; i++) {
                files[i] = Load<T>(guids[i]);
            }
            return files;
        }

        public static UnityEngine.Object[] LoadAsObject<T>(string[] guids) {
            var length = guids.Length;
            UnityEngine.Object[] files = new UnityEngine.Object[length];
            for(int i = 0; i < length; i++) {
                files[i] = LoadAsObject<T>(guids[i]);
            }
            return files;
        }

        #endregion

        #region Path creation

        public static bool CreatePath(string path) {
            if(AssetDatabase.IsValidFolder(path)) {
                return true;
            }

            var p = System.IO.Path.HasExtension(path) ? System.IO.Path.GetDirectoryName(path) : path;
            var subFolders = p.Split('\\', '/');
            var length = subFolders.Length;
            if(length == 0)
                return false;
            string currentFolder = subFolders[0];
            for(int i = 1; i < length; i++) {
                var nextFolder = currentFolder + "/" + subFolders[i];
                if(!AssetDatabase.IsValidFolder(nextFolder)) {
                    AssetDatabase.CreateFolder(currentFolder, subFolders[i]);
                }
                currentFolder = nextFolder;
            }

            return AssetDatabase.IsValidFolder(currentFolder);
        }

        #endregion

        #region Classes

        public static IEnumerable<Type> GetAllTypes() {
            var unityAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies()
                .Select(x => x.name);
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => unityAssemblies.Contains(x.GetName().Name))
                .SelectMany(x => x.GetTypes());
            return assemblies;
        }

        public static IEnumerable<Type> GetAllTypes(UnityEditor.Compilation.AssembliesType type) {
            try {
                var unityAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(type)
                    .Select(x => x.name);
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => unityAssemblies.Contains(x.GetName().Name))
                    .SelectMany(x => x.GetTypes());
                return assemblies;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion
    }
}
