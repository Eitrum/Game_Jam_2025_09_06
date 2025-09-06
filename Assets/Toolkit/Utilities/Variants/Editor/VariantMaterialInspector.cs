using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(VariantMaterial))]
    [CanEditMultipleObjects]
    public class VariantMaterialInspector : Editor
    {
        private SerializedProperty activateAtAwake;
        private SerializedProperty materials;
        private SerializedProperty renderers;
        private ReorderableList materialList;
        private ReorderableList rendererList;

        private void OnEnable() {
            activateAtAwake = serializedObject.FindProperty("activateAtAwake");
            materials = serializedObject.FindProperty("materials");
            renderers = serializedObject.FindProperty("renderers");

            materialList = new ReorderableList(serializedObject, materials);
            materialList.drawHeaderCallback += OnDrawMaterialHeader;
            materialList.drawElementCallback += OnDrawMaterialElement;

            rendererList = new ReorderableList(serializedObject, renderers);
            rendererList.drawHeaderCallback += OnDrawRendererHeader;
            rendererList.drawElementCallback += OnDrawRendererElement;
        }

        #region Renderer Draw

        private void OnDrawRendererHeader(Rect rect) {
            renderers.isExpanded = EditorGUI.Foldout(rect.Pad(14, 0, 0, 0), renderers.isExpanded, $"Renderers", true);
        }

        private void OnDrawRendererElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect.Pad(0, 0, 2, 2), renderers.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Material Draw

        private void OnDrawMaterialHeader(Rect rect) {
            rect.SplitHorizontal(out Rect foldoutarea, out Rect weightArea, out Rect applyArea, 1f - (160f / rect.width), 80f / rect.width, 2f);
            materials.isExpanded = EditorGUI.Foldout(foldoutarea.Pad(14, 0, 0, 0), materials.isExpanded, $"Materials", true);
            EditorGUI.LabelField(weightArea, "Weight");
            // EditorGUI.LabelField(applyArea, "");
        }

        private void OnDrawMaterialElement(Rect rect, int index, bool isActive, bool isFocused) {
            var matblock = materials.GetArrayElementAtIndex(index);
            var weight = matblock.FindPropertyRelative("weight");
            var material = matblock.FindPropertyRelative("material");

            rect.Pad(0, 0, 2, 2).SplitHorizontal(out Rect matArea, out Rect weightArea, out Rect applyArea, 1f - (160f / rect.width), 80f / rect.width, 2f);

            EditorGUI.PropertyField(matArea, material, GUIContent.none);
            EditorGUI.PropertyField(weightArea, weight, GUIContent.none);

            if(GUI.Button(applyArea, "Apply")) {
                var vm = target as VariantMaterial;
                vm.SetVariant(index);
            }
        }

        #endregion

        public override void OnInspectorGUI() {
            if(targets != null && targets.Length > 1) {
                var vm = target as VariantMaterial;
                using(new EditorGUILayout.VerticalScope("box")) {
                    for(int i = 0, length = vm.VariantCount; i < length; i++) {
                        var variant = vm.GetVariant(i);
                        if(variant == null)
                            continue;
                        using(new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField(variant.name);
                            if(GUILayout.Button("Apply", GUILayout.Width(80))) {
                                targets.Select(x => x as VariantMaterial).Where(x => x != null && x.GetVariant(i) == variant).Foreach(x => x.SetVariant(i));
                            }
                        }
                    }
                }
            }
            else {
                serializedObject.Update();
                var vm = target as VariantMaterial;
                using(new EditorGUILayout.HorizontalScope("box")) {
                    if(GUILayout.Button("Apply Random", GUILayout.Width(120f))) {
                        vm.SetVariant();
                    }
                    if(GUILayout.Button("Bake Renderers", GUILayout.Width(120f))) {
                        var childRenderers = vm.GetComponentsInChildren<Renderer>();
                        renderers.arraySize = childRenderers.Length;
                        for(int i = 0, length = childRenderers.Length; i < length; i++) {
                            renderers.GetArrayElementAtIndex(i).objectReferenceValue = childRenderers[i];
                        }
                    }
                }

                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(activateAtAwake);
                }

                if(materials.isExpanded) {
                    if(materialList != null)
                        materialList.DoLayoutList();
                }
                else {
                    var headerArea = GUILayoutUtility.GetRect(1, materialList.headerHeight + 2);
                    OnDrawMaterialHeader(headerArea);
                }

                if(renderers.isExpanded) {
                    if(rendererList != null)
                        rendererList.DoLayoutList();
                }
                else {
                    var headerArea = GUILayoutUtility.GetRect(1, materialList.headerHeight + 2);
                    OnDrawRendererHeader(headerArea);
                }

                if(serializedObject.hasModifiedProperties)
                    serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
