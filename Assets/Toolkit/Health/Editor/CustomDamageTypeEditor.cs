using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditorInternal;
using Toolkit.CodeGenerator;

namespace Toolkit.Health
{
    public class CustomDamageTypeEditor
    {
        public class EnumValues
        {
            public string name;
            public int value;

            public EnumValues() { }

            public EnumValues(string name, int value) {
                this.name = name;
                this.value = value;
            }
        }

        static CustomDamageTypeEditor() {
            Load(customEnumValues, typeof(DamageTypeCustom));
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onCanAddCallback += (list) => list.count < 32;

            Load(defaultList, typeof(DamageType));
            previewDefaultList.drawElementCallback += DrawPreviewDefaultElement;
            previewDefaultList.drawHeaderCallback += DrawPreviewHeader;
            previewDefaultList.draggable = false;
            previewDefaultList.onCanAddCallback = (d) => false;
            previewDefaultList.onCanRemoveCallback = (d) => false;

            Load(dndList, typeof(DamageTypeDnD));
            previewDndList.drawElementCallback += DrawPreviewDndElement;
            previewDndList.drawHeaderCallback += DrawDndPreviewHeader;
            previewDndList.draggable = false;
            previewDndList.onCanAddCallback = (d) => false;
            previewDndList.onCanRemoveCallback = (d) => false;
        }

        private static void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Custom Damage Type Editor");
        }

        private static void DrawPreviewHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Damage Types");
        }

        private static void DrawDndPreviewHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Dnd Damage Types");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect label, 30f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString());
            customEnumValues[index].name = EditorGUI.TextField(label, customEnumValues[index].name);
            customEnumValues[index].value = 1 << index;
        }

        private static void DrawPreviewDefaultElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect label, 30f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString());
            EditorGUI.TextField(label, defaultList[index].name);
        }

        private static void DrawPreviewDndElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect label, 30f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString());
            EditorGUI.TextField(label, dndList[index].name);
        }

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));
        private static List<EnumValues> defaultList = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList previewDefaultList = new ReorderableList(defaultList, typeof(EnumValues));
        private static List<EnumValues> dndList = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList previewDndList = new ReorderableList(dndList, typeof(EnumValues));

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Damage Type", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStylesUtility.BoldLabel);
                DamageTypeEditor.DamageTypeSystem = (DamageTypeSystem)EditorGUILayout.Popup(EditorGUIUtility.TrTextContent("System"), DamageTypeEditor.DamageTypeSystem.ToInt(), DamageTypeSystemContent);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Customize", EditorStylesUtility.BoldLabel);
            reorderableList.DoLayoutList();
            GUILayout.BeginHorizontal();
            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                Save(customEnumValues);
            }

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Reset", GUILayout.Width(80f))) {
                Load(customEnumValues, typeof(DamageTypeCustom));
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(12f);
            var enabled = GUI.enabled;
            GUI.enabled = false;
            previewDndList.DoLayoutList();
            previewDefaultList.DoLayoutList();
            GUI.enabled = enabled;
        }

        private static void Load(List<EnumValues> enumValues, Type customType) {
            enumValues.Clear();

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                enumValues.Add(new EnumValues(names[i], (int)values.GetValue(i)));
            }
        }

        private static void Save(IReadOnlyList<EnumValues> enumValues) {
            var custom = AssetDatabaseUtility.LoadAssetByName<MonoScript>("DamageTypeCustom", "Assets/Toolkit");
            var defaultDamageType = AssetDatabaseUtility.LoadAssetByName<MonoScript>("DamageType", "Assets/Toolkit");
            var path = AssetDatabase.GetAssetPath(defaultDamageType);
            var customPath = path.Replace("DamageType.cs", "");

            var copy = enumValues
                .Select(x => new CodeGenerator.GenUtility.EnumValues(x.name, x.value))
                .ToList();
            copy.Insert(0, new CodeGenerator.GenUtility.EnumValues("None", 0));

            var codeFile = CodeGenerator.GenUtility.CreateEnumClass(
                "DamageTypeCustom",
                "Toolkit.Health",
                copy);

            GenerateUtilityClass(codeFile, enumValues);

            codeFile.CreateFile(customPath);
            AssetDatabase.ImportAsset(System.IO.Path.Combine(customPath, "DamageTypeCustom.cs"), ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateUtilityClass(CodeFile file, IReadOnlyList<EnumValues> enumValues) {
            var ns = file.GetContainers<CodeNamespace>()[0];
            var c = ns.AddClass(new CodeClass("DamageTypeCustomUtility", AccessModifier.PublicStatic));

            List<string> toString = new List<string>();
            toString.Add("switch(type) {");
            foreach(var val in enumValues) {
                toString.Add($"{GenUtility.INDENT}case DamageTypeCustom.{GenUtility.VerifyName(val.name)}: return \"{val.name}\";");
            }
            toString.Add("}");
            toString.Add("return \"Unknown\";");

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "string",
               "ToString",
               new CodeVariable[] { new CodeVariable(AccessModifier.This, typeof(DamageTypeCustom), "type") },
               new CodeBlock(toString)));
        }

        private static GUIContent[] DamageTypeSystemContent = new GUIContent[]{
            new GUIContent("None", "Default view!"),
            new GUIContent("Dungeons and Dragons", "Uses the 5th edition damage types."),
            new GUIContent("Custom", "Has a custom damage type.")
        };
    }
}
