using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit {
    [CustomPropertyDrawer(typeof(TraitEntry))]
    public class TraitEntryPropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 2f + 4f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.PadRef(0, 0, 2, 2);
            GUI.Box(position, "");
            position.SplitVertical(out Rect typeArea, out Rect valueArea, 0.5f);
            var typeProp = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(typeArea, typeProp, label);
            valueArea.SplitHorizontal(out Rect negArea, out Rect sliderArea, out Rect posArea, 120f / valueArea.width, (valueArea.width - 240f) / valueArea.width, 0f);
            var type = (TraitType)typeProp.intValue;
            EditorGUI.LabelField(negArea, TraitsUtility.GetNegativeName(type), EditorStylesUtility.CenterAlignedLabel);
            EditorGUI.LabelField(posArea, TraitsUtility.GetPositiveName(type), EditorStylesUtility.CenterAlignedLabel);
            var prop = property.FindPropertyRelative("value");
            float newValue = prop.floatValue;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUI.Slider(sliderArea, prop.floatValue, TraitsUtility.MIN, TraitsUtility.MAX);
            if(EditorGUI.EndChangeCheck())
                prop.floatValue = EditorStylesUtility.IsHoldingAlt ? Mathf.RoundToInt(newValue) : newValue;
        }
    }
}
