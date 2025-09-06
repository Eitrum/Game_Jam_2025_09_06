using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    public static class CustomizableSettingMissingFileCreator
    {
        [InitializeOnLoadMethod]
        private static void Initialize() {
            /*var settings = MonoImporter.GetAllRuntimeMonoScripts()
                .Where(x => x?.GetClass()?.BaseType?.Name?.StartsWith("CustomizableSetting") ?? false)
                .Where(x => !x.GetClass().IsSubclassOf(typeof(Editor)));

            settings.Foreach(x => {
                var assets = AssetDatabaseUtility.FindAssets(x.GetClass());
                if(assets.Length == 0) {
                    var path = AssetDatabase.GetAssetPath(x);
                    var obj = ScriptableObject.CreateInstance(x.GetClass());
                    path = path.Replace(".cs", ".asset");
                    AssetDatabase.CreateAsset(obj, path);
                }
            });*/
        }
    }
}
