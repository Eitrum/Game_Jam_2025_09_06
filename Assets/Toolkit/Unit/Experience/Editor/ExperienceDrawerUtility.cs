using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit
{
    public static class ExperienceDrawerUtility
    {
        public static float GetExperienceDrawerHeight(bool includeLabel) => EditorGUIUtility.singleLineHeight * (includeLabel ? 5 : 4);

        #region Drawing

        public static void DrawLayout(IExperience exp) {
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField("Experience", GUILayout.Width(90));
                    var area = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                    DrawExperienceBar(area, exp.LevelingPercentage);
                }
                using(new EditorGUI.IndentLevelScope()) {
                    DrawCorrectField("Current Experience", exp, ApplyExp, RetrieveExp);
                    DrawCorrectField("Total Experience", exp, ApplyTotalExp, RetrieveTotalExp);
                }
            }
            using(new EditorGUILayout.HorizontalScope("box")) {
                var newLevel = ExperienceUtility.ClampLevel(exp.Table, EditorGUILayout.DelayedIntField("Level", exp.Level));
                if(exp.Level != newLevel) {
                    exp.Level = newLevel;
                }
            }
        }

        public static void Draw(Rect position, IExperience exp) {
            GUI.Box(position, "");
            position.SplitVertical(out Rect topArea, out Rect botArea, 0.5f);
            topArea.SplitVertical(out Rect experienceArea, out Rect currentExperienceArea, 0.5f);
            botArea.SplitVertical(out Rect totalExperienceArea, out Rect levelArea, 0.5f);

            experienceArea.SplitHorizontal(out Rect labelArea, out Rect experienceBarArea, 90f / experienceArea.width);
            EditorGUI.LabelField(labelArea, "Experience");
            DrawExperienceBar(experienceBarArea, exp.LevelingPercentage);

            using(new EditorGUI.IndentLevelScope()) {
                DrawCorrectField(currentExperienceArea, "Current Experience", exp, ApplyExp, RetrieveExp);
                DrawCorrectField(totalExperienceArea, "Total Experience", exp, ApplyTotalExp, RetrieveTotalExp);
            }

            var newLevel = ExperienceUtility.ClampLevel(exp.Table, EditorGUI.DelayedIntField(levelArea, "Level", exp.Level));
            if(exp.Level != newLevel) {
                exp.Level = newLevel;
            }
        }

        public static void Draw(Rect position, GUIContent label, IExperience exp) {
            GUI.Box(position, "");
            position.SplitVertical(out Rect topArea, out Rect midArea, out Rect levelArea, 0.4f, 0.4f, 0f);
            topArea.SplitVertical(out Rect headerArea, out Rect experienceArea, 0.5f);
            midArea.SplitVertical(out Rect currentExperienceArea, out Rect totalExperienceArea, 0.5f);

            EditorGUI.LabelField(headerArea, label, EditorStyles.boldLabel);
            experienceArea.SplitHorizontal(out Rect labelArea, out Rect experienceBarArea, 90f / experienceArea.width);
            EditorGUI.LabelField(labelArea, "Experience");
            DrawExperienceBar(experienceBarArea, exp.LevelingPercentage);

            using(new EditorGUI.IndentLevelScope()) {
                DrawCorrectField(currentExperienceArea, "Current Experience", exp, ApplyExp, RetrieveExp);
                DrawCorrectField(totalExperienceArea, "Total Experience", exp, ApplyTotalExp, RetrieveTotalExp);
            }

            var newLevel = ExperienceUtility.ClampLevel(exp.Table, EditorGUI.DelayedIntField(levelArea, "Level", exp.Level));
            if(exp.Level != newLevel) {
                exp.Level = newLevel;
            }
        }

        public static void DrawExperienceBar(Rect area, IExperience exp) => DrawExperienceBar(area, exp.LevelingPercentage);

        public static void DrawExperienceBar(Rect area, float percentage) {
            EditorGUI.DrawRect(area, Color.gray);
            area.ShrinkRef(2f);
            EditorGUI.DrawRect(area, 0.2f.ToColor());
            var label = area;
            area.width *= percentage;
            EditorGUI.DrawRect(area, new Color(0.2f, 0.8f, 0.2f, 1f));
            EditorGUI.LabelField(label, $"{Mathf.RoundToInt(percentage * 100f)}%", EditorStyles.centeredGreyMiniLabel);
        }

        #endregion

        #region Private Draw utility

        private static void DrawCorrectField(Rect area, string label, IExperience exp, Action<IExperience, object> assign, Func<IExperience, object> retrieve) {
            assign(exp, EditorGUI.DoubleField(area, label, (double)retrieve(exp)));
        }

        private static void DrawCorrectField(string label, IExperience exp, Action<IExperience, object> assign, Func<IExperience, object> retrieve) {
            assign(exp, EditorGUILayout.DoubleField(label, (double)retrieve(exp)));
        }

        private static void ApplyExp(IExperience exp, object value) {
            typeof(IExperience).GetProperty("Experience").SetValue(exp, value);
        }

        private static object RetrieveExp(IExperience exp) {
            return typeof(IExperience).GetProperty("Experience").GetValue(exp);
        }

        private static void ApplyTotalExp(IExperience exp, object value) {
            typeof(IExperience).GetProperty("TotalExperience").SetValue(exp, value);
        }

        private static object RetrieveTotalExp(IExperience exp) {
            return typeof(IExperience).GetProperty("TotalExperience").GetValue(exp);
        }

        #endregion
    }
}
