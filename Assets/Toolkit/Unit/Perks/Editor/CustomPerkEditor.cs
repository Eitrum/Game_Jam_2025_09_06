using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Toolkit.CodeGenerator;
using System;
using UnityEditorInternal;
using System.Linq;

namespace Toolkit.Unit {
    public static class CustomPerkEditor {

        #region Classes

        public class MaskDrawerInstance<T> where T : System.Enum {

            #region Variables

            public int NonEditable = 10;
            private string header;
            private List<EnumValue> values = new List<EnumValue>();
            private UnityEditorInternal.ReorderableList reorderableList;

            #endregion

            #region Constructor

            public MaskDrawerInstance(string header, int nonEditable) {
                this.header = header;
                this.NonEditable = nonEditable;
                var arr = FastEnum<T>.Array;

                for(int i = 0, length = arr.Count; i < length; i++) {
                    var p = arr[i];
                    var val = p.ToInt();
                    values.Add(new EnumValue(GetName(p), val));
                }
                reorderableList = new UnityEditorInternal.ReorderableList(values, typeof(EnumValue));
                reorderableList.drawElementCallback += DrawElement;
                reorderableList.drawHeaderCallback += DrawHeader;
                reorderableList.onAddCallback += OnAdd;
                reorderableList.onCanRemoveCallback += OnCheckRemove;
                reorderableList.draggable = false;
                reorderableList.displayAdd = false;
                reorderableList.displayRemove = false;
            }

            #endregion

            #region Draw

            public void Draw() {
                reorderableList.DoLayoutList();
            }

            private bool OnCheckRemove(ReorderableList list) {
                return list.index > 9;
            }

            private void OnAdd(ReorderableList list) {
                values.Add(new EnumValue("", 1 << values.Count));
            }

            private void DrawHeader(Rect rect) {
                rect.PadRef(12, 0, 0, 0);
                rect.SplitHorizontal(out Rect indexArea, out Rect labelArea, out Rect valueArea, 100f / rect.width, (rect.width - 250f) / rect.width, 2f);
                EditorGUI.LabelField(indexArea, "ID");
                EditorGUI.LabelField(labelArea, header, EditorStyles.boldLabel);
                EditorGUI.LabelField(valueArea, "1 << #");
            }

            private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                using(new EditorGUI.DisabledScope(index < NonEditable)) {
                    rect.PadRef(0, 0, 2, 2);
                    rect.SplitHorizontal(out Rect indexArea, out Rect labelArea, out Rect valueArea, 100f / rect.width, (rect.width - 250f) / rect.width, 2f);
                    var val = values[index];

                    EditorGUI.LabelField(indexArea, $"{val.index}");
                    val.name = EditorGUI.DelayedTextField(labelArea, val.name);
                    if(val.index > 0 && Mathf.IsPowerOfTwo(val.index)) {
                        EditorGUI.LabelField(valueArea, $"1 << {val.index.GetFlagIndex()}");
                    }
                }
            }

            #endregion

            #region Util

            private static string GetName(T value) {
                var type = value.GetType();
                var memInfo = type.GetMember(value.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(InspectorNameAttribute), false);
                return (attributes.Length > 0) ? ((InspectorNameAttribute)attributes[0]).displayName : value.ToString();
            }

            #endregion
        }

        public class EnumValue {
            public string name = "";
            public int index = 0;
            public bool isDefault;

            public EnumValue() { }
            public EnumValue(string name, int index) {
                this.name = name;
                this.index = index;
            }

            public string ValueName {
                get {
                    return name.Split('/', '\\').LastOrDefault();
                }
            }
            public string VerifiedName => GenUtility.VerifyName(name);
        }

        #endregion

        #region Variables

        private static MaskDrawerInstance<PerkType> perkTypes;
        private static MaskDrawerInstance<PerkCategory> perkCategories;
        private static bool isInitialized;

        #endregion

        #region Loading

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Perks", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        static void Load() {
            if(isInitialized)
                return;
            isInitialized = true;
            perkTypes = new MaskDrawerInstance<PerkType>("Perk Types", 10);
            perkCategories = new MaskDrawerInstance<PerkCategory>("Perk Categories", 15);
        }

        #endregion

        #region Drawing

        private static void OnGUI(string obj) {
            Load();
            perkTypes.Draw();
            perkCategories.Draw();
            /*using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Save", GUILayout.Width(80))) {
                    Save(null);
                }
            }*/
        }

        #endregion

        #region Saving

        public static void Save(IReadOnlyList<EnumValue> values) {
            if(values == null)
                return;
            var copy = values
                .Where(x => !string.IsNullOrEmpty(x.name))
                .Insert(0, new EnumValue("None", 0))
                .Unique(x => x.VerifiedName);

            var file = new CodeFile("Perk");
            file.SetCreatorTag(typeof(CustomPerkEditor));
            file.UseCleanProcess = true;
            file.AddUsing("System");
            file.AddUsing("UnityEngine");

            var ns = file.AddNamespace("Toolkit.Unit");
            ns.AddEnum(new CodeEnum(AccessModifier.Public, "Perk", copy.Select(x => new CodeVariable(AccessModifier.Public, "Perk", x.name, x.index.ToString())).ToArray()));

            var intPerk = ns.Add(new CodeInterface("IPerk"));
            intPerk.AddProperty(new CodeProperty(AccessModifier.Public, "string", "Name", CodeBlock.Empty()));
            intPerk.AddProperty(new CodeProperty(AccessModifier.Public, "string", "Description", CodeBlock.Empty()));
            intPerk.AddProperty(new CodeProperty(AccessModifier.Public, "Perk", "Type", CodeBlock.Empty()));
            intPerk.AddProperty(new CodeProperty(AccessModifier.Public, "Texture2D", "Icon", CodeBlock.Empty()));
            intPerk.AddMethod(new CodeMethod("OnActivate", new CodeVariable("IUnit", "unit"), CodeBlock.Empty()));
            intPerk.AddMethod(new CodeMethod("OnDeactivate", new CodeVariable("IUnit", "unit"), CodeBlock.Empty()));

            GenerateUtilityClass(file, copy.ToArray());

            // var path = file.CreateFile("Assets/Toolkit/Unit");
            // AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void GenerateUtilityClass(CodeFile file, IReadOnlyList<EnumValue> values) {
            var ns = file.GetContainers<CodeNamespace>()[0];
            var c = ns.AddClass(AccessModifier.PublicStatic, "PerkUtility");

            var cachedcases = values.Select(x => $"Perk.{x.VerifiedName}").ToArray();

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable("Perk", "perk"),
                CodeBlock.CreateReturnSwitchBlock(
                    "perk",
                    cachedcases,
                    values.Select(x => $"\"{x.name}\"").ToArray(),
                    "return \"\";")));

        }

        #endregion
    }
}
