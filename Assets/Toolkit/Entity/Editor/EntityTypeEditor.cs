using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Toolkit.CodeGenerator;
using System.Linq;

namespace Toolkit
{
    public static class EntityTypeEditor
    {
        [InitializeOnLoadMethod]
        private static void Load() {
            customEnumValues.Clear();
            var customType = typeof(EntityType);

            var names = System.Enum.GetNames(customType);
            var values = System.Enum.GetValues(customType);
            for(int i = 1, length = names.Length; i < length; i++) {
                customEnumValues.Add(EntityTypeUtility.GetName((EntityType)i));
            }

            if(reorderableList == null) {
                reorderableList = new UnityEditorInternal.ReorderableList(customEnumValues, typeof(string));
                reorderableList.drawElementCallback += DrawElement;
                reorderableList.drawHeaderCallback += DrawHeader;
                reorderableList.onAddCallback += (r) => customEnumValues.Add("");
            }
        }

        private static void DrawHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, 20f / rect.width);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(nameArea, "Name");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, 20f / rect.width);
            EditorGUI.LabelField(indexArea, index.ToString(), EditorStyles.boldLabel);
            customEnumValues[index] = EditorGUI.TextField(nameArea, customEnumValues[index]);
        }

        private static List<string> customEnumValues = new List<string>();
        private static UnityEditorInternal.ReorderableList reorderableList;

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Entity", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
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

        public static void Generate() {
            var types = customEnumValues.Insert<string>(0, "Uncategorized").Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var file = new CodeFile("EntityType");
            var ns = file.AddNamespace("Toolkit");
            var enu = ns.AddEnum(new CodeEnum("EntityType", types));

            var c = ns.AddClass(AccessModifier.PublicStatic, "EntityTypeUtility");
            c.AddMethod(
                new CodeMethod(
                AccessModifier.PublicStatic,
                "string",
                "GetName",
                new CodeVariable(enu.Name, "type"),
                CodeBlock.CreateReturnSwitchBlock("type", types.Select(x => $"{enu.Name}.{GenUtility.VerifyName(x)}").ToArray(), types.Select(x => $"\"{x}\"").ToArray(), "return \"\";")));

            var path = file.CreateFile("Assets/Toolkit/Utilities");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
