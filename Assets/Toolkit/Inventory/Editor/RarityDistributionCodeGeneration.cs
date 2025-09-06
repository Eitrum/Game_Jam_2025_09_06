using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Toolkit;
using Toolkit.CodeGenerator;

namespace Toolkit.Inventory
{
    public static class RarityDistributionCodeGeneration
    {
        public static void Generate(IReadOnlyList<string> rarities) {
            var file = new CodeFile("RarityDistribution");
            file.SetCreatorTag(typeof(RarityDistributionCodeGeneration));
            file.UseCleanProcess = false;
            file.AddUsing("System");
            file.AddUsing("UnityEngine");

            var ns = file.AddNamespace("Toolkit.Inventory");
            var c = ns.AddClass(new CodeClass(AccessModifier.Public, "RarityDistribution", "ScriptableObject"));
            c.AddAttribute(typeof(CreateAssetMenuAttribute), "fileName = \"Rarity Distribution\", menuName = \"Toolkit/Inventory/Rarity Distribution\"");
            var sc = c.AddClass(AccessModifier.Public, "Weight");
            sc.AddAttribute(typeof(System.SerializableAttribute));

            var array = c.AddVariable(new CodeVariable(AccessModifier.Private, "Weight[]", "weight"));
            array.AddAttribute(typeof(SerializeField));
            var levelRanged = c.AddVariable(new CodeVariable(AccessModifier.Private, "bool", "levelBased", "true"));
            levelRanged.AddAttribute(typeof(SerializeField));

            foreach(var r in rarities) {
                var v = sc.AddVariable(new CodeVariable(AccessModifier.Private, "float", r.ToLower(), "0"));
                v.AddAttribute(typeof(SerializeField));
            }

            var total = sc.AddVariable(new CodeVariable(AccessModifier.Private, "float", "total", "0"));
            total.AddAttribute(typeof(SerializeField));

            foreach(var r in rarities) {
                sc.AddProperty(new CodeProperty(AccessModifier.Public, "float", $"{GenUtility.VerifyName(r.ToUpper(0))}Weight", $"return {GenUtility.VerifyName(r.ToLower())};"));
                sc.AddProperty(new CodeProperty(AccessModifier.Public, "float", $"{GenUtility.VerifyName(r.ToUpper(0))}Percentage", $"return {GenUtility.VerifyName(r.ToLower())} / total;"));
            }

            c.AddMethod(new CodeMethod(AccessModifier.Public, "Weight", "GetWeightByIndex", new CodeVariable("int", "index"), new CodeBlock("return weight[index];")));
            c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    "Weight",
                    "GetWeightByLevel",
                    new CodeVariable("int", "level"),
                    new CodeBlock("return weight[Mathf.Clamp(level, Toolkit.Unit.ExperienceUtility.MIN_LEVEL, Toolkit.Unit.ExperienceUtility.MAX_LEVEL) - Toolkit.Unit.ExperienceUtility.MIN_LEVEL];")));

            var lines = new List<string>();
            lines.Add("var value = UnityEngine.Random.value * total;");
            foreach(var r in rarities) {
                lines.Add($"if (value <= {GenUtility.VerifyName(r.ToLower())})");
                lines.Add($"{GenUtility.INDENT}return Rarity.{GenUtility.VerifyName(r)};");
            }
            lines.Add($"return Rarity.None;");

            sc.AddMethod(new CodeMethod(AccessModifier.Public, "Rarity", "GetRarity", new CodeBlock(lines)));
            lines[0] = "var value = (float)(random.NextDouble()) * total;";
            sc.AddMethod(new CodeMethod(AccessModifier.Public, "Rarity", "GetRarity", new CodeVariable(typeof(System.Random), "random"), new CodeBlock(lines)));
            c.AddProperty(new CodeProperty(AccessModifier.Public, "int", "WeightCount", new CodeBlock("return weight.Length;")));
            c.AddProperty(new CodeProperty(AccessModifier.Public, "bool", "IsLevelBased", new CodeBlock("return levelBased;")));

            var path = file.CreateFile("Assets/Toolkit/Inventory");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
