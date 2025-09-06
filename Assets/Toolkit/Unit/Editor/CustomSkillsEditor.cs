using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Toolkit.CodeGenerator;
using System.Linq;
using System;
using UnityEditorInternal;

namespace Toolkit.Unit
{
    public static class CustomSkillsEditor
    {
        public class EditorSkillType
        {
            public string SkillName;
            public List<string> SkillCategories = new List<string>();

            public string FormattedSkillName => GenUtility.VerifyName(SkillName);
            public string FormattedSkillCategories {
                get {
                    if(SkillCategories.Count == 0) {
                        return "SkillCategory.None";
                    }
                    return string.Join(" | ", SkillCategories.Select(x => $"SkillCategory.{GenUtility.VerifyName(x)}"));
                }
            }

            public EditorSkillType() { }
            public EditorSkillType(string name) { this.SkillName = name; }
            public EditorSkillType(string name, SkillCategory category) {
                this.SkillName = name;
                var categoryType = typeof(SkillCategory);
                var categoryValues = System.Enum.GetValues(categoryType);
                for(int i = 1; i < categoryValues.Length; i++) {
                    var cat = (SkillCategory)categoryValues.GetValue(i);
                    if(category.HasFlag(cat)) {
                        var t = SkillUtility.GetName(cat);
                        SkillCategories.Add(t);
                    }
                }
            }
        }

        private static List<EditorSkillType> skillTypes = new List<EditorSkillType>();
        private static UnityEditorInternal.ReorderableList skillTypesList;

        private static List<string> skillCategories = new List<string>();
        private static UnityEditorInternal.ReorderableList skillCategoriesList;

        public static IReadOnlyList<string> Categories => skillCategories;
        public static IReadOnlyList<EditorSkillType> Types => skillTypes;


        [InitializeOnLoadMethod]
        private static void Load() {
            skillCategories.Clear();
            skillTypes.Clear();

            var categoryType = typeof(SkillCategory);
            var categoryValues = System.Enum.GetValues(categoryType);
            for(int i = 1; i < categoryValues.Length; i++) {
                var cat = (SkillCategory)categoryValues.GetValue(i);
                var name = SkillUtility.GetName(cat);
                skillCategories.Add(name);
            }

            var skillType = typeof(SkillType);
            var skillValues = System.Enum.GetValues(skillType);
            for(int i = 1; i < skillValues.Length; i++) {
                var type = (SkillType)i;
                var category = SkillUtility.GetSkillCategory(type);
                var name = SkillUtility.GetName(type);
                skillTypes.Add(new EditorSkillType(name, category));
            }

            if(skillCategoriesList == null) {
                skillCategoriesList = new UnityEditorInternal.ReorderableList(skillCategories, typeof(string));
                skillCategoriesList.drawElementCallback += DrawSkillCategoryElement;
                skillCategoriesList.drawHeaderCallback += DrawSkillCategoryHeader;
                skillCategoriesList.onAddCallback += (r) => { if(skillCategories.Count < 32) skillCategories.Add(""); };
                skillCategoriesList.onRemoveCallback += OnRemoveSkillCategory;
                skillCategoriesList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
            }

            if(skillTypesList == null) {
                skillTypesList = new UnityEditorInternal.ReorderableList(skillTypes, typeof(EditorSkillType));
                skillTypesList.drawElementCallback += DrawSkillTypeElement;
                skillTypesList.drawHeaderCallback += DrawSkillTypeHeader;
                skillTypesList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
            }
        }

        private static void OnRemoveSkillCategory(ReorderableList list) {
            var category = skillCategories[list.index];
            skillCategories.RemoveAt(list.index);
            skillTypes.Foreach(x => x.SkillCategories.Remove(category));
        }

        private static void DrawSkillCategoryHeader(Rect rect) {
            rect.PadRef(16, 0, 0, 0);
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect countArea, 20f / rect.width, 1f - (80f / rect.width), 4f);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(nameArea, "Categories");
            EditorGUI.LabelField(countArea, "Uses");
        }

        private static void DrawSkillCategoryElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 2, 2);
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect countArea, 20f / rect.width, 1f - (80f / rect.width), 4f);
            EditorGUI.LabelField(indexArea, index.ToString());
            var current = skillCategories[index];
            var newName = EditorGUI.DelayedTextField(nameArea, current).Trim();
            if(skillCategories[index] != newName) {
                skillTypes.Foreach(x => {
                    if(x.SkillCategories.Remove(current)) {
                        x.SkillCategories.Add(newName);
                    }
                });
                skillCategories[index] = newName;
            }
            var uses = skillTypes.Sum(x => x.SkillCategories.Contains(current) ? 1 : 0);
            EditorGUI.LabelField(countArea, uses.ToString());
        }

        private static void DrawSkillTypeHeader(Rect rect) {
            rect.PadRef(16f, 0, 0, 0);
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect categoryArea, 20f / rect.width, 1f - (140f / rect.width), 4f);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(nameArea, "Skills");
            EditorGUI.LabelField(categoryArea, "Category");
        }

        private static void DrawSkillTypeElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect categoryArea, 20f / rect.width, 1f - (140f / rect.width), 4f);
            EditorGUI.LabelField(indexArea, index.ToString());
            skillTypes[index].SkillName = EditorGUI.TextField(nameArea, skillTypes[index].SkillName);
            var currentCategories = skillTypes[index].SkillCategories;
            var allCategories = Categories.ToArray();
            int selected = 0;
            for(int i = 0; i < allCategories.Length; i++) {
                if(currentCategories.Contains(allCategories[i]))
                    selected |= 1 << i;
            }

            var output = EditorGUI.MaskField(categoryArea, selected, allCategories);
            if(output != selected) {
                currentCategories.Clear();
                if(output == -1) {
                    currentCategories.AddRange(allCategories);
                }
                else {
                    for(int i = 0; i < allCategories.Length; i++) {
                        var ii = 1 << i;
                        if((output & (ii)) == ii) {
                            currentCategories.Add(allCategories[i]);
                        }
                    }
                }
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Skills", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            skillCategoriesList.DoLayoutList();
            skillTypesList.DoLayoutList();

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Save", GUILayout.Width(80f))) {
                    // Generate();
                    GenerateBehaviourClass();
                }

                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Reload", GUILayout.Width(80f))) {
                    Load();
                }
                if(GUILayout.Button("Reset", GUILayout.Width(80f))) {
                    skillCategories.Clear();
                    skillTypes.Clear();
                }
            }
        }

        private static void Generate() {
            var file = new CodeFile("SkillType");
            file.SetCreatorTag(typeof(CustomSkillsEditor));
            file.UseCleanProcess = true;
            file.AddUsing("System");
            file.AddUsing("UnityEngine");
            file.AddUsing("System.Linq");
            var ns = file.AddNamespace("Toolkit.Unit");

            var tEnu = ns.AddEnum(new CodeEnum(AccessModifier.Public, "SkillType", Types.Select(x => x.SkillName).Insert(0, "None").ToArray()));
            int index = -2;
            var catEnu = ns.AddEnum(new CodeEnum(AccessModifier.Public, "SkillCategory", Categories.Insert(0, "None").Select(x => new CodeVariable("int", x, ((++index) >= 0 ? (1 << index) : 0).ToString())).ToArray()));
            catEnu.AddAttribute(typeof(System.FlagsAttribute));

            var c = ns.AddClass(new CodeClass("SkillUtility", AccessModifier.PublicStatic));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "SKILL_COUNT", Types.Count.ToString()));

            var typeCases = Types.Select(x => $"SkillType.{x.FormattedSkillName}").ToArray();

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                GenUtility.VerifyName(catEnu.Name),
                "GetSkillCategory",
                new CodeVariable(tEnu.Name, "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    typeCases,
                    Types.Select(x => $"{x.FormattedSkillCategories}").ToArray(),
                    $"return SkillCategory.None;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "bool",
                "IsSkillCategory",
                new CodeVariable[]{
                    new CodeVariable("SkillType", "type"),
                    new CodeVariable("SkillCategory", "category")
                },
                new CodeBlock(new string[]{
                    "var skillCat = GetSkillCategory(type);",
                    "return (skillCat == SkillCategory.None && category == SkillCategory.None) || ((skillCat & category) == category);"
                })));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable("SkillType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    typeCases,
                    Types.Select(x => $"\"{x.SkillName}\"").ToArray(),
                    "return \"\";")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable("SkillCategory", "category"),
                CodeBlock.CreateReturnSwitchBlock(
                    "category",
                    Categories.Select(x => $"SkillCategory.{GenUtility.VerifyName(x)}").ToArray(),
                    Categories.Select(x => $"\"{x}\"").ToArray(),
                    "return \"\";")));

            var path = file.CreateFile("Assets/Toolkit/Unit/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateBehaviourClass() {
            var file = new CodeFile("Skills");
            file.SetCreatorTag(typeof(CustomSkillsEditor));
            file.UseCleanProcess = true;
            file.AddUsing("System");
            file.AddUsing("UnityEngine");
            file.AddUsing("System.Linq");
            var ns = file.AddNamespace("Toolkit.Unit");

            // ISkills Interface
            var inf = ns.Add(new CodeInterface("ISkills", AccessModifier.Public));
            foreach(var t in Types)
                inf.AddProperty(new CodeProperty(AccessModifier.Public, "ISkill", t.FormattedSkillName, new CodeBlock("...")));

            inf.AddMethod(new CodeMethod(AccessModifier.Public, "ISkill", "GetSkill", new CodeVariable("SkillType", "type"), CodeBlock.Empty()));
            inf.AddMethod(new CodeMethod(AccessModifier.Public, "ISkill[]", "GetSkills", new CodeVariable("SkillCategory", "category"), CodeBlock.Empty()));
            inf.AddMethod(new CodeMethod(AccessModifier.Public, "ISkill[]", "GetAllSkills", CodeBlock.Empty()));

            // Base Class
            var c = ns.AddClass(new CodeClass(
            "Skills",
            AccessModifier.Public,
            "ISkills"));

            foreach(var t in Types) {
                var va = c.AddVariable(new CodeVariable(AccessModifier.Private, "Skill", $"_{t.FormattedSkillName}", $"new Skill(SkillType.{t.FormattedSkillName})"));
                va.AddAttribute(typeof(SerializeField));
                c.AddProperty(new CodeProperty(AccessModifier.Public, "ISkill", t.FormattedSkillName, new CodeBlock($"return _{t.FormattedSkillName};")));
            }

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "ISkill[]",
                "GetAllSkills",
                new CodeBlock($"return new ISkill[]{{{string.Join(", ", Types.Select(x => $"_{x.FormattedSkillName}"))}}};")));

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "ISkill",
                "GetSkill",
                new CodeVariable("SkillType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                     Types.Select(x => $"SkillType.{x.FormattedSkillName}").ToArray(),
                     Types.Select(x => $"_{x.FormattedSkillName}").ToArray(),
                     "return null;")));

            c.AddMethod(new CodeMethod(AccessModifier.Public,
                "ISkill[]",
                "GetSkills",
                new CodeVariable("SkillCategory", "category"),
                new CodeBlock("return GetAllSkills().Where(x => SkillUtility.IsSkillCategory(x.SkillType, category)).ToArray();")));

            var bFile = new CodeFile("SkillsBehaviour");
            bFile.SetCreatorTag(typeof(CustomSkillsEditor));
            bFile.UseCleanProcess = true;
            bFile.AddUsing("System");
            bFile.AddUsing("UnityEngine");
            bFile.AddUsing("System.Linq");
            var bns = bFile.AddNamespace("Toolkit.Unit");
            var bc = bns.AddClass(new CodeClass(
                "SkillsBehaviour",
                AccessModifier.Public,
                new string[] { "MonoBehaviour", "ISkills" }));
            bc.AddAttribute(new CodeAttribute(typeof(AddComponentMenu), "\"Toolkit/Unit/Skills\""));
            var nodes = c.CodeNodes;
            foreach(var n in nodes)
                bc.AddCustom(n);

            var path = file.CreateFile("Assets/Toolkit/Unit/");
            var bpath = bFile.CreateFile("Assets/Toolkit/Unit/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
