using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Verify
{
    public static class VerifyEditor
    {
        #region Variables

        #endregion

        #region Properties

        #endregion

        #region Get Objects

        public static IVerify[] GetObjects(string path) => GetObjects(new string[] { path }, null);
        public static IVerify[] GetObjects(VerifyObject preset) => GetObjects(preset.Paths, preset.Blacklist);
        public static IVerify[] GetObjects(IReadOnlyList<string> paths, IReadOnlyList<string> blacklist) {
            var objects = AssetDatabaseUtility.LoadAssetsAsObject<UnityEngine.Object>(paths);
            if(blacklist == null || blacklist.Count == 0)
                return objects
                    .Where(x => x is IVerify)
                    .Select(x => x as IVerify)
                    .Unique()
                    .ToArray();

            return objects.Where(x => {
                if(!(x is IVerify))
                    return false;
                var path = AssetDatabase.GetAssetPath(x);
                return !blacklist.Any(y => path.StartsWith(y));
            })
                .Select(x => x as IVerify)
                .Unique()
                .ToArray();
        }

        public static IEnumerable<IVerify> GetObjectsEnumerator(string path) => GetObjectsEnumerator(new string[] { path }, null);
        public static IEnumerable<IVerify> GetObjectsEnumerator(VerifyObject preset) => GetObjectsEnumerator(preset.Paths, preset.Blacklist);
        public static IEnumerable<IVerify> GetObjectsEnumerator(IReadOnlyList<string> paths, IReadOnlyList<string> blacklist) {
            bool hasBlacklist = blacklist != null && blacklist.Count > 0;
            foreach(var p in paths) {
                var guids = AssetDatabaseUtility.FindAssets<Object>(p).Unique();
                foreach(var guid in guids) {
                    if(hasBlacklist) {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if(!blacklist.Any(y => path.StartsWith(y))) {
                            if(AssetDatabase.AssetPathToGUID(path) != guid)
                                continue;
                            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                            var ver = obj as IVerify;
                            if(ver != null)
                                yield return ver;
                        }
                    }
                    else {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if(AssetDatabase.AssetPathToGUID(path) != guid)
                            continue;
                        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                        var ver = obj as IVerify;
                        if(ver != null)
                            yield return ver;
                    }
                }
            }
        }

        #endregion
    }
}
