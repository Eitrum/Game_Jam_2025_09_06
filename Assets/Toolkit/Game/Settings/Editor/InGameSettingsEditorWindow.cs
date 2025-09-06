using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit.Game {
    public class InGameSettingsEditorWindow : EditorWindow {

        private InGameSettings.Group selectedGroup;

        [MenuItem("Window/Toolkit/In-Game Settings")]
        private static void OpenWindow() {
            var w = GetWindow<InGameSettingsEditorWindow>("In-Game Settings");
            w.Show();
        }

        private void OnGUI() {
            var area = new Rect(10, 10, position.width - 20, position.height - 20);
            try {
                GUILayout.BeginArea(area);
                using(new EditorGUILayout.HorizontalScope("box")) {
                    DrawHeader();
                }
                GUILayout.Space(16);
                using(new EditorGUILayout.HorizontalScope()) {
                    using(new GUILayout.VerticalScope("box", GUILayout.MinWidth(200))) {
                        DrawGroupSelector();
                    }
                    GUILayout.Space(20);
                    var split = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawRect(split, Color.black.MultiplyAlpha(0.1f));
                    using(new GUILayout.VerticalScope("box")) {
                        DrawSelectedGroup();
                    }
                }
            }
            finally {
                GUILayout.EndArea();
            }

            Repaint();
        }

        private void DrawHeader() {
            if(GUILayout.Button("Reset All")) {
                InGameSettings.ResetAll();
            }
            if(GUILayout.Button("Discard All")) {
                InGameSettings.DiscardAllChanges();
            }
            if(GUILayout.Button("Apply All Changes")) {
                InGameSettings.ApplyAllChanges(false);
            }

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Load")) {
                InGameSettings.Load();
            }
            GUILayout.Space(20);
            if(GUILayout.Button("Save")) {
                InGameSettings.Save();
            }
        }

        private void DrawGroupSelector() {
            foreach(var g in InGameSettings.Groups) {
                DrawGroupButton(g.Key, g.Value);
            }

            GUILayout.FlexibleSpace();
        }

        private void DrawGroupButton(string key, InGameSettings.Group value) {
            bool isSelected = value == selectedGroup;
            var name = value.IsDirty ? $"{key}*" : key;
            if(GUILayout.Button(isSelected ? ($"-> {name} <-") : name)) {
                selectedGroup = value;
            }
        }

        private void DrawSelectedGroup() {
            if(selectedGroup == null)
                return;
            using(new EditorGUILayout.HorizontalScope("box")) {
                EditorGUILayout.LabelField(selectedGroup.Name, EditorStyles.boldLabel);
                using(new EditorGUILayout.HorizontalScope()) {
                    using(new EditorGUI.DisabledScope(!selectedGroup.IsDirty))
                        if(GUILayout.Button("Discard")) {
                            selectedGroup.DiscardAllChanges();
                        }
                    if(GUILayout.Button("Reset")) {
                        selectedGroup.ResetAll();
                    }
                    if(GUILayout.Button("Apply Changes")) {
                        selectedGroup.ApplyAllChanges();
                    }
                }
            }
            GUILayout.Space(8);
            foreach(var e in selectedGroup.Entries) {
                DrawEntry(e.Key, e.Value);
            }
            GUILayout.FlexibleSpace();
        }

        private void DrawEntry(string key, InGameSettings.Entry value) {
            var name = value.IsDirty ? $"{value.Id}*" : value.Id;
            switch(value) {
                case InGameSettings.IEnumEntry enumEntry:
                    enumEntry.GenericEnumValue = EditorGUILayout.EnumPopup(name, enumEntry.GenericEnumValue);
                    break;
                case InGameSettings.IArrayEntry arrayEntry:
                    EditorGUI.BeginChangeCheck();
                    var selected = EditorGUILayout.Popup(name, arrayEntry.Selected, arrayEntry.Names);
                    if(EditorGUI.EndChangeCheck())
                        arrayEntry.Selected = selected;
                    break;
                case InGameSettings.Toggleable toggable:
                    toggable.ModifiedValue = EditorGUILayout.Toggle(name, toggable.ModifiedValue);
                    break;
                case InGameSettings.RangeEntry rangeEntry:
                    rangeEntry.ModifiedValue = EditorGUILayout.Slider(name, rangeEntry.ModifiedValue, rangeEntry.Min, rangeEntry.Max);
                    break;
                case InGameSettings.IntRangeEntry intRangeEntry:
                    intRangeEntry.ModifiedValue = EditorGUILayout.IntSlider(name, intRangeEntry.ModifiedValue, intRangeEntry.Min, intRangeEntry.Max);
                    break;
                case InGameSettings.Entry<string> stringEntry:
                    stringEntry.ModifiedValue = EditorGUILayout.DelayedTextField(name, stringEntry.ModifiedValue);
                    break;
                case InGameSettings.Entry<int> intEntry:
                    intEntry.ModifiedValue = EditorGUILayout.IntField(name, intEntry.ModifiedValue);
                    break;
                case InGameSettings.Entry<float> floatEntry:
                    floatEntry.ModifiedValue = EditorGUILayout.FloatField(name, floatEntry.ModifiedValue);
                    break;
                default:
                    EditorGUILayout.HelpBox($"{name} Is not supported type", MessageType.Warning);
                    break;
            }
        }
    }
}
