using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(MetricSuffix))]
    public class MetricSuffixPropertyDrawer : PropertyDrawer
    {
        public enum MetricSuffixEditor
        {
            [InspectorName("Yocto = 10^-24")]
            Yocto = -24,
            [InspectorName("Zepto = 10^-21")]
            Zepto = -21,
            [InspectorName("Atto = 10^-18")]
            Atto = -18,
            [InspectorName("Femto = 10^-15")]
            Femto = -15,
            [InspectorName("Pico = 10^-12")]
            Pico = -12,
            [InspectorName("Nano = 10^-9")]
            Nano = -9,
            [InspectorName("Micro = 10^-6")]
            Micro = -6,
            [InspectorName("Milli = 10^-3")]
            Milli = -3,
            [InspectorName("Centi = 10^-2")]
            Centi = -2,
            [InspectorName("Deci = 10^-1")]
            Deci = -1,

            [InspectorName("None = 10^0")]
            None = 0,

            [InspectorName("Deca = 10^1")]
            Deca = 1,
            [InspectorName("Hecto = 10^2")]
            Hecto = 2,
            [InspectorName("Kilo = 10^3")]
            Kilo = 3,
            [InspectorName("Mega = 10^6")]
            Mega = 6,
            [InspectorName("Giga = 10^9")]
            Giga = 9,
            [InspectorName("Tera = 10^12")]
            Tera = 12,
            [InspectorName("Peta = 10^15")]
            Peta = 15,
            [InspectorName("Exa = 10^18")]
            Exa = 18,
            [InspectorName("Zetta = 10^21")]
            Zetta = 21,
            [InspectorName("Yotta = 10^24")]
            Yotta = 24
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, false);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var editorValue = (MetricSuffixEditor)property.intValue;
            EditorGUI.BeginChangeCheck();
            editorValue = (MetricSuffixEditor)EditorGUI.EnumPopup(position, label, editorValue);
            if(EditorGUI.EndChangeCheck()) {
                property.intValue = (int)editorValue;
            }
        }
    }
}
