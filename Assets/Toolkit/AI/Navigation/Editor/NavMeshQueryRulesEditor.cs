using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    public static class NavMeshQueryRulesEditor
    {
        public class QueryRule
        {
            public string Name = "No Name";
            public int AgentTypeId = 0;
            public int AreaMask = -1;

            public float[] AreaCostMultiplier = new float[32];

            public QueryRule() {

            }

            public QueryRule(string name) : this() {
                Name = name;
                for(int i = 0; i < 32; i++) {
                    AreaCostMultiplier[i] = 1f;
                }
            }

            public QueryRule(string name, NavMeshQueryFilter queryFilter) : this() {
                this.Name = name;
                this.AgentTypeId = queryFilter.agentTypeID;
                this.AreaMask = queryFilter.areaMask;
                for(int i = 0; i < 32; i++) {
                    AreaCostMultiplier[i] = queryFilter.GetAreaCost(i);
                }
            }

            public bool IsEqual(QueryRule other) {
                if(Name != other.Name || AgentTypeId != other.AgentTypeId || AreaMask != other.AreaMask)
                    return false;
                for(int i = 0; i < 32; i++) {
                    if(AreaCostMultiplier[i] != other.AreaCostMultiplier[i])
                        return false;
                }

                return true;
            }
        }

        private static List<QueryRule> queryRules = new List<QueryRule>();
        private static ReorderableList queryRulesList;

        private static GUIContent smartSaveContent = new GUIContent("Smart Save", "This will verify all navigation settings and generate code for all changed.");
        private static GUIContent forceSaveContent = new GUIContent("Force Save", "This will generate new code even if there is no changes to it.");

        private static IReadOnlyList<string> names;

        public static IReadOnlyList<QueryRule> Rules => queryRules;
        public static IReadOnlyList<QueryRule> UnchangedRules {
            get {
                var temp = NavMeshQueryFilterRules.AllQueryFilters;
                QueryRule[] r = new QueryRule[temp.Count];
                for(int i = 0; i < temp.Count; i++) {
                    r[i] = new QueryRule(NavMeshQueryFilterRules.GetQueryFilterName(i), temp[i]);
                }
                return r;
            }
        }

        [InitializeOnLoadMethod]
        private static void Load() {
            queryRules.Clear();

            for(int i = 0; i < NavMeshQueryFilterRules.RULES; i++) {
                queryRules.Add(new QueryRule(NavMeshQueryFilterRules.GetQueryFilterName(i), NavMeshQueryFilterRules.GetQueryFilter(i)));
            }

            if(queryRulesList == null) {
                queryRulesList = new ReorderableList(queryRules, typeof(QueryRule), true, true, true, true);
                queryRulesList.drawElementCallback = DrawRuleListElement;
                queryRulesList.drawHeaderCallback = DrawRuleListHeader;
                queryRulesList.onRemoveCallback = RemoveRule;
                queryRulesList.onAddCallback = AddRule;
                queryRulesList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                queryRulesList.onReorderCallbackWithDetails = ReorderRule;
                queryRulesList.index = 0;
            }
        }

        #region Query Rules List

        private static void ReorderRule(ReorderableList list, int oldIndex, int newIndex) {
            if(oldIndex == 0 || newIndex == 0) {
                return;
            }
            var item = queryRules[oldIndex];
            queryRules[oldIndex] = queryRules[newIndex];
            queryRules[newIndex] = item;
        }

        private static void DrawRuleListElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect generationRuleIndexArea, 30f / rect.width, 1f - 120f / rect.width, 2f);
            using(new EditorGUI.DisabledScope(index == 0)) {
                EditorGUI.LabelField(indexArea, index.ToString());
                EditorGUI.LabelField(nameArea, queryRules[index].Name);
                var nameIndex = (names.Count > index && index >= 0) ? queryRules[index].AgentTypeId : 0;
                EditorGUI.LabelField(generationRuleIndexArea, names[nameIndex], EditorStyles.boldLabel);
            }
        }

        private static void DrawRuleListHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect indexArea, out Rect nameArea, out Rect generationRuleIndexArea, 30f / rect.width, 1f - 120f / rect.width, 2f);
            EditorGUI.LabelField(indexArea, "#");
            EditorGUI.LabelField(nameArea, "Query Rule Name");
            EditorGUI.LabelField(generationRuleIndexArea, "Generation");
        }

        private static void RemoveRule(ReorderableList list) {
            if(list.index <= 0)
                return;
            queryRules.RemoveAt(list.index);
        }

        private static void AddRule(ReorderableList list) {
            queryRules.Add(new QueryRule("New Rule"));
        }

        #endregion

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/AI/Navigation/Query Rules", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string obj) {
            names = NavMeshGeneratorSettingsEditor.RuleTypes;
            queryRulesList.DoLayoutList();

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button(smartSaveContent, GUILayout.Width(80f))) {
                    NavMeshCodeGenerator.Generate();
                }
                if(GUILayout.Button(forceSaveContent, GUILayout.Width(80f))) {
                    NavMeshCodeGenerator.GenerateQueryFilterRules(true);
                }
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Reload", GUILayout.Width(80f))) {
                    Load();
                }
                if(GUILayout.Button("Reset", GUILayout.Width(80f))) {
                    queryRules.Clear();
                    queryRules.Add(new QueryRule("Default"));
                }
            }
            EditorGUILayout.Space(12f);
            RenderRule(queryRulesList.index);
        }

        private static void RenderRule(int index) {
            if(index < 0 || index >= queryRules.Count)
                return;
            var rule = queryRules[index];
            var isDefault = index == 0;
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUI.DisabledScope(isDefault)) {
                    rule.Name = EditorGUILayout.TextField("Name", rule.Name);
                }
                rule.AgentTypeId = EditorGUILayout.Popup("Generation", rule.AgentTypeId, names.ToArray());
                rule.AreaMask = EditorGUILayout.MaskField("Area Mask", rule.AreaMask, NavMesh.GetAreaNames());
                EditorGUILayout.Space(4);
                NavigationAreaCostEditor.DrawCustomLayout(rule.AreaCostMultiplier);
                EditorGUILayout.HelpBox("Note: The area cost multiplier fields is what being saved and not the individual cost.", MessageType.Warning);
            }
        }
    }
}
