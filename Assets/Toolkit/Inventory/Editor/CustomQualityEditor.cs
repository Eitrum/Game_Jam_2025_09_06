using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Inventory
{
    public static class CustomQualityEditor
    {
        public class EnumValues
        {
            public string name;
            public float qualityValue;
            public Color32 color;

            public EnumValues() {
                name = "new quality";
                qualityValue = 0f;
                color = Color.white;
            }

            public EnumValues(Quality quality) {
                this.name = QualityUtility.GetName(quality);
                this.qualityValue = QualityUtility.GetMinimumValue(quality);
                this.color = QualityUtility.GetColor(quality);
            }
        }

        private static void DrawHeader(Rect rect) {
            rect.ShrinkRef(16f, RectExtensions.ShrinkSide.Left);
            rect.SplitHorizontal(out Rect left, out Rect right, 0.5f);
            left.SplitHorizontal(out Rect indexArea, out Rect labelArea, 20f / left.width);
            right.SplitHorizontal(out Rect valueArea, out Rect colorArea, 1f - 40f / left.width);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(labelArea, "Names");
            EditorGUI.LabelField(valueArea, "Minimum Values");
            EditorGUI.LabelField(colorArea, "Colors");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect left, out Rect right, 0.5f);
            left.SplitHorizontal(out Rect indexArea, out Rect labelArea, 20f / left.width);
            right.SplitHorizontal(out Rect valueArea, out Rect colorArea, 1f - 40f / left.width);

            EditorGUI.LabelField(indexArea, index.ToString());
            customEnumValues[index].name = EditorGUI.TextField(labelArea, customEnumValues[index].name);
            customEnumValues[index].qualityValue = EditorGUI.FloatField(valueArea, customEnumValues[index].qualityValue);
            customEnumValues[index].color = EditorGUI.ColorField(colorArea, customEnumValues[index].color);
        }

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList;


        [InitializeOnLoadMethod]
        private static void Load() {
            customEnumValues.Clear();
            var customType = typeof(Quality);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                var quality = (Quality)i;
                customEnumValues.Add(new EnumValues(quality));
            }

            if(reorderableList == null) {
                reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));
                reorderableList.drawElementCallback += DrawElement;
                reorderableList.drawHeaderCallback += DrawHeader;
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Inventory/Quality", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string obj) {
            reorderableList.DoLayoutList();

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Save", GUILayout.Width(80f))) {
                    Generate();
                }

                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Reload", GUILayout.Width(80f))) {
                    Load();
                }
                if(GUILayout.Button("Reset", GUILayout.Width(80f))) {
                    customEnumValues.Clear();
                    customEnumValues.Add(new EnumValues());
                }
            }
        }

        private static void Generate() {
            var list = new List<EnumValues>(customEnumValues);
            list.Insert(0, new EnumValues(Quality.None));

            // Setup file
            var file = new CodeFile("Quality");
            file.SetCreatorTag(typeof(CustomQualityEditor));
            file.UseCleanProcess = true;
            file.AddUsing("System");
            file.AddUsing("UnityEngine");
            var ns = file.AddNamespace("Toolkit.Inventory");

            // Add enum
            ns.AddEnum("Quality", list.Select(x => x.name).ToArray());

            // Add interface
            var ci = ns.Add(new CodeInterface("IQuality"));
            ci.AddProperty(new CodeProperty("Quality", "Quality", "..."));
            ci.AddProperty(new CodeProperty("float", "QualityValue", "..."));

            // Add utility class
            var c = ns.AddClass(new CodeClass("QualityUtility", AccessModifier.PublicStatic));


            var cases = list.Select(x => $"Quality.{GenUtility.VerifyName(x.name)}").ToArray();
            // Color
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "Color",
                "GetColor",
                new CodeVariable(AccessModifier.This, "Quality", "quality"),
                CodeBlock.CreateReturnSwitchBlock(
                    "quality",
                    cases,
                    list.Select(x => $"new Color32({x.color.r},{x.color.g},{x.color.b},{x.color.a});").ToArray(),
                    "return Color.black;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "Quality",
                "GetQuality",
                new CodeVariable("float", "value"),
                new CodeBlock(
                    string.Join(
                        GenUtility.NEWLINE,
                        list.Select(x => $"if(value >= {x.qualityValue:0.0###}f) return Quality.{GenUtility.VerifyName(x.name)};"))
                            + $"{GenUtility.NEWLINE}return Quality.None;")));


            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "float",
                "GetMinimumValue",
                new CodeVariable(AccessModifier.This, "Quality", "quality"),
                CodeBlock.CreateReturnSwitchBlock(
                    "quality",
                    cases,
                    list.Select(x => $"{x.qualityValue:0.0###}f;").ToArray(),
                    "return 0f;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable(AccessModifier.This, "Quality", "quality"),
                CodeBlock.CreateReturnSwitchBlock(
                    "quality",
                    cases,
                    list.Select(x => $"\"{x.name}\";").ToArray(),
                    "return \"\";")));

            var path = file.CreateFile("Assets/Toolkit/Inventory");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
