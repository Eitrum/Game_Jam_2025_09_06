using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(TypeFilter))]
    public class TypeFilterEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType == SerializedPropertyType.ObjectReference || property.propertyType == SerializedPropertyType.ExposedReference) {
                var t = (TypeFilter)attribute;
                var filter = t.type;

                EditorGUI.BeginChangeCheck(); ;

                var obj = (label == GUIContent.none) ?
                        EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(UnityEngine.Object), true) :
                        EditorGUI.ObjectField(position, EditorGUIUtility.TrTempContent($"{label.text} ({filter.Name})"), property.objectReferenceValue, typeof(UnityEngine.Object), true);
                if(obj != null) {
                    if(obj is GameObject go && go.TryGetComponent(filter, out Component component)) {
                        obj = component;
                    }
                    else if(!filter.IsAssignableFrom(obj.GetType())) {
                        obj = null;
                    }
                }
                if(EditorGUI.EndChangeCheck())
                    property.objectReferenceValue = obj;
            }
            else {
                EditorGUI.LabelField(position, label.text + " (Only works with UnityEngine.Object)");
            }
        }
    }
}
