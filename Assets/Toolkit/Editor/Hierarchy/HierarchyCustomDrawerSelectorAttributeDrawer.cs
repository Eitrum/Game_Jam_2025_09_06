using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(HierarchyCustomDrawerSelectorAttribute))]
    public class HierarchyCustomDrawerSelectorAttributeDrawer : PropertyDrawer {

        private static string[] cachedOptions;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(cachedOptions == null) {
                cachedOptions = HierarchyDrawer.RegisteredCustomDrawers.Keys.ToArray();
            }

            var index = cachedOptions.IndexOf(property.stringValue);
            EditorGUI.BeginChangeCheck();
            var newIndex = EditorGUI.Popup(position, label.text, index, cachedOptions);
            if(EditorGUI.EndChangeCheck()) {
                if(newIndex != index) {
                    property.stringValue = cachedOptions[newIndex];
                }
            }
        }
    }
}
