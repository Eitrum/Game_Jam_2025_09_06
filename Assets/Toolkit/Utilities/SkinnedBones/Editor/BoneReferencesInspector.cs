using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(BoneReferences))]
    public class BoneReferencesInspector : Editor
    {
        private static bool debugDrawBones = false;
        private static bool debugDrawLabels = false;

        public override void OnInspectorGUI() {
            var br = target as BoneReferences;
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUI.BeginChangeCheck();
                debugDrawBones = EditorGUILayout.Toggle($"Draw Bones", debugDrawBones);
                if(debugDrawBones)
                    using(new EditorGUI.IndentLevelScope(1))
                        debugDrawLabels = EditorGUILayout.Toggle("Draw Labels", debugDrawLabels);
                if(EditorGUI.EndChangeCheck()) {
                    SceneView.RepaintAll();
                }
            }

            if(debugDrawBones)
                using(new EditorGUILayout.VerticalScope("box"))
                    RecursiveDrawBone(br.transform);
        }

        private void RecursiveDrawBone(Transform bone) {
            EditorGUILayout.LabelField($"{bone.name} : {bone.name.GetHash32()}");
            EditorGUI.indentLevel++;
            var childCount = bone.childCount;
            for(int i = 0; i < childCount; i++) {
                RecursiveDrawBone(bone.GetChild(i));
            }
            EditorGUI.indentLevel--;
        }

        private void OnSceneGUI() {
            var br = target as BoneReferences;
            if(debugDrawBones) {
                DrawLinesRecursive(br.transform);
            }
        }

        private void DrawLinesRecursive(Transform root) {
            var pos = root.position;
            if(debugDrawLabels)
                Handles.Label(pos, $"{root.name}", EditorStyles.label);
            var childCount = root.childCount;
            for(int i = 0; i < childCount; i++) {
                var child = root.GetChild(i);
                var cpos = child.position;
                Handles.DrawLine(pos, cpos);
                DrawLinesRecursive(child);
            }
        }
    }
}
