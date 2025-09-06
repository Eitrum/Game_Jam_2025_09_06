using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using Toolkit.CodeGenerator;

namespace Toolkit.Currency
{
    public static class CurrencyEditor
    {
        private class Names
        {
            public string name = "";
            public string suffix = "";
            public bool unlocked = false;
            public int conversion = 100;

            public Texture2D editorTexture;

            public string Suffix => string.IsNullOrEmpty(suffix) ? GetSuffix(name) : suffix;
        }

        private static int[] STEPS = {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        private static List<Names> names = new List<Names>();
        private static ReorderableList reorderableList = new ReorderableList(names, typeof(Names));
        private static bool hasModifiedConversion = false;
        private static bool hasModifiedUnlocked = false;

        public static string GetSuffix(string name) {
            if(string.IsNullOrEmpty(name)) {
                return "";
            }
            var split = name.Split(' ');
            return string.Join("", split.Select(x => x.ToLower().FirstOrDefault()));
        }

        #region Initialization

        static CurrencyEditor() {
            Load();
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onCanAddCallback += (list) => list.count < 32;
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 2f + 8f;
            reorderableList.draggable = false;
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Currency", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void Load() {
            names.Clear();
            var values = System.Enum.GetValues(typeof(CurrencyType));
            for(int i = 0; i < values.Length; i++) {
                var curType = (CurrencyType)values.GetValue(i);
                var n = CurrencyUtility.GetName(curType);
                var s = CurrencyUtility.GetSuffix(curType);
                var val = CurrencyUtility.GetValue(curType);
                names.Add(new Names() {
                    name = n,
                    suffix = s == GetSuffix(n) ? "" : s,
                    conversion = val,
                    unlocked = !STEPS.Contains(val, 0, STEPS.Length),
                    editorTexture = CurrencyEditorIcons.GetTexture(curType)
                });
            }
        }

        #endregion

        #region Drawing

        private static void OnGUI(string obj) {
            if(hasModifiedUnlocked || (hasModifiedConversion && Event.current.type == EventType.MouseUp && Event.current.button == 0)) {
                hasModifiedConversion = false;
                hasModifiedUnlocked = false;
                Sort.Merge(names, (a, b) => a.conversion.CompareTo(b.conversion));
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
                reorderableList.DoLayoutList();
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("Save", GUILayout.Width(80f))) {
                        Generate(names);
                    }
                }
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);
                int calculation = 1;
                int last = 1;
                for(int i = 0; i < names.Count; i++) {
                    var n = names[i];
                    var diff = n.conversion / last;
                    var rem = n.conversion % last;
                    calculation *= diff;
                    last = n.conversion;

                    EditorGUILayout.LabelField($"{i}. <i>{n.name}</i> ({n.Suffix})", EditorStylesUtility.RichTextBoldLabel);
                    EditorGUILayout.LabelField($"{int.MaxValue / n.conversion:N0} total pieces");
                    EditorGUILayout.LabelField($"{(diff > 0 ? "+" : "")}{diff}x increased value");
                    EditorGUILayout.LabelField($"{rem} remaining value {(rem != 0 ? $"<i><size=10>(this should be always be 0)</size></i>" : "")}", EditorStylesUtility.RichTextLabel);
                    EditorGUILayout.Space();
                }
            }
        }

        private static void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Currency Tiers", EditorStyles.boldLabel);
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var n = names[index];
            rect.ShrinkRef(3f);
            // Setup Area
            rect.SplitHorizontal(out Rect imageArea, out rect, (EditorGUIUtility.singleLineHeight * 2f + 2f) / rect.width, 2f);
            rect.SplitVertical(out Rect nameArea, out Rect valueArea, 0.5f, 2f);
            nameArea.SplitHorizontal(out nameArea, out Rect suffixArea, 1f - (90f / nameArea.width), 4f);

            // Draw Editor Texture
            n.editorTexture = EditorGUI.ObjectField(imageArea, n.editorTexture, typeof(Texture2D), false) as Texture2D;

            // Draw Name + Suffix
            n.name = EditorGUI.TextField(nameArea, n.name);
            if(string.IsNullOrEmpty(n.name)) {
                EditorGUI.LabelField(nameArea, "Enter name...", EditorStylesUtility.ItalicLabel);
            }
            n.suffix = EditorGUI.TextField(suffixArea, n.suffix);
            if(string.IsNullOrEmpty(n.suffix)) {
                EditorGUI.LabelField(suffixArea, $"Suffix... {(string.IsNullOrEmpty(n.name) ? "" : $"({GetSuffix(n.name)})")}", EditorStylesUtility.ItalicLabel);
            }

            // Draw values
            n.unlocked = EditorGUI.Toggle(valueArea.Pad(0, valueArea.width - 16f, 0, 0), n.unlocked);

            valueArea.PadRef(18f, 0, 0, 0);
            EditorGUI.BeginChangeCheck();
            if(n.unlocked) {
                n.conversion = Mathf.Clamp(EditorGUI.DelayedIntField(valueArea, "Value", n.conversion), 1, int.MaxValue);
            }
            else {
                var stepValue = 0;
                for(int i = 0, length = STEPS.Length; i < length; i++) {
                    if(STEPS[i] == n.conversion) {
                        stepValue = i;
                    }
                }
                stepValue = EditorGUI.IntSlider(valueArea, "Value - " + n.conversion, stepValue, 0, STEPS.Length - 1);
                n.conversion = STEPS[stepValue];
            }
            if(EditorGUI.EndChangeCheck()) {
                hasModifiedConversion = true;
                hasModifiedUnlocked = n.unlocked;
            }
        }

        #endregion

        #region Code Generation

        private static void Generate(IReadOnlyList<Names> array) {
            // Verify unique-ness
            HashSet<string> temp = new HashSet<string>();
            foreach(var a in array) {
                if(temp.Contains(GenUtility.VerifyName(a.name))) {
                    Debug.LogError("Currency types do not have unique names");
                    return;
                }
                else {
                    temp.Add(GenUtility.VerifyName(a.name));
                }
            }

            // Fix calculations
            List<int> diffs = new List<int>();
            int last = 1;
            for(int i = 0; i < array.Count; i++) {
                var n = array[i];
                var diff = n.conversion / last;
                var rem = n.conversion % last;
                last = n.conversion;
                if(i > 0)
                    diffs.Add(diff);
            }


            var switchCases = array.Select(x => $"CurrencyType.{GenUtility.VerifyName(x.name)}").ToArray();
            GenerateExtension(array, diffs, switchCases);
            GenerateUtility(array, diffs, switchCases);
            GenerateCurrencyType(array);
            GenerateCurrencyEditorIcons(array);
        }

        private static void GenerateExtension(IReadOnlyList<Names> array, IReadOnlyList<int> diffs, IReadOnlyList<string> switchCases) {
            var file = new CodeFile("CurrencyExtended");
            file.UseCleanProcess = true;
            file.SetCreatorTag(typeof(CurrencyEditor));
            var ns = file.AddNamespace("Toolkit.Currency");
            var c = ns.AddStruct(new CodeStruct(AccessModifier.Public | AccessModifier.Partial, "Currency"));

            array.Foreach((arr, i) => {
                c.AddProperty(new CodeProperty(
                    AccessModifier.Public,
                    "int",
                    arr.name,
                    new CodeBlock($"(value {(arr.conversion == 1 ? "" : $"/ {arr.conversion}")}){(diffs.Count > i ? $" % {diffs[i]}" : "")};")));

                c.AddProperty(new CodeProperty(
                    AccessModifier.Public,
                    "float",
                    $"Total{GenUtility.VerifyName(arr.name)}",
                    new CodeBlock($"value / {arr.conversion}f;")));
            });

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "Add",
                new CodeVariable[]{
                    new CodeVariable("CurrencyType", "type"),
                    new CodeVariable("int", "amount")
                },
                new CodeBlock(
                    switchCases
                        .SelectWithIndex((x, i) => $"case {x}: value += amount * {array[i].conversion}; break;")
                        .Insert(0, "switch(type){")
                        .AddEnumerator("}")
                        .ToArray())));

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "Remove",
                new CodeVariable[]{
                    new CodeVariable("CurrencyType", "type"),
                    new CodeVariable("int", "amount")
                },
                new CodeBlock(
                    switchCases
                        .SelectWithIndex((x, i) => $"case {x}: value -= amount * {array[i].conversion}; break;")
                        .Insert(0, "switch(type){")
                        .AddEnumerator("}")
                        .ToArray())));

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "int",
                "GetAmount",
                new CodeVariable("CurrencyType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    switchCases,
                    array.Select(x => $"{GenUtility.VerifyName(x.name)}").ToArray(),
                    "return value;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.Public | AccessModifier.Override,
                "string",
                "ToString",
                new CodeBlock($"$\"{string.Join(" ", array.SelectWithIndex((x, i) => $"{{{GenUtility.VerifyName(x.name)}}}{array[i].Suffix}"))}\";")));

            var path = file.CreateFile("Assets/Toolkit/Currency");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateUtility(IReadOnlyList<Names> array, IReadOnlyList<int> diffs, IReadOnlyList<string> switchCases) {
            var file = new CodeFile("CurrencyUtility");
            file.UseCleanProcess = true;
            file.SetCreatorTag(typeof(CurrencyEditor));
            file.AddUsing("System");
            file.AddUsing("UnityEngine");

            var ns = file.AddNamespace("Toolkit.Currency");
            var c = ns.AddClass(AccessModifier.PublicStatic, "CurrencyUtility");

            foreach(var arr in array) {
                c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", $"{arr.name}_VALUE", arr.conversion.ToString()));
            }
            array.Foreach((x, i) => {
                c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", $"{x.name}_REM", (diffs.Count > i ? diffs[i].ToString() : ($"int.MaxValue / {x.conversion}"))));
            });

            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "CURRENCY_TIERS", array.Count.ToString()));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable("CurrencyType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    switchCases,
                    array.Select(x => $"\"{x.name}\"").ToArray(),
                    "return \"\";")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetSuffix",
                new CodeVariable("CurrencyType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    switchCases,
                    array.Select(x => $"\"{x.Suffix}\"").ToArray(),
                    "return \"\";")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "int",
                "GetValue",
                new CodeVariable("CurrencyType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    switchCases,
                    array.Select(x => $"{x.conversion}").ToArray(),
                    "return 1;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.InternalStatic,
                "int",
                "GetRem",
                new CodeVariable("CurrencyType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    switchCases,
                    array.Select(x => $"{GenUtility.VerifyName(x.name)}_REM").ToArray(),
                    "return int.MaxValue;")));

            var path = file.CreateFile("Assets/Toolkit/Currency");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateCurrencyType(IReadOnlyList<Names> array) {
            var file = new CodeFile("CurrencyType");
            file.SetCreatorTag(typeof(CurrencyEditor));
            file.UseCleanProcess = true;
            var ns = file.AddNamespace("Toolkit.Currency");
            ns.AddEnum(new CodeEnum(
                AccessModifier.Public,
                "CurrencyType",
                array.Select(x => x.name).ToArray()));

            var path = file.CreateFile("Assets/Toolkit/Currency");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateCurrencyEditorIcons(IReadOnlyList<Names> array) {
            var file = new CodeFile("CurrencyEditorIcons");
            file.SetCreatorTag(typeof(CurrencyEditor));
            file.UseCleanProcess = true;
            file.AddUsing(typeof(Texture2D));
            file.AddUsing(typeof(AssetDatabase));

            var ns = file.AddNamespace("Toolkit.Currency");
            var c = ns.AddClass(AccessModifier.PublicStatic, "CurrencyEditorIcons");

            var toAdd = array.Where(x => x.editorTexture != null);

            toAdd.Foreach((x, i) => {
                c.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string", $"{GenUtility.VerifyName(x.name)}IconPath", $"\"{AssetDatabase.GetAssetPath(x.editorTexture)}\""));
                c.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "Texture2D", $"{GenUtility.VerifyName(x.name)}Icon"));
            });

            c.AddMethod(new CodeMethod(AccessModifier.PublicStatic,
                "Texture2D",
                "GetTexture",
                new CodeVariable("CurrencyType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    toAdd.Select(x => $"CurrencyType.{GenUtility.VerifyName(x.name)}").ToArray(),
                    toAdd.Select(x => $"{GenUtility.VerifyName(x.name)}Icon").ToArray(),
                    "return null;")));

            c.AddCustom(new CodeAttribute(typeof(InitializeOnLoadMethodAttribute)));
            var init = c.AddMethod(new CodeMethod(
                AccessModifier.PrivateStatic,
                "Initialize",
                new CodeBlock(
                    toAdd.Select(x => $"{GenUtility.VerifyName(x.name)}Icon = AssetDatabase.LoadAssetAtPath<Texture2D>({GenUtility.VerifyName(x.name)}IconPath);").ToArray())));

            var path = file.CreateFile("Assets/Toolkit/Currency/Editor");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        #endregion
    }
}
