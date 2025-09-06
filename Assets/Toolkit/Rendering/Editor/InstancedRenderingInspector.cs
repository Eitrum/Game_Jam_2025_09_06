using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace Toolkit.Rendering
{
    [CustomEditor(typeof(InstancedRendering))]
    public class InstancedRenderingInspector : Editor
    {
        private SerializedProperty updateMode;
        private SerializedProperty layer;
        private SerializedProperty receiveShadows;
        private SerializedProperty shadowCasting;
        private SerializedProperty data;
        private SerializedProperty defaultDrawAmount;
        private ReorderableList list;
        private int pageIndex = 0;

        private static bool sceneEditorEnabled = false;
        private static Vector3 sceneEditLocation = new Vector3(16, 16, 16);
        private static float sceneEditRadius = 3f;

        private void OnEnable() {
            updateMode = serializedObject.FindProperty("updateMode");
            layer = serializedObject.FindProperty("layer");
            receiveShadows = serializedObject.FindProperty("receiveShadows");
            shadowCasting = serializedObject.FindProperty("shadowCasting");
            data = serializedObject.FindProperty("data");
            defaultDrawAmount = serializedObject.FindProperty("defaultDrawAmount");
            list = new ReorderableList(serializedObject, data);
            list.displayAdd = false;
            list.displayRemove = false;
            list.headerHeight = 0f;
            list.draggable = false;

            pageIndex = 0;
            list.elementHeightCallback += (int index) => EditorGUI.GetPropertyHeight(data.GetArrayElementAtIndex(index));
            list.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) => { using(new EditorGUI.IndentLevelScope(1)) EditorGUI.PropertyField(rect, data.GetArrayElementAtIndex(index)); };
        }

        private void OnSceneGUI() {
            if(!sceneEditorEnabled)
                return;

            sceneEditLocation = Handles.PositionHandle(sceneEditLocation, Quaternion.identity);
            Handles.DrawWireDisc(sceneEditLocation, Vector3.up, sceneEditRadius);
            Handles.DrawWireDisc(sceneEditLocation, Vector3.right, sceneEditRadius);
            Handles.DrawWireDisc(sceneEditLocation, Vector3.forward, sceneEditRadius);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var ir = target as InstancedRendering;
            EditorGUILayout.PropertyField(updateMode);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(layer);
                EditorGUILayout.PropertyField(receiveShadows);
                EditorGUILayout.PropertyField(shadowCasting);
                if(!Application.isPlaying)
                    EditorGUILayout.PropertyField(defaultDrawAmount);
                else {
                    EditorGUI.BeginChangeCheck();
                    var val = EditorGUILayout.Slider("Draw Amount", ir.DrawAmount, 0f, 1f);
                    if(EditorGUI.EndChangeCheck()) {
                        ir.DrawAmount = val;
                    }
                }
            }
            EditorGUILayout.Space();
            using(new EditorGUILayout.VerticalScope("box")) {
                var pages = ((data.arraySize - 1) / 8 + 1);
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("<", GUILayout.Width(80f))) {
                        pageIndex--;
                        if(pageIndex < 0)
                            pageIndex = 0;
                    }
                    GUILayout.Label($"{pageIndex + 1} / {pages}", EditorStylesUtility.CenterAlignedBoldLabel);
                    if(GUILayout.Button(">", GUILayout.Width(80f))) {
                        pageIndex++;
                        if(pageIndex >= pages)
                            pageIndex = pages - 1;
                    }
                }

                var length = Mathf.Min(pageIndex * 8 + 8, data.arraySize);
                for(int i = pageIndex * 8; i < length; i++) {
                    var obj = data.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(obj);
                }
            }
            EditorGUILayout.Space();
            // list.DoLayoutList();

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Bake", GUILayout.Width(80))) {
                    RenderingBakeUtility.BakeInstanceDataArray(data);
                }
                if(GUILayout.Button("Clear", GUILayout.Width(80)) && EditorUtility.DisplayDialog("Clear", "Do you want to clear all the data inside this component", "Yes", "Cancel! Panic!")) {
                    data.arraySize = 0;
                }
                if(GUILayout.Button("Toggle Scene Editing", GUILayout.Width(160))) {
                    sceneEditorEnabled = !sceneEditorEnabled;
                    if(sceneEditorEnabled) {
                        var ray = SceneView.lastActiveSceneView.camera.transform.ToRay();
                        if(ray.Hit(out RaycastHit hit)) {
                            sceneEditLocation = hit.point;
                        }
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            if(sceneEditorEnabled) {
                SceneView.RepaintAll();
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Scene Editing", EditorStylesUtility.BoldLabel);
                    EditorGUILayout.LabelField("Position", sceneEditLocation.ToString());
                    sceneEditRadius = EditorGUILayout.Slider("Radius", sceneEditRadius, 0f, 10f);
                    if(GUILayout.Button("Clear inside area", GUILayout.Width(120))) {
                        ir.RemoveAt(sceneEditLocation, sceneEditRadius);
                        serializedObject.Update();
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }

        }
    }
}
