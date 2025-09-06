using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

namespace Toolkit.Procedural.Names
{
    [CustomEditor(typeof(SimpleNameGenerator))]
    public class SimpleNameGeneratorInspector : Editor
    {
        private enum Mode
        {
            Prefix,
            First,
            Middle,
            Last,
            Suffix
        }

        #region Variables

        private Mode mode = Mode.Prefix;
        private static GUIContent probabilityContent = new GUIContent("Probability");
        private string genName = "";

        private SerializedProperty prefixProbability = null;
        private SerializedProperty prefixNames = null;
        private ReorderableList prefixList = null;

        private SerializedProperty firstName;
        private SerializedProperty firstNames;
        private ReorderableList firstNamesList;

        private SerializedProperty middleNameCount;
        private SerializedProperty middleNameCountWeight;
        private SerializedProperty middleNames;
        private ReorderableList middleNamesList;

        private SerializedProperty lastNameCount;
        private SerializedProperty lastNameCountWeight;
        private SerializedProperty lastNames;
        private ReorderableList lastNamesList;

        private SerializedProperty suffixProbability = null;
        private SerializedProperty suffixNames = null;
        private ReorderableList suffixList = null;

        #endregion

        private void OnEnable() {
            prefixProbability = serializedObject.FindProperty("prefixProbability");
            prefixNames = serializedObject.FindProperty("prefixNames");
            prefixList = new ReorderableList(serializedObject, prefixNames);
            prefixList.drawElementCallback += PrefixElement;
            prefixList.headerHeight = 0f;

            firstName = serializedObject.FindProperty("firstName");
            firstNames = serializedObject.FindProperty("firstNames");
            firstNamesList = new ReorderableList(serializedObject, firstNames);
            firstNamesList.drawElementCallback += FirstNameElement;
            firstNamesList.headerHeight = 0f;

            middleNameCount = serializedObject.FindProperty("middleNameCount");
            middleNameCountWeight = serializedObject.FindProperty("middleNameCountWeight");
            middleNames = serializedObject.FindProperty("middleNames");
            middleNamesList = new ReorderableList(serializedObject, middleNames);
            middleNamesList.drawElementCallback += MiddleNameElement;
            middleNamesList.headerHeight = 0f;

            lastNameCount = serializedObject.FindProperty("lastNameCount");
            lastNameCountWeight = serializedObject.FindProperty("lastNameCountWeight");
            lastNames = serializedObject.FindProperty("lastNames");
            lastNamesList = new ReorderableList(serializedObject, lastNames);
            lastNamesList.drawElementCallback += LastNameElement;
            lastNamesList.headerHeight = 0f;

            suffixProbability = serializedObject.FindProperty("suffixProbability");
            suffixNames = serializedObject.FindProperty("suffixNames");
            suffixList = new ReorderableList(serializedObject, suffixNames);
            suffixList.drawElementCallback += SuffixElement;
            suffixList.headerHeight = 0f;
        }

        public override void OnInspectorGUI() {
            var header = GUILayoutUtility.GetRect(1, 20);
            var buttons = header.SplitHorizontal(5, 2f);
            var ev = Event.current;

            EditorGUI.DrawRect(header, Color.black);
            for(int i = 0; i < buttons.Length; i++) {
                var mode = (Mode)i;
                var enabled = this.mode == mode;
                EditorGUI.DrawRect(buttons[i], enabled ? ColorTable.DimGrey : ColorTable.Gray);
                EditorGUI.LabelField(buttons[i], mode.ToString(), EditorStylesUtility.CenterAlignedBoldLabel);
                if(ev.type == EventType.MouseDown && ev.button == 0 && buttons[i].Contains(ev.mousePosition)) {
                    this.mode = mode;
                    Repaint();
                }
            }
            EditorGUILayout.Space();
            using(var scope = new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField(mode.ToString(), EditorStylesUtility.BoldLabel);
                switch(mode) {
                    case Mode.Prefix:
                        DrawPrefix();
                        break;
                    case Mode.First:
                        DrawFirstName();
                        break;
                    case Mode.Middle:
                        DrawMiddleName();
                        break;
                    case Mode.Last:
                        DrawLastName();
                        break;
                    case Mode.Suffix:
                        DrawSuffix();
                        break;
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            var line = GUILayoutUtility.GetRect(1, 2);
            EditorGUI.DrawRect(line, Color.gray);

            EditorGUILayout.Space();
            var genButton = GUILayoutUtility.GetRect(2, 16);
            genButton.width = 120f;

            EditorGUI.DrawRect(genButton, ColorTable.DimGray);
            EditorGUI.LabelField(genButton, "Generate", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUILayout.LabelField(genName, EditorStylesUtility.ItalicLabel);
            if(ev.type == EventType.MouseDown && ev.button == 0 && genButton.Contains(ev.mousePosition)) {
                genName = (target as SimpleNameGenerator).Generate();
                Repaint();
            }
        }

        #region Prefix

        private void DrawPrefix() {
            EditorGUILayout.Slider(prefixProbability, 0f, 1f, probabilityContent);
            HandleCSV(prefixNames);
            prefixList.DoLayoutList();
        }

        private void PrefixElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, prefixNames.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region First Names

        private void DrawFirstName() {
            EditorGUILayout.PropertyField(firstName);
            HandleCSV(firstNames);
            firstNamesList.DoLayoutList();
        }

        private void FirstNameElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, firstNames.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Middle Names

        private void DrawMiddleName() {
            EditorGUILayout.PropertyField(middleNameCount);
            EditorGUILayout.PropertyField(middleNameCountWeight);
            HandleCSV(middleNames);
            middleNamesList.DoLayoutList();
        }

        private void MiddleNameElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, middleNames.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Last Names

        private void DrawLastName() {
            EditorGUILayout.PropertyField(lastNameCount);
            EditorGUILayout.PropertyField(lastNameCountWeight);
            HandleCSV(lastNames);
            lastNamesList.DoLayoutList();
        }

        private void LastNameElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, lastNames.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Suffix

        private void DrawSuffix() {
            EditorGUILayout.Slider(suffixProbability, 0f, 1f, probabilityContent);
            HandleCSV(suffixNames);
            suffixList.DoLayoutList();
        }

        private void SuffixElement(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, suffixNames.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Utility

        private void HandleCSV(SerializedProperty array) {
            using(new EditorGUILayout.HorizontalScope()) {
                var csv = EditorGUILayout.DelayedTextField("CSV", "");
                if(!string.IsNullOrEmpty(csv)) {
                    var entries = csv
                        .Split(',', '.', '\t', ' ')
                        .AddRangeBefore(GetValues(array))
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Unique()
                        .ToArray();
                    var count = entries.Length;
                    array.arraySize = count;
                    entries.Foreach((x, i) => {
                        var element = array.GetArrayElementAtIndex(i);
                        element.stringValue = x;
                    });
                }
                if(GUILayout.Button("Clear", GUILayout.Width(80f))) {
                    array.arraySize = 0;
                }
            }
        }

        private static IEnumerable<string> GetValues(SerializedProperty array) {
            for(int i = 0, length = array.arraySize; i < length; i++) {
                yield return array.GetArrayElementAtIndex(i).stringValue;
            }
        }

        #endregion
    }
}
