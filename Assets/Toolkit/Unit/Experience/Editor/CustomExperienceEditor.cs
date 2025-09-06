using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.CodeGenerator;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Unit
{
    public static class CustomExperienceEditor
    {
        private static int maximumLevel = 99;
        private static List<double> experiencePerLevel = new List<double>();
        private static double totalExperience = 0d;
        private static UnityEditorInternal.ReorderableList reorderableList = new UnityEditorInternal.ReorderableList(experiencePerLevel, typeof(double));


        [InitializeOnLoadMethod]
        static void Load() {
            experiencePerLevel.Clear();
            experiencePerLevel.AddRange(ExperienceUtility.DifferenceTable.Select(x => (double)x));
            maximumLevel = ExperienceUtility.Levels;

            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
        }

        private static void DrawHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect level, out Rect experienceArea, out Rect totalExperienceArea, 0.1f, 0.5f, 2f);
            EditorGUI.LabelField(level, "Level");
            EditorGUI.LabelField(experienceArea, "Experience");
            EditorGUI.LabelField(totalExperienceArea, "Total Experience");
        }

        private static void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect level, out Rect experienceArea, out Rect totalExperienceArea, 0.1f, 0.5f, 2f);
            EditorGUI.LabelField(level, (index).ToString());
            if(index == 0)
                totalExperience = 0;
            if(index <= 1) {
                EditorGUI.LabelField(experienceArea, experiencePerLevel[index].ToString());
                totalExperience += experiencePerLevel[index];
            }
            else {
                experiencePerLevel[index] = EditorGUI.DoubleField(experienceArea, experiencePerLevel[index]);
                totalExperience += experiencePerLevel[index];
            }
            EditorGUI.LabelField(totalExperienceArea, totalExperience.ToString(), EditorStyles.centeredGreyMiniLabel);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Unit/Experience", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string s) {
            using(new EditorGUI.DisabledScope(true))
                EditorGUILayout.IntField("Max Level", maximumLevel);

            int totalLevels = maximumLevel;

            GUILayout.Space(12f);
            EditorGUILayout.BeginHorizontal();
            var csvInput = EditorGUILayout.DelayedTextField("CSV Input", "");
            if(!string.IsNullOrEmpty(csvInput)) {
                if(csvInput.Contains(",")) {
                    var fields = csvInput.Replace(" ", "").Split(',').Select(x => double.TryParse(x, out double output) ? output : 0);
                    experiencePerLevel.Clear();
                    experiencePerLevel.AddRange(fields);
                }
                else {
                    // Check if using space seperated
                    var fields = csvInput.Split(' ').Select(x => double.TryParse(x, out double output) ? output : 0);
                    experiencePerLevel.Clear();
                    experiencePerLevel.AddRange(fields);
                }
            }

            if(GUILayout.Button("Copy CSV", GUILayout.Width(80f))) {
                EditorGUIUtility.systemCopyBuffer = string.Join(",", experiencePerLevel);
            }

            EditorGUILayout.EndHorizontal();

            reorderableList.DoLayoutList();

            var save = GUILayout.Button("Save", GUILayout.Width(80f));
            if(save) {
                GenerateDefaultTable();
            }
        }

        private static void GenerateDefaultTable() {
            // Setup file
            var file = new CodeFile("ExperienceUtilityDefaultTable");
            file.SetCreatorTag(typeof(CustomExperienceEditor));
            file.AddUsing("System.Collections.Generic");
            file.AddUsing("UnityEngine");
            var ns = file.AddNamespace("Toolkit.Unit");

            var util = ns.AddClass(new CodeClass("ExperienceUtility", AccessModifier.PublicStatic | AccessModifier.Partial));
            util.AddVariable(new CodeVariable(
                AccessModifier.PrivateReadonly | AccessModifier.Static,
                $"double[]",
                "experienceTable",
                $"{{{string.Join(",", experiencePerLevel)}}}\n"));

            var path = file.CreateFile("Assets/Toolkit/Unit/Experience");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
