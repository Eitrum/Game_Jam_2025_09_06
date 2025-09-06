using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    public class CustomPartEditor : Editor
    {
        public class PartContainer
        {
            public string name = "";
            public int index = 0;

            public PartContainer() { }
            public PartContainer(string name) => this.name = name;
            public PartContainer(string name, int index) {
                this.name = name;
                this.index = index;
            }
        }

        private static List<PartContainer> categories = new List<PartContainer>();
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(categories, typeof(PartContainer));
        private static int highestIndex = 0;

        #region Init

        static CustomPartEditor() {
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onAddCallback += OnAdd;

            categories.Clear();

            var parts = Part.None.GetArray();
            for(int i = 1, length = parts.Length; i < length; i++) {
                var path = PartUtility.GetPath(parts[i]);
                if(string.IsNullOrEmpty(path)) {
                    continue;
                }
                categories.Add(new PartContainer(path, parts[i].ToInt()));
            }
            foreach(var ca in categories)
                highestIndex = Mathf.Max(ca.index, highestIndex);
        }

        private static void OnAdd(ReorderableList list) {
            categories.Add(new PartContainer("new part", ++highestIndex));
        }

        #endregion

        #region Drawing

        private static void DrawHeader(Rect rect) {
            rect.PadRef(12, 0, 0, 0);
            rect.SplitHorizontal(out Rect indexArea, out Rect label, out Rect idArea, 30f / rect.width, (rect.width - 100) / rect.width, 2f);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(label, "Path");
            EditorGUI.LabelField(idArea, "Index");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect label, out Rect idArea, 30f / rect.width, (rect.width - 100) / rect.width, 2f);
            EditorGUI.LabelField(indexArea, index.ToString());
            categories[index].name = EditorGUI.TextField(label, categories[index].name);
            EditorGUI.LabelField(idArea, categories[index].index.ToString());
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Procedural/Items/Parts", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            reorderableList.DoLayoutList();

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Save", GUILayout.Width(80f))) {
                    Save(categories);
                }

                if(GUILayout.Button(new GUIContent("Reset", "This will remove all the parts."), GUILayout.Width(80f))) {
                    categories.Clear();
                    highestIndex = 0;
                }

            }
        }

        #endregion

        #region Save

        public static void Save(IReadOnlyList<PartContainer> enumValues) {
            var enuVal = enumValues
                    .Insert(0, new PartContainer("None", 0))
                    .Select(x => new CodeGenerator.GenUtility.EnumValues(x.name.Replace("/", "__"), x.index))
                    .ToArray();


            var enumFile = CodeGenerator.GenUtility.CreateEnumClass(
                "Part",
                "Toolkit.Procedural.Items",
                enuVal);

            var ns = enumFile.GetContainers<CodeNamespace>()[0];
            var c = ns.AddClass(new CodeClass(AccessModifier.PublicStatic, "PartUtility"));

            c.AddMethod(new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetPath",
                new CodeVariable("Part", "part"),
                CodeBlock.CreateReturnSwitchBlock(
                    "part",
                    enumValues.Select(x => $"Part.{GenUtility.VerifyName(x.name.Replace("/", "__"))}").ToArray(),
                    enumValues.Select(x => $"\"{x.name}\"").ToArray(),
                    "return \"None\";")));

            var path = enumFile.CreateFile("Assets/Toolkit/Procedural/Items");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        #endregion
    }
}
