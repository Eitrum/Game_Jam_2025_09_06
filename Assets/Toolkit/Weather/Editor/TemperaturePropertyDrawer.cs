using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Weather
{
    [CustomPropertyDrawer(typeof(Temperature))]
    public class TemperaturePropertyDrawer : PropertyDrawer
    {
        private static TemperatureType type = TemperatureType.Kelvin;

        public static TemperatureType Type {
            get => type;
            set {
                if(type != value) {
                    type = value;
                    EditorPrefs.SetInt("TemperatureType", (int)value);
                }
            }
        }

        static TemperaturePropertyDrawer() {
            type = (TemperatureType)EditorPrefs.GetInt("TemperatureType", 0);
        }

        private bool foldout = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * (foldout ? 4f : 1f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.SplitVertical(out position, out Rect bottom, foldout ? 0.25f : 1f);
            position.SplitHorizontal(out Rect foldoutArea, out Rect valueArea, out Rect typeArea, 0.4f, 0.4f, 1f);
            var prop = property.FindPropertyRelative("value");
            if(foldout = EditorGUI.Foldout(foldoutArea, foldout, label, true)) {
                bottom.SplitVertical(out Rect top, out Rect mid, out Rect bot, 0.33333f, 0.33333f, 0f);
                GUI.Box(bottom, "");
                using(new EditorGUI.IndentLevelScope()) {
                    EditorGUI.LabelField(top, $"{prop.floatValue: 0.00}K");
                    EditorGUI.LabelField(mid, $"{ Temperature.ConvertKelvinToCelcius(prop.floatValue): 0.00}°C");
                    EditorGUI.LabelField(bot, $"{Temperature.ConvertKelvinToFahrenheit(prop.floatValue): 0.00}°F");
                }
            }
            switch(type) {
                case TemperatureType.Kelvin:
                    prop.floatValue = Mathf.Max(0f, EditorGUI.FloatField(valueArea, prop.floatValue));
                    break;
                case TemperatureType.Celcius:
                    prop.floatValue = Mathf.Max(0f, Temperature.ConvertCelciusToKelvin(EditorGUI.FloatField(valueArea, Temperature.ConvertKelvinToCelcius(prop.floatValue))));
                    break;
                case TemperatureType.Fahrenheit:
                    prop.floatValue = Mathf.Max(0f, Temperature.ConvertFahrenheitToKelvin(EditorGUI.FloatField(valueArea, Temperature.ConvertKelvinToFahrenheit(prop.floatValue))));
                    break;
            }

            Type = (TemperatureType)EditorGUI.EnumPopup(typeArea, type);
        }

    }
}
