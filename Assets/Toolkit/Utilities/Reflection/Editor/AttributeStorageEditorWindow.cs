using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class AttributeStorageEditorWindow : EditorWindow {
        #region FoldoutData

        private class FoldoutData {
            public bool main;
            public bool classes;
            public bool fields;
            public bool properties;
            public bool methods;
        }

        #endregion

        #region Open

        [MenuItem("Window/Toolkit/Attribute Storage")]
        private static void OpenWindow() {
            GetWindow<AttributeStorageEditorWindow>("Attribute Storage").Show();
        }

        #endregion

        #region Variables

        private bool showUnityAttributes;
        private bool showSystemAttributes;
        private string filter;
        private Vector2 scroll;

        private Dictionary<AttributeStorage.Storage, FoldoutData> foldout = new Dictionary<AttributeStorage.Storage, FoldoutData>();

        #endregion

        #region Should Draw Check

        public bool ShouldShow(AttributeStorage.Storage storage) {
            var name = storage.AttributeType.FullName;

            if(!string.IsNullOrEmpty(filter) && !name.Contains(filter))
                return false;

            if(!showUnityAttributes) {
                if(name.StartsWith("UnityEngine."))
                    return false;
                if(name.StartsWith("UnityEngineInternal."))
                    return false;
                if(name.StartsWith("UnityEditor."))
                    return false;
                if(name.StartsWith("Unity."))
                    return false;
            }
            if(!showSystemAttributes) {
                if(name.StartsWith("System."))
                    return false;
            }

            return true;
        }

        #endregion

        #region Draw

        private void OnGUI() {
            try {
                GUILayout.BeginArea(new Rect(Vector2.zero, position.size));
                using(new GUILayout.VerticalScope("box")) {
                    filter = EditorGUILayout.TextField("Search", filter);
                    showUnityAttributes = EditorGUILayout.Toggle("Show Unity Attributes", showUnityAttributes);
                    showSystemAttributes = EditorGUILayout.Toggle("Show System Attributes", showSystemAttributes);
                }
                using(var s = new GUILayout.ScrollViewScope(scroll)) {
                    scroll = s.scrollPosition;
                    if(!AttributeStorage.IsInitialized) {
                        EditorGUILayout.HelpBox("Loading...", MessageType.Warning);
                        return;
                    }

                    var storages = AttributeStorage.GetAll();
                    foreach(var storage in storages) {
                        DrawStorage(storage);
                    }
                }
            }
            finally { GUILayout.EndArea(); }
        }

        private void DrawStorage(AttributeStorage.Storage s) {
            if(!ShouldShow(s))
                return;
            if(!foldout.TryGetValue(s, out var foldoutValue))
                foldout.Add(s, foldoutValue = new FoldoutData());
            foldoutValue.main = EditorGUILayout.Foldout(foldoutValue.main, s.AttributeType.FullName, true);
            if(!foldoutValue.main)
                return;

            var ev = Event.current;

            using(new EditorGUI.IndentLevelScope(1)) {
                if(s.ClassAttributes.Count > 0) {
                    foldoutValue.classes = EditorGUILayout.Foldout(foldoutValue.classes, $"Classes ({s.ClassAttributes.Count})", true);
                    if(foldoutValue.classes) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            foreach(var f in s.ClassAttributes) {
                                EditorGUILayout.LabelField(f.ToString());
                                if(ev.type == EventType.MouseDown && ev.button == 0 && GUILayoutUtility.GetLastRect().Contains(ev.mousePosition)) {
                                    OpenFile(f.Class, f.Class.Name);
                                }
                            }
                        }
                    }
                }
                if(s.FieldAttributes.Count > 0) {
                    foldoutValue.fields = EditorGUILayout.Foldout(foldoutValue.fields, $"Fields ({s.FieldAttributes.Count})", true);
                    if(foldoutValue.fields) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            foreach(var f in s.FieldAttributes) {
                                EditorGUILayout.LabelField(f.ToString());
                                if(ev.type == EventType.MouseDown && ev.button == 0 && GUILayoutUtility.GetLastRect().Contains(ev.mousePosition)) {
                                    OpenFile(f.Class, f.FieldInfo.Name);
                                }
                            }
                        }
                    }
                }
                if(s.PropertyAttributes.Count > 0) {
                    foldoutValue.properties = EditorGUILayout.Foldout(foldoutValue.properties, $"Properties ({s.PropertyAttributes.Count})", true);
                    if(foldoutValue.properties) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            foreach(var p in s.PropertyAttributes) {
                                EditorGUILayout.LabelField(p.ToString());
                                if(ev.type == EventType.MouseDown && ev.button == 0 && GUILayoutUtility.GetLastRect().Contains(ev.mousePosition)) {
                                    OpenFile(p.Class, p.PropertyInfo.Name);
                                }
                            }
                        }
                    }
                }
                if(s.MethodAttributes.Count > 0) {
                    foldoutValue.methods = EditorGUILayout.Foldout(foldoutValue.methods, $"Methods ({s.MethodAttributes.Count})", true);
                    if(foldoutValue.methods) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            foreach(var m in s.MethodAttributes) {
                                EditorGUILayout.LabelField(m.ToString());
                                if(ev.type == EventType.MouseDown && ev.button == 0 && GUILayoutUtility.GetLastRect().Contains(ev.mousePosition)) {
                                    OpenFile(m.Class, m.MethodInfo.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Open File

        private static void OpenFile(Type classType, string targetText) {
            try {
                var ns = classType.Namespace;
                var name = $" {classType.Name} "; // <- Some sneaky way to find correct classes

                var script = AssetDatabase
                    .FindAssets("t:MonoScript")
                    .Select(x=>AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(x)))
                                            .Where(x => x.text.Contains(ns))
                                            .Where(x => x.text.Contains(targetText))
                                            .Where(x => x.text.Contains(name)).
                                            FirstOrDefault();
                var parentObject = classType.BaseType;
                if(script == null) {
                    if(parentObject != null) {
                        if(EditorUtility.DisplayDialog("Open File", $"Failed to locate the script file for:\n{classType.FullName}.{targetText}\n\nLook in into parent object:\n{parentObject.FullName}", "Yes", "Cancel"))
                            OpenFile(parentObject, targetText);
                    }
                    else
                        EditorUtility.DisplayDialog("Open File", $"Failed to locate the script file for:\n{classType.FullName}.{targetText}", "Ok");
                    return;
                }
                else {
                    var lines = script.text.Split('\n');
                    for(int i = 0; i < lines.Length; i++) {
                        if(lines[i].Contains(targetText)) {
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(AssetDatabase.GetAssetPath(script), i + 1);
                            return;
                        }
                    }
                    if(parentObject != null) {
                        if(EditorUtility.DisplayDialog("Open File", $"Failed to locate line for:\n{classType.FullName}.{targetText}\n\nLook in into parent object:\n{parentObject.FullName}", "Yes", "Cancel"))
                            OpenFile(parentObject, targetText);
                    }
                    else
                        EditorUtility.DisplayDialog("Open File", $"Failed to locate line for:\n{classType.FullName}.{targetText}", "Ok");
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion
    }
}
