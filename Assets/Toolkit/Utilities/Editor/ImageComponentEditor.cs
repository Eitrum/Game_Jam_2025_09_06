using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(ImageComponent))]
    public class ImageComponentEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var reference = property.FindPropertyRelative("reference");
            EditorGUI.BeginChangeCheck();
            var obj = EditorGUI.ObjectField(position, label, reference.objectReferenceValue, typeof(UnityEngine.Object), true);
            if(EditorGUI.EndChangeCheck()) {
                if(obj == null)
                    reference.objectReferenceValue = null;
                else if(obj is GameObject go)
                    reference.objectReferenceValue = ImageComponent.FindInChildren(go).Reference;
                else if(obj is Component comp)
                    reference.objectReferenceValue = ImageComponent.FindInChildren(comp).Reference;
            }
        }
    }
}
