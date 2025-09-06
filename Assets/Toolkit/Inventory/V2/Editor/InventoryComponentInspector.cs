using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit.Inventory.V2 {
    [CustomEditor(typeof(InventoryComponent))]
    public class InventoryComponentInspector : Editor {

        private SerializedProperty containers;
        private UnityEditorInternal.ReorderableList containerList;
        private Vector2 scroll;
        private string serializedInventory;

        private void OnEnable() {
            containers = serializedObject.FindProperty("containers");
            containerList = new UnityEditorInternal.ReorderableList(serializedObject, containers);
            containerList.drawElementCallback += OnDrawElement;
            containerList.elementHeightCallback += OnCalculateHeight;
            containerList.headerHeight = 0f;
        }

        private float OnCalculateHeight(int index) {
            return EditorGUI.GetPropertyHeight(containers.GetArrayElementAtIndex(index), true);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var element = containers.GetArrayElementAtIndex(index);
            EditorGUI.DrawRect(rect, ToolkitEditorUtility.GetColor(index).MultiplyAlpha(0.35f));
            EditorGUI.PropertyField(rect, element, true);
        }

        public override void OnInspectorGUI() {
            using(var editor = new ToolkitEditorUtility.InspectorScope(this)) {
                containerList.DoLayoutList();
            }

            var t = (InventoryComponent)target;
            var allItems = t.GetAllItems();
            using(var scope = new EditorGUILayout.ScrollViewScope(scroll)) {
                scroll = scope.scrollPosition;
                foreach(var i in allItems) {
                    GUILayout.Label($"{i.UIDAsName} {(i.TryGetData<ItemMetadata>(out var metadata) ? metadata.Name : "noname")} ({(i.TryGetData<IStackable>(out var stackable) ? stackable.Formatted() : "1/1")})");
                }
            }
            EditorGUILayout.Space();
            if(GUILayout.Button("Serialize", GUILayout.Width(80))) {
                var node = t.Serialize();
                serializedInventory = Toolkit.IO.TML.TMLParser.ToString(node, true);
            }

            EditorGUILayout.TextArea(serializedInventory);
        }
    }

    [CustomPropertyDrawer(typeof(Container))]
    public class ContainerDrawer : PropertyDrawer {

        private const float Indent = 15;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var prios = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("priorities"), true);
            var filters = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("filter"), true);
            return EditorGUIUtility.singleLineHeight * 2f + Mathf.Max(prios, filters);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var headerArea = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var bodyArea = new Rect(position.x + Indent, position.y + EditorGUIUtility.singleLineHeight, position.width - Indent, EditorGUIUtility.singleLineHeight);
            var arrayArea = new Rect(position.x + Indent, position.y + EditorGUIUtility.singleLineHeight * 2f, position.width - Indent, position.height - EditorGUIUtility.singleLineHeight * 2f);

            var name = property.FindPropertyRelative("name");
            var basePriority = property.FindPropertyRelative("basePriority");
            var capacity = property.FindPropertyRelative("capacity");
            var priorities = property.FindPropertyRelative("priorities");
            var filters = property.FindPropertyRelative("filter");

            GUI.Label(headerArea, label, EditorStyles.boldLabel);
            bodyArea.SplitHorizontal(out var nameField, out var capacityField, out var basePriorityField, 0.7f, 0.15f, 2f);
            name.stringValue = EditorGUI.TextField(nameField, name.stringValue);
            EditorGUI.PropertyField(capacityField, capacity, GUIContent.none);
            EditorGUI.PropertyField(basePriorityField, basePriority, GUIContent.none);

            if(string.IsNullOrEmpty(name.stringValue))
                GUI.Label(nameField, "Name", EditorStylesUtility.GrayItalicLabel);

            GUI.Label(capacityField, "Capacity", EditorStylesUtility.RightAlignedGrayMiniLabel);
            GUI.Label(basePriorityField, "Priority", EditorStylesUtility.RightAlignedGrayMiniLabel);

            arrayArea.SplitHorizontal(out var prioArea, out var filterArea, 0.5f, 10f);
            EditorGUI.PropertyField(prioArea, priorities, true);
            EditorGUI.PropertyField(filterArea, filters, true);
        }
    }

    [CustomPropertyDrawer(typeof(Container.Priority))]
    public class ContainerPriorityDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var type = property.FindPropertyRelative("Type");
            var value = property.FindPropertyRelative("Value");

            position.SplitHorizontal(out var left, out var right, 0.5f, 2f);
            EditorGUI.PropertyField(left, type, GUIContent.none);
            EditorGUI.PropertyField(right, value, GUIContent.none);
            GUI.Label(right, "Priority", EditorStylesUtility.RightAlignedGrayMiniLabel);
        }
    }
}
