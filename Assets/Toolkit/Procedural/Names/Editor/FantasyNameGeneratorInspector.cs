using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

namespace Toolkit.Procedural.Names
{
    [CustomEditor(typeof(FantasyNameGenerator))]
    public class FantasyNameGeneratorInspector : Editor
    {
        private enum Mode
        {
            First_Part1,
            First_Part2,
            Last_Part1,
            Last_Part2
        }

        #region Variables

        private Mode mode = Mode.First_Part1;
        private string genName = "";

        private SerializedProperty nm1;
        private ReorderableList nm1List;

        private SerializedProperty nm2;
        private ReorderableList nm2List;

        private SerializedProperty nm3;
        private ReorderableList nm3List;

        private SerializedProperty nm4 = null;
        private ReorderableList nm4List = null;

        #endregion

        private void OnEnable() {
            nm1 = serializedObject.FindProperty("nm1");
            nm1List = new ReorderableList(serializedObject, nm1);
            nm1List.drawElementCallback += Nm1Element;
            nm1List.headerHeight = 0f;

            nm2 = serializedObject.FindProperty("nm2");
            nm2List = new ReorderableList(serializedObject, nm2);
            nm2List.drawElementCallback += Nm2Element;
            nm2List.headerHeight = 0f;

            nm3 = serializedObject.FindProperty("nm3");
            nm3List = new ReorderableList(serializedObject, nm3);
            nm3List.drawElementCallback += Nm3Element;
            nm3List.headerHeight = 0f;

            nm4 = serializedObject.FindProperty("nm4");
            nm4List = new ReorderableList(serializedObject, nm4);
            nm4List.drawElementCallback += Nm4Element;
            nm4List.headerHeight = 0f;
        }

        public override void OnInspectorGUI() {
            var header = GUILayoutUtility.GetRect(1, 20);
            var buttons = header.SplitHorizontal(4, 2f);
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
                    case Mode.First_Part1:
                        DrawNm1();
                        break;
                    case Mode.First_Part2:
                        DrawNm2();
                        break;
                    case Mode.Last_Part1:
                        DrawNm3();
                        break;
                    case Mode.Last_Part2:
                        DrawNm4();
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
                genName = (target as FantasyNameGenerator).Generate();
                Repaint();
            }
        }

        #region Nm1

        private void DrawNm1() {
            HandleCSV(nm1);
            nm1List.DoLayoutList();
        }

        private void Nm1Element(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, nm1.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Nm2

        private void DrawNm2() {
            HandleCSV(nm2);
            nm2List.DoLayoutList();
        }

        private void Nm2Element(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, nm2.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Nm3

        private void DrawNm3() {
            HandleCSV(nm3);
            nm3List.DoLayoutList();
        }

        private void Nm3Element(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, nm3.GetArrayElementAtIndex(index), GUIContent.none);
        }

        #endregion

        #region Nm4

        private void DrawNm4() {
            HandleCSV(nm4);
            nm4List.DoLayoutList();
        }

        private void Nm4Element(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.PropertyField(rect, nm4.GetArrayElementAtIndex(index), GUIContent.none);
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
