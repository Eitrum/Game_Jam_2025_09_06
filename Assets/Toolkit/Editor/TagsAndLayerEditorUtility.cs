using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public static class TagsAndLayerEditorUtility
    {
        private static SerializedObject tagManager;

        public static SerializedObject TagManager {
            get {
                if(tagManager == null) {
                    tagManager = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/TagManager.asset"));
                }
                return tagManager;
            }
        }

        [MenuItem("Toolkit/Editor/Fill Unused Layers")]
        public static void FillLayers() {
            var tm = TagManager;
            tm.Update();
            var l = tm.FindProperty("layers");
            if(l.arraySize < 32)
                l.arraySize = 32;

            for(int i = 0; i < 32; i++) {
                var name = LayerMask.LayerToName(i);
                if(string.IsNullOrEmpty(name)) {
                    l.GetArrayElementAtIndex(i).stringValue = $"Unused/{i}";
                }
            }
            if(tm.hasModifiedProperties)
                tm.ApplyModifiedProperties();
        }
    }
}
