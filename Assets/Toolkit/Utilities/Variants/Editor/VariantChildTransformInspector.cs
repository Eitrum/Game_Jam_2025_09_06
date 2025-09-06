using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(VariantChildTransform))]
    public class VariantChildTransformInspector : Editor
    {
        private ReorderableList list;
        private SerializedProperty onAwakeProp;
        private SerializedProperty arrayProperty;

        private void OnEnable() {
            arrayProperty = serializedObject.FindProperty("transforms");
            onAwakeProp = serializedObject.FindProperty("activateAtAwake");
            list = new ReorderableList(serializedObject, arrayProperty);
            list.onAddCallback += OnAdd;
            list.drawElementCallback += OnDrawElement;
            list.drawHeaderCallback += OnDrawHeader;
            list.elementHeight = EditorGUIUtility.singleLineHeight + 4f;
        }

        private void OnDrawHeader(Rect rect) {
            rect.PadRef(12f, 0, 0, 0);
            rect.SplitHorizontal(out Rect idArea, out Rect transformArea, out Rect weightArea, 20f / rect.width, 1f - (200f / rect.width), 2f);
            EditorGUI.LabelField(idArea, "#");
            EditorGUI.LabelField(transformArea, "Transform");
            EditorGUI.LabelField(weightArea, "Weight");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 2, 2);
            rect.SplitHorizontal(out Rect idArea, out Rect transformArea, out Rect weightArea, 20f / rect.width, 1f - (200f / rect.width), 2f);
            EditorGUI.LabelField(idArea, $"{index}");
            var prop = arrayProperty.GetArrayElementAtIndex(index);
            var tProp = prop.FindPropertyRelative("transform");
            EditorGUI.PropertyField(transformArea, tProp, GUIContent.none);
            weightArea.width = 118f;
            EditorGUI.PropertyField(weightArea, prop.FindPropertyRelative("weight"), GUIContent.none);
            var butRect = new Rect(weightArea);
            butRect.width = 60f;
            butRect.x += weightArea.width + 2f;

            var trans = tProp.objectReferenceValue;

            if(trans is Transform t) {
                if(GUI.Button(butRect, t.gameObject.activeSelf ? "Disable" : "Enable")) {
                    t.gameObject.SetActive(!t.gameObject.activeSelf);
                }
            }
            else {
                using(new EditorGUI.DisabledScope(true)) {
                    GUI.Button(butRect, "Enable");
                }
            }
        }

        private void OnAdd(ReorderableList list) {
            arrayProperty.arraySize++;
            var prop = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
            prop.FindPropertyRelative("weight").floatValue = 1f;
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button("Disable All Variants", GUILayout.Width(180f))) {
                    for(int i = arrayProperty.arraySize - 1; i >= 0; i--) {
                        var prop = arrayProperty.GetArrayElementAtIndex(i);
                        var tProp = prop.FindPropertyRelative("transform");
                        var refValue = tProp.objectReferenceValue;
                        if(refValue is Transform trans) {
                            trans.SetActive(false);
                        }
                    }
                }
                if(GUILayout.Button("Enable All Variants", GUILayout.Width(180f))) {
                    for(int i = arrayProperty.arraySize - 1; i >= 0; i--) {
                        var prop = arrayProperty.GetArrayElementAtIndex(i);
                        var tProp = prop.FindPropertyRelative("transform");
                        var refValue = tProp.objectReferenceValue;
                        if(refValue is Transform trans) {
                            trans.SetActive(true);
                        }
                    }
                }
            }
            EditorGUILayout.PropertyField(onAwakeProp);

            if(list != null)
                list.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
