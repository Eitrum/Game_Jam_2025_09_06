using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(NSOReferenceAttribute))]
    public class NSOReferenceAttributePropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return NSOEditor.CalculateHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            NSOEditor.DrawProperty(position, property, label, (attribute as NSOReferenceAttribute)?.Type);
        }
    }
}
