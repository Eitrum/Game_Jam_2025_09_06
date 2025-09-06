using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public class DatabaseReferencesEditor {


        private static void VerifyDatabaseReferences() {
            var instance = Resources.Load<ScriptableObject>(Database.Types.DATABASE_REFERENCES.Name);
            if(!instance) { // Check again if instance actually existed
                            // Create new object
                instance = ScriptableObject.CreateInstance(Database.Types.DATABASE_REFERENCES);
                var monoScript = UnityEditor.MonoScript.FromScriptableObject(instance);
                var scriptPath = UnityEditor.AssetDatabase.GetAssetPath(monoScript);
                var resourcePath = scriptPath.Replace(monoScript.name + ".cs", "Resources");
                var filePath = resourcePath + $"/{Database.Types.DATABASE_REFERENCES.Name}.asset";
                if(!UnityEditor.AssetDatabase.IsValidFolder(resourcePath)) {
                    UnityEditor.AssetDatabase.CreateFolder(resourcePath.Replace("/Resources", ""), "Resources");
                }
                UnityEditor.AssetDatabase.CreateAsset(instance, filePath);
                UnityEditor.AssetDatabase.ImportAsset(filePath);
            }
            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(instance);
            var sub = so.FindProperty("subDatabases");
            sub.ClearArray();
            var assets = AssetDatabaseUtility.LoadAssets<SubDatabase>();
            for(int i = 0; i < assets.Length; i++) {
                var db = assets[i];
                sub.InsertArrayElementAtIndex(i);
                sub.GetArrayElementAtIndex(i).objectReferenceValue = db;
            }
            if(so.hasModifiedProperties) {
                so.ApplyModifiedProperties();
            }
        }

    }
}
