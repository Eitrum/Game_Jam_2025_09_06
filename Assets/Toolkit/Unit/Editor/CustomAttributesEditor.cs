using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEngine;

/*
namespace Toolkit.Unit {
    public static class CustomAttributesEditor {

        public class EditorAttributes {
            public string Name = "";
            private string shortName = "";
            public string ShortName {
                get {
                    if(string.IsNullOrEmpty(shortName)) {
                        if(string.IsNullOrEmpty(Name))
                            return "";
                        return Name.Substring(0, Mathf.Min(3, Name.Length));
                    }
                    return shortName;
                }
                set {
                    if(!string.IsNullOrEmpty(Name)) {
                        var comparename = Name.Substring(0, Mathf.Min(3, Name.Length));
                        if(comparename == value) {
                            shortName = "";
                            return;
                        }
                    }
                    shortName = value;
                }
            }
            public float startValue = 10;
        }

        public enum AttributeValueType {
            Integer,
            Float,
            Stat
        }

        public enum AttributeRequirementValueType {
            Integer,
            Float,
        }

        [InitializeOnLoadMethod]
        private static void Load() {
            Load(attributeEditorList);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
        }

        private static void DrawHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect nameArea, out Rect shortNameArea, out Rect defaultValueArea, 0.4f, 0.3f, 2f);
            EditorGUI.LabelField(nameArea, "Name");
            EditorGUI.LabelField(shortNameArea, "Short");
            EditorGUI.LabelField(defaultValueArea, "Default");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect nameArea, out Rect shortNameArea, out Rect defaultValueArea, 0.4f, 0.3f, 2f);

            attributeEditorList[index].Name = EditorGUI.TextField(nameArea, attributeEditorList[index].Name);
            attributeEditorList[index].ShortName = EditorGUI.TextField(shortNameArea, attributeEditorList[index].ShortName);
            attributeEditorList[index].startValue = EditorGUI.FloatField(defaultValueArea, attributeEditorList[index].startValue);
        }

        private static AttributeValueType selectedValueType = AttributeValueType.Integer;
        private static AttributeRequirementValueType selectedRequirementValueType = AttributeRequirementValueType.Integer;
        private static List<EditorAttributes> attributeEditorList = new List<EditorAttributes>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(attributeEditorList, typeof(EditorAttributes));

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Attributes", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            selectedValueType = (AttributeValueType)EditorGUILayout.EnumPopup("Attribute Value Type", selectedValueType);
            selectedRequirementValueType = (AttributeRequirementValueType)EditorGUILayout.EnumPopup("Attribute Requirement Value Type", selectedRequirementValueType);

            GUILayout.Space(12f);
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                GenerateAttributesClass(attributeEditorList, selectedValueType, selectedRequirementValueType);
            }
        }

        private static void Load(List<EditorAttributes> attributeList) {
            attributeList.Clear();

            var customType = typeof(AttributeType);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            var attributeClass = new Attributes();
            for(int i = 1, length = names.Length; i < length; i++) {
                var type = (AttributeType)i;
                attributeList.Add(new EditorAttributes() {
                    Name = Attributes.GetFullName(type),
                    ShortName = Attributes.GetShortName(type),
                    startValue = attributeClass.GetStat(type)
                });
            }

            var field = typeof(Attributes).GetFields().FirstOrDefault(x => !x.IsStatic);
            if(field != null) {
                var type = field.FieldType;
                if(type == typeof(int)) {
                    selectedValueType = AttributeValueType.Integer;
                }
                if(type == typeof(float)) {
                    selectedValueType = AttributeValueType.Float;
                }
                if(type == typeof(Toolkit.Stat)) {
                    selectedValueType = AttributeValueType.Stat;
                }
            }
            var fieldReq = typeof(AttributeRequirements).GetFields().FirstOrDefault();
            if(fieldReq != null) {
                var type = fieldReq.FieldType;
                if(type == typeof(int)) {
                    selectedRequirementValueType = AttributeRequirementValueType.Integer;
                }
                if(type == typeof(float)) {
                    selectedRequirementValueType = AttributeRequirementValueType.Float;
                }
            }
        }

        public static void GenerateAttributesClass(
            IReadOnlyList<EditorAttributes> attributes,
            AttributeValueType attributeValueType,
            AttributeRequirementValueType attributeRequirementValueType
            ) {
            var attributeType = attributeValueType == AttributeValueType.Float ? "float" : (attributeValueType == AttributeValueType.Integer ? "int" : typeof(Toolkit.Stat).FullName);
            var valueType = attributeRequirementValueType == AttributeRequirementValueType.Float ? "float" : "int";
            // Setup File
            var file = new CodeFile("Attributes");
            file.AddUsing("UnityEngine");
            file.AddUsing("System");
            var ns = file.AddNamespace("Toolkit.Unit");

            // Add enum
            List<string> attributeTypes = attributes.Select(x => x.Name).ToList();
            attributeTypes.Insert(0, "None");
            ns.AddEnum(new CodeEnum("AttributeType", attributeTypes));

            // Add interface
            var intFace = ns.Add(new CodeInterface("IAttributes"));
            intFace.AddCustom(new CodeCustom($"{attributeType} this[AttributeType type] {{ get; set; }}"));
            intFace.AddCustom(new CodeCustom($"{attributeType} this[int index] {{ get; set; }}"));
            intFace.AddMethod(new CodeMethod(attributeType, "GetStat", new CodeVariable("AttributeType", "type"), null));
            intFace.AddMethod(new CodeMethod("void", "SetStat", new CodeVariable[] { new CodeVariable("AttributeType", "type"), new CodeVariable(attributeType, "value") }, null));

            // Add req interface
            var intReqFace = ns.Add(new CodeInterface("IAttributeRequirements"));
            intReqFace.AddCustom(new CodeCustom($"{valueType} this[AttributeType type] {{ get; set; }}"));
            intReqFace.AddCustom(new CodeCustom($"{valueType} this[int index] {{ get; set; }}"));
            intReqFace.AddMethod(new CodeMethod(valueType, "GetStat", new CodeVariable("AttributeType", "type"), null));
            intReqFace.AddMethod(new CodeMethod("void", "SetStat", new CodeVariable[] { new CodeVariable("AttributeType", "type"), new CodeVariable(valueType, "value") }, null));


            // Add req class
            var cr = ns.AddClass(new CodeClass("AttributeRequirements", AccessModifier.Public, intReqFace.Name));
            cr.AddAttribute(typeof(System.SerializableAttribute));

            // add req methods
            foreach(var att in attributes) {
                cr.AddVariable(new CodeVariable(
                    AccessModifier.Public,
                    valueType,
                    att.Name,
                    "0"));
            }

            cr.AddConstructor();
            cr.AddConstructor(CodeMethod.CreateAssignConstructor(
                cr,
                attributes.Select(x => new CodeVariable(valueType, "_" + x.Name.ToLower(0))).ToArray(),
                attributes.Select(x => x.Name).ToArray()));

            cr.AddCustom(new CodeCustom($"public {valueType} this[AttributeType type] {{ get => GetStat(type); set => SetStat(type, value); }}"));
            cr.AddCustom(new CodeCustom($"public {valueType} this[int index] {{ get => GetStat((AttributeType)(index-1)); set => SetStat((AttributeType)(index-1), value); }}"));

            // Add req Get Stat
            // public Stat GetStat(AttributeType type) {
            cr.AddMethod(new CodeMethod(
                AccessModifier.Public,
                valueType,
                "GetStat",
                new CodeVariable("AttributeType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    attributes.Select(x => "AttributeType." + GenUtility.VerifyName(x.Name)).ToArray(),
                    attributes.Select(x => GenUtility.VerifyName(x.Name)).ToArray(),
                    "throw new System.Exception(\"Attribute Type not supported\");")));

            // Add reqSet Stat
            // public void SetStat(AttributeType type, Stat value) {
            cr.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "void",
                "SetStat",
               new CodeVariable[]{
                    new CodeVariable("AttributeType", "type"),
                    new CodeVariable(valueType, "value"),
               },
                CodeBlock.CreateAssignSwitchBlock(
                    "type",
                    "value",
                    attributes.Select(x => "AttributeType." + GenUtility.VerifyName(x.Name)).ToArray(),
                    attributes.Select(x => GenUtility.VerifyName(x.Name)).ToArray()
                    )));


            // Add class
            var c = ns.AddClass(new CodeClass("Attributes", AccessModifier.Public, intFace.Name));
            c.AddAttribute(typeof(System.SerializableAttribute));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "ATTRIBUTES", attributes.Count.ToString()));

            // Add Methods
            foreach(var att in attributes) {
                c.AddVariable(new CodeVariable(
                    AccessModifier.Public,
                    attributeType,
                    att.Name,
                    attributeValueType == AttributeValueType.Stat ?
                        $"new Stat({att.startValue}f, 1f, 1f)" :
                        (attributeValueType == AttributeValueType.Float ? $"{att.startValue}f" : $"{Mathf.RoundToInt(att.startValue)}")));
            }

            c.AddCustom(new CodeCustom($"public {attributeType} this[AttributeType type] {{ get => GetStat(type); set => SetStat(type, value); }}"));
            c.AddCustom(new CodeCustom($"public {attributeType} this[int index] {{ get => GetStat((AttributeType)(index-1)); set => SetStat((AttributeType)(index-1), value); }}"));

            // Add Get Stat
            // public Stat GetStat(AttributeType type) {
            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                attributeType,
                "GetStat",
                new CodeVariable("AttributeType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    attributes.Select(x => "AttributeType." + GenUtility.VerifyName(x.Name)).ToArray(),
                    attributes.Select(x => GenUtility.VerifyName(x.Name)).ToArray(),
                    "throw new System.Exception(\"Attribute Type not supported\");")));

            // Add Set Stat
            // public void SetStat(AttributeType type, Stat value) {
            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "void",
                "SetStat",
               new CodeVariable[]{
                    new CodeVariable("AttributeType", "type"),
                    new CodeVariable(attributeType, "value"),
               },
                CodeBlock.CreateAssignSwitchBlock(
                    "type",
                    "value",
                    attributes.Select(x => "AttributeType." + GenUtility.VerifyName(x.Name)).ToArray(),
                    attributes.Select(x => GenUtility.VerifyName(x.Name)).ToArray()
                    )));

            // Add Has Requirement Check
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "bool",
                "HasRequirements",
                new CodeVariable[]{
                    new CodeVariable(intFace.Name, "attributes"),
                    new CodeVariable(intFace.Name, "requirements"),
                },
                new CodeBlock(new string[]{
                    $"for(int i = 0; i < ATTRIBUTES; i++) ",
                    $"{GenUtility.INDENT}if(attributes.GetStat((AttributeType)i + 1) < requirements.GetStat((AttributeType)i + 1))",
                    $"{GenUtility.INDENT}{GenUtility.INDENT}return false;",
                    $"return true;"
                })));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "bool",
                "HasRequirements",
                new CodeVariable[]{
                    new CodeVariable(intFace.Name, "attributes"),
                    new CodeVariable(intReqFace.Name, "requirements"),
                },
                new CodeBlock(new string[]{
                    $"for(int i = 0; i < ATTRIBUTES; i++) ",
                    $"{GenUtility.INDENT}if(attributes.GetStat((AttributeType)i + 1) < requirements.GetStat((AttributeType)i + 1))",
                    $"{GenUtility.INDENT}{GenUtility.INDENT}return false;",
                    $"return true;"
                })));


            // Add Get Full Name
            // public static string GetFullName(AttributeType type) {
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetFullName",
                new CodeVariable("AttributeType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    attributes.Select(x => "AttributeType." + GenUtility.VerifyName(x.Name)).ToArray(),
                    attributes.Select(x => $"\"{x.Name}\"").ToArray(),
                    "return \"\";")));

            // Add Get Short Name
            // public static string GetShortName(AttributeType type) {
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetShortName",
                new CodeVariable("AttributeType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    attributes.Select(x => "AttributeType." + GenUtility.VerifyName(x.Name)).ToArray(),
                    attributes.Select(x => $"\"{x.ShortName}\"").ToArray(),
                    "return \"\";")));

            // Create Attribute Behaviour
            // Effective cloning non-static elements
            var behaviourFile = new CodeFile("AttributeBehaviour");
            behaviourFile.AddUsing("UnityEngine");
            var bns = behaviourFile.AddNamespace("Toolkit.Unit");
            var bc = bns.AddClass(new CodeClass("AttributeBehaviour", AccessModifier.Public, new string[] { "MonoBehaviour", "IAttributes" }));
            bc.AddAttribute("AddComponentMenu", "\"Toolkit/Unit/Attributes\"");
            var nodes = c.CodeNodes;
            for(int i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                if(node is CodeVariable codeVar) {
                    if(!codeVar.AccessModifier.IsStatic())
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
*/
