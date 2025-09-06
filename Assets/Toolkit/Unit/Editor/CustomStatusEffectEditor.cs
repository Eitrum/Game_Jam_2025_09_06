using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
/*
namespace Toolkit.Unit
{
    public static class CustomStatusEffectEditor
    {

        public class EditorStatusType
        {
            public string Name = "";
            public string Description;
            public int index;
        }

        public enum StatusDurationType
        {
            Integer,
            Float,
        }

        [InitializeOnLoadMethod]
        private static void Load() {
            var customType = typeof(StatusEffectType);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            var attributeClass = new Attributes();
            for(int i = 2, length = names.Length; i < length; i++) {
                var value = (int)values.GetValue(i);
                var type = (StatusEffectType)value;
                highestIndex = Mathf.Max(highestIndex, value);
                statusTypeList.Add(new EditorStatusType() {
                    Name = StatusEffectUtility.GetDefaultName(type),
                    Description = StatusEffectUtility.GetDefaultDescription(type),
                    index = value
                });
            }


            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 3;
            reorderableList.onAddCallback += OnAdd;
        }

        private static void OnAdd(ReorderableList list) {
            statusTypeList.Add(new EditorStatusType() { index = (++highestIndex) });
        }

        private static void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Status Effects Type Editor");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitVertical(out Rect top, out Rect bot, 0.33f);
            statusTypeList[index].Name = EditorGUI.TextField(top, statusTypeList[index].Name);
            statusTypeList[index].Description = EditorGUI.TextArea(bot, statusTypeList[index].Description);
        }

        private static int highestIndex = 0;
        private static StatusDurationType selectedDurationType = StatusDurationType.Integer;
        private static List<EditorStatusType> statusTypeList = new List<EditorStatusType>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(statusTypeList, typeof(EditorStatusType));

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Status Effects", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            selectedDurationType = (StatusDurationType)EditorGUILayout.EnumPopup("Duration Type", selectedDurationType);

            GUILayout.Space(12f);
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                GenerateStatus(statusTypeList, selectedDurationType);
            }
        }

        private static void GenerateStatus(IReadOnlyList<EditorStatusType> statusTypes, StatusDurationType durationType) {
            GenerateStatusTypeFile(statusTypes);
        }

        private static void GenerateStatusTypeFile(IReadOnlyList<EditorStatusType> statusTypes) {
            var file = new CodeFile("StatusEffectType");
            var ns = file.AddNamespace("Toolkit.Unit");
            var tempTypes = new List<EditorStatusType>(statusTypes);
            tempTypes.Insert(0, new EditorStatusType() { Name = "None" });
            tempTypes.Insert(1, new EditorStatusType() { Name = "Custom" });
            ns.AddEnum(new CodeEnum("StatusEffectType", tempTypes.Select(x => new CodeVariable("int", x.Name, x.index.ToString())).ToArray()));
            var c = ns.AddClass(new CodeClass("StatusEffectUtility", AccessModifier.PublicStatic));

            // Add Name method
            // public static string GetDefaultName(StatusType type) {
            var nameFilter = statusTypes.Where(x => !string.IsNullOrEmpty(x.Name));
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetDefaultName",
                new CodeVariable(typeof(StatusEffectType), "type"),
                CodeBlock.CreateReturnSwitchBlock("type",
                    nameFilter.Select(x => $"StatusEffectType.{GenUtility.VerifyName(x.Name)}").ToArray(),
                     nameFilter.Select(x => $"\"{x.Name}\"").ToArray(), "return \"\";")));

            // Add Description
            // public static string GetDefaultDescription(StatusType type)
            var descriptionFilter = statusTypes.Where(x => !string.IsNullOrEmpty(x.Description));
            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetDefaultDescription",
                new CodeVariable(typeof(StatusEffectType), "type"),
                CodeBlock.CreateReturnSwitchBlock("type",
                    descriptionFilter.Select(x => $"StatusEffectType.{GenUtility.VerifyName(x.Name)}").ToArray(),
                     descriptionFilter.Select(x => $"\"{x.Description}\"").ToArray(), "return \"\";")));

            var path = file.CreateFile("Assets/Toolkit/Unit/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
*/
