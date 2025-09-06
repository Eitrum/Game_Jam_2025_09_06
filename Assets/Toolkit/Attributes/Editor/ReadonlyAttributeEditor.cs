using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(ReadonlyAttribute), true)]
    public class ReadonlyAttributeEditor : PropertyDrawer {
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var ro = attribute as ReadonlyAttribute;
            var enabled = GUI.enabled;
            GUI.enabled = ro.onlyInGame && !Application.isPlaying;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = enabled;
        }
    }
}
