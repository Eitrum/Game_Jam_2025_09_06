using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Inventory
{
    // Attachment points depends on equipment slots!
    public class CustomAttachmentPointsEditor
    {
        #region Classes

        public class EnumValues
        {
            public string name = "";
            public bool isEquipmentSlot = false;

            public EnumValues() { }
            public EnumValues(string name) {
                this.name = name;
            }
            public EnumValues(string name, bool isEquipmentSlot) {
                this.name = name;
                this.isEquipmentSlot = isEquipmentSlot;
            }
        }

        #endregion

        #region Variables

        private static List<EnumValues> customEnumValues = new List<EnumValues>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(EnumValues));

        #endregion

        #region Properties

        public static IReadOnlyList<EnumValues> EquipmentSlotEnumValues => customEnumValues;

        #endregion

        #region Initialization

        static CustomAttachmentPointsEditor() {
            Load();
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onCanAddCallback += (list) => list.count < 32;
            reorderableList.onCanRemoveCallback += (list) => !customEnumValues[list.index].isEquipmentSlot;
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Inventory/Attachment Points", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }


        private static void Load() {
            customEnumValues.Clear();

            var equipmentSlots = typeof(EquipmentSlot);
            var customType = typeof(AttachmentPoint);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);

            var equipNames = System.Enum.GetNames(equipmentSlots);
            var equipValues = System.Enum.GetValues(equipmentSlots);

            for(int i = 1; i < equipNames.Length; i++) {
                customEnumValues.Add(new EnumValues(EquipmentSlotUtility.ToString((EquipmentSlot)(int)equipValues.GetValue(i)), true));
            }

            for(int i = 1, length = names.Length; i < length; i++) {
                if(equipNames.Contains(names[i])) {
                    continue;
                }
                customEnumValues.Add(new EnumValues(AttachmentPointsUtility.ToString((AttachmentPoint)(int)values.GetValue(i))));
            }
        }

        #endregion

        #region Drawing

        private static void OnGUI(string obj) {
            // Verify Equipment Slots exists
            var slots = CustomEquipmentSlotEditor.EquipmentSlotEnumValues;
            for(int i = 0; i < customEnumValues.Count; i++) {
                if(customEnumValues[i].isEquipmentSlot) {
                    customEnumValues.RemoveAt(i);
                    i--;
                }
            }
            for(int i = 0; i < slots.Count; i++) {
                customEnumValues.Insert(i, new EnumValues(slots[i].name, true));
            }

            // Draw
            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                Save(customEnumValues);
            }
        }

        private static void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Attachment Slots Editor");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect typeLabel, out Rect label, 20f / rect.width, 70f / rect.width, 0f);
            bool isEquipment = customEnumValues[index].isEquipmentSlot;
            using(new EditorGUI.DisabledGroupScope(isEquipment)) {
                EditorGUI.LabelField(indexArea, index.ToString());
                EditorGUI.LabelField(typeLabel, isEquipment ? "Equipment" : "Custom");
                customEnumValues[index].name = EditorGUI.TextField(label, customEnumValues[index].name);
            }
        }


        #endregion

        #region Saving / Generation

        public static void Save(IReadOnlyList<EnumValues> points) {
            var copy = points
                .Where(x => !string.IsNullOrEmpty(x.name))
                .Unique(x => x.name)
                .ToList();
            copy.Insert(0, new EnumValues("None"));

            var enumValues = copy
                .SelectWithIndex((x, i) => new Toolkit.CodeGenerator.GenUtility.EnumValues(x.name, i))
                .ToArray();

            var codeFile = CodeGenerator.GenUtility.CreateEnumClass(
                "AttachmentPoint",
                "Toolkit.Inventory",
                enumValues);
            codeFile.SetCreatorTag(typeof(CustomAttachmentPointsEditor));
            codeFile.UseCleanProcess = true;

            GenerateUtilityClass(codeFile, copy);

            var path = codeFile.CreateFile("Assets/Toolkit/Inventory/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            GenerateBehaviourClass(copy);
        }


        private static void GenerateUtilityClass(CodeFile file, IReadOnlyList<EnumValues> enumValues) {
            var ns = file.GetContainers<CodeNamespace>()[0];
            var intf = ns.Add(new CodeInterface("IAttachmentPoints", AccessModifier.Public));
            var c = ns.AddClass(new CodeClass("AttachmentPointsUtility", AccessModifier.PublicStatic));
            file.AddUsing(typeof(Transform));

            var equipmentSlotOnly = enumValues.Where(x => x.isEquipmentSlot);

            intf.AddCustom(new CodeCustom("Transform this[AttachmentPoint point] { get; set; }"));
            intf.AddMethod(new CodeMethod(AccessModifier.Public, "Transform", "GetAttachment", new CodeVariable("AttachmentPoint", "point"), null));
            intf.AddMethod(new CodeMethod(AccessModifier.Public, "bool", "SetAttachment", new CodeVariable[] { new CodeVariable("AttachmentPoint", "point"), new CodeVariable("Transform", "transform") }, null));
            intf.AddMethod(new CodeMethod(AccessModifier.Public, "bool", "HasAttachment", new CodeVariable("AttachmentPoint", "point"), null));

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "EquipmentSlot",
               "GetEquipmentSlot",
               new CodeVariable(AccessModifier.This, "AttachmentPoint", "point"),
                   CodeBlock.CreateReturnSwitchBlock(
                       "point",
                       equipmentSlotOnly.Select(x => $"AttachmentPoint.{GenUtility.VerifyName(x.name)}").ToArray(),
                       equipmentSlotOnly.Select(x => $"EquipmentSlot.{GenUtility.VerifyName(x.name)}").ToArray(),
                       "return EquipmentSlot.None;")));

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "AttachmentPoint",
               "GetAttachmentPoint",
               new CodeVariable(AccessModifier.This, "EquipmentSlot", "slot"),
                   CodeBlock.CreateReturnSwitchBlock(
                       "slot",
                       equipmentSlotOnly.Select(x => $"EquipmentSlot.{GenUtility.VerifyName(x.name)}").ToArray(),
                       equipmentSlotOnly.Select(x => $"AttachmentPoint.{GenUtility.VerifyName(x.name)}").ToArray(),
                       "return AttachmentPoint.None;")));

            c.AddMethod(new CodeMethod(
               AccessModifier.PublicStatic,
               "bool",
               "IsEquipmentSlot",
               new CodeVariable(AccessModifier.This, "AttachmentPoint", "point"),
                   CodeBlock.CreateReturnSwitchBlock(
                       "point",
                       enumValues.Select(x => $"AttachmentPoint.{GenUtility.VerifyName(x.name)}").ToArray(),
                       enumValues.Select(x => $"{x.isEquipmentSlot}".ToLower()).ToArray(),
                       "return false;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "ToString",
                new CodeVariable(AccessModifier.This, "AttachmentPoint", "point"),
                    CodeBlock.CreateReturnSwitchBlock(
                        "point",
                        enumValues.Select(x => $"AttachmentPoint.{GenUtility.VerifyName(x.name)}").ToArray(),
                        enumValues.Select(x => $"\"{x.name}\"").ToArray(),
                        "return \"Unknown\";")));

        }

        private static void GenerateBehaviourClass(IReadOnlyList<EnumValues> enumValues) {
            var copy = enumValues.Skip(1);
            var file = new CodeFile("AttachmentPointsBehaviour");
            file.SetCreatorTag(typeof(CustomAttachmentPointsEditor));
            file.UseCleanProcess = true;
            file.AddUsing(typeof(UnityEngine.Transform));
            var ns = file.AddNamespace("Toolkit.Inventory");
            var c = ns.AddClass(new CodeClass(AccessModifier.Public, "AttachmentPointsBehaviour", new string[] { "MonoBehaviour", "IAttachmentPoints" }));
            c.AddAttribute(new CodeAttribute(typeof(AddComponentMenu), "\"Toolkit/Inventory/Attachment Points\""));

            copy.Foreach(x => {
                var v = c.AddVariable(new CodeVariable(AccessModifier.Private, typeof(Transform), x.name.ToLower(), "null"));
                v.AddAttribute(typeof(SerializeField));
            });

            copy.Foreach(x => c.AddProperty(new CodeProperty(AccessModifier.Public, typeof(Transform), x.name, new CodeBlock($"{GenUtility.VerifyName(x.name).ToLower()};"))));

            c.AddCustom(new CodeCustom("public Transform this[AttachmentPoint point] { get => GetAttachment(point); set => SetAttachment(point, value); }"));

            var switchValues = copy.Select(x => $"AttachmentPoint.{GenUtility.VerifyName(x.name)}").ToArray();

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                typeof(Transform),
                "GetAttachment",
                new CodeVariable(typeof(AttachmentPoint), "point"),
                CodeBlock.CreateReturnSwitchBlock(
                    "point",
                    switchValues,
                    copy.Select(x => $"{GenUtility.VerifyName(x.name.ToLower())}").ToArray(),
                    "return null;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "bool",
                "SetAttachment",
                new CodeVariable[]{
                    new CodeVariable(typeof(AttachmentPoint), "point"),
                    new CodeVariable(typeof(Transform), "transform")
                },
                CodeBlock.CreateAssignAndReturnSwitchBlock(
                    "point",
                    "transform",
                    switchValues,
                    copy.Select(x => $"{GenUtility.VerifyName(x.name.ToLower())}").ToArray(),
                    copy.Select(x => "true").ToArray(),
                    "return false;")));

            c.AddMethod(new CodeMethod(
                AccessModifier.Public,
                "bool",
                "HasAttachment",
                new CodeVariable(typeof(AttachmentPoint), "point"),
                new CodeBlock("GetAttachment(point) != null;")));

            var path = file.CreateFile("Assets/Toolkit/Inventory/");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        #endregion
    }
}
