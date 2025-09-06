using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    public static class NavMeshCodeGenerator
    {


        public static void Generate() {
            GenerateBuildRules();
            GenerateBuildRuleMask();
            GenerateQueryFilterRules();
        }

        public static void GenerateQueryFilterRules(bool force = false) {
            var rules = NavMeshQueryRulesEditor.Rules;
            var generationRules = NavMeshGeneratorSettingsEditor.Rules;
            FixUniqueNames(rules, x => x.Name, (x, n) => x.Name = n);
            var unchanged = NavMeshQueryRulesEditor.UnchangedRules;
            // Check if is unchanged
            if(!force && rules.Count == unchanged.Count) {
                bool isEqual = true;
                for(int i = 0; i < rules.Count; i++) {
                    isEqual &= rules[i].IsEqual(unchanged[i]);
                }
                if(isEqual)
                    return;
            }

            // Setup File
            var file = new CodeFile("NavMeshQueryFilterRules");
            file.SetCreatorTag(typeof(NavMeshCodeGenerator));
            file.UseCleanProcess = true;
            file.AddUsing(typeof(NavMesh));
            file.AddUsing(typeof(Mathf));
            file.AddUsing(typeof(IReadOnlyList<string>));
            var ns = file.AddNamespace("Toolkit.AI.Navigation");

            // Add Enum
            var enu = ns.AddEnum(new CodeEnum(AccessModifier.Public, "NavMeshQueryFilterType", rules.Select(x => x.Name).ToArray()));

            // Add Class
            var c = ns.AddClass(new CodeClass("NavMeshQueryFilterRules", AccessModifier.PublicStatic));

            // Add Variables
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "RULES", rules.Count.ToString()));

            List<string> cons = new List<string>();
            foreach(var r in rules) {
                var id = r.AgentTypeId < generationRules.Count ? r.AgentTypeId : 0;
                var f = c.AddVariable(new CodeVariable(AccessModifier.PublicStaticReadonly, typeof(NavMeshQueryFilter), r.Name, $"new NavMeshQueryFilter(){{ agentTypeID = {id}, areaMask = {r.AreaMask}}}"));
                var mp = c.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "float[]", $"{GenUtility.VerifyName(r.Name)}_Multiplier", $"new float[]{{{string.Join(",", r.AreaCostMultiplier)}}}"));

                cons.Add("for(int i = 0; i < 32; i++) {");
                cons.Add($"{GenUtility.INDENT}if({mp.Name}[i] != 1f)");
                cons.Add($"{GenUtility.INDENT}{GenUtility.INDENT}{GenUtility.VerifyName(f.Name)}.SetAreaCost(i, {mp.Name}[i]);");
                cons.Add("}");
            }

            // Add Properties
            c.AddProperty(new CodeProperty(AccessModifier.PublicStatic, "IReadOnlyList<NavMeshQueryFilter>", "AllQueryFilters", new CodeBlock($"return new NavMeshQueryFilter[]{{{string.Join(",", rules.Select(x => GenUtility.VerifyName(x.Name)))}}};")));

            // Add Constructor
            c.AddConstructor(AccessModifier.Static, new CodeBlock(cons));

            // Add Getters
            c.AddMethod(new CodeMethod(AccessModifier.PublicStatic, typeof(NavMeshQueryFilter), "GetQueryFilter", new CodeVariable(enu.Name, "type"), new CodeBlock("return GetQueryFilter((int)type);")));
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                typeof(NavMeshQueryFilter),
                "GetQueryFilter",
                new CodeVariable("int", "index"),
                CodeBlock.CreateReturnSwitchBlock("index", ToIndex(rules).Select(x => x.ToString()).ToArray(), rules.Select(x => GenUtility.VerifyName(x.Name)).ToArray(), "return Default;")));

            c.AddMethod(new CodeMethod(AccessModifier.PublicStatic, "string", "GetQueryFilterName", new CodeVariable(enu.Name, "type"), new CodeBlock("return GetQueryFilterName((int)type);")));
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetQueryFilterName",
                new CodeVariable("int", "index"),
                CodeBlock.CreateReturnSwitchBlock("index", ToIndex(rules).Select(x => x.ToString()).ToArray(), rules.Select(x => $"\"{x.Name}\"").ToArray(), "return \"Default\";")));


            var path = file.CreateFile("Assets/Toolkit/AI/Navigation");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public static void GenerateBuildRules(bool force = false) {
            var rules = NavMeshGeneratorSettingsEditor.Rules;
            FixUniqueNames(rules, x => x.Name, (x, n) => x.Name = n);
            var unchanged = NavMeshGeneratorSettingsEditor.UnchangedRules;

            // Check if is unchanged
            if(!force && rules.Count == unchanged.Count) {
                bool isEqual = true;
                for(int i = 0; i < rules.Count; i++) {
                    isEqual &= rules[i].IsEqual(unchanged[i]);
                }
                isEqual &=
                    (NavMeshBuildRules.INSTANCED == NavMeshGeneratorSettingsEditor.Instanced &&
                    NavMeshBuildRules.TILE_SIZE == NavMeshGeneratorSettingsEditor.TileSize &&
                    NavMeshBuildRules.VOXEL_SIZE_PER_AGENT_RADIUS == NavMeshGeneratorSettingsEditor.VoxelsPerRadius);
                if(isEqual)
                    return;
            }

            // Setup File
            var file = new CodeFile("NavMeshBuildRules");
            file.AddUsing(typeof(NavMesh));
            file.AddUsing(typeof(IReadOnlyList<string>));
            var ns = file.AddNamespace("Toolkit.AI.Navigation");
            // Add Enum
            var en = ns.AddEnum(new CodeEnum("NavMeshBuildRuleType", rules.Select(x => x.Name).ToArray()));

            // Add class
            var c = ns.AddClass(new CodeClass("NavMeshBuildRules", AccessModifier.PublicStatic));

            // Add consts
            // c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "bool", "REMOVE_UNITY_AGENTS", "true"));
            c.AddVariable(new CodeVariable(AccessModifier.PublicStatic, "bool", "INSTANCED", NavMeshGeneratorSettingsEditor.Instanced.ToString().ToLower()));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "TILE_SIZE", NavMeshGeneratorSettingsEditor.TileSize.ToString()));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "float", "VOXEL_SIZE_PER_AGENT_RADIUS", $"{NavMeshGeneratorSettingsEditor.VoxelsPerRadius:0.00}f"));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "RULES", rules.Count.ToString()));

            // Add Build Settings variables
            foreach(var rule in rules) {
                c.AddVariable(new CodeVariable(AccessModifier.PublicStatic, typeof(NavMeshBuildSettings), rule.Name));
            }

            // Add static constructor
            List<string> constructor = new List<string>();
            constructor.Add("for(int i = NavMesh.GetSettingsCount() - 1; i >= 1; i--) {");
            constructor.Add($"{GenUtility.INDENT}var setting = NavMesh.GetSettingsByIndex(i);");
            constructor.Add($"{GenUtility.INDENT}NavMesh.RemoveSettings(setting.agentTypeID);");
            constructor.Add("}");
            constructor.Add("");
            constructor.Add("Default = NavMesh.GetSettingsByIndex(0);");
            constructor.Add("Default.overrideTileSize = true;");
            constructor.Add("Default.overrideVoxelSize = true;");
            constructor.Add("Default.tileSize = TILE_SIZE;");
            constructor.Add("Default.voxelSize = Default.agentRadius / VOXEL_SIZE_PER_AGENT_RADIUS;");
            int index = 0;
            for(int i = 1; i < rules.Count; i++) {
                var rule = rules[i];
                var name = GenUtility.VerifyName(rule.Name);
                index++;
                constructor.Add($"");
                constructor.Add($"{name} = NavMesh.CreateSettings();");
                constructor.Add($"{name}.agentTypeID = {index};");
                constructor.Add($"{name}.agentRadius = {rule.Radius:0.00}f;");
                constructor.Add($"{name}.agentHeight = {rule.Height:0.00}f;");
                constructor.Add($"{name}.agentClimb = {rule.StepHeight:0.00}f;");
                constructor.Add($"{name}.agentSlope = {rule.MaxSlope:0.00}f;");

                constructor.Add($"{name}.minRegionArea = {rule.Radius * 2f:0.00}f;");
                constructor.Add($"{name}.overrideTileSize = true;");
                constructor.Add($"{name}.overrideVoxelSize = true;");
                constructor.Add($"{name}.tileSize = TILE_SIZE;");
                constructor.Add($"{name}.voxelSize = {rule.Radius:0.00}f / VOXEL_SIZE_PER_AGENT_RADIUS;");
            }
            c.AddConstructor(new CodeMethod(
                AccessModifier.Static,
                "",
                c.Name,
                new CodeBlock(constructor)));

            // Add Getters
            c.AddProperty(new CodeProperty(
                AccessModifier.PublicStatic,
                "IReadOnlyList<NavMeshBuildSettings>",
                "AllRules",
                new CodeBlock($"return new NavMeshBuildSettings[]{{{string.Join(",", rules.Select(x => GenUtility.VerifyName(x.Name)))}}};")));

            c.AddMethod(new CodeMethod(AccessModifier.PublicStatic, typeof(NavMeshBuildSettings), "GetSetting", new CodeVariable(en.Name, "type"), new CodeBlock("return GetSetting((int)type);")));
            var indexArray = ToIndex(rules).Select(x => x.ToString()).ToArray();
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                typeof(NavMeshBuildSettings),
                "GetSetting",
                new CodeVariable("int", "index"),
                CodeBlock.CreateReturnSwitchBlock(
                    "index",
                    indexArray,
                    rules.Select(x => GenUtility.VerifyName(x.Name)).ToArray(),
                    "return Default;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetSettingName",
                new CodeVariable("int", "index"),
                CodeBlock.CreateReturnSwitchBlock(
                    "index",
                    indexArray,
                    rules.Select(x => $"\"{x.Name}\"").ToArray(),
                    "return \"Default\";")));

            file.SetCreatorTag(typeof(NavMeshCodeGenerator));
            file.UseCleanProcess = true;

            var path = file.CreateFile("Assets/Toolkit/AI/Navigation");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public static void GenerateBuildRuleMask(bool force = false) {
            var rules = NavMeshGeneratorSettingsEditor.Rules;
            FixUniqueNames(rules, x => x.Name, (x, n) => x.Name = n);
            var unchanged = NavMeshGeneratorSettingsEditor.UnchangedRules;

            if(!force && rules.Count == unchanged.Count) {
                bool isEqual = true;
                for(int i = 0; i < rules.Count; i++) {
                    isEqual &= rules[i].IsEqual(unchanged[i]);
                }
                if(isEqual)
                    return;
            }

            // setup file
            var file = new CodeFile("NavMeshBuildRuleMask");
            file.SetCreatorTag(typeof(NavMeshCodeGenerator));
            file.UseCleanProcess = true;
            file.AddUsing(typeof(UnityEngine.ScriptableObject));
            var ns = file.AddNamespace("Toolkit.AI.Navigation");
            var c = ns.AddClass(new CodeClass("NavMeshBuildRuleMask", AccessModifier.Public, typeof(ScriptableObject)));

            // Add variables
            foreach(var r in rules) {
                var v = new CodeVariable(AccessModifier.Private, "bool", r.Name, "true");
                v.AddAttribute(CodeAttribute.SerializeField);
                c.AddVariable(v);
            }

            // Add method
            c.AddMethod(new CodeMethod(AccessModifier.Public,
                "bool",
                "IsActive",
                new CodeVariable(typeof(NavMeshBuildRuleType), "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    rules.Select(x => $"NavMeshBuildRuleType.{GenUtility.VerifyName(x.Name)}").ToArray(),
                    rules.Select(x => GenUtility.VerifyName(x.Name)).ToArray(),
                    "return false;")));

            var path = file.CreateFile("Assets/Toolkit/AI/Navigation");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void FixUniqueNames<T>(IReadOnlyList<T> array, System.Func<T, string> nameFunction, System.Action<T, string> setNameFunction) {
            List<string> names = new List<string>();
            for(int i = 0; i < array.Count; i++) {
                var n = nameFunction(array[i]);
                if(names.Contains(n)) {
                    int index = 0;
                    while(true) {
                        var temp = n + index;
                        index++;
                        if(!names.Contains(temp)) {
                            setNameFunction(array[i], temp);
                            names.Add(temp);
                            break;
                        }
                    }
                }
                else {
                    names.Add(n);
                }
            }
        }

        private static int[] ToIndex<T>(IReadOnlyList<T> array) {
            int[] indexArray = new int[array.Count];
            for(int i = 0; i < indexArray.Length; i++) {
                indexArray[i] = i;
            }
            return indexArray;
        }
    }
}
