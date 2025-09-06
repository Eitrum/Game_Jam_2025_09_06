using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.PhysicEx
{
    [CustomPropertyDrawer(typeof(RigidbodySettings))]
    public class RigidbodySettingsPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 14f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var foldoutArea = new Rect(position);
            foldoutArea.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldoutArea, property.isExpanded, label, true);

            if(property.isExpanded) {
                position = position.Pad(0, 0, EditorGUIUtility.singleLineHeight, 0);
                GUI.Box(position, "");
                using(new EditorGUI.IndentLevelScope(1)) {
                    var lines = position.Pad(0, 0, 1, 4).SplitVertical(14, 2f);
                    EditorGUI.PropertyField(lines[0], property.FindPropertyRelative("mass"));
                    EditorGUI.PropertyField(lines[1], property.FindPropertyRelative("drag"));
                    EditorGUI.PropertyField(lines[2], property.FindPropertyRelative("angularDrag"));

                    EditorGUI.PropertyField(lines[4], property.FindPropertyRelative("useGravity"));
                    EditorGUI.PropertyField(lines[5], property.FindPropertyRelative("isKinematic"));
                    EditorGUI.PropertyField(lines[6], property.FindPropertyRelative("detectCollisions"));

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.LabelField(lines[8], "Constraints");
                    var constraints = property.FindPropertyRelative("constraints");
                    int constraintsMask = constraints.intValue;
                    using(new EditorGUI.IndentLevelScope(1)) {
                        EditorGUI.IndentedRect(lines[9]).SplitHorizontal(out Rect positionRect, out Rect positionTogglesArea, 0.35f, 2f);
                        EditorGUI.IndentedRect(lines[10]).SplitHorizontal(out Rect rotationRect, out Rect rotationTogglesArea, 0.35f, 2f);
                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        positionTogglesArea.width = 30;
                        rotationTogglesArea.width = 30;

                        var allPos = constraintsMask.HasFlag(14);
                        var newPos = EditorGUI.ToggleLeft(positionRect, "Position", allPos);
                        if(allPos != newPos)
                            constraintsMask = newPos ? (constraintsMask | 14) : (constraintsMask & ~14);

                        constraintsMask = EditorGUI.ToggleLeft(positionTogglesArea, "X", constraintsMask.HasFlag(2)) ? (constraintsMask | 2) : (constraintsMask & ~2);
                        constraintsMask = EditorGUI.ToggleLeft(positionTogglesArea.Pad(30, -30, 0, 0), "Y", constraintsMask.HasFlag(4)) ? (constraintsMask | 4) : (constraintsMask & ~4);
                        constraintsMask = EditorGUI.ToggleLeft(positionTogglesArea.Pad(60, -60, 0, 0), "Z", constraintsMask.HasFlag(8)) ? (constraintsMask | 8) : (constraintsMask & ~8);


                        var allRot = constraintsMask.HasFlag(112);
                        var newRot = EditorGUI.ToggleLeft(rotationRect, "Rotation", allRot);
                        if(allRot != newRot)
                            constraintsMask = newRot ? (constraintsMask | 112) : (constraintsMask & ~112);

                        constraintsMask = EditorGUI.ToggleLeft(rotationTogglesArea, "X", constraintsMask.HasFlag(16)) ? (constraintsMask | 16) : (constraintsMask & ~16);
                        constraintsMask = EditorGUI.ToggleLeft(rotationTogglesArea.Pad(30, -30, 0, 0), "Y", constraintsMask.HasFlag(32)) ? (constraintsMask | 32) : (constraintsMask & ~32);
                        constraintsMask = EditorGUI.ToggleLeft(rotationTogglesArea.Pad(60, -60, 0, 0), "Z", constraintsMask.HasFlag(64)) ? (constraintsMask | 64) : (constraintsMask & ~64);

                        EditorGUI.indentLevel = indent;
                    }
                    if(EditorGUI.EndChangeCheck()) {
                        constraints.intValue = constraintsMask;
                    }

                    EditorGUI.PropertyField(lines[12], property.FindPropertyRelative("interpolation"));
                    EditorGUI.PropertyField(lines[13], property.FindPropertyRelative("collisionDetection"));
                }
            }
        }

        public static string GetConstraintsName(int mask) {
            return $"Pos[{(mask.HasFlag(2) ? "X" : "-")}{(mask.HasFlag(4) ? "Y" : "-")}{(mask.HasFlag(8) ? "Z" : "-")}] Rot[{(mask.HasFlag(16) ? "X" : "-")}{(mask.HasFlag(32) ? "Y" : "-")}{(mask.HasFlag(64) ? "Z" : "-")}]";
        }
    }
}
