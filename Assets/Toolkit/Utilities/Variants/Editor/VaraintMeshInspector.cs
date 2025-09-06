using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(VariantMesh))]
    [CanEditMultipleObjects]
    public class VaraintMeshInspector : Editor
    {

        private SerializedProperty activateAtAwake;
        private SerializedProperty meshes;
        private SerializedProperty meshFilters;
        private SerializedProperty perObject;
        private ReorderableList meshesList;
        private ReorderableList meshFilterList;

        private void OnEnable() {
            activateAtAwake = serializedObject.FindProperty("activateAtAwake");
            meshes = serializedObject.FindProperty("meshes");
            meshFilters = serializedObject.FindProperty("meshFilters");
            perObject = serializedObject.FindProperty("perObject");

            meshesList = new ReorderableList(serializedObject, meshes);
            meshesList.drawHeaderCallback += OnDrawMeshHeader;
            meshesList.drawElementCallback += OnDrawMeshElement;

            meshFilterList = new ReorderableList(serializedObject, meshFilters);
            meshFilterList.drawHeaderCallback += OnDrawMeshFiltersHeader;
            meshFilterList.drawElementCallback += OnDrawMeshFiltersElement;
        }

        #region Renderer Draw

        private void OnDrawMeshFiltersHeader(Rect rect) {
            meshFilters.isExpanded = EditorGUI.Foldout(rect.Pad(14, 0, 0, 0), meshFilters.isExpanded, $"Mesh Filters", true);
        }

        private void OnDrawMeshFiltersElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect.Pad(0, 0, 2, 2), meshFilters.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Material Draw

        private void OnDrawMeshHeader(Rect rect) {
            rect.SplitHorizontal(out Rect foldoutarea, out Rect weightArea, out Rect applyArea, 1f - (160f / rect.width), 80f / rect.width, 2f);
            meshes.isExpanded = EditorGUI.Foldout(foldoutarea.Pad(14, 0, 0, 0), meshes.isExpanded, $"Meshes", true);
            EditorGUI.LabelField(weightArea, "Weight");
        }

        private void OnDrawMeshElement(Rect rect, int index, bool isActive, bool isFocused) {
            var matblock = meshes.GetArrayElementAtIndex(index);
            var weight = matblock.FindPropertyRelative("weight");
            var mesh = matblock.FindPropertyRelative("mesh");

            rect.Pad(0, 0, 2, 2).SplitHorizontal(out Rect matArea, out Rect weightArea, out Rect applyArea, 1f - (160f / rect.width), 80f / rect.width, 2f);

            EditorGUI.PropertyField(matArea, mesh, GUIContent.none);
            EditorGUI.PropertyField(weightArea, weight, GUIContent.none);

            if(GUI.Button(applyArea, "Apply")) {
                var vm = target as VariantMesh;
                vm.SetVariant(index);
            }
        }

        #endregion

        public override void OnInspectorGUI() {
            if(targets != null && targets.Length > 1) {
                var vm = target as VariantMesh;
                using(new EditorGUILayout.VerticalScope("box")) {
                    for(int i = 0, length = vm.VariantCount; i < length; i++) {
                        var variant = vm.GetVariant(i);
                        if(variant == null)
                            continue;
                        using(new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField(variant.name);
                            if(GUILayout.Button("Apply", GUILayout.Width(80))) {
                                targets.Select(x => x as VariantMesh).Where(x => x != null && x.GetVariant(i) == variant).Foreach(x => x.SetVariant(i));
                            }
                        }
                    }
                }
            }
            else {
                serializedObject.Update();
                var vm = target as VariantMesh;
                using(new EditorGUILayout.HorizontalScope("box")) {
                    if(GUILayout.Button("Apply Random", GUILayout.Width(120f))) {
                        vm.SetVariant();
                    }
                    if(GUILayout.Button("Bake Mesh Filters", GUILayout.Width(120f))) {
                        var childRenderers = vm.GetComponentsInChildren<MeshFilter>();
                        meshFilters.arraySize = childRenderers.Length;
                        for(int i = 0, length = childRenderers.Length; i < length; i++) {
                            meshFilters.GetArrayElementAtIndex(i).objectReferenceValue = childRenderers[i];
                        }
                    }
                }

                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(activateAtAwake);
                    EditorGUILayout.PropertyField(perObject);
                }

                if(meshes.isExpanded) {
                    if(meshesList != null)
                        meshesList.DoLayoutList();
                }
                else {
                    var headerArea = GUILayoutUtility.GetRect(1, meshesList.headerHeight + 2);
                    OnDrawMeshHeader(headerArea);
                }

                if(meshFilters.isExpanded) {
                    if(meshFilterList != null)
                        meshFilterList.DoLayoutList();
                }
                else {
                    var headerArea = GUILayoutUtility.GetRect(1, meshesList.headerHeight + 2);
                    OnDrawMeshFiltersHeader(headerArea);
                }

                if(serializedObject.hasModifiedProperties)
                    serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
