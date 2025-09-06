using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit
{
    public static class CustomTraitEditor
    {

        public class EditorTrait
        {
            public string Positive = "";
            public string Negative = "";
            public float defaultValue = 0f;

            public string PrivateVariableName => $"_{FullName.ToLower(0)}";

            public string FullName {
                get {
                    return GenUtility.VerifyName(Positive + Negative);
                }
            }
        }

        public enum TraitValueType
        {
            Integer,
            Float,
        }

        [InitializeOnLoadMethod]
        private static void Load() {
            Load(editorTraitValues);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
        }

        private static void DrawHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect indexArea, out Rect textFields, out Rect other, 30f / rect.width);
            textFields.SplitHorizontal(out Rect positive, out Rect negative, 0.5f);
            other.SplitHorizontal(out Rect defaultValue, out Rect output, 0.3f);


            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(positive, "Positive");
            EditorGUI.LabelField(negative, "Negative");
            EditorGUI.LabelField(defaultValue, "Default");
            EditorGUI.LabelField(output, "EnumValue");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect textFields, out Rect other, 30f / rect.width);
            textFields.SplitHorizontal(out Rect positive, out Rect negative, 0.5f);
            other.SplitHorizontal(out Rect defaultValue, out Rect output, 0.3f);

            EditorGUI.LabelField(indexArea, index.ToString());
            editorTraitValues[index].Positive = EditorGUI.TextField(positive, editorTraitValues[index].Positive);
            editorTraitValues[index].Negative = EditorGUI.TextField(negative, editorTraitValues[index].Negative);

            editorTraitValues[index].defaultValue = EditorGUI.FloatField(defaultValue, editorTraitValues[index].defaultValue);
            EditorGUI.LabelField(output, editorTraitValues[index].FullName);
        }

        private static float minimumValue = -10;
        private static float maximumValue = 10;
        private static TraitValueType traitValueType = TraitValueType.Integer;
        private static List<EditorTrait> editorTraitValues = new List<EditorTrait>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(editorTraitValues, typeof(EditorTrait));

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Trait", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            traitValueType = (TraitValueType)EditorGUILayout.EnumPopup("Trait Value Type", traitValueType);
            if(traitValueType == TraitValueType.Float) {
                minimumValue = EditorGUILayout.FloatField("Most Negative", minimumValue);
                maximumValue = EditorGUILayout.FloatField("Most Positive", maximumValue);
            }
            else {
                minimumValue = EditorGUILayout.IntField("Most Negative", Mathf.RoundToInt(minimumValue));
                maximumValue = EditorGUILayout.IntField("Most Positive", Mathf.RoundToInt(maximumValue));
            }
            GUILayout.Space(12f);
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                GenerateTraits(traitValueType, new MinMax(minimumValue, maximumValue), editorTraitValues);
            }
        }

        private static void Load(List<EditorTrait> traitTypes) {
            traitTypes.Clear();

            var customType = typeof(TraitType);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            var defaultTraitBlock = new Traits();
            for(int i = 1, length = names.Length; i < length; i++) {
                var trait = (TraitType)i;
                var positive = TraitsUtility.GetPositiveName(trait);
                var negative = TraitsUtility.GetNegativeName(trait);
                traitTypes.Add(new EditorTrait() {
                    Positive = positive,
                    Negative = negative,
                    defaultValue = defaultTraitBlock.GetTrait(trait)
                });
            }
        }

        public static void GenerateTraits(TraitValueType valueType, MinMax range, IReadOnlyList<EditorTrait> editorTraits) {
            var valueTypeString = valueType == TraitValueType.Float ? "float" : "int";

            // Setup namespace
            var file = new CodeFile("Traits");
            file.AddUsing("UnityEngine");
            file.UseCleanProcess = true;
            file.SetCreatorTag(typeof(CustomTraitEditor));
            var ns = file.AddNamespace("Toolkit.Unit");

            // Add Trait Type
            var enumArray = editorTraits.Select(x => new CodeVariable("int", x.FullName)).ToList();
            enumArray.Foreach((x, i) => {
                x.AddAttribute(typeof(InspectorNameAttribute), $"\"{editorTraits[i].Negative} - {editorTraits[i].Positive}\"");
            });
            enumArray.Insert(0, new CodeVariable("", "None"));
            ns.AddEnum(new CodeEnum("TraitType", enumArray));

            // Add Callback
            ns.Add(new CodeCustom($"public delegate void OnTraitChangedCallback(TraitType type, {valueTypeString} change);"));

            // Add Interface
            var intface = ns.Add(new CodeInterface("ITrait", AccessModifier.Public));
            intface.AddCustom(new CodeCustom($"{valueTypeString} this[TraitType type] {{ get; set; }}"));
            intface.AddMethod(new CodeMethod(valueTypeString, "GetTrait", new CodeVariable[] { new CodeVariable("TraitType", "type") }, null));
            intface.AddMethod(new CodeMethod("void", "SetTrait", new CodeVariable[] { new CodeVariable("TraitType", "type"), new CodeVariable(valueTypeString, "value") }, null));
            intface.AddCustom(new CodeCustom("event OnTraitChangedCallback OnTraitChanged;"));

            // Add Singular Trait class
            var sc = ns.AddClass(AccessModifier.Public, "Trait");
            sc.AddVariable(new CodeVariable(AccessModifier.PublicConst, valueTypeString, "MIN", valueType == TraitValueType.Float ? $"{range.min}f" : Mathf.RoundToInt(range.min).ToString()));
            sc.AddVariable(new CodeVariable(AccessModifier.PublicConst, valueTypeString, "MAX", valueType == TraitValueType.Float ? $"{range.max}f" : Mathf.RoundToInt(range.max).ToString()));
            sc.AddAttribute(typeof(System.SerializableAttribute));
            var sctt = sc.AddVariable(new CodeVariable(AccessModifier.Private, "TraitType", "type", "TraitType.None"));
            sctt.AddAttribute(typeof(SerializeField));
            var scv = sc.AddVariable(new CodeVariable(AccessModifier.Private, valueTypeString, "value", "0"));
            scv.AddAttribute(
                new CodeAttribute(
                    new CodeAttribute.Attribute[] {
                        new CodeAttribute.Attribute(typeof(SerializeField)),
                        new CodeAttribute.Attribute(typeof(RangeAttribute), "MIN", "MAX") }));
            sc.AddProperty(new CodeProperty(AccessModifier.Public, "TraitType", "Type", new CodeBlock("return type;"), new CodeBlock("type = value;")));
            sc.AddProperty(new CodeProperty(
                    AccessModifier.Public,
                    valueTypeString,
                    "Value",
                    $"return this.value;",
                    $"this.value = Mathf.Clamp(value, MIN, MAX);"));
            sc.AddConstructor();
            sc.AddConstructor(
                new CodeVariable[] {
                    new CodeVariable("TraitType", "type"),
                    new CodeVariable(valueTypeString, "value")
                },
                new CodeBlock(new string[] {
                    "this.type = type;",
                    "this.value = Mathf.Clamp(value, MIN, MAX);" }));

            // Add Class
            var c = ns.AddClass(new CodeClass("Traits", AccessModifier.Public, intface.Name));
            c.AddAttribute(typeof(System.SerializableAttribute));
            // Add Consts
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, valueTypeString, "MIN", valueType == TraitValueType.Float ? $"{range.min}f" : Mathf.RoundToInt(range.min).ToString()));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, valueTypeString, "MAX", valueType == TraitValueType.Float ? $"{range.max}f" : Mathf.RoundToInt(range.max).ToString()));

            // Add variables
            foreach(var trait in editorTraits) {
                c.AddCustom(new CodeCustom($"[SerializeField, Range(MIN, MAX)] private {valueTypeString} {trait.PrivateVariableName} = {(valueType == TraitValueType.Float ? $"{trait.defaultValue}f" : Mathf.RoundToInt(trait.defaultValue).ToString())};"));
            }
            c.AddVariable(new CodeVariable(AccessModifier.Public | AccessModifier.Event, "OnTraitChangedCallback", "OnTraitChanged"));

            // Add Accessors
            // public float LawfulChaotic { get => lawfulChaotic; set => Mathf.Clamp(value, MIN, MAX); }
            foreach(var trait in editorTraits) {
                c.AddProperty(new CodeProperty(
                    AccessModifier.Public,
                    valueTypeString,
                    trait.FullName,
                    new CodeBlock($"return {trait.PrivateVariableName};"),
                    new CodeBlock(new string[]{
                        $"var old = {trait.PrivateVariableName};",
                        $"{trait.PrivateVariableName} = Mathf.Clamp(value, MIN, MAX);",
                        $"var diff = {trait.PrivateVariableName} - old;",
                        "if (diff != 0)",
                        $"  OnTraitChanged?.Invoke(TraitType.{GenUtility.VerifyName(trait.FullName)}, diff);"
                    })));
            }

            // Add this accessor
            c.AddCustom(new CodeCustom($"public {valueTypeString} this[TraitType type] {{get => GetTrait(type); set => SetTrait(type, value);}}"));

            // Add Get Trait
            // public float GetTrait(TraitType type) {

            List<string> getTraitString = new List<string>();
            getTraitString.Add("switch(type){");
            foreach(var trait in editorTraits) {
                getTraitString.Add($"{GenUtility.INDENT}case TraitType.{trait.FullName}: return {trait.FullName};");
            }
            getTraitString.Add("}");
            getTraitString.Add("throw new System.Exception(\"Not a valid TraitType\");");

            var getMethod = c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                valueTypeString,
                "GetTrait",
                new CodeVariable("TraitType", "type"),
                new CodeBlock(getTraitString)));

            // Add Set Trait
            //  public void SetTrait(TraitType type, float value) {

            List<string> setTraitString = new List<string>();
            setTraitString.Add("switch(type){");
            foreach(var trait in editorTraits) {
                setTraitString.Add($"{GenUtility.INDENT}case TraitType.{trait.FullName}:");
                setTraitString.Add($"{GenUtility.INDENT}{GenUtility.INDENT}{trait.FullName} = value;");
                setTraitString.Add($"{GenUtility.INDENT}{GenUtility.INDENT}break;");
            }
            setTraitString.Add("}");

            var setMethod = c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "void",
                "SetTrait",
                new CodeVariable[] {
                    new CodeVariable("TraitType", "type"),
                    new CodeVariable(valueTypeString, "value"),
                },
                new CodeBlock(setTraitString)));

            // Add Positive name
            //  public static string GetPositiveName(TraitType type)
            List<string> getTraitNamePositiveString = new List<string>();
            getTraitNamePositiveString.Add("switch(type){");
            foreach(var trait in editorTraits) {
                getTraitNamePositiveString.Add($"{GenUtility.INDENT}case TraitType.{trait.FullName}: return \"{trait.Positive}\";");
            }
            getTraitNamePositiveString.Add("}");
            getTraitNamePositiveString.Add("throw new System.Exception(\"Not a valid TraitType\");");

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetPositiveName",
                new CodeVariable("TraitType", "type"),
                new CodeBlock(getTraitNamePositiveString)));

            // Add Negative name
            //  public static string GetPositiveName(TraitType type)
            List<string> getTraitNameNegativeString = new List<string>();
            getTraitNameNegativeString.Add("switch(type){");
            foreach(var trait in editorTraits) {
                getTraitNameNegativeString.Add($"{GenUtility.INDENT}case TraitType.{trait.FullName}: return \"{trait.Negative}\";");
            }
            getTraitNameNegativeString.Add("}");
            getTraitNameNegativeString.Add("throw new System.Exception(\"Not a valid TraitType\");");

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetNegativeName",
                new CodeVariable("TraitType", "type"),
                new CodeBlock(getTraitNameNegativeString)));

            // Generate behaviour file
            // Effective cloning non-static elements
            var behaviourFile = new CodeFile("TraitsBehaviour");
            behaviourFile.AddUsing("UnityEngine");
            var bns = behaviourFile.AddNamespace("Toolkit.Unit");
            var bc = bns.AddClass(new CodeClass("TraitsBehaviour", AccessModifier.Public, new string[] { "MonoBehaviour", "ITrait" }));
            bc.AddAttribute("AddComponentMenu", "\"Toolkit/Unit/Traits\"");
            var nodes = c.CodeNodes;
            for(int i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                if(node is CodeVariable codeVar) {
                    // need to add the constants
                    bc.AddVariable(codeVar);
                }
                else if(node is CodeMethod codeMet) {
                    if(!codeMet.AccessModifier.IsStatic()) {
                        bc.AddMethod(codeMet);
                    }
                }
                else if(node is CodeProperty codePro) {
                    if(!codePro.AccessModifier.IsStatic()) {
                        bc.AddProperty(codePro);
                    }
                }
                else {
                    bc.AddCustom(node);
                }
            }

            var path = file.CreateFile("Assets/Toolkit/Unit/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            var bFile = behaviourFile.CreateFile("Assets/Toolkit/Unit/");
            AssetDatabase.ImportAsset(bFile, ImportAssetOptions.ForceUpdate);
        }
    }
}
