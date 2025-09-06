using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(MaterialProperty))]
    public class MaterialPropertyPropertyDrawer : PropertyDrawer
    {
        private static MaterialPropertyType propertyType;
        private static SerializedProperty objectReferences;
        private static SerializedProperty valueArray;
        private static SerializedProperty vectorArray;

        private static ReorderableList objectList;
        private static ReorderableList floatList;
        private static ReorderableList vectorList;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(!property.isExpanded) {
                return EditorGUIUtility.singleLineHeight;
            }

            switch(property.FindPropertyRelative("type").intValue.ToEnum<MaterialPropertyType>()) {
                case MaterialPropertyType.Color:
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Texture:
                case MaterialPropertyType.Texture3D:
                case MaterialPropertyType.TextureArray:
                case MaterialPropertyType.Vector2:
                case MaterialPropertyType.Vector3:
                case MaterialPropertyType.Vector4:
                    return EditorGUIUtility.singleLineHeight * 4f;

                case MaterialPropertyType.Color_Range:
                case MaterialPropertyType.Float_Range:
                case MaterialPropertyType.Vector2_Range:
                case MaterialPropertyType.Vector3_Range:
                case MaterialPropertyType.Vector4_Range:
                    return EditorGUIUtility.singleLineHeight * 5f;
                case MaterialPropertyType.FloatArray:
                case MaterialPropertyType.Float_Random:
                    return EditorGUIUtility.singleLineHeight * Mathf.Max(5, 4 + property.FindPropertyRelative("valueArray").arraySize);
                case MaterialPropertyType.Color_Random:
                case MaterialPropertyType.Vector2_Random:
                case MaterialPropertyType.Vector3_Random:
                case MaterialPropertyType.Vector4_Random:
                    return EditorGUIUtility.singleLineHeight * Mathf.Max(5, 4 + property.FindPropertyRelative("vectorArray").arraySize);

                case MaterialPropertyType.Texture3D_Random:
                case MaterialPropertyType.Texture_Random:
                case MaterialPropertyType.TextureArray_Random:
                    return EditorGUIUtility.singleLineHeight * Mathf.Max(5, 4 + property.FindPropertyRelative("objectReferences").arraySize);
            }
            return EditorGUIUtility.singleLineHeight * 4f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var name = property.FindPropertyRelative("name");
            var type = property.FindPropertyRelative("type");
            objectReferences = property.FindPropertyRelative("objectReferences");
            valueArray = property.FindPropertyRelative("valueArray");
            vectorArray = property.FindPropertyRelative("vectorArray");
            propertyType = type.intValue.ToEnum<MaterialPropertyType>();

            if(objectList == null) {
                objectList = new ReorderableList(property.serializedObject, objectReferences);
                objectList.drawElementCallback += DrawObjectElement;
                objectList.headerHeight = 0f;
                objectList.elementHeight = EditorGUIUtility.singleLineHeight;
                objectList.footerHeight = EditorGUIUtility.singleLineHeight;
            }
            else {
                objectList.serializedProperty = objectReferences;
            }

            if(vectorList == null) {
                vectorList = new ReorderableList(property.serializedObject, vectorArray);
                vectorList.drawElementCallback += DrawVectorElement;
                vectorList.headerHeight = 0f;
                vectorList.elementHeight = EditorGUIUtility.singleLineHeight;
                vectorList.footerHeight = EditorGUIUtility.singleLineHeight;
            }
            else {
                vectorList.serializedProperty = vectorArray;
            }

            if(floatList == null) {
                floatList = new ReorderableList(property.serializedObject, valueArray);
                floatList.drawElementCallback += DrawValueElement;
                floatList.headerHeight = 0f;
                floatList.elementHeight = EditorGUIUtility.singleLineHeight;
                floatList.footerHeight = EditorGUIUtility.singleLineHeight;
            }
            else {
                floatList.serializedProperty = valueArray;
            }

            string nameValue = $"{label.text} ({name.stringValue}, {type.intValue.ToEnum<MaterialPropertyType>()})";
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, nameValue, true);

            if(property.isExpanded) {
                using(new EditorGUI.IndentLevelScope(1)) {
                    position.y += position.height;
                    EditorGUI.PropertyField(position, name);
                    position.y += position.height;
                    EditorGUI.BeginChangeCheck();
                    type.intValue = EditorGUI.EnumPopup(position, "Type", type.intValue.ToEnum<MaterialPropertyType>()).ToInt();
                    if(EditorGUI.EndChangeCheck()) {
                        propertyType = type.intValue.ToEnum<MaterialPropertyType>();
                        objectReferences.arraySize = 0;
                        valueArray.arraySize = 0;
                        vectorArray.arraySize = 0;
                    }
                    position.y += position.height;

                    switch(type.intValue.ToEnum<MaterialPropertyType>()) {
                        case MaterialPropertyType.Texture: {
                                objectReferences.arraySize = 1;
                                var element = objectReferences.GetArrayElementAtIndex(0);
                                element.objectReferenceValue = EditorGUI.ObjectField(position, "Texture", element.objectReferenceValue, typeof(Texture2D), false);
                            }
                            break;
                        case MaterialPropertyType.Texture3D: {
                                objectReferences.arraySize = 1;
                                var element = objectReferences.GetArrayElementAtIndex(0);
                                element.objectReferenceValue = EditorGUI.ObjectField(position, "Texture", element.objectReferenceValue, typeof(Texture3D), false);
                            }
                            break;
                        case MaterialPropertyType.TextureArray: {
                                objectReferences.arraySize = 1;
                                var element = objectReferences.GetArrayElementAtIndex(0);
                                element.objectReferenceValue = EditorGUI.ObjectField(position, "Texture", element.objectReferenceValue, typeof(Texture2DArray), false);
                            }
                            break;
                        case MaterialPropertyType.Float: {
                                valueArray.arraySize = 1;
                                var element = valueArray.GetArrayElementAtIndex(0);
                                element.floatValue = EditorGUI.FloatField(position, "Value", element.floatValue);
                            }
                            break;
                        case MaterialPropertyType.Color: {
                                vectorArray.arraySize = 1;
                                var element = vectorArray.GetArrayElementAtIndex(0);
                                element.vector4Value = EditorGUI.ColorField(position, "Color", element.vector4Value);
                            }
                            break;

                        case MaterialPropertyType.Float_Range: {
                                valueArray.arraySize = 2;
                                var element0 = valueArray.GetArrayElementAtIndex(0);
                                var element1 = valueArray.GetArrayElementAtIndex(1);
                                element0.floatValue = EditorGUI.FloatField(position, "Minimum", element0.floatValue);
                                position.y += position.height;
                                element1.floatValue = EditorGUI.FloatField(position, "Maximum", element1.floatValue);
                            }
                            break;
                        case MaterialPropertyType.Vector2_Range: {
                                vectorArray.arraySize = 2;
                                var element0 = vectorArray.GetArrayElementAtIndex(0);
                                var element1 = vectorArray.GetArrayElementAtIndex(1);
                                element0.vector4Value = EditorGUI.Vector2Field(position, "Minimum", element0.vector4Value);
                                position.y += position.height;
                                element1.vector4Value = EditorGUI.Vector2Field(position, "Maximum", element1.vector4Value);
                            }
                            break;
                        case MaterialPropertyType.Vector3_Range: {
                                vectorArray.arraySize = 2;
                                var element0 = vectorArray.GetArrayElementAtIndex(0);
                                var element1 = vectorArray.GetArrayElementAtIndex(1);
                                element0.vector4Value = EditorGUI.Vector3Field(position, "Minimum", element0.vector4Value);
                                position.y += position.height;
                                element1.vector4Value = EditorGUI.Vector3Field(position, "Maximum", element1.vector4Value);
                            }
                            break;
                        case MaterialPropertyType.Vector4_Range: {
                                vectorArray.arraySize = 2;
                                var element0 = vectorArray.GetArrayElementAtIndex(0);
                                var element1 = vectorArray.GetArrayElementAtIndex(1);
                                element0.vector4Value = EditorGUI.Vector4Field(position, "Minimum", element0.vector4Value);
                                position.y += position.height;
                                element1.vector4Value = EditorGUI.Vector4Field(position, "Maximum", element1.vector4Value);
                            }
                            break;
                        case MaterialPropertyType.Color_Range: {
                                vectorArray.arraySize = 2;
                                var element0 = vectorArray.GetArrayElementAtIndex(0);
                                var element1 = vectorArray.GetArrayElementAtIndex(1);
                                element0.vector4Value = EditorGUI.ColorField(position, "Minimum", element0.vector4Value);
                                position.y += position.height;
                                element1.vector4Value = EditorGUI.ColorField(position, "Maximum", element1.vector4Value);
                            }
                            break;

                        case MaterialPropertyType.Texture_Random:
                        case MaterialPropertyType.Texture3D_Random:
                        case MaterialPropertyType.TextureArray_Random:
                            objectList.DoList(EditorGUI.IndentedRect(position));
                            break;

                        case MaterialPropertyType.Float_Random:
                        case MaterialPropertyType.FloatArray:
                            floatList.DoList(EditorGUI.IndentedRect(position));
                            break;

                        case MaterialPropertyType.Color_Random:
                        case MaterialPropertyType.Vector2_Random:
                        case MaterialPropertyType.Vector3_Random:
                        case MaterialPropertyType.Vector4_Random:
                            vectorList.DoList(EditorGUI.IndentedRect(position));
                            break;

                        default:
                            EditorGUI.HelpBox(position, "Not implemented!", MessageType.Error);
                            break;
                    }
                }
            }
        }

        private void DrawValueElement(Rect rect, int index, bool isActive, bool isFocused) {
            var name = $"Element {index}";
            var element = valueArray.GetArrayElementAtIndex(index);
            element.floatValue = EditorGUI.FloatField(rect, name, element.floatValue);
        }

        private void DrawVectorElement(Rect rect, int index, bool isActive, bool isFocused) {
            var name = $"Element {index}";
            var element = vectorArray.GetArrayElementAtIndex(index);
            switch(propertyType) {
                case MaterialPropertyType.Color_Random:
                    element.vector4Value = EditorGUI.ColorField(rect, name, element.vector4Value);
                    break;
                case MaterialPropertyType.Vector2_Random:
                    element.vector4Value = EditorGUI.Vector2Field(rect, name, element.vector4Value);
                    break;
                case MaterialPropertyType.Vector3_Random:
                    element.vector4Value = EditorGUI.Vector3Field(rect, name, element.vector4Value);
                    break;
                case MaterialPropertyType.Vector4_Random:
                    element.vector4Value = EditorGUI.Vector4Field(rect, name, element.vector4Value);
                    break;
            }
        }

        private void DrawObjectElement(Rect rect, int index, bool isActive, bool isFocused) {
            var name = $"Element {index}";
            var element = objectReferences.GetArrayElementAtIndex(index);
            switch(propertyType) {
                case MaterialPropertyType.Texture_Random:
                    element.objectReferenceValue = EditorGUI.ObjectField(rect.Pad(0, 0, 1, 1), name, element.objectReferenceValue, typeof(Texture2D), false);
                    break;
                case MaterialPropertyType.Texture3D_Random:
                    element.objectReferenceValue = EditorGUI.ObjectField(rect.Pad(0, 0, 1, 1), name, element.objectReferenceValue, typeof(Texture3D), false);
                    break;
                case MaterialPropertyType.TextureArray_Random:
                    element.objectReferenceValue = EditorGUI.ObjectField(rect.Pad(0, 0, 1, 1), name, element.objectReferenceValue, typeof(Texture2DArray), false);
                    break;
            }
        }
    }
}
