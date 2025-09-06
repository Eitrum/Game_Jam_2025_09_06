using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Toolkit.CodeGenerator;

namespace Toolkit.Unit {
    public static class CustomIUnitEditor {

        [System.Flags]
        private enum BasicImplementationMode {
            None = 0,
            GetComponent = 1,
            SerializeField = 2,
            NonSerialized = 4
        }

        private class Types {
            public bool Enabled;
            public System.Type Type;
            public BasicImplementationMode BasicUnitImplementation;
            private string _name;
            public string Name {
                get {
                    if(string.IsNullOrEmpty(_name))
                        return Type.Name;
                    return _name;
                }
                set => _name = value;
            }

            public bool IsCustom = false;

            public string Namespace {
                get {
                    switch(Name) {
                        case "Name":
                        case "Id":
                        case "ControllerType":
                            return "Basic";
                    }
                    return Type.Namespace;
                }
            }

            public Types() { }
            public Types(System.Type type) {
                this.Type = type;
                var att = Type.GetCustomAttributes(typeof(IUnitSupportAttribute), true).FirstOrDefault();
                if(att != null && att is IUnitSupportAttribute unitAtt) {
                    Name = unitAtt.Name;
                    IsCustom = true;
                }
            }
            public Types(System.Type type, string name) {
                this.Type = type;
                this.Name = name;
            }

            public Types(System.Type type, string name, BasicImplementationMode basicUnitImpl) {
                this.Type = type;
                this.Name = name;
                this.BasicUnitImplementation = basicUnitImpl;
            }
        }

        private static List<Types> SupportedType = new List<Types>();

        [InitializeOnLoadMethod]
        static void Load() {
            SupportedType.Clear();

            // Find Custom Types
            var allScripts = AssetDatabaseUtility.LoadAssets<MonoScript>();
            var assemblies = allScripts
                .Select(x => x.GetClass()?.Assembly)
                .Where(x => x != null)
                .Unique();
            var iUnitSupportAttributeType = typeof(IUnitSupportAttribute);
            var types = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsDefined(iUnitSupportAttributeType, true));

            // Add Basic Types
            SupportedType.Add(new Types(typeof(string), "Name", BasicImplementationMode.SerializeField));
            SupportedType.Add(new Types(typeof(int), "Id", BasicImplementationMode.NonSerialized));
            SupportedType.Add(new Types(typeof(UnitControllerType), "ControllerType", BasicImplementationMode.SerializeField));
            SupportedType.Add(new Types(typeof(MonoBehaviour), "Behaviour", BasicImplementationMode.None));
            SupportedType.Add(new Types(typeof(Vector3), "Position", BasicImplementationMode.None));
            SupportedType.Add(new Types(typeof(Quaternion), "Rotation", BasicImplementationMode.None));
            SupportedType.Add(new Types(typeof(Toolkit.Health.IHealth), "Health", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Inventory.IInventory), "Inventory", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Inventory.IEquipment), "Equipment", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Unit.IExperience), "Experience", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Unit.IAttributes), "Attributes", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Unit.ITrait), "Traits", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Unit.IPerks), "Perks", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Unit.IStatusEffectList), "StatusEffects", BasicImplementationMode.GetComponent));
            SupportedType.Add(new Types(typeof(Toolkit.Unit.IStats), "Stats", BasicImplementationMode.SerializeField));

            // TODO: Add Custom Types
            // SupportedType.AddRange(types.Select(x => new Types(x)));

            // Setup Enabled Types
            var properties = typeof(IUnit).GetProperties();
            properties
                .Foreach(x => SupportedType
                    .Where(y => y.Name == x.Name && y.Type == x.GetMethod.ReturnType)
                    .Foreach(z => z.Enabled = true)
                );
        }


        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            RenderBasicTypes();
            GUILayout.Space(12f);

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                if(SupportedType.Any(x => x.Enabled && x.IsCustom) &&
                    EditorUtility.DisplayDialog("IUnit", "You are attempting to create a IUnit with custom data types, this requires the object to be located in main assembly to work. \nThe file will move to 'Assets/'", "Ok", "Cancel")) {
                    GenerateIUnitClass();
                }
                else {
                    GenerateIUnitClass();
                }
            }
        }

        private static void RenderBasicTypes() {
            EditorGUI.indentLevel++;
            var ns = "";
            int length = SupportedType.Count;
            for(int i = 0; i < length; i++) {
                if(SupportedType[i].Namespace != ns) {
                    if(!string.IsNullOrEmpty(ns))
                        EditorGUILayout.EndVertical();
                    ns = SupportedType[i].Namespace;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField(ns, EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                }
                EditorGUILayout.BeginHorizontal();
                SupportedType[i].Enabled = EditorGUILayout.Toggle(SupportedType[i].Name, SupportedType[i].Enabled);
                GUILayout.Label(SupportedType[i].Type.FullName, EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private static void GenerateIUnitClass() {
            bool hasCustomType = SupportedType.Any(x => x.Enabled && x.IsCustom);
            var uniqueNamespaces = SupportedType
                .Where(x => x.Enabled && x.Namespace != "Basic")
                .Select(x => x.Namespace)
                .Unique();
            // Setup File
            var file = new CodeFile(hasCustomType ? "Toolkit.IUnit" : "IUnit");
            var ns = file.AddNamespace("Toolkit.Unit");
            var c = ns.Add(new CodeInterface("IUnit", AccessModifier.Public));

            // Setup Basic Unit File
            var bFile = new CodeFile("BasicUnit");
            var bns = bFile.AddNamespace("Toolkit.Unit");
            bFile.AddUsing("UnityEngine");
            var bc = bns.AddClass(new CodeClass("BasicUnit", AccessModifier.Public, new string[] { "MonoBehaviour", "IUnit" }));
            bc.AddAttribute(typeof(AddComponentMenu), "\"Toolkit/Unit/Unit\"");

            // Add Namespaces
            uniqueNamespaces.Foreach(x => {
                file.AddUsing(x);
                bFile.AddUsing(x);
            });

            // Setup awake method for basic unit class
            List<string> awakeMethod = new List<string>();

            int length = SupportedType.Count;
            for(int i = 0; i < length; i++) {
                var t = SupportedType[i];
                if(!t.Enabled)
                    continue;
                // Add IUnit Interface property
                c.AddProperty(new CodeProperty(t.Type, t.Name, "..."));

                // Add Basic Unit variables
                bool hasImplementation = t.BasicUnitImplementation != BasicImplementationMode.None;
                if(hasImplementation) {
                    var variable = new CodeVariable(AccessModifier.Private, t.Type, t.Name.ToLower(0));

                    if(t.BasicUnitImplementation.HasFlag(BasicImplementationMode.SerializeField))
                        variable.AddAttribute(typeof(UnityEngine.SerializeField));

                    if(t.BasicUnitImplementation.HasFlag(BasicImplementationMode.GetComponent))
                        awakeMethod.Add($"{GenUtility.VerifyName(variable.Name)} = this.GetComponent<{t.Type.FullName}>();");

                    if(variable.Name != "name")// Ignore basic name
                        bc.AddVariable(variable);
                    if(variable.Name == "id") {
                        awakeMethod.Add("id = UnitIndexManager.Add(this);");
                        // Add destroy method
                        bc.AddMethod(new CodeMethod(AccessModifier.Private, "OnDestroy", new CodeBlock("UnitIndexManager.Remove(this, id);")));
                    }
                }

                var nonImplemented = NonImplementedText(t);

                bc.AddProperty(new CodeProperty(
                    AccessModifier.Public,
                    t.Type,
                    t.Name,
                    hasImplementation ? $"return {GenUtility.VerifyName(t.Name.ToLower(0))};" : nonImplemented));
            }

            // Add Awake to Basic Unit
            bc.AddMethod(new CodeMethod(AccessModifier.Private, "Awake", new CodeBlock(awakeMethod)));

            // Handle Custom Files
            if(SupportedType.Any(x => x.Enabled && x.IsCustom)) {
                AssetDatabase.DeleteAsset("Assets/Toolkit/Unit/IUnit.cs");
                AssetDatabase.DeleteAsset("Assets/Toolkit/Unit/BasicUnit.cs");

                var path = file.CreateFile("Assets/");
                var bpath = bFile.CreateFile("Assets/");
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset(bpath, ImportAssetOptions.ForceUpdate);
            }
            else {
                AssetDatabase.DeleteAsset("Assets/Toolkit.IUnit.cs");
                AssetDatabase.DeleteAsset("Assets/BasicUnit.cs");
                file.UseCleanProcess = true;
                var path = file.CreateFile("Assets/Toolkit/Unit/");
                bFile.UseCleanProcess = true;
                var bpath = bFile.CreateFile("Assets/Toolkit/Unit/");
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset(bpath, ImportAssetOptions.ForceUpdate);
            }
        }

        private static string NonImplementedText(Types t) {
            switch(t.Name) {
                case "Behaviour": return "return this;";
                case "Position": return "return transform.position;";
                case "Rotation": return "return transform.rotation;";
            }
            return $"throw new System.NotImplementedException(\"Property '{t.Name}' Is not implemented in BasicUnit\");";
        }
    }
}
