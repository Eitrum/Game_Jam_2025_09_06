using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.SceneManagement
{
    [CustomPropertyDrawer(typeof(SceneObject))]
    public class SceneObjectPropertyDrawer : PropertyDrawer
    {
        string[] names;
        string[] paths;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(names == null) {
                paths = AssetDatabase.FindAssets("t:SceneAsset")
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .Insert(0, "[NONE]")
                    .ToArray();
                names = paths
                    .Select(x => System.IO.Path.GetFileNameWithoutExtension(x))
                    .ToArray();
            }

            var nameProp = property.FindPropertyRelative("name");
            var pathProp = property.FindPropertyRelative("path");

            int index = 0;
            for(int i = 0; i < paths.Length; i++) {
                if(pathProp.stringValue == paths[i]) {
                    index = i;
                    break;
                }
            }
            if(index == 0) {
                for(int i = 0; i < names.Length; i++) {
                    if(nameProp.stringValue == names[i]) {
                        index = i;
                        break;
                    }
                }
            }

            index = EditorGUI.Popup(position, label.text, index, names);
            nameProp.stringValue = index > 0 ? names[index] : "";
            pathProp.stringValue = index > 0 ? paths[index] : "";
        }
    }

    [CustomPropertyDrawer(typeof(SceneObjectAttribute))]
    public class SceneObjectAttributePropertyDrawer : PropertyDrawer
    {
        string[] names;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType == SerializedPropertyType.String) {
                if(names == null) {
                    names = AssetDatabase.FindAssets("t:SceneAsset")
                        .Select(x => AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(x)).name)
                        .Insert(0, "[NONE]")
                        .ToArray();
                }
                int index = 0;
                for(int i = 0; i < names.Length; i++) {
                    if(property.stringValue == names[i]) {
                        index = i;
                        break;
                    }
                }

                index = EditorGUI.Popup(position, label.text, index, names);
                property.stringValue = index > 0 ? names[index] : "";
            }
            else {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
