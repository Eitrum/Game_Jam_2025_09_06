using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Toolkit.CodeGenerator;
using System;
using UnityEditorInternal;

namespace Toolkit.Inventory
{
    public class CustomItemTypeEditor : Editor
    {

        #region Enum Values

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

        #endregion

        #region Variables

        public static event Action OnBeforeSave;
        public static event Action OnAfterSave;

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));
        private static int highestIndex = 0;
        public static IReadOnlyList<EnumValues> ItemTypeEnumValues => customEnumValues;

        #endregion

        #region Drawing

        private static void DrawHeader(Rect rect) {
            rect.PadRef(12, 0, 0, 0);
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect idArea, 20f / rect.width, 1f - 80f / rect.width, 2f);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(idArea, "Id");
            EditorGUI.LabelField(nameArea, "Item Type");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect idArea, 20f / rect.width, 1f - 80f / rect.width, 2f);
            EditorGUI.LabelField(indexArea, $"{index}");
            EditorGUI.LabelField(idArea, $"={customEnumValues[index].value}");
            customEnumValues[index].name = EditorGUI.TextField(nameArea, customEnumValues[index].name);
        }

        private static void OnGUI(string s) {
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                Save(customEnumValues);
            }
        }

        #endregion

        #region Loading

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Inventory", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        static CustomItemTypeEditor() {
            Load(customEnumValues);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onAddCallback += Add;
        }

        private static void Add(ReorderableList list) {
            var id = ++highestIndex;
            customEnumValues.Add(new EnumValues("new", id));
        }

        private static void Load(List<EnumValues> enumValues) {
            enumValues.Clear();

            var customType = typeof(ItemType);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                enumValues.Add(new EnumValues(ItemTypeUtility.ToString((ItemType)(int)values.GetValue(i)), (int)values.GetValue(i)));
            }
            highestIndex = enumValues.Max(x => x.value);
        }

        #endregion

        #region Saving Generating

        private static void Save(IReadOnlyList<EnumValues> enumValues) {
            var copy = enumValues
                .Select(x => new CodeGenerator.GenUtility.EnumValues(x.name, x.value))
                .ToList();
            copy.Insert(0, new CodeGenerator.GenUtility.EnumValues("None", 0));

            var codeFile = CodeGenerator.GenUtility.CreateEnumClass(
                "ItemType",
                "Toolkit.Inventory",
                copy);


            GenerateUtilityClass(codeFile, enumValues);
            OnBeforeSave?.Invoke();
            var path = codeFile.CreateFile("Assets/Toolkit/Inventory/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            OnAfterSave?.Invoke();
        }

        private static void GenerateUtilityClass(CodeFile file, IReadOnlyList<EnumValues> enumValues) {
            file.AddUsing(typeof(UnityEngine.Color));
            var ns = file.GetContainers<CodeNamespace>()[0];
            var c = ns.AddClass(new CodeClass("ItemTypeUtility", AccessModifier.PublicStatic));

            List<string> toString = new List<string>();
            toString.Add("switch(slot) {");
            toString.Add($"{GenUtility.INDENT}case ItemType.None: return \"None\";");
            foreach(var val in enumValues) {
                toString.Add($"{GenUtility.INDENT}case ItemType.{GenUtility.VerifyName(val.name)}: return \"{val.name}\";");
            }
            toString.Add("}");
            toString.Add("return \"Unknown\";");

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "string",
               "ToString",
               new CodeVariable[] { new CodeVariable(AccessModifier.This, typeof(ItemType), "slot") },
               new CodeBlock(toString)));
        }

        #endregion
    }
}
