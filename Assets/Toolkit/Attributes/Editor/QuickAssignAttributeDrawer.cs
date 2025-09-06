using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(QuickAssignAttribute))]
    public class QuickAssignAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType == SerializedPropertyType.ObjectReference) {
                if(property.objectReferenceValue == null) {
                    var aaa = attribute as QuickAssignAttribute;
                    if(aaa != null) {
                        var t = aaa.Type ?? typeof(UnityEngine.Object);
                        var p = aaa.Path;
                        if(string.IsNullOrEmpty(aaa.Path)) {
                            var assets = AssetDatabaseUtility.FindAssets(t);
                            if(assets.Length >= 1) {
                                p = AssetDatabase.GUIDToAssetPath(assets[0]);
                            }
                        }
                        property.objectReferenceValue = AssetDatabase.LoadAssetAtPath(p, t);
                    }
                }
                EditorGUI.PropertyField(position, property, label);
            }
            else {
                EditorGUI.HelpBox(position, $"{label.text} - 'QuickAssign' does not work on non-object reference variables", MessageType.Error);
            }

        }
    }
}
