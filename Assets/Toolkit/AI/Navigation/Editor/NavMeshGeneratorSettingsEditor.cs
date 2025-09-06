using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AI;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    public static class NavMeshGeneratorSettingsEditor
    {
        public class RuleData
        {
            public string Name = "No Name";
            public float Radius = 0.5f;
            public float Height = 2f;
            public float StepHeight = 0.2f;
            public float MaxSlope = 45f;

            public RuleData() {
            }

            public RuleData(string name) : this() {
                Name = name;
            }

            public RuleData(NavMeshBuildSettings settings) : this() {
                Name = NavMeshBuildRules.GetSettingName(settings.agentTypeID);
                Radius = settings.agentRadius;
                Height = settings.agentHeight;
                StepHeight = settings.agentClimb;
                MaxSlope = settings.agentSlope;
            }

            public bool IsEqual(RuleData other) {
                return
                    other.Name == Name &&
                    other.Radius == Radius &&
                    other.Height == Height &&
                    other.StepHeight == StepHeight &&
                    other.MaxSlope == MaxSlope;
            }
        }

        private static bool instanced = false;
        private static int tileSize = 32;
        private static float voxelsPerRadius = 3f;

        private static List<RuleData> ruleData = new List<RuleData>();
        private static UnityEditorInternal.ReorderableList ruleDataList;

        public static IReadOnlyList<RuleData> Rules => ruleData;
        public static IReadOnlyList<RuleData> UnchangedRules => NavMeshBuildRules.AllRules.Select(x => new RuleData(x)).ToArray();
        public static IReadOnlyList<string> RuleTypes => ruleData.Select(x => x.Name).ToArray();


        private static GUIContent instancedContent = new GUIContent("Instanced", "If enabled, each 'NavMeshSource' will attempt to get parent generator and make a local nav mesh.");
        private static GUIContent tileSizeContent = new GUIContent("Tile Size", "The NavMesh is built in square tiles in order to build the mesh in parallel and to control maximum memory usage. It also helps to make the carving changes more local. If you plan to update NavMesh at runtime, a good tile size is around 32–128 voxels (roughly 5 to 20 meters for human size characters).");
        private static GUIContent voxelPerRadiusContent = new GUIContent("Voxels Per Radius", "The NavMesh is built by first voxelizing the Scene, and then figuring out walkable spaces from the voxelized representation of the Scene. The voxel size controls how closely the NavMesh fits the geometry of your Scene, and is defined in world units.");

        private static GUIContent smartSaveContent = new GUIContent("Smart Save", "This will verify all navigation settings and generate code for all changed.");
        private static GUIContent forceSaveContent = new GUIContent("Force Save", "This will generate new code even if there is no changes to it.");

        public static bool Instanced => instanced;
        public static int TileSize => tileSize;
        public static float VoxelsPerRadius => voxelsPerRadius;

        [InitializeOnLoadMethod]
        private static void Load() {
            instanced = NavMeshBuildRules.INSTANCED;
            tileSize = NavMeshBuildRules.TILE_SIZE;
            voxelsPerRadius = NavMeshBuildRules.VOXEL_SIZE_PER_AGENT_RADIUS;

            ruleData.Clear();
            for(int i = 0; i < NavMeshBuildRules.RULES; i++) {
                ruleData.Add(new RuleData(NavMeshBuildRules.GetSetting(i)));
            }
            if(ruleDataList == null) {
                ruleDataList = new UnityEditorInternal.ReorderableList(ruleData, typeof(RuleData), false, false, true, true);
                ruleDataList.drawElementCallback = DrawRuleListElement;
                ruleDataList.drawHeaderCallback = DrawRuleListHeader;
                ruleDataList.onRemoveCallback = RemoveRule;
                ruleDataList.onAddCallback = AddRule;
                ruleDataList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                ruleDataList.index = 0;
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/AI/Navigation", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string obj) {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Nav Mesh Generation Settings", EditorStyles.boldLabel);
                instanced = EditorGUILayout.Toggle(instancedContent, instanced);
                using(new EditorGUILayout.HorizontalScope()) {
                    int maskIndex = tileSize.GetFlagIndex();
                    tileSize = 1 << EditorGUILayout.IntSlider(tileSizeContent, maskIndex, 1, 8);
                    EditorGUILayout.LabelField(tileSize.ToString(), EditorStyles.boldLabel, GUILayout.Width(40f));
                }
                voxelsPerRadius = Mathf.Max(0.001f, EditorGUILayout.FloatField(voxelPerRadiusContent, voxelsPerRadius));

                EditorGUILayout.Space(8f);
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button(smartSaveContent, GUILayout.Width(80f))) {
                        NavMeshCodeGenerator.Generate();
                    }
                    if(GUILayout.Button(forceSaveContent, GUILayout.Width(80f))) {
                        NavMeshCodeGenerator.GenerateBuildRules(true);
                    }
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("Reload", GUILayout.Width(80f))) {
                        Load();
                    }
                    if(GUILayout.Button("Reset", GUILayout.Width(80f))) {
                        instanced = false;
                        tileSize = 32;
                        voxelsPerRadius = 3f;
                        ruleData.Clear();
                        ruleData.Add(new RuleData("Default"));
                    }
                }
            }
            EditorGUILayout.Space(12f);
            using(new EditorGUILayout.VerticalScope("box")) {
                ruleDataList.DoLayoutList();
                var line = GUILayoutUtility.GetRect(0f, 12f);
                line.height = 2f;
                line.y += 5f;
                EditorGUI.DrawRect(line, Color.black);
                DrawRule();
            }
        }



        #region Rule Data List

        private static void AddRule(ReorderableList list) {
            ruleData.Add(new RuleData("New Rule"));
        }

        private static void RemoveRule(ReorderableList a) {
            if(a.index > 0) {
                ruleData.RemoveAt(a.index);
            }
        }

        private static void DrawRuleListElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.height -= 2;

            bool isDefault = index == 0;
            using(new EditorGUI.DisabledScope(isDefault)) {
                GUI.Label(rect, ruleData[index].Name);
            }
        }

        private static void DrawRuleListHeader(Rect rect) {
            GUI.Label(rect, "Generation Rules", EditorStyles.boldLabel);
        }

        #endregion

        #region Draw Rule Settings

        private static void DrawRule() {
            if(ruleDataList.index < 0 || ruleDataList.index >= ruleData.Count) {
                return;
            }
            RuleData agent = ruleData[ruleDataList.index];
            bool isDefault = ruleDataList.index == 0;
            EditorGUILayout.BeginVertical("box");
            if(isDefault) {
                agent.Name = "Default";
                using(new EditorGUI.DisabledScope(isDefault)) {
                    EditorGUILayout.TextField("Name", agent.Name);
                }
            }
            else {
                agent.Name = EditorGUILayout.TextField("Name", agent.Name);
            }
            var copy = EditorGUILayout.ObjectField("Copy (Approximation)", null, typeof(UnityEngine.GameObject), true);
            if(copy != null && copy is GameObject go) {
                var c = go.GetComponent<Collider>();
                var radius = c.bounds.extents.To_XZ().magnitude * 0.707106781f;
                agent.Radius = radius;
                agent.Height = c.bounds.size.y;
                agent.StepHeight = agent.Height * 0.2f;
            }

            Rect agentDiagramRect = EditorGUILayout.GetControlRect(false, 120f);
            NavMeshEditorHelpers.DrawAgentDiagram(agentDiagramRect, agent.Radius, agent.Height, agent.StepHeight, agent.MaxSlope);

            agent.Radius = Mathf.Max(0.05f, EditorGUILayout.FloatField("Radius", agent.Radius));
            agent.Height = Mathf.Max(0.001f, EditorGUILayout.FloatField("Height", agent.Height));
            agent.StepHeight = EditorGUILayout.Slider("Step Height", agent.StepHeight, 0f, agent.Height);
            agent.MaxSlope = EditorGUILayout.Slider("Max Slope", agent.MaxSlope, 0f, 60f);

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
