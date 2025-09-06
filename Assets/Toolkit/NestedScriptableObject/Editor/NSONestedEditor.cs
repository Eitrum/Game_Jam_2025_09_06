using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class NSONestedEditor : System.IDisposable {

        #region Variables

        private const string TAG = "[Toolkit.NSONestedEditor] - ";

        private bool dynamicDisplayName = false;
        private string displayName;
        private SerializedProperty property;
        private UnityEngine.Object target => property.objectReferenceValue;
        private FieldInfo fieldInfo;
        private System.Type type;
        private NSOReferenceAttribute referenceAttribute;
        private Editor editor;

        #endregion

        #region Constructor

        public NSONestedEditor(SerializedProperty property) {
            this.property = property;
            displayName = this.property.name;
            if(property == null)
                Debug.LogError(TAG + "Property provided is null!");
            ScriptAttributeUtility.TryGetHandler(property, out var handler);
            PropertyDrawer drawer = UnityInternalUtility.TryGetPropertyValue(handler, "propertyDrawer", out var propValue) ? propValue as PropertyDrawer : null;
            fieldInfo = drawer.fieldInfo;
            Debug.Log(this.FormatLog(fieldInfo.FieldType.Name, property.propertyPath, property.name));
            if(fieldInfo.FieldType.IsSubclassOf(typeof(NSOReference))) {
                NSOReference nsoReference = fieldInfo.GetValue(property.serializedObject.targetObject) as NSOReference;
                type = nsoReference.NestedScriptableObjectType;
            }
            referenceAttribute = fieldInfo.GetCustomAttribute(typeof(NSOReferenceAttribute), true) as NSOReferenceAttribute;
            if(referenceAttribute != null) {
                type = referenceAttribute.Type;
            }
            if(property.propertyType != SerializedPropertyType.ObjectReference) {
                if(property.propertyType == SerializedPropertyType.Generic) {
                    var child = property.Copy();
                    child.NextVisible(true);
                    this.property = child;
                }
            }
        }

        public NSONestedEditor(SerializedProperty property, System.Type type, bool dynamicDisplayName = true) {
            this.property = property;
            displayName = this.property.name;
            this.dynamicDisplayName = dynamicDisplayName;

            if(property == null)
                Debug.LogError(TAG + "Property provided is null!");

            this.type = type;

            if(property.propertyType != SerializedPropertyType.ObjectReference) {
                if(property.propertyType == SerializedPropertyType.Generic) {
                    var child = property.Copy();
                    child.NextVisible(true);
                    this.property = child;
                }
            }
        }

        #endregion

        #region Dispose

        public void Dispose() {
            if(editor != null)
                Editor.DestroyImmediate(editor);
        }

        #endregion

        #region Draw

        public void DrawLayout() {
            if(property == null)
                return;

            var position = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var tdisplayname = (dynamicDisplayName && target != null) ? System.IO.Path.GetFileName(target.name) : displayName;
            NSOEditor.DrawObjectField(position, property, tdisplayname, type);

            if(target == null)
                return;

            if(NSOEditorSettings.Mode != NSOMode.NestedRendering || !property.isExpanded)
                return;

            // Handle drawing the nested editor
            using(var scope = new EditorGUILayout.VerticalScope()) {
                var color = NSOUtility.GetColor(0);
                var outline = EditorGUI.IndentedRect(scope.rect);
                EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y, 2f, outline.height), color);
                EditorGUI.DrawRect(new Rect(outline.x - 6, outline.y + outline.height - 2f, outline.width, 2f), color);
                if(editor == null) {
                    editor = Editor.CreateEditor(target);
                }
                else if(editor.serializedObject.targetObject != target) {
                    Editor.DestroyImmediate(editor);
                    editor = Editor.CreateEditor(target);
                }

                editor.OnInspectorGUI();
                EditorGUILayout.Space(4);
            }
            EditorGUILayout.Space(4);
        }

        #endregion
    }



    public class NSONestedArrayEditor : System.IDisposable {

        class Styles {
            public static GUIContent UpArrow = EditorGUIUtility.TrIconContent("d_scrollup");
            public static GUIContent DownArrow = EditorGUIUtility.TrIconContent("d_scrolldown");
            public static GUIContent Select = EditorGUIUtility.TrIconContent("d_Toolbar Minus");
        }

        #region Variables

        private const string TAG = "[Toolkit.NSONestedArrayEditor] - ";

        private string displayName;
        private SerializedProperty property;
        private bool alwaysExpanded = false;
        private bool reorderable;
        private System.Type type;
        private FieldInfo fieldInfo;
        private int selected = -1;
        private List<NSONestedEditor> nestedEditors = new List<NSONestedEditor>();
        private static bool editMode = false;
        public bool DrawHeader = true;

        public event System.Action OnUpdated;

        #endregion

        #region Constructor

        public NSONestedArrayEditor(SerializedProperty property, string header = null, bool alwaysExpanded = false, bool reorderable = true) {
            this.property = property.FindPropertyRelative("scriptables");
            displayName = string.IsNullOrEmpty(header) ? property.displayName : header;
            this.alwaysExpanded = alwaysExpanded;
            this.reorderable = reorderable;
            if(property == null)
                Debug.LogError(TAG + "Property provided is null!");


            ScriptAttributeUtility.TryGetHandler(property, out var handler);
            PropertyDrawer drawer = UnityInternalUtility.TryGetPropertyValue(handler, "propertyDrawer", out var propValue) ? propValue as PropertyDrawer : null;
            fieldInfo = drawer.fieldInfo;

            if(fieldInfo.FieldType.IsSubclassOf(typeof(NSOReferenceArray))) {
                NSOReferenceArray nsoReference = fieldInfo.GetValue(property.serializedObject.targetObject) as NSOReferenceArray;
                type = nsoReference.NestedScriptableObjectType;
            }

            if(type == null) {
                return;
            }

            var size = this.property.arraySize;
            for(int i = 0; i < size; i++)
                nestedEditors.Add(new NSONestedEditor(this.property.GetArrayElementAtIndex(i), type));
        }

        public void RebuildEditors() {
            foreach(var e in nestedEditors)
                e?.Dispose();

            nestedEditors.Clear();

            var size = this.property.arraySize;
            for(int i = 0; i < size; i++)
                nestedEditors.Add(new NSONestedEditor(this.property.GetArrayElementAtIndex(i), type));
        }

        #endregion

        #region Dispose

        public void Dispose() {
            if(nestedEditors == null)
                return;
            foreach(var n in nestedEditors)
                n?.Dispose();
        }

        #endregion

        #region Add / Remove

        public void Add() {
            EditorApplication.delayCall += () => {
                var insertPosition = selected >= 0 ? selected : property.arraySize;
                property.InsertArrayElementAtIndex(insertPosition);
                var prop = property.GetArrayElementAtIndex(insertPosition);
                prop.objectReferenceValue = null;
                var len = property.arraySize;
                for(int i = len - 1; i > insertPosition; i--) {
                    var wasExpanded = property.GetArrayElementAtIndex(i-1).isExpanded;
                    property.GetArrayElementAtIndex(i).isExpanded = wasExpanded;
                }
                property.serializedObject.ApplyModifiedProperties();
                RebuildEditors();
                OnUpdated?.Invoke();
            };
        }

        private void Remove() {
            if(selected >= 0)
                RemoveSelected();
            else
                RemoveLast();
        }

        public void Remove(int index) {
            if(index < 0 || index >= nestedEditors.Count) {
                Debug.LogError(TAG + "Attempting to remove index out of range!");
                return;
            }

            EditorApplication.delayCall += () => {
                for(int i = index, len = property.arraySize - 1; i < len; i++) {
                    var wasExpanded = property.GetArrayElementAtIndex(i+1).isExpanded;
                    property.GetArrayElementAtIndex(i).isExpanded = wasExpanded;
                }

                var element = property.GetArrayElementAtIndex(index);
                if(element.objectReferenceValue != null) {
                    NSOUtility.VerifyAndDestroy(element);
                    element.objectReferenceValue = null;
                }
                property.DeleteArrayElementAtIndex(index);

                property.serializedObject.ApplyModifiedProperties();

                RebuildEditors();
                OnUpdated?.Invoke();
            };
        }

        public void RemoveSelected() => Remove(selected);
        public void RemoveLast() => Remove(nestedEditors.Count - 1);

        #endregion

        #region Move

        public void Swap(int a, int b) {
            if(a == b)
                return;
            if(a < 0 || b < 0)
                return;
            if(a >= nestedEditors.Count || b >= nestedEditors.Count)
                return;

            EditorApplication.delayCall += () => {
                var propa = property.GetArrayElementAtIndex(a);
                var propb = property.GetArrayElementAtIndex(b);

                var obja = propa.objectReferenceValue;
                var objb = propb.objectReferenceValue;

                propa.objectReferenceValue = objb;
                propb.objectReferenceValue = obja;

                property.serializedObject.ApplyModifiedProperties();
                OnUpdated?.Invoke();
            };
        }

        public void MoveUp(int index) {
            if(index < 1 || index >= nestedEditors.Count)
                return;
            Swap(index, index - 1);
        }

        public void MoveDown(int index) {
            if(index < 0 || index >= (nestedEditors.Count - 1))
                return;
            Swap(index, index + 1);
        }

        #endregion

        #region Draw

        public void DrawLayout() {
            if(property == null) {
                EditorGUILayout.HelpBox("Property is null", MessageType.Warning);
                return;
            }

            if(NSOEditorSettings.Mode == NSOMode.Default) {
                EditorGUILayout.PropertyField(property);
                return;
            }

            if(DrawHeader) {
                if(alwaysExpanded)
                    EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel);
                else
                    property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, displayName, true);

                if(!property.isExpanded && !alwaysExpanded)
                    return;
            }

            for(int i = 0, len = nestedEditors.Count; i < len; i++) {
                using(var rowScope = new EditorGUILayout.HorizontalScope()) {
                    if(selected == i) {
                        EditorGUI.DrawRect(rowScope.rect, new Color(0f, 0.3f, 1f, 0.3f));
                    }
                    using(var scope = new EditorGUILayout.VerticalScope("box")) {
                        nestedEditors[i].DrawLayout();
                    }
                    EditorGUILayout.Space(2, false);

                    if(editMode) {
                        using(var scope = new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight), GUILayout.ExpandWidth(false))) {

                            if(reorderable && GUILayout.Button(Styles.UpArrow)) {
                                MoveUp(i);
                            }
                            if(GUILayout.Button(Styles.Select)) {
                                if(selected == i)
                                    selected = -1;
                                else
                                    selected = i;
                                OnUpdated?.Invoke();
                            }
                            if(reorderable && GUILayout.Button(Styles.DownArrow)) {
                                MoveDown(i);
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                }
                EditorGUILayout.Space(2, false);
            }
            using(new EditorGUILayout.HorizontalScope("box")) {
                GUILayout.FlexibleSpace();

                if(GUILayout.Button("Edit", GUILayout.Width(50))) {
                    editMode = !editMode;
                    selected = -1;
                }
                if(GUILayout.Button("Add", GUILayout.Width(40))) {
                    Add();
                }
                using(new EditorGUI.DisabledScope(property.arraySize == 0)) {
                    if(GUILayout.Button("Remove", GUILayout.Width(60))) {
                        if(selected < 0 && property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue != null) {
                            if(EditorUtility.DisplayDialog("Delete", $"Do you want to delete '{property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue.name}' at last index", "Yes")) {
                                Remove();
                            }
                        }
                        else
                            Remove();
                    }
                }
            }

            EditorGUILayout.Space(4, false);
        }

        #endregion
    }
}
