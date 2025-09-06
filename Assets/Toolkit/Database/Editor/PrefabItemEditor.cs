using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Toolkit {
    public static class PrefabItemEditor {

        [MenuItem("Assets/Create/" + Database.MENU_PATH + PrefabItem.FILE_NAME)]
        public static void CreatePrefabItem() {
            var objs = Selection.objects;

            UnityEngine.Object[] newSelection = new UnityEngine.Object[objs.Length];

            for(int i = 0; i < objs.Length; i++) {
                var obj = objs[i];
                if(ProjectWindowUtil.IsFolder(obj.GetInstanceID())) {
                    ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PrefabItem>(), AssetDatabase.GetAssetPath(obj) + "/" + PrefabItem.FILE_NAME + ".asset");
                    return;
                }
                if(obj is GameObject go) {
                    var asset = ScriptableObject.CreateInstance<PrefabItem>();
                    if(char.IsNumber(go.name[0])) {
                        var split = go.name.Split('_');
                        if(split.Length >= 2 && int.TryParse(split[0], out int result)) {
                            asset.order = result;
                            asset.ItemName = string.Join("_", split, 1, split.Length - 1);
                        }
                    }
                    asset.name = go.name;
                    asset.reference = go;
                    AssetDatabase.CreateAsset(asset, AssetDatabase.GetAssetPath(go).Replace(".prefab", ".asset"));
                    newSelection[i] = (asset);
                }
            }
            Selection.objects = newSelection;
        }

    }
}
