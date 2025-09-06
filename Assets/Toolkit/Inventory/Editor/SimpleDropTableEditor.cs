using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit.Inventory
{
    [CustomEditor(typeof(SimpleDropTable))]
    public class SimpleDropTableEditor : Editor
    {
        #region Variables

        private SerializedProperty overrideName;
        private SerializedProperty description;
        private SerializedProperty commonTableProp;
        private SerializedProperty rareTableAmountProp;
        private SerializedProperty rareTableWeightProp;
        private SerializedProperty rareTableProp;
        private SerializedProperty uniqueDropsProp;

        private ReorderableList commonList;
        private ReorderableList rareList;
        private float totalWeight = 0f;
        private float rareItemsDrops = 0f;

        #endregion

        #region Init

        private void OnEnable() {
            overrideName = serializedObject.FindProperty("overrideName");
            description = serializedObject.FindProperty("description");
            commonTableProp = serializedObject.FindProperty("commonDropTable");
            uniqueDropsProp = serializedObject.FindProperty("uniqueDrops");
            rareTableAmountProp = serializedObject.FindProperty("rareDropTableAmount");
            rareTableWeightProp = serializedObject.FindProperty("rareDropTableWeight");
            rareTableProp = serializedObject.FindProperty("rareDropTable");

            commonList = new ReorderableList(serializedObject, commonTableProp);
            commonList.drawElementCallback += OnCommonList;
            commonList.elementHeight = EditorGUIUtility.singleLineHeight * 3f + 10f;
            commonList.onAddCallback += OnCommonAdd;
            commonList.headerHeight = 0f;

            rareList = new ReorderableList(serializedObject, rareTableProp);
            rareList.headerHeight = 0f;
            rareList.onAddCallback += OnRareAdd;
            rareList.drawElementCallback += OnRareList;
            rareList.elementHeight = EditorGUIUtility.singleLineHeight * 3f + 10f;
        }

        #endregion

        #region Rare List

        private void OnRareList(Rect rect, int index, bool isActive, bool isFocused) {
            var element = rareTableProp.GetArrayElementAtIndex(index);
            GUI.Box(rect, "");
            rect.PadRef(4f, 0f, 6f, 2f);
            rect.SplitVertical(out Rect percentageArea, out Rect countArea, out Rect objectReferenceArea, 0.333f, 0.333f, 1f);
            percentageArea.SplitHorizontal(out Rect pArea, out Rect rArea, 0.65f, 2f);
            var minmaxProp = element.FindPropertyRelative("range");
            var weightProp = element.FindPropertyRelative("rangeWeight");
            var minmax = new MinMaxInt(minmaxProp.FindPropertyRelative("min").intValue, minmaxProp.FindPropertyRelative("max").intValue);
            var amount = ItemAmountDropChance(minmax, weightProp.animationCurveValue, 0.01f);

            EditorGUI.PropertyField(pArea, element.FindPropertyRelative("percentage"), new GUIContent("Weight"));
            EditorGUI.LabelField(rArea, $"{((element.FindPropertyRelative("percentage").floatValue / totalWeight) * 100f):0.00##}% (amount: {amount:0.0#})", EditorStylesUtility.CenterAlignedBoldLabel);
            countArea.SplitHorizontal(out Rect rangeArea, out Rect weightArea, 0.65f, 4f);
            EditorGUI.PropertyField(rangeArea, minmaxProp);
            EditorGUI.PropertyField(weightArea, weightProp, GUIContent.none);
            EditorGUI.PropertyField(objectReferenceArea, element.FindPropertyRelative("item"));
        }

        private void OnRareAdd(ReorderableList list) {
            rareTableProp.arraySize++;
            var element = rareTableProp.GetArrayElementAtIndex(rareTableProp.arraySize - 1);
            element.FindPropertyRelative("rangeWeight").animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            element.FindPropertyRelative("range").FindPropertyRelative("min").intValue = 1;
            element.FindPropertyRelative("range").FindPropertyRelative("max").intValue = 1;
        }

        private static float CalculateTotalWeight(SerializedProperty table) {
            float res = 0f;
            var size = table.arraySize;
            for(int i = 0; i < size; i++) {
                var ele = table.GetArrayElementAtIndex(i);
                res += ele.FindPropertyRelative("percentage").floatValue;
            }
            return res;
        }

        #endregion

        #region Common List

        private void OnCommonAdd(ReorderableList list) {
            commonTableProp.arraySize++;
            var element = commonTableProp.GetArrayElementAtIndex(commonTableProp.arraySize - 1);
            element.FindPropertyRelative("rangeWeight").animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            element.FindPropertyRelative("range").FindPropertyRelative("min").intValue = 1;
            element.FindPropertyRelative("range").FindPropertyRelative("max").intValue = 1;
        }

        private void OnCommonList(Rect rect, int index, bool isActive, bool isFocused) {
            var element = commonTableProp.GetArrayElementAtIndex(index);
            GUI.Box(rect, "");
            rect.PadRef(4f, 0f, 6f, 2f);
            rect.SplitVertical(out Rect percentageArea, out Rect countArea, out Rect objectReferenceArea, 0.333f, 0.333f, 1f);

            EditorGUI.Slider(percentageArea, element.FindPropertyRelative("percentage"), 0f, 1f);
            countArea.SplitHorizontal(out Rect rangeArea, out Rect weightArea, 0.65f, 4f);
            EditorGUI.PropertyField(rangeArea, element.FindPropertyRelative("range"));
            EditorGUI.PropertyField(weightArea, element.FindPropertyRelative("rangeWeight"), GUIContent.none);
            EditorGUI.PropertyField(objectReferenceArea, element.FindPropertyRelative("item"));

        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            var name = EditorGUILayout.TextField(overrideName.stringValue);
            var r = GUILayoutUtility.GetLastRect();
            if(string.IsNullOrEmpty(name)) {
                EditorGUI.LabelField(r, "Name: " + target.name, EditorStylesUtility.GrayItalicLabel);
            }
            if(EditorGUI.EndChangeCheck()) {
                overrideName.stringValue = name;
            }
            description.stringValue = EditorGUILayout.TextField("Description", description.stringValue);

            totalWeight = CalculateTotalWeight(rareTableProp);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Default", EditorStyles.boldLabel);
                commonList.DoLayoutList();
            }
            EditorGUILayout.Space();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Weighted", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(rareTableAmountProp);
                EditorGUILayout.PropertyField(rareTableWeightProp);
                EditorGUILayout.PropertyField(uniqueDropsProp);
                EditorGUILayout.Space();
                rareItemsDrops = ItemAmountDropChance(new MinMaxInt(rareTableAmountProp.FindPropertyRelative("min").intValue, rareTableAmountProp.FindPropertyRelative("max").intValue), rareTableWeightProp.animationCurveValue, 0.01f);
                EditorGUILayout.LabelField("Average rare drops", $"{rareItemsDrops:0.0##}", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                rareList.DoLayoutList();
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Calculations

        public float ItemAmountDropChance(MinMaxInt minMax, AnimationCurve weight, float accuracy = 0.02f) {
            if(accuracy < Mathf.Epsilon)
                accuracy = 0.001f;

            float drops = 0f;

            for(float val = 0; val <= 1f; val += accuracy) {
                drops += minMax.Evaluate(weight.Evaluate(val));
            }
            return drops * accuracy;
        }

        #endregion
    }
}
