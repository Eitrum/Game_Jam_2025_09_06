using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit.CodeGenerator;
using System.Linq;
using UnityEditor;

namespace Toolkit
{
    public static class CustomizableSettingCodeGen
    {
        public static void Save<T, TSetting>(T obj) where T : CustomizableSettingInspector<T, TSetting> {
            var settingFileName = GenUtility.VerifyName(obj.ClassName.Split('.').Last());
            var settingNamespace = obj.ClassName.Remove(obj.ClassName.Length - settingFileName.Length - 1);
            var groups = obj.Groups;

            var file = new CodeFile(settingFileName);
            file.SetCreatorTag(typeof(CustomizableSettingCodeGen));
            file.UseCleanProcess = false;
            file.AddUsing("UnityEngine");
            file.AddUsing("System");
            file.AddUsing("Toolkit");

            var ns = file.AddNamespace(settingNamespace);
            var c = ns.AddClass(new CodeClass(AccessModifier.Public, settingFileName, $"CustomizableSetting<{settingFileName}>"));

            foreach(var g in groups) {
                foreach(var f in g.fields) {
                    var name = $"_{GenUtility.VerifyName(f.content.text.ToLower(0))}";
                    var @var = c.AddVariable(new CodeVariable(AccessModifier.Private, f.type, name, f.defaultValue));
                    var.AddAttribute(typeof(SerializeField));
                }
            }

            foreach(var g in groups) {
                foreach(var f in g.fields) {
                    var name = f.content.text.ToUpper(0);
                    var prop = c.AddProperty(
                        new CodeProperty(
                            AccessModifier.PublicStatic,
                            string.IsNullOrEmpty(f.accessorType) ? f.type.FullName : f.accessorType,
                            name,
                            new CodeBlock($"return Instance._{GenUtility.VerifyName(name.ToLower(0))};")));
                }
            }

            var inspectorFileName = settingFileName + "Inspector";
            var ifile = new CodeFile(inspectorFileName);
            ifile.SetCreatorTag(typeof(CustomizableSettingCodeGen));
            ifile.UseCleanProcess = false;
            ifile.AddUsing("UnityEngine");
            ifile.AddUsing("System");
            ifile.AddUsing("Toolkit");

            var ifdef = new CodeIfDef("UNITY_EDITOR");
            ifile.AddContainer(ifdef);
            ifdef.Add(new CodeUsingDirective("UnityEditor"));

            var ins = ifdef.Add(new CodeNamespace(settingNamespace));
            var ic = ins.AddClass(new CodeClass(AccessModifier.Public, inspectorFileName, $"CustomizableSettingInspector<{inspectorFileName}, {settingFileName}>"));
            ic.AddAttribute(new CodeAttribute("CustomEditor", $"typeof({settingFileName})"));

            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "groupName", $"new string[]\n{{ {string.Join(",\n", groups.Select(x => $"\"{x.name}\""))} \n}}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "int[]", "groupSize", $"new int[]\n{{ {string.Join(",\n", groups.Select(x => $"{x.fields.Count}"))} \n}}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "propertyPaths", $"new string[]\n{{ {string.Join(",\n", groups.SelectMany(x => x.fields).Select(x => $"\"_{GenUtility.VerifyName(x.content.text.ToLower(0))}\""))} \n}}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "defaultValue", $"new string[]\n{{ {string.Join(",\n", groups.SelectMany(x => x.fields).Select(x => $"\"{x.defaultValue}\""))}\n}}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "customPropertyType", $"new string[]\n{{ {string.Join(",\n", groups.SelectMany(x => x.fields).Select(x => $"\"{x.accessorType}\""))}\n}}"));
            ic.AddVariable(new CodeVariable(
                AccessModifier.PrivateStatic,
                "GUIContent[]",
                "fields",
                $"new GUIContent[]\n{{ {string.Join(",\n", groups.SelectMany(x => x.fields).Select(x => $"new GUIContent(\"{x.content.text}\", \"{x.content.tooltip}\")"))} \n}}"));
            ic.AddVariable(new CodeVariable(
                AccessModifier.PrivateStatic,
                "Type[]",
                "fieldsType",
                $"new Type[]\n{{ {string.Join(",\n", groups.SelectMany(x => x.fields).Select(x => $"typeof({x.type.FullName})"))}\n}}"));

            ic.AddMethod(new CodeMethod(AccessModifier.Public | AccessModifier.Override, "OnInspectorGUI", new CodeBlock("Draw();")));

            var scriptPath = AssetDatabase.GetAssetPath(obj.ScriptReference);
            var inspectorPath = AssetDatabase.GetAssetPath(obj.InspectorReference);

            if(settingFileName != typeof(TSetting).Name) {
                AssetDatabase.RenameAsset(scriptPath, settingFileName);
                AssetDatabase.RenameAsset(inspectorPath, inspectorFileName);
                var tobj = obj.serializedObject.targetObject;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(tobj), settingFileName);
            }
            scriptPath = System.IO.Path.GetDirectoryName(scriptPath).Replace('\\', '/');
            inspectorPath = System.IO.Path.GetDirectoryName(inspectorPath).Replace('\\', '/');

            var scriptResultPath = file.CreateFile(scriptPath);
            var inspectorResultPath = ifile.CreateFile(inspectorPath);

            AssetDatabase.ImportAsset(scriptResultPath, ImportAssetOptions.ForceUpdate);
        }

        public static void CreateNew(string name) {
            var settingFileName = GenUtility.VerifyName(name.Split('.').Last());
            var settingNamespace = name.Remove(name.Length - settingFileName.Length - 1);

            var file = new CodeFile(settingFileName);
            file.SetCreatorTag(typeof(CustomizableSettingCodeGen));
            file.UseCleanProcess = false;
            file.AddUsing("UnityEngine");
            file.AddUsing("System");
            file.AddUsing("Toolkit");

            var ns = file.AddNamespace(settingNamespace);
            var c = ns.AddClass(new CodeClass(AccessModifier.Public, settingFileName, $"CustomizableSetting<{settingFileName}>"));

            var inspectorFileName = settingFileName + "Inspector";
            var ifile = new CodeFile(inspectorFileName);
            ifile.SetCreatorTag(typeof(CustomizableSettingCodeGen));
            ifile.UseCleanProcess = true;
            ifile.AddUsing("UnityEngine");
            ifile.AddUsing("System");
            ifile.AddUsing("Toolkit");

            var ifdef = new CodeIfDef("UNITY_EDITOR");
            ifile.AddContainer(ifdef);
            ifdef.Add(new CodeUsingDirective("UnityEditor"));

            var ins = ifdef.Add(new CodeNamespace(settingNamespace));
            var ic = ins.AddClass(new CodeClass(AccessModifier.Public, inspectorFileName, $"CustomizableSettingInspector<{inspectorFileName}, {settingFileName}>"));
            ic.AddAttribute(new CodeAttribute("CustomEditor", $"typeof({settingFileName})"));

            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "groupName", $"new string[]{{ }}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "int[]", "groupSize", $"new int[]{{ }}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "propertyPaths", $"new string[]{{ }}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "defaultValue", $"new string[]{{ }}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "string[]", "customPropertyType", $"new string[]{{ }}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "GUIContent[]", "fields", $"new GUIContent[]{{  }}"));
            ic.AddVariable(new CodeVariable(AccessModifier.PrivateStatic, "Type[]", "fieldsType", $"new Type[]{{ }}"));

            ic.AddMethod(new CodeMethod(AccessModifier.Public | AccessModifier.Override, "OnInspectorGUI", new CodeBlock("Draw();")));

            var scriptResultPath = file.CreateFile("Assets/");
            var inspectorResultPath = ifile.CreateFile("Assets/");

            AssetDatabase.ImportAsset(scriptResultPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(inspectorResultPath, ImportAssetOptions.ForceUpdate);
        }

        [MenuItem("Assets/Create/Toolkit/Customizable Settings")]
        private static void CreateNew() {
            var name = $"temporary.newsetting{Random.Range(10000, 99999)}";
            CreateNew(name);
        }
    }
}
