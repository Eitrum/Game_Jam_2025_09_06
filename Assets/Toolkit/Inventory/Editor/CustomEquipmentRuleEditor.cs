using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Toolkit.CodeGenerator;

namespace Toolkit.Inventory {
    public static class CustomEquipmentRuleEditor {

        public class EquipmentRules {
            public EquipmentSlot slot = EquipmentSlot.None;
            public List<ItemType> typesSupported = new List<ItemType>();
            public UnityEditorInternal.ReorderableList drawer;

            public EquipmentRules(EquipmentSlot slot) {
                this.slot = slot;
                drawer = new UnityEditorInternal.ReorderableList(typesSupported, typeof(ItemType));
                var arr = ItemType.None.GetArray();
                for(int i = 0, length = arr.Length; i < length; i++) {
                    var type = arr[i];
                    if(EquipmentSlotUtility.CanEquip(slot, type)) {
                        typesSupported.Add(type);
                    }
                }
                drawer.drawHeaderCallback += DrawHeader;
                drawer.drawElementCallback += DrawElement;
            }

            private void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, EquipmentSlotUtility.ToString(slot));
            }

            private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                rect.SplitHorizontal(out Rect indexArea, out Rect label, 30f / rect.width);
                EditorGUI.LabelField(indexArea, index.ToString());
                typesSupported[index] = (ItemType)EditorGUI.EnumPopup(label, typesSupported[index]);
            }

            public void RenderLayout() {
                drawer.DoLayoutList();
            }
        }

        private static List<EquipmentRules> rules = new List<EquipmentRules>();

        private static int selectedRuleEditor = 0;
        private static bool isInitialized;

        public static void Load() {
            if(isInitialized)
                return;
            isInitialized = true;
            rules.Clear();
            rules.AddRange(EquipmentSlot.None.GetArray().Select(x => new EquipmentRules(x)));
            rules.RemoveAt(0);
        }

        public static void OnGUI() {
            Load();
            GUILayout.Space(12f);

            GUILayout.BeginVertical("box");
            GUILayout.Label("Equipment Rule Editor");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(120f));
            GUILayout.BeginVertical("box", GUILayout.Width(120f));

            var ev = Event.current;

            for(int i = 0; i < rules.Count; i++) {
                GUILayout.Label((i == selectedRuleEditor ? "-> " : "") + EquipmentSlotUtility.ToString(rules[i].slot));
                var area = GUILayoutUtility.GetLastRect();
                if(ev.type == EventType.MouseDown && ev.button == 0 && area.Contains(ev.mousePosition)) {
                    selectedRuleEditor = i;
                    ev.Use();
                }
            }

            GUILayout.EndVertical();
            var save = GUILayout.Button("Save", GUILayout.Width(110f));
            if(save) {
                CustomEquipmentSlotEditor.Save(CustomEquipmentSlotEditor.EquipmentSlotEnumValues);
            }
            GUILayout.EndVertical();
            GUILayout.Space(2f);
            GUILayout.BeginVertical("box");
            rules[selectedRuleEditor].RenderLayout();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// public static bool CanEquip(EquipmentSlot slot, ItemType type) {
        ///    return CanEquip(type, slot);
        /// }

        public static void GenerateEquipmentRules(IReadOnlyList<CustomEquipmentSlotEditor.EnumValues> enumValues, CodeClass c) {
            rules = new List<EquipmentRules>(rules.Where(x => enumValues.Any(y => y.name.Equals(EquipmentSlotUtility.ToString(x.slot)))));

            // Add both ways support
            c.AddMethod(new CodeMethod(AccessModifier.PublicStatic, "bool", "CanEquip", new CodeVariable[]{
                new CodeVariable(typeof(EquipmentSlot), "slot"),
                new CodeVariable(typeof(ItemType), "type")
            }, new CodeBlock("return CanEquip(type, slot);")));


            List<string> codeRules = new List<string>();
            codeRules.Add("switch(slot){");
            foreach(var rule in rules) {
                List<ItemType> tempTypes = new List<ItemType>();
                foreach(var t in rule.typesSupported)
                    if(!tempTypes.Contains(t))
                        tempTypes.Add(t);
                rule.typesSupported.Clear();
                rule.typesSupported.AddRange(tempTypes);
                var ruleCount = rule.typesSupported.Count;
                if(ruleCount == 0)
                    continue;
                codeRules.Add($"{GenUtility.INDENT}case EquipmentSlot.{rule.slot}:");
                codeRules.Add($"{GenUtility.INDENT}{GenUtility.INDENT}switch(type){{");
                for(int i = 0; i < ruleCount; i++) {
                    codeRules.Add($"{GenUtility.INDENT}{GenUtility.INDENT}{GenUtility.INDENT}case ItemType.{rule.typesSupported[i]}:return true;");
                }
                codeRules.Add($"{GenUtility.INDENT}{GenUtility.INDENT}}}");

                codeRules.Add($"{GenUtility.INDENT}break;");

            }
            codeRules.Add("}");
            codeRules.Add("return false;");

            c.AddMethod(new CodeMethod(AccessModifier.PublicStatic, "bool", "CanEquip", new CodeVariable[]{
                new CodeVariable(typeof(ItemType), "type"),
                new CodeVariable(typeof(EquipmentSlot), "slot")
            }, new CodeBlock(codeRules)));

            List<PerferredSlotRule> slotRules = new List<PerferredSlotRule>();
            for(int i = 0; i < rules.Count; i++) {
                var items = rules[i].typesSupported;
                foreach(var item in items) {
                    if(!slotRules.Any(x => x.ItemType == item)) {
                        slotRules.Add(new PerferredSlotRule() { ItemType = item, SlotType = rules[i].slot });
                    }
                }
            }

            List<string> preferredRules = new List<string>();
            preferredRules.Add("switch(itemType){");
            foreach(var rule in slotRules) {
                preferredRules.Add($"{GenUtility.INDENT}case ItemType.{rule.ItemType}: return EquipmentSlot.{rule.SlotType};");
            }
            preferredRules.Add("}");
            preferredRules.Add("return EquipmentSlot.None;");

            c.AddMethod(
                new CodeMethod(
                    AccessModifier.PublicStatic, 
                    typeof(EquipmentSlot), 
                    "GetPreferredSlot", 
                    new CodeVariable[] { new CodeVariable(typeof(ItemType), "itemType") }, 
                    new CodeBlock(preferredRules)));
        }

        private struct PerferredSlotRule {
            public ItemType ItemType;
            public EquipmentSlot SlotType;
        }
    }
}
