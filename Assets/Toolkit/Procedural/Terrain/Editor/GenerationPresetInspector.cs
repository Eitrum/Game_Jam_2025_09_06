using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CustomEditor(typeof(GenerationPreset))]
    public class GenerationPresetInspector : Editor
    {
        private SerializedProperty seed;
        private SerializedProperty steps;
        private ReorderableList list;

        private Data generationData;
        private int step = 0;

        private void OnEnable() {
            seed = serializedObject.FindProperty("seed");
            steps = serializedObject.FindProperty("steps");
            list = new ReorderableList(serializedObject, steps);
            list.drawHeaderCallback += OnDrawHeader;
            list.drawElementCallback += OnDrawElement;
            list.elementHeight = EditorGUIUtility.singleLineHeight + 4f;
        }

        private void OnDrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Steps", EditorStyles.boldLabel);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var element = steps.GetArrayElementAtIndex(index);
            element.objectReferenceValue = EditorGUI.ObjectField(rect.Pad(0, 0, 2, 2), GUIContent.none, element.objectReferenceValue, typeof(IProceduralTerrainGeneration), false);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(seed);
            EditorGUILayout.Space();
            list.DoLayoutList();

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Apply To Terrain", GUILayout.Width(120))) {
                    var terrain = GameObject.FindFirstObjectByType<UnityEngine.Terrain>();
                    if(terrain != null) {
                        if(EditorUtility.DisplayDialog("Generate", $"Generate Terrain on '{terrain.name}'", "Apply", "Cancel")) {
                            var genPreset = target as GenerationPreset;
                            genPreset.Generate(terrain);
                        }
                    }
                    else {
                        EditorUtility.DisplayDialog("Generate Error", "No Terrain to generate on, please add a new terrain object in the scene", "Ok");
                    }
                    generationData = null;
                }

                if(GUILayout.Button("Step Generate", GUILayout.Width(120))) {
                    var terrain = GameObject.FindFirstObjectByType<UnityEngine.Terrain>();
                    if(terrain != null) {
                        generationData = new Data(terrain, seed.intValue);
                        step = 0;
                    }
                    else {
                        EditorUtility.DisplayDialog("Generate Error", "No Terrain to generate on, please add a new terrain object in the scene", "Ok");
                    }
                }

                if(GUILayout.Button("Step 1 Random Seed", GUILayout.Width(120))) {
                    seed.intValue = Toolkit.Mathematics.Random.Int;
                    var terrain = GameObject.FindFirstObjectByType<UnityEngine.Terrain>();
                    if(terrain != null) {
                        var preset = target as GenerationPreset;
                        generationData = new Data(terrain, seed.intValue);
                        preset[0].Generate(generationData);
                        generationData.Apply(terrain);
                        step = 1;
                    }
                    else {
                        EditorUtility.DisplayDialog("Generate Error", "No Terrain to generate on, please add a new terrain object in the scene", "Ok");
                    }
                }
            }

            if(generationData != null) {
                using(new EditorGUILayout.HorizontalScope("box")) {
                    var preset = target as GenerationPreset;
                    bool update = false;
                    EditorGUILayout.LabelField($"Generation [{step}/{preset.StepCount}]");
                    using(new EditorGUI.DisabledScope(step >= preset.StepCount)) {
                        if(GUILayout.Button("Next", GUILayout.Width(120))) {
                            preset[step].Generate(generationData);
                            step++;
                            update = true;
                        }
                    }
                    if(update) {
                        var terrain = GameObject.FindFirstObjectByType<UnityEngine.Terrain>();
                        if(terrain != null) {
                            generationData.Apply(terrain);
                        }
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
