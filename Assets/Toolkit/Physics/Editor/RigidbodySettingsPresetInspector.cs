using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(RigidbodySettingsPreset))]
    public class RigidbodySettingsPresetInspector : Editor
    {
        #region Variables

        private SerializedProperty mass;

        private SerializedProperty drag;
        private SerializedProperty angularDrag;

        private SerializedProperty useGravity;
        private SerializedProperty isKinematic;
        private SerializedProperty detectCollisions;

        private SerializedProperty interpolation;
        private SerializedProperty collisionDetection;
        private SerializedProperty constraints;

        #endregion

        #region Init

        private void OnEnable() {
            mass = serializedObject.FindProperty("mass");
            drag = serializedObject.FindProperty("drag");
            angularDrag = serializedObject.FindProperty("angularDrag");

            useGravity = serializedObject.FindProperty("useGravity");
            isKinematic = serializedObject.FindProperty("isKinematic");
            detectCollisions = serializedObject.FindProperty("detectCollisions");

            interpolation = serializedObject.FindProperty("interpolation");
            collisionDetection = serializedObject.FindProperty("collisionDetection");
            constraints = serializedObject.FindProperty("constraints");
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Options", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(mass);
                EditorGUILayout.PropertyField(drag);
                EditorGUILayout.PropertyField(angularDrag);
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Interaction", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(useGravity);
                EditorGUILayout.PropertyField(isKinematic);
                EditorGUILayout.PropertyField(detectCollisions);
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                int constraintsMask = constraints.intValue;
                EditorGUILayout.LabelField($"Constraints = {GetConstraintsName(constraintsMask)}");
                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Space(15f);
                    var allPos = constraintsMask.HasFlag(14);
                    var newPos = EditorGUILayout.ToggleLeft("Position", allPos, GUILayout.Width(150f));
                    if(allPos != newPos)
                        constraintsMask = newPos ? (constraintsMask | 14) : (constraintsMask & ~14);

                    constraintsMask = EditorGUILayout.ToggleLeft("X", constraintsMask.HasFlag(2), GUILayout.Width(30f)) ? (constraintsMask | 2) : (constraintsMask & ~2);
                    constraintsMask = EditorGUILayout.ToggleLeft("Y", constraintsMask.HasFlag(4), GUILayout.Width(30f)) ? (constraintsMask | 4) : (constraintsMask & ~4);
                    constraintsMask = EditorGUILayout.ToggleLeft("Z", constraintsMask.HasFlag(8), GUILayout.Width(30f)) ? (constraintsMask | 8) : (constraintsMask & ~8);
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Space(15f);
                    var allRot = constraintsMask.HasFlag(112);
                    var newRot = EditorGUILayout.ToggleLeft("Rotation", allRot, GUILayout.Width(150f));
                    if(allRot != newRot)
                        constraintsMask = newRot ? (constraintsMask | 112) : (constraintsMask & ~112);

                    constraintsMask = EditorGUILayout.ToggleLeft("X", constraintsMask.HasFlag(16), GUILayout.Width(30f)) ? (constraintsMask | 16) : (constraintsMask & ~16);
                    constraintsMask = EditorGUILayout.ToggleLeft("Y", constraintsMask.HasFlag(32), GUILayout.Width(30f)) ? (constraintsMask | 32) : (constraintsMask & ~32);
                    constraintsMask = EditorGUILayout.ToggleLeft("Z", constraintsMask.HasFlag(64), GUILayout.Width(30f)) ? (constraintsMask | 64) : (constraintsMask & ~64);
                }
                if(EditorGUI.EndChangeCheck()) {
                    constraints.intValue = constraintsMask;
                }
            }
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Quality", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(interpolation);
                EditorGUILayout.PropertyField(collisionDetection);
            }
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        public static string GetConstraintsName(int mask) {
            return $"Pos[{(mask.HasFlag(2) ? "X" : "-")}{(mask.HasFlag(4) ? "Y" : "-")}{(mask.HasFlag(8) ? "Z" : "-")}] Rot[{(mask.HasFlag(16) ? "X" : "-")}{(mask.HasFlag(32) ? "Y" : "-")}{(mask.HasFlag(64) ? "Z" : "-")}]";
        }

        #endregion
    }
}
