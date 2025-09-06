using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using Toolkit.Unit;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(RarityDistribution))]
    public class RarityDistributionEditor : Editor
    {
        private SerializedProperty levelBased;
        private SerializedProperty weightProp;
        private ReorderableList list;
        private int rarityCount;

        private void OnEnable() {
            weightProp = serializedObject.FindProperty("weight");
            levelBased = serializedObject.FindProperty("levelBased");
            list = new ReorderableList(serializedObject, weightProp);
            list.displayAdd = false;
            list.displayRemove = false;
            list.drawElementCallback += OnDrawElement;
            list.drawHeaderCallback += OnDrawHeader;
            rarityCount = Rarity.None.GetLength() - 1;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect body, 40f / rect.width, 4f);
            var element = weightProp.GetArrayElementAtIndex(index);
            var total = element.FindPropertyRelative("total");
            var totalVal = total.floatValue;
            EditorGUI.LabelField(indexArea, levelBased.boolValue ? ($"{0 + index}") : ($"{index}"));
            var splits = body.SplitHorizontal(rarityCount, 2f);
            var totalRes = 0f;
            for(int i = 0; i < rarityCount; i++) {
                element.Next(i == 0);
                EditorGUI.PropertyField(splits[i], element, GUIContent.none);
                var val = element.floatValue;
                totalRes += val;
                EditorGUI.LabelField(splits[i], $"{(val / totalVal) * 100f:0.0#}%", EditorStylesUtility.RightAlignedGrayMiniLabel);
            }
            total.floatValue = totalRes;
        }

        private void OnDrawHeader(Rect rect) {
            rect.PadRef(12f, 0, 0, 0);
            rect.SplitHorizontal(out Rect indexArea, out Rect body, 40f / rect.width, 4f);
            var splits = body.SplitHorizontal(rarityCount, 2f);
            var isLevel = levelBased.boolValue;
            EditorGUI.LabelField(indexArea, isLevel ? "LEVEL" : "#");
            for(int i = 0; i < rarityCount; i++) {
                EditorGUI.LabelField(splits[i], RarityUtility.ToString((Rarity)i + 1));
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(levelBased);
                if(levelBased.boolValue && weightProp.arraySize != ExperienceUtility.Levels) {
                    weightProp.arraySize = ExperienceUtility.Levels;
                }
                list.displayAdd = !levelBased.boolValue;
                list.displayRemove = !levelBased.boolValue;


                GUILayout.FlexibleSpace();

                using(new EditorGUI.DisabledScope(string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))) {
                    if(GUILayout.Button("Paste", GUILayout.Width(80f))) {
                        var paste = EditorGUIUtility.systemCopyBuffer;
                        var rows = paste.Split('\n');
                        var count = rows.Length;
                        if(!levelBased.boolValue)
                            weightProp.arraySize = count;

                        for(int i = 0, length = Mathf.Min(weightProp.arraySize, count); i < length; i++) {
                            var ele = weightProp.GetArrayElementAtIndex(i);
                            var pastas = rows[i].Split('\t', ' ');
                            for(int x = 0; x < pastas.Length; x++) {
                                ele.Next(x == 0);
                                if(float.TryParse(pastas[x], out float res))
                                    ele.floatValue = res;
                            }
                        }
                    }
                }
            }

            list.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
