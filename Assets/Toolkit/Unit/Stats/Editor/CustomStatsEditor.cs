using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit {
    public static class CustomStatsEditor {
        public class EnumValues {
            public string name;
            public Color32 color;

            public EnumValues() {
                name = "new stat";
            }

            public EnumValues(string name) {
                this.name = name;
            }
        }

        #region Variables

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList;

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Load() {
            customEnumValues.Clear();
            var customType = typeof(StatType);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                var statType = (StatType)i;
                customEnumValues.Add(new EnumValues() {
                    name = StatsUtility.GetName(statType),
                    color = StatsUtility.GetColor(statType),
                });
            }

            if(reorderableList == null) {
                reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));
                reorderableList.drawElementCallback += DrawElement;
                reorderableList.drawHeaderCallback += DrawHeader;
                reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Stats", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        #endregion

        #region List Draw

        private static void DrawHeader(Rect rect) {
            rect.ShrinkRef(16f, RectExtensions.ShrinkSide.Left);
            rect.SplitHorizontal(out Rect indexArea, out Rect labelArea, 20f / rect.width);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(labelArea, "Names");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.PadRef(0, 0, 2, 2);
            rect.SplitHorizontal(out Rect indexArea, out Rect labelArea, out Rect colorArea, 20f / rect.width, 0.6f, 4f);
            EditorGUI.LabelField(indexArea, index.ToString());
            customEnumValues[index].name = EditorGUI.TextField(labelArea, customEnumValues[index].name);
            customEnumValues[index].color = EditorGUI.ColorField(colorArea, customEnumValues[index].color);
        }

        #endregion

        #region GUI Draw

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
                }
            }
        }

        #endregion

        #region Code Gen

        private static void Generate() {
            var list = customEnumValues.Unique();

            var file = new CodeFile("StatType");
            file.UseCleanProcess = true;
            file.SetCreatorTag(typeof(CustomStatsEditor));
            file.AddUsing("UnityEngine");
            file.AddUsing("System");
            var ns = file.AddNamespace("Toolkit.Unit");

            var enu = ns.AddEnum(new CodeEnum(AccessModifier.Public, "StatType", list.Select(x => x.name).Insert(0, "None").ToArray()));

            var c = ns.AddClass(new CodeClass(AccessModifier.PublicStatic | AccessModifier.Partial, "StatsUtility"));
            c.AddVariable(new CodeVariable(AccessModifier.PublicConst, "int", "TYPES", list.Count().ToString()));

            // Add GetName Method
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable("StatType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    list.Select(x => $"StatType.{GenUtility.VerifyName(x.name)}").ToArray(),
                    list.Select(x => $"\"{x.name}\"").ToArray(),
                    "return \"\";")));

            // Add GetColor Method
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                typeof(UnityEngine.Color32),
                "GetColor",
                new CodeVariable("StatType", "type"),
                CodeBlock.CreateReturnSwitchBlock(
                    "type",
                    list.Select(x => $"StatType.{GenUtility.VerifyName(x.name)}").ToArray(),
                    list.Select(x => $"new Color32({x.color.r}, {x.color.g}, {x.color.b}, {x.color.a});").ToArray(),
                    "return Color.white;")));


            var path = file.CreateFile("Assets/Toolkit/Unit/Stats/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        #endregion
    }
}
