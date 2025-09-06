using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        private static string[] layerNames = { };

        [InitializeOnLoadMethod]
        private static void Initialize() {
            layerNames = new string[32];
            for(int i = 0; i < 32; i++) {
                layerNames[i] = LayerMask.LayerToName(i);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType == SerializedPropertyType.Integer) {
                var att = attribute as LayerAttribute;
                EditorGUI.BeginChangeCheck();
                int result = att.mask ? EditorGUI.MaskField(position, label, property.intValue, layerNames) : EditorGUI.Popup(position, label.text, property.intValue, layerNames);
                if(EditorGUI.EndChangeCheck()) {
                    property.intValue = result;
                }
            }
            else {
                EditorGUI.HelpBox(position, $"Field '{label.text}' is not of a supported type!", MessageType.Error);
            }
        }
    }
}
