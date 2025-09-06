using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Verify
{
    [CustomEditor(typeof(VerifyObject))]
    public class VerifyObjectInspector : Editor
    {
        #region Variables

        private SerializedProperty paths;
        private ReorderableList pathsList;

        private SerializedProperty blacklist;
        private ReorderableList blacklistList;

        IVerify[] objectsToVerify;
        int index = 0;
        [System.NonSerialized] private bool isVerifying = false;
        [System.NonSerialized] private int successes = 0;
        [System.NonSerialized] private List<(UnityEngine.Object, string)> errors = new List<(UnityEngine.Object, string)>();

        #endregion

        #region Initialize

        private void OnEnable() {
            paths = serializedObject.FindProperty("paths");
            pathsList = new ReorderableList(serializedObject, paths);
            pathsList.drawHeaderCallback += OnPathsHeader;
            pathsList.drawElementCallback += OnPathsElement;

            blacklist = serializedObject.FindProperty("blacklist");
            blacklistList = new ReorderableList(serializedObject, blacklist);
            blacklistList.drawHeaderCallback += OnBlacklistHeader;
            blacklistList.drawElementCallback += OnBlacklistElement;
        }

        private void OnDisable() {
            isVerifying = false;
        }

        #endregion

        #region Drawing

        private void OnPathsHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Paths", EditorStylesUtility.BoldLabel);
        }

        private void OnPathsElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 2, 1);
            EditorGUI.BeginChangeCheck();
            var i = FolderEditor.Folders.IndexOf(paths.GetArrayElementAtIndex(index).stringValue);
            if(i < 0) {
                i = 0;
                paths.GetArrayElementAtIndex(index).stringValue = FolderEditor.Folders[0];
            }

            i = EditorGUI.Popup(rect, i, FolderEditor.Folders);

            if(EditorGUI.EndChangeCheck()) {
                paths.GetArrayElementAtIndex(index).stringValue = FolderEditor.Folders[i];
            }
        }

        private void OnBlacklistHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Blacklist", EditorStylesUtility.BoldLabel);
        }

        private void OnBlacklistElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 2, 1);
            EditorGUI.BeginChangeCheck();
            var i = FolderEditor.Folders.IndexOf(blacklist.GetArrayElementAtIndex(index).stringValue);
            if(i < 0) {
                i = 0;
                blacklist.GetArrayElementAtIndex(index).stringValue = FolderEditor.Folders[0];
            }

            i = EditorGUI.Popup(rect, i, FolderEditor.Folders);

            if(EditorGUI.EndChangeCheck()) {
                blacklist.GetArrayElementAtIndex(index).stringValue = FolderEditor.Folders[i];
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStylesUtility.BoldLabel);
                var area = GUILayoutUtility.GetLastRect();
                var ev = Event.current;
                if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && area.Contains(ev.mousePosition)) {
                    paths.isExpanded = !paths.isExpanded;
                    ev.Use();
                }

                if(paths.isExpanded) {
                    EditorGUILayout.Space();
                    pathsList.DoLayoutList();
                    EditorGUILayout.Space();
                    blacklistList.DoLayoutList();
                }
                EditorGUILayout.Space();
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Verify", EditorStylesUtility.BoldLabel);
                using(new EditorGUILayout.HorizontalScope()) {
                    if(DrawFancyButton("Verify", 80f)) {
                        BeginVerify();
                    }
                    EditorGUILayout.Space(8f);
                    if(isVerifying) {
                        if(DrawFancyButton("Cancel", 80f)) {
                            Cancel();
                        }
                        EditorGUILayout.Space(8f);
                        if(index >= objectsToVerify.Length && DrawFancyButton("Print", 80f)) {
                            if(errors.Count == 0) {
                                Debug.Log($"[{target.name}] - Success! No issues found.");
                            }
                            else {
                                Debug.LogError($"[{target.name}] - Fail! {errors.Count} issues found.\n{errors.Select(x => $"{x.Item1.name} - {x.Item2}").CombineToString()}");
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"{index} / {objectsToVerify?.Length ?? 0}", EditorStylesUtility.CenterAlignedBoldLabel);
                }
                EditorGUILayout.Space();
                if(isVerifying && Event.current.type == EventType.Layout) {
                    VerifyChunk();
                    Repaint();
                }
                if(objectsToVerify != null) {
                    var area = GUILayoutUtility.GetRect(1f, 18f);
                    EditorGUI.DrawRect(area, new Color(0.3f, 0.3f, 0.3f));
                    EditorGUI.DrawRect(area.Pad(1, area.width - (area.width * (index / (float)objectsToVerify.Length)), 1, 1), ColorTable.LawnGreen);
                    EditorGUI.LabelField(area, $"{(index / (float)objectsToVerify.Length):P0}", EditorStylesUtility.CenterAlignedBoldLabel);
                }
                EditorGUILayout.Space();
                var ev = Event.current;
                foreach(var err in errors) {
                    EditorGUILayout.LabelField($"{err.Item1.name} - {err.Item2}");
                    if(ev != null && ev.type == EventType.MouseDown && ev.button == 0) {
                        var labelArea = GUILayoutUtility.GetLastRect();
                        if(labelArea.Contains(ev.mousePosition)) {
                            ev.Use();
                            EditorGUIUtility.PingObject(err.Item1);
                        }
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        private bool DrawFancyButton(string name, float width) {
            var ev = Event.current;
            var button = GUILayoutUtility.GetRect(width, 18f);
            EditorGUI.DrawRect(button, Color.gray);
            EditorGUI.LabelField(button, name, EditorStylesUtility.CenterAlignedBoldLabel);
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && button.Contains(ev.mousePosition)) {
                ev.Use();
                return true;
            }
            return false;
        }

        #endregion

        #region Verification Process

        public void Cancel() {
            objectsToVerify = null;
            isVerifying = false;
        }

        public void BeginVerify() {
            EditorUtility.DisplayProgressBar("Loading", "Finding all the files in the project", 0.1f);
            errors.Clear();
            objectsToVerify = VerifyEditor.GetObjects(target as VerifyObject);
            index = 0;
            isVerifying = true;
            successes = 0;
            EditorUtility.ClearProgressBar();
        }

        public void VerifyChunk() {
            int temp = 0;
            int toCheck = UnityEngine.Random.Range(7, 13);
            while(temp++ < toCheck && index < objectsToVerify.Length) {
                var t = objectsToVerify[index];
                if(t != null) {
                    if(!t.Verify(out string error)) {
                        errors.Add((t as UnityEngine.Object, error));
                    }
                    else {
                        successes++;
                    }
                }
                index++;
            }
        }

        #endregion
    }
}
