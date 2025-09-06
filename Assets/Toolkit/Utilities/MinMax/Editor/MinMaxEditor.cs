using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(MinMaxCurve))]
    [CustomPropertyDrawer(typeof(MinMaxColor))]
    [CustomPropertyDrawer(typeof(MinMaxGradient))]
    [CustomPropertyDrawer(typeof(MinMaxInt))]
    [CustomPropertyDrawer(typeof(MinMax))]
    public class MinMaxEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            MinMaxEditorUtility.DrawMinMax(position, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(MinMaxVector2))]
    [CustomPropertyDrawer(typeof(MinMaxVector3))]
    [CustomPropertyDrawer(typeof(MinMaxVector4))]
    public class MinMaxVectorEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            MinMaxEditorUtility.DrawMinMaxVectors(position, property, label);
        }
    }

    internal static class MinMaxEditorUtility
    {
        public static void DrawMinMax(Rect position, SerializedProperty property, GUIContent label) {
            position.SplitHorizontal(out Rect left, out Rect mid, out Rect right, 0.4f, 5f);
            EditorGUI.LabelField(left, label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            mid.SplitHorizontal(out Rect minLabel, out Rect minValue, 40f / mid.width);
            EditorGUI.LabelField(minLabel, "Min");
            var minProperty = property.FindPropertyRelative("min");
            DrawField(minValue, minProperty);

            var maxProperty = property.FindPropertyRelative("max");
            right.SplitHorizontal(out Rect maxLabel, out Rect maxValue, 40f / right.width);
            EditorGUI.LabelField(maxLabel, "Max");
            DrawField(maxValue, maxProperty);

            EditorGUI.indentLevel = indent;
        }

        public static void DrawMinMaxVectors(Rect position, SerializedProperty property, GUIContent label) {
            GUI.Box(position, "");
            var splits = position.SplitVertical(3, 0f);
            splits[0].SplitHorizontal(out Rect labelArea, out Rect linearArea, 1f - (80f / splits[0].width), 0f);
            var linear = property.FindPropertyRelative("linear");
            EditorGUI.LabelField(labelArea, label);
            linear.boolValue = EditorGUI.ToggleLeft(linearArea.Pad(0, 8, 0, 0), "Linear", linear.boolValue);
            using(new EditorGUI.IndentLevelScope(1)) {
                var min = property.FindPropertyRelative("min");
                var max = property.FindPropertyRelative("max");
                switch(min.propertyType) {
                    case SerializedPropertyType.Vector2:
                        min.vector2Value = EditorGUI.Vector2Field(splits[1], "Min", min.vector2Value);
                        max.vector2Value = EditorGUI.Vector2Field(splits[2], "Max", max.vector2Value);
                        break;
                    case SerializedPropertyType.Vector3:

                        min.vector3Value = EditorGUI.Vector3Field(splits[1], "Min", min.vector3Value);
                        max.vector3Value = EditorGUI.Vector3Field(splits[2], "Max", max.vector3Value);
                        break;
                    case SerializedPropertyType.Vector4:
                        min.vector4Value = EditorGUI.Vector4Field(splits[1], "Min", min.vector4Value);
                        max.vector4Value = EditorGUI.Vector4Field(splits[2], "Max", max.vector4Value);
                        break;
                }
            }
        }

        public static void DrawField(Rect position, SerializedProperty property) {
            if(property == null) {
                EditorGUI.LabelField(position, "Property is null");
                return;
            }
            switch(property.propertyType) {
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.FloatField(position, property.floatValue);
                    return;
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntField(position, property.intValue);
                    return;
                case SerializedPropertyType.Color:
                    property.colorValue = EditorGUI.ColorField(position, property.colorValue);
                    return;
                case SerializedPropertyType.Gradient:
                    EditorGUI.PropertyField(position, property, null);
                    return;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = EditorGUI.CurveField(position, property.animationCurveValue);
                    return;
            }
            EditorGUI.PropertyField(position, property, null);
        }
    }
}
