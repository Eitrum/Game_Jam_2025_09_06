using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(VariantRelay))]
    public class VariantRelayInspector : Editor
    {
        private SerializedProperty activateAtAwake;
        private SerializedProperty variants;
        private ReorderableList variantsList;

        private void OnEnable() {
            activateAtAwake = serializedObject.FindProperty("activateAtAwake");
            variants = serializedObject.FindProperty("variants");
            variantsList = new ReorderableList(serializedObject, variants);
            variantsList.headerHeight = 0f;
            variantsList.drawElementCallback += OnDrawElement;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect.Pad(0, 0, 2, 2), variants.GetArrayElementAtIndex(index), GUIContent.none);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var vr = target as VariantRelay;
            using(new EditorGUILayout.HorizontalScope("box")) {
                if(GUILayout.Button("Apply Random", GUILayout.Width(120f))) {
                    vr.SetVariant();
                }
                if(GUILayout.Button("Add all children", GUILayout.Width(120f))) {
                    var childVariants = vr.GetComponentsInChildren<IVariant>();
                    variants.arraySize = childVariants.Length;
                    for(int i = 0, length = childVariants.Length; i < length; i++) {
                        variants.GetArrayElementAtIndex(i).objectReferenceValue = childVariants[i] as UnityEngine.Object;
                    }
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(activateAtAwake);
            }

            if(variantsList != null)
                variantsList.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
