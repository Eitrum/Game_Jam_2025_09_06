using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    [CustomPropertyDrawer(typeof(NavAreaAttribute))]
    public class NavAreaAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();

            if(property.propertyType != SerializedPropertyType.Integer) {
                EditorGUI.LabelField(position, "Field must be of integer type");
                return;
            }

            position.SplitHorizontal(out Rect colorRect, out Rect rest, 25f / position.width, 5f);
            int newValue = property.intValue;
            if(attribute is NavAreaAttribute naa && naa.IsMask)
                newValue = EditorGUI.MaskField(rest, label.text, property.intValue, UnityEngine.AI.NavMesh.GetAreaNames());
            else
                newValue = EditorGUI.Popup(rest, label.text, property.intValue, UnityEngine.AI.NavMesh.GetAreaNames());

            EditorGUI.DrawRect(colorRect, Color.black);
            EditorGUI.DrawRect(colorRect.Shrink(1f), Color.grey);
            EditorGUI.DrawRect(colorRect.Shrink(1f), Toolkit.AI.Navigation.NavigationAreaCostEditor.GetAreaColor(property.intValue, 0.7f));

            if(EditorGUI.EndChangeCheck()) {
                property.intValue = newValue;
            }
        }
    }
}
