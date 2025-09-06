using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit
{
    public abstract class CustomizableSettingInspector<T, TSettings> : Editor where T : CustomizableSettingInspector<T, TSettings>
    {
        #region Variables

        private bool editMode = false;

        private string className = "";
        private List<Group> groups = new List<Group>();
        private ReorderableList reorderableList;

        private static string[] supportedTypeNames => CustomizableSettingEditor.SupportedTypePaths;
        private static Type[] supportedTypes => CustomizableSettingEditor.SupportedTypes;

        #endregion

        #region Properties

        public string ClassName => className;
        public IReadOnlyList<Group> Groups => groups;
        public MonoScript ScriptReference => serializedObject.FindProperty("scriptReference").objectReferenceValue as MonoScript;
        public MonoScript InspectorReference => serializedObject.FindProperty("inspectorReference").objectReferenceValue as MonoScript;

        #endregion

        #region Initializing

        protected void Generate() {
            groups.Clear();
            if(reorderableList == null) {
                reorderableList = new ReorderableList(groups, typeof(Group));
                reorderableList.elementHeightCallback += OnGroupHeightCallback;
                reorderableList.drawHeaderCallback += OnDrawEditHeader;
                reorderableList.drawElementCallback += OnDrawGroup;
            }

            var groupNameArray = (string[])typeof(T).GetField("groupName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
            var groupSizeArray = (int[])typeof(T).GetField("groupSize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);

            var propertyPathsArray = (string[])typeof(T).GetField("propertyPaths", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
            var fieldsArray = (GUIContent[])typeof(T).GetField("fields", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
            var fieldsTypeArray = (Type[])typeof(T).GetField("fieldsType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
            var defaultValueArray = (string[])typeof(T).GetField("defaultValue", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
            var customPropertyTypeArray = (string[])typeof(T).GetField("customPropertyType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);


            className = typeof(TSettings).FullName;
            int index = 0;
            for(int g = 0, length = groupSizeArray.Length; g < length; g++) {
                var group = new Group();
                group.name = groupNameArray[g];
                groups.Add(group);
                var size = groupSizeArray[g];
                for(int i = 0; i < size; i++) {
                    var type = fieldsTypeArray[index];
                    int val = 0;
                    for(int x = 0, typesLength = supportedTypes.Length; x < typesLength; x++) {
                        if(supportedTypes[x] == type) {
                            val = x;
                        }
                    }

                    group.fields.Add(new Group.Field() {
                        content = new GUIContent(fieldsArray[index].text, fieldsArray[index].tooltip),
                        propertyPath = propertyPathsArray[index],
                        type = supportedTypes[val],
                        typeIndex = val,
                        defaultValue = defaultValueArray[index],
                        accessorType = customPropertyTypeArray[index],
                    });
                    index++;
                }
            }
        }

        private void OnEnable() {
            Generate();
        }

        #endregion

        #region Drawing

        private void OnDrawGroup(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 14f, 4f);
            var groupNameArea = new Rect(rect);
            groupNameArea.height = EditorGUIUtility.singleLineHeight;
            groups[index].name = EditorGUI.TextField(groupNameArea, groups[index].name);
            if(string.IsNullOrEmpty(groups[index].name))
                using(new EditorGUI.DisabledScope(true))
                    EditorGUI.TextField(groupNameArea, "Group name...");
            rect.PadRef(0, 0, groupNameArea.height + 4f, 0);
            groups[index].Draw(rect);
        }

        private void OnDrawEditHeader(Rect rect) {
            rect.PadRef(12f, 0, 0, 0);
            EditorGUI.LabelField(rect, "Groups", EditorStyles.boldLabel);
        }

        private float OnGroupHeightCallback(int index) {
            return 70f + 8f + Mathf.Max(1, groups[index].fields.Count) * 80f;
        }

        public void Draw() {
            serializedObject.Update();

            using(new EditorGUILayout.HorizontalScope("box")) {
                if(editMode) {
                    className = EditorGUILayout.TextField(className);
                }
                else {
                    GUILayout.Label(className, EditorStyles.boldLabel);
                }
                if(GUILayout.Button(editMode ? "Cancel" : "Edit", GUILayout.Width(60))) {
                    editMode = !editMode;
                    Generate();
                }
                if(editMode) {
                    if(GUILayout.Button("Save", GUILayout.Width(60))) {
                        CustomizableSettingCodeGen.Save<T, TSettings>(this as T);
                    }
                }
                if(GUILayout.Button("Delete", GUILayout.Width(60))) {

                }
            }

            if(editMode) {
                DrawEditMode();
            }
            else {
                DrawNormalMode();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawNormalMode() {
            foreach(var group in groups) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(group.name, EditorStyles.boldLabel);
                foreach(var field in group.fields) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(field.propertyPath), field.content, true);
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawEditMode() {
            reorderableList.DoLayoutList();
            EditorGUILayout.Space();
            using(new EditorGUI.DisabledScope(true)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Script References", EditorStyles.boldLabel);
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.ObjectField(ScriptReference, typeof(MonoScript), false);
                        EditorGUILayout.ObjectField(InspectorReference, typeof(MonoScript), false);
                    }
                    EditorGUILayout.ObjectField("Setting Reference", target, typeof(TSettings), false);
                }
            }
        }

        #endregion

        #region Group

        public class Group
        {
            #region Variables

            public string name = "";
            public List<Field> fields = new List<Field>();
            private ReorderableList reorderableList;

            #endregion

            #region Constructor

            public Group() {
                reorderableList = new ReorderableList(fields, typeof(Field));
                reorderableList.drawElementCallback += OnDrawElement;
                reorderableList.headerHeight = 0f;
                reorderableList.elementHeight = 80f;
            }

            #endregion

            #region Drawing

            public void Draw(Rect area) {
                reorderableList.DoList(area);
            }

            private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                rect.PadRef(0f, 0f, 4f, 4f);
                rect.SplitVertical(out Rect topArea, out Rect tooltipArea, out Rect bottomArea, 0.25f, 0.5f, 2f);
                topArea.SplitHorizontal(out Rect typeArea, out Rect nameArea, 0.5f, 4f);
                bottomArea.SplitHorizontal(out Rect propTypeArea, out Rect defaultValueArea, 0.5f, 4f);
                var field = fields[index];

                field.typeIndex = EditorGUI.Popup(typeArea, field.typeIndex, supportedTypeNames);
                field.type = supportedTypes[field.typeIndex];

                field.content.text = EditorGUI.TextField(nameArea, field.content.text);
                if(string.IsNullOrEmpty(field.content.text))
                    using(new EditorGUI.DisabledScope(true))
                        EditorGUI.TextArea(nameArea, "Name...");

                field.content.tooltip = EditorGUI.TextArea(tooltipArea, field.content.tooltip);
                if(string.IsNullOrEmpty(field.content.tooltip))
                    using(new EditorGUI.DisabledScope(true))
                        EditorGUI.TextArea(tooltipArea, "Tooltip...");

                field.accessorType = EditorGUI.TextField(propTypeArea, field.accessorType);
                if(string.IsNullOrEmpty(field.accessorType))
                    using(new EditorGUI.DisabledScope(true))
                        EditorGUI.TextArea(propTypeArea, "Custom Property...");

                field.defaultValue = EditorGUI.TextField(defaultValueArea, field.defaultValue);
                if(string.IsNullOrEmpty(field.defaultValue))
                    using(new EditorGUI.DisabledScope(true))
                        EditorGUI.TextArea(defaultValueArea, "Default Value...");
            }

            #endregion

            public class Field
            {
                public Type type;
                public int typeIndex;
                public string defaultValue;
                public string accessorType;

                public GUIContent content = new GUIContent();
                public string propertyPath;
            }
        }

        #endregion
    }
}
