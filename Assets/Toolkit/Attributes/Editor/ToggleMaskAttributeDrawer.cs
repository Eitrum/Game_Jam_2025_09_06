using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(ToggleMaskAttribute))]
    public class ToggleMaskAttributeDrawer : PropertyDrawer
    {
        private static GUIStyle wrapped;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            switch(property.propertyType) {
                case SerializedPropertyType.Float:
                    return property.type == "double" ? EditorGUIUtility.singleLineHeight * 5f : EditorGUIUtility.singleLineHeight * 3f;
                case SerializedPropertyType.Integer:
                    return property.type == "ulong" || property.type == "long" ? EditorGUIUtility.singleLineHeight * 5f : EditorGUIUtility.singleLineHeight * 3f;

                default:
                    return EditorGUIUtility.singleLineHeight;
            }
        }

        unsafe public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(wrapped == null) {
                wrapped = new GUIStyle(EditorStylesUtility.CenterAlignedLabel);
                wrapped.fontSize -= 2;
                wrapped.wordWrap = true;
            }

            switch(property.propertyType) {
                case SerializedPropertyType.Integer:
                    EditorGUI.BeginChangeCheck();
                    if(property.type == "ulong" || property.type == "long") {
                        var value = property.longValue;
                        var result = HandleLong(position, *(ulong*)&value, label);
                        if(EditorGUI.EndChangeCheck()) {
                            property.longValue = *(long*)&result;
                        }
                    }
                    else {
                        var value = property.intValue;
                        var result = HandleInteger(position, *(uint*)&value, label);
                        if(EditorGUI.EndChangeCheck()) {
                            property.intValue = *(int*)&result;
                        }
                    }
                    break;

                case SerializedPropertyType.Float:
                    EditorGUI.BeginChangeCheck();
                    if(property.type == "double") {
                        var value = property.doubleValue;
                        var result = HandleLong(position, *(ulong*)&value, label);
                        if(EditorGUI.EndChangeCheck()) {
                            property.doubleValue = *(double*)&result;
                        }
                    }
                    else {
                        var value = property.floatValue;
                        var result = HandleInteger(position, *(uint*)&value, label);
                        if(EditorGUI.EndChangeCheck()) {
                            property.floatValue = *(float*)&result;
                        }
                    }
                    break;
                default:
                    EditorGUI.HelpBox(position, label.text + " is not of a supported type", MessageType.Warning);
                    break;
            }
        }

        private static uint HandleInteger(Rect position, uint mask, GUIContent label) {
            const uint BASE = 1;
            var height = EditorGUIUtility.singleLineHeight;

            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            EditorGUI.LabelField(position.Pad(0, 0, 0, height * 2f), $"{label.text} : {mask}", EditorStyles.boldLabel);

            position.PadRef(0, 0, height, 0);
            position.width = 16f;
            position.x += 2f;

            for(int i = 0; i < 32; i++) {
                EditorGUI.LabelField(position, $"{i}", wrapped);
                uint m = BASE << i;
                var isChecked = (mask & m) == m;
                var val = EditorGUI.Toggle(position.Pad(0, 0, height, 0f), isChecked);
                if(isChecked != val) {
                    mask = val ? mask | m : mask ^ m;
                }
                position.x += 18f;
            }


            return mask;
        }

        private static ulong HandleLong(Rect position, ulong mask, GUIContent label) {
            const ulong BASE = 1;
            var height = EditorGUIUtility.singleLineHeight;

            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            EditorGUI.LabelField(position.Pad(0, 0, 0, height * 4f), $"{label.text} : {mask}", EditorStyles.boldLabel);

            position.y += height;
            position.height = height * 2f;
            position.width = 16f;
            position.x += 2f;

            for(int i = 0; i < 32; i++) {
                EditorGUI.LabelField(position, $"{i}", wrapped);
                ulong m = BASE << i;
                var isChecked = (mask & m) == m;
                var val = EditorGUI.Toggle(position.Pad(0, 0, height, 0f), isChecked);
                if(isChecked != val) {
                    mask = val ? mask | m : mask ^ m;
                }
                position.x += 18f;
            }

            position.y += height * 2f;
            position.x -= 18f * 32f;

            for(int i = 32; i < 64; i++) {
                EditorGUI.LabelField(position, $"{i}", wrapped);
                ulong m = BASE << i;
                var isChecked = (mask & m) == m;
                var val = EditorGUI.Toggle(position.Pad(0, 0, height, 0f), isChecked);
                if(isChecked != val) {
                    mask = val ? mask | m : mask ^ m;
                }
                position.x += 18f;
            }

            return mask;
        }

    }
}
