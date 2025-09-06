using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.Procedural.Items
{

    [CustomPropertyDrawer(typeof(Part))]
    public class PartPropertyDrawer : PropertyDrawer
    {
        static string[] partNames;
        static PartPropertyDrawer() {
            partNames = Part.None.GetArray().Select(x => PartUtility.GetPath(x)).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.Popup(position, label.text, property.enumValueIndex, partNames);
            if(EditorGUI.EndChangeCheck()) {
                property.enumValueIndex = newValue;
            }
        }
    }
}
