using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Inventory
{
    public class CustomEquipmentSlotEditor
    {

        #region Events

        public static event Action OnBeforeSave;
        public static event Action OnAfterSave;

        #endregion

        #region Classes

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

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));
        private static int highestIndex = 0;
        public static IReadOnlyList<EnumValues> EquipmentSlotEnumValues => customEnumValues;

        #endregion

        #region Initialization

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Inventory/Equipment Slots", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        static CustomEquipmentSlotEditor() {
            Load(customEnumValues);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onAddCallback += Add;
            reorderableList.displayRemove = false;
            reorderableList.draggable = false;
        }

        private static void Add(ReorderableList list) {
            customEnumValues.Add(new EnumValues("new", ++highestIndex));
        }

        #endregion

        #region Drawing / enum values

        private static void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Equipment Slots Editor");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect label, 30f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString());
            label.ShrinkRef(1);
            customEnumValues[index].name = EditorGUI.TextField(label, customEnumValues[index].name);
        }

        private static void OnGUI(string s) {
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                Save(customEnumValues);
            }

            CustomEquipmentRuleEditor.OnGUI();
        }

        #endregion

        #region Loading Saving

        private static void Load(List<EnumValues> enumValues) {
            enumValues.Clear();

            var customType = typeof(EquipmentSlot);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                enumValues.Add(new EnumValues(EquipmentSlotUtility.ToString((EquipmentSlot)(int)values.GetValue(i)), (int)values.GetValue(i)));
            }
            highestIndex = enumValues.Max(x => x.value);
        }

        public static void Save(IReadOnlyList<EnumValues> enumValues) {
            var copy = enumValues
                .Select(x => new CodeGenerator.GenUtility.EnumValues(x.name, x.value))
                .ToList();
            copy.Insert(0, new CodeGenerator.GenUtility.EnumValues("None", 0));

            var codeFile = CodeGenerator.GenUtility.CreateEnumClass(
                "EquipmentSlot",
                "Toolkit.Inventory",
                copy);


            GenerateUtilityClass(codeFile, enumValues);
            OnBeforeSave?.Invoke();
            var path = codeFile.CreateFile("Assets/Toolkit/Inventory/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            OnAfterSave?.Invoke();
            GenerateEquipmentSlotMaskClass(enumValues);
        }

        private static void GenerateUtilityClass(CodeFile file, IReadOnlyList<EnumValues> enumValues) {
            file.AddUsing(typeof(UnityEngine.Color));
            var ns = file.GetContainers<CodeNamespace>()[0];
            var c = ns.AddClass(new CodeClass("EquipmentSlotUtility", AccessModifier.PublicStatic));

            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, typeof(int), "SLOTS", enumValues.Count.ToString()));

            List<string> toString = new List<string>();
            toString.Add("switch(slot) {");
            toString.Add($"{GenUtility.INDENT}case EquipmentSlot.None: return \"None\";");
            foreach(var val in enumValues) {
                toString.Add($"{GenUtility.INDENT}case EquipmentSlot.{GenUtility.VerifyName(val.name)}: return \"{val.name}\";");
            }
            toString.Add("}");
            toString.Add("return \"Unknown\";");

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "string",
               "ToString",
               new CodeVariable[] { new CodeVariable(AccessModifier.This, typeof(EquipmentSlot), "slot") },
               new CodeBlock(toString)));

            CustomEquipmentRuleEditor.GenerateEquipmentRules(enumValues, c);
        }

        private static void GenerateEquipmentSlotMaskClass(IReadOnlyList<EnumValues> enumValues) {
            var file = new CodeFile("EquipmentSlotMask");
            var ns = file.AddNamespace("Toolkit.Inventory");

            var c = ns.AddClass(new CodeClass("EquipmentSlotMask", AccessModifier.Public, typeof(ScriptableObject)));
            foreach(var val in enumValues)
                c.AddVariable(new CodeVariable(AccessModifier.Public, "bool", val.name, "true"));

            List<string> slotToBool = new List<string>();
            slotToBool.Add("switch(slot){");
            foreach(var val in enumValues) {
                var name = GenUtility.VerifyName(val.name);
                slotToBool.Add($"{GenUtility.INDENT}case EquipmentSlot.{name}: return {name};");
            }
            slotToBool.Add("}");
            slotToBool.Add("return false;");
            c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    "bool",
                    "IsSlotEnabled",
                    new CodeVariable[] { new CodeVariable("EquipmentSlot", "slot") },
                    new CodeBlock(slotToBool)));

            var path = file.CreateFile("Assets/Toolkit/Inventory/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        #endregion
    }
}
