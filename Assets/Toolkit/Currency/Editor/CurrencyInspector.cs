using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Currency
{
    [CustomPropertyDrawer(typeof(Currency))]
    public class CurrencyInspector : PropertyDrawer
    {



        [InitializeOnLoadMethod]
        private static void Load() {

        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.isExpanded)
                return EditorGUIUtility.singleLineHeight * (2 + CurrencyUtility.CURRENCY_TIERS);
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var valProp = property.FindPropertyRelative("value");
            var cur = new Currency(valProp.intValue);
            position.height = EditorGUIUtility.singleLineHeight;

            var isExpanded = property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, $"{label.text} - {cur.ToString()}");
            if(isExpanded) {
                using(new EditorGUI.IndentLevelScope(1)) {
                    EditorGUI.BeginChangeCheck();
                    position.y += position.height;
                    cur = new Currency(EditorGUI.DelayedIntField(position, "Raw Value", cur.Value));
                    position.SplitHorizontal(out Rect imageArea, out Rect valueArea, 18f / position.width, 4f);
                    imageArea.x += EditorGUI.indentLevel * 16f;

                    for(int i = 0; i < CurrencyUtility.CURRENCY_TIERS; i++) {
                        imageArea.y += position.height;
                        valueArea.y += position.height;

                        var cType = (CurrencyType)i;
                        var amount = cur.GetAmount(cType);
                        var tex = CurrencyEditorIcons.GetTexture(cType);
                        if(tex != null)
                            GUI.DrawTexture(imageArea, tex);
                        var newValue = EditorGUI.IntSlider(valueArea, CurrencyUtility.GetName(cType), amount, 0, CurrencyUtility.GetRem(cType) - 1);
                        if(newValue != amount) {
                            cur.Add(cType, newValue - amount);
                        }
                    }

                    if(EditorGUI.EndChangeCheck())
                        valProp.intValue = cur.Value;
                }
            }
        }

    }
}
