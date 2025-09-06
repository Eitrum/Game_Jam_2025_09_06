using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Health
{
    [CustomPropertyDrawer(typeof(Damage))]
    public class DamageEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, false);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var damageTypeProperty = property.FindPropertyRelative("damageType");
            var amountProperty = property.FindPropertyRelative("amount");

            position.SplitHorizontal(out Rect labelArea, out Rect amountArea, out Rect damageTypeArea, 0.4f, 5f);

            EditorGUI.LabelField(labelArea, label);

            amountArea.SplitHorizontal(out Rect amountLabelArea, out Rect amountValueArea, 50f / amountArea.width);
            EditorGUI.LabelField(amountLabelArea, "Amount");
            amountProperty.floatValue = EditorGUI.FloatField(amountValueArea, amountProperty.floatValue);

            damageTypeArea.SplitHorizontal(out Rect typeLabelArea, out Rect typeValueArea, 35f / damageTypeArea.width);
            EditorGUI.LabelField(typeLabelArea, "Type");
            damageTypeProperty.intValue = DamageTypeEditor.Draw(typeValueArea, damageTypeProperty.intValue, false);
        }
    }
}
