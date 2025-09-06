using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(VariantPrefab))]
    public class VariantPrefabInspector : Editor
    {
        private ReorderableList list;
        private SerializedProperty arrayProperty;
        private SerializedProperty containerProp;
        private SerializedProperty spawnAtAwakeProp;
        private SerializedProperty spawnAsChildProp;

        private void OnEnable() {
            arrayProperty = serializedObject.FindProperty("prefabs");
            containerProp = serializedObject.FindProperty("container");
            spawnAtAwakeProp = serializedObject.FindProperty("spawnAtAwake");
            spawnAsChildProp = serializedObject.FindProperty("spawnAsChild");

            list = new ReorderableList(serializedObject, arrayProperty);
            list.onAddCallback += OnAdd;
            list.drawElementCallback += OnDrawElement;
            list.drawHeaderCallback += OnDrawHeader;
            list.elementHeight = EditorGUIUtility.singleLineHeight + 4f;
        }

        private void OnDrawHeader(Rect rect) {
            rect.PadRef(12f, 0, 0, 0);
            rect.SplitHorizontal(out Rect idArea, out Rect prefabArea, out Rect weightArea, 20f / rect.width, 1f - (200f / rect.width), 2f);
            EditorGUI.LabelField(idArea, "#");
            EditorGUI.LabelField(prefabArea, "Prefab");
            EditorGUI.LabelField(weightArea, "Weight");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 2, 2);
            rect.SplitHorizontal(out Rect idArea, out Rect prefabArea, out Rect weightArea, 20f / rect.width, 1f - (200f / rect.width), 2f);
            EditorGUI.LabelField(idArea, $"{index}");
            var prop = arrayProperty.GetArrayElementAtIndex(index);
            var tProp = prop.FindPropertyRelative("prefab");
            EditorGUI.PropertyField(prefabArea, tProp, GUIContent.none);
            weightArea.width = 118f;
            EditorGUI.PropertyField(weightArea, prop.FindPropertyRelative("weight"), GUIContent.none);
            var butRect = new Rect(weightArea);
            butRect.width = 60f;
            butRect.x += weightArea.width + 2f;

            var trans = tProp.objectReferenceValue;

            if(trans is GameObject go) {
                if(GUI.Button(butRect, "Spawn") && target is VariantPrefab vp) {
                    vp.Spawn(index);
                }
            }
            else {
                using(new EditorGUI.DisabledScope(true)) {
                    GUI.Button(butRect, "Spawn");
                }
            }
        }

        private void OnAdd(ReorderableList list) {
            arrayProperty.arraySize++;
            var prop = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
            prop.FindPropertyRelative("weight").floatValue = 1f;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button("Spawn Random", GUILayout.Width(120f)) && target is VariantPrefab vp) {
                    vp.Spawn();
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(spawnAsChildProp);
                if(spawnAsChildProp.boolValue)
                    using(new EditorGUI.IndentLevelScope(1))
                        EditorGUILayout.PropertyField(containerProp);
                EditorGUILayout.PropertyField(spawnAtAwakeProp);
            }

            if(list != null)
                list.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
