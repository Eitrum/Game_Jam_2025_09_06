using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    public class CustomRarityEditor
    {
        public class EnumValues
        {
            public string name;
            public int value;
            public Color32 color;

            public EnumValues() { }

            public EnumValues(string name, int value, Color32 color) {
                this.name = name;
                this.value = value;
                this.color = color;
            }
        }

        static CustomRarityEditor() {
            Load(customEnumValues);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onCanAddCallback += (list) => list.count < 32;
        }

        private static void DrawHeader(Rect rect) {
            rect.ShrinkRef(16f, RectExtensions.ShrinkSide.Left);
            rect.SplitHorizontal(out Rect indexArea, out Rect label, out Rect colorArea, 30f / rect.width);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(label, "Names");
            EditorGUI.LabelField(colorArea, "Colors");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect label, out Rect colorArea, 30f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString());
            customEnumValues[index].name = EditorGUI.TextField(label, customEnumValues[index].name);
            customEnumValues[index].color = EditorGUI.ColorField(colorArea, customEnumValues[index].color);
            customEnumValues[index].value = index + 1;
        }

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));


        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Inventory/Rarity", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                Save(customEnumValues);
            }
        }

        private static void Load(List<EnumValues> enumValues) {
            enumValues.Clear();

            var customType = typeof(Rarity);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                enumValues.Add(new EnumValues(names[i], (int)values.GetValue(i), RarityUtility.ToColor((Rarity)(int)values.GetValue(i))));
            }
        }

        private static void Save(IReadOnlyList<EnumValues> enumValues) {
            var copy = enumValues
                .Select(x => new CodeGenerator.GenUtility.EnumValues(x.name, x.value))
                .ToList();
            copy.Insert(0, new CodeGenerator.GenUtility.EnumValues("None", 0));

            var codeFile = CodeGenerator.GenUtility.CreateEnumClass(
                "Rarity",
                "Toolkit.Inventory",
                copy);


            GenerateUtilityClass(codeFile, enumValues);

            var path = codeFile.CreateFile("Assets/Toolkit/Inventory/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            RarityDistributionCodeGeneration.Generate(enumValues.Select(x => x.name).ToArray());
        }

        private static void GenerateUtilityClass(CodeFile file, IReadOnlyList<EnumValues> enumValues) {
            file.AddUsing(typeof(UnityEngine.Color));
            var ns = file.GetContainers<CodeNamespace>()[0];
            var c = ns.AddClass(new CodeClass("RarityUtility", AccessModifier.PublicStatic));

            List<string> colorReturn = new List<string>();
            colorReturn.Add("switch(rarity) {");
            foreach(var val in enumValues) {
                var col = val.color;
                colorReturn.Add($"{GenUtility.INDENT}case Rarity.{GenUtility.VerifyName(val.name)}: return new Color32({col.r},{col.g},{col.b},{col.a});");
            }
            colorReturn.Add("}");
            colorReturn.Add("return new Color32(0,0,0,0);");

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                typeof(UnityEngine.Color32),
                "ToColor",
                new CodeVariable[] { new CodeVariable(AccessModifier.This, typeof(Rarity), "rarity") },
                new CodeBlock(colorReturn)));

            List<string> hexReturn = new List<string>();
            hexReturn.Add("switch(rarity) {");
            foreach(var val in enumValues) {
                var col = val.color;
                hexReturn.Add($"{GenUtility.INDENT}case Rarity.{GenUtility.VerifyName(val.name)}: return \"{col.ToHex32()}\";");
            }
            hexReturn.Add("}");
            hexReturn.Add("return \"00000000\";");

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "ToHex",
                new CodeVariable[] { new CodeVariable(AccessModifier.This, typeof(Rarity), "rarity") },
                new CodeBlock(hexReturn)));

            List<string> toString = new List<string>();
            toString.Add("switch(rarity) {");
            foreach(var val in enumValues) {
                toString.Add($"{GenUtility.INDENT}case Rarity.{GenUtility.VerifyName(val.name)}: return \"{val.name}\";");
            }
            toString.Add("}");
            toString.Add("return \"\";");

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "string",
               "ToString",
               new CodeVariable[] { new CodeVariable(AccessModifier.This, typeof(Rarity), "rarity") },
               new CodeBlock(toString)));
        }
    }
}
