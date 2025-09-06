using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using Toolkit.CodeGenerator;
using System.Linq;

namespace Toolkit {
    public static class CommandLineArgumentsEditor {
        public class Setting {
            #region Variables

            public CLAType Type;
            public string Name = "";

            public bool DefaultBool = false;
            public int DefaultInt = 0;
            public float DefaultFloat = 0f;
            public string DefaultString = "";

            #endregion

            #region Constructor

            public Setting(string name) {
                Type = CLAType.Bool;
                this.Name = name;
            }

            public Setting(string name, FieldInfo fieldInfo) {
                var t = fieldInfo.FieldType;
                Name = name;

                if(t == typeof(bool)) {
                    Type = CLAType.Bool;
                    DefaultBool = (bool)fieldInfo.GetValue(null);
                }
                else if(t == typeof(int)) {
                    Type = CLAType.Int;
                    DefaultInt = (int)fieldInfo.GetValue(null);
                }
                else if(t == typeof(float)) {
                    Type = CLAType.Float;
                    DefaultFloat = (float)fieldInfo.GetValue(null);
                }
                else if(t == typeof(string)) {
                    Type = CLAType.String;
                    DefaultString = (string)fieldInfo.GetValue(null);
                }
            }

            #endregion
        }

        public enum CLAType {
            Bool,
            Int,
            Float,
            String,
        }

        #region Variables

        private const string LOCAL_PATH = "Library/Toolkit/CommandLineArguments.cfg";
        private static List<Setting> settings = new List<Setting>();
        private static ReorderableList settingsList;

        private static string editorlaunchArgs;

        #endregion

        #region Init

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitLaunchArgs() {
            if(!System.IO.File.Exists(LOCAL_PATH)) {
                return;
            }
            editorlaunchArgs = System.IO.File.ReadAllText(LOCAL_PATH);
            CommandLineArguments.Add(editorlaunchArgs);
        }

        [InitializeOnLoadMethod]
        private static void Load() {
            settings.Clear();
            int index = 0;
            var fields = typeof(CommandLineArguments).GetFields(BindingFlags.Static | BindingFlags.Public);
            fields.Foreach(x => settings.Add(new Setting(CommandLineArguments.GetName(index++), x)));

            if(settingsList == null) {
                settingsList = new ReorderableList(settings, typeof(Setting), true, true, true, true);
                settingsList.drawElementCallback = DrawElement;
                settingsList.drawHeaderCallback = DrawHeader;
                settingsList.onAddCallback = AddRule;
                settingsList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if(System.IO.File.Exists(LOCAL_PATH))
                editorlaunchArgs = System.IO.File.ReadAllText(LOCAL_PATH);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Command Line Arguments", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        #endregion

        #region Draw

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect typeArea, 20f / rect.width, 1f - 80f / rect.width, 2f);
            EditorGUI.LabelField(indexArea, index.ToString());
            EditorGUI.LabelField(nameArea, settings[index].Name);
            EditorGUI.LabelField(typeArea, settings[index].Type.ToString());
        }

        private static void DrawHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect typeArea, 20f / rect.width, 1f - 80f / rect.width, 2f);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(nameArea, "Arguments", EditorStyles.boldLabel);
            EditorGUI.LabelField(typeArea, "Type");
        }

        private static void OnGUI(string obj) {
            settingsList.DoLayoutList();

            if(settingsList.index >= 0 && settingsList.index < settings.Count) {
                DrawSetting(settings[settingsList.index]);
            }

            EditorGUILayout.Space();

            if(GUILayout.Button("Save", GUILayout.Width(80f))) {
                Generate();
            }

            EditorGUILayout.Space(80);
            EditorGUILayout.LabelField("Debug Launch Arguments:", EditorStylesUtility.BoldLabel);
            EditorGUI.BeginChangeCheck();
            var textUpdate = EditorGUILayout.DelayedTextField(editorlaunchArgs, GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));
            if(EditorGUI.EndChangeCheck()) {
                editorlaunchArgs = textUpdate;
                try {
                    var dirPath = System.IO.Path.GetDirectoryName(LOCAL_PATH);
                    if(!System.IO.Directory.Exists(dirPath)) {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                    System.IO.File.WriteAllText(LOCAL_PATH, editorlaunchArgs);
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                }
            }

            EditorGUILayout.Space(40);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Cached:", EditorStylesUtility.BoldLabel);
                foreach(var cached in CommandLineArguments.CachedArgumentsWithValues)
                    EditorGUILayout.LabelField(cached.Key, cached.Value);
            }
        }

        private static void DrawSetting(Setting s) {
            using(new EditorGUILayout.VerticalScope("box")) {
                s.Name = EditorGUILayout.TextField("Name", s.Name);
                s.Type = (CLAType)EditorGUILayout.EnumPopup("Type", s.Type);
                switch(s.Type) {
                    case CLAType.Int:
                        s.DefaultInt = EditorGUILayout.IntField("Default Value", s.DefaultInt);
                        break;
                    case CLAType.Float:
                        s.DefaultFloat = EditorGUILayout.FloatField("Default Value", s.DefaultFloat);
                        break;
                    case CLAType.String:
                        s.DefaultString = EditorGUILayout.TextField("Default Value", s.DefaultString);
                        break;
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);
                using(new EditorGUI.IndentLevelScope()) {
                    switch(s.Type) {
                        case CLAType.Bool:
                            EditorGUILayout.LabelField(GetCommandLineArgumentName(s.Name));
                            break;
                        case CLAType.Int:
                        case CLAType.Float:
                        case CLAType.String:
                            EditorGUILayout.LabelField($"-{CodeGenerator.GenUtility.VerifyName(s.Name.ToLower())}=[value]");
                            break;
                    }
                    EditorGUILayout.LabelField($"CommandLineArgument.{CodeGenerator.GenUtility.VerifyName(s.Name)}");
                }
            }
        }

        #endregion

        #region Callbacks

        private static void AddRule(ReorderableList list) {
            settings.Add(new Setting("New Argument"));
        }

        #endregion

        #region Code Generation

        private static void Generate() {
            var settings = CommandLineArgumentsEditor.settings.Unique(x => x.Name);
            var file = new CodeFile("CommandLineArguments_Generated");
            file.SetCreatorTag(typeof(CommandLineArgumentsEditor));
            var ns = file.AddNamespace("Toolkit");
            var c = ns.AddClass(AccessModifier.PublicStatic | AccessModifier.Partial, "CommandLineArguments");

            // Add Variables
            foreach(var s in settings)
                c.AddVariable(new CodeVariable(AccessModifier.PublicStatic, NameFromType(s.Type), s.Name, DefaultValueAsString(s)));

            // Add Load Method
            List<string> load = new List<string>();
            foreach(var s in settings) {
                var varName = GenUtility.VerifyName(s.Name);
                switch(s.Type) {
                    case CLAType.Bool:
                    case CLAType.Int:
                    case CLAType.Float:
                    case CLAType.String:
                        load.Add($"if(TryGetValue(\"{GetCommandLineArgumentName(s.Name)}\", out {NameFromType(s.Type)} {GenUtility.VerifyName(s.Name)}))");
                        load.Add($"{GenUtility.INDENT}UnityEngine.Debug.Log(TAG + \"Launching with {s.Name} = \" + {GenUtility.VerifyName(s.Name)});");
                        break;
                }
            }
            c.AddMethod(new CodeMethod(AccessModifier.PrivateStatic, "LoadPresets", new CodeBlock(load)));

            c.AddMethod(new CodeMethod(AccessModifier.InternalStatic,
                "string",
                "GetName",
                new CodeVariable("int", "index"),
                CodeBlock.CreateReturnSwitchBlock("index", ToIndex(settings).ToArray(), settings.Select(x => $"\"{x.Name}\"").ToArray(), "return \"unknown\";")));

            var path = file.CreateFile("Assets/Toolkit/Utilities");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static string GetCommandLineArgumentName(string name) {
            return $"-{CodeGenerator.GenUtility.VerifyName(name.ToLower())}";
        }

        private static string NameFromType(CLAType type) {
            switch(type) {
                case CLAType.Bool: return "bool";
                case CLAType.Int: return "int";
                case CLAType.Float: return "float";
                case CLAType.String: return "string";

            }
            return "bool";
        }

        private static string DefaultValueAsString(Setting s) {
            switch(s.Type) {
                case CLAType.Bool: return "false";
                case CLAType.Int: return $"{s.DefaultInt}";
                case CLAType.Float: return $"{s.DefaultFloat}f";
                case CLAType.String: return $"\"{s.DefaultString}\"";
            }
            return "";
        }

        private static IEnumerable<string> ToIndex<T>(IEnumerable<T> other) {
            int index = 0;
            foreach(var enu in other) {
                yield return $"{index++}";
            }
        }

        #endregion
    }
}
