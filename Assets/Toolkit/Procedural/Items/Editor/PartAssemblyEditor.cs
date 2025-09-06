using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Toolkit.Procedural.Items
{
    [CustomEditor(typeof(PartAssembly))]
    public class PartAssemblyEditor : Editor
    {
        private SerializedProperty attachBlueprintItemOnRefProperty;
        [System.NonSerialized] private PartBlueprint blueprint;
        [System.NonSerialized] private PreviewRenderer renderer;
        private Vector2 scroll;

        static bool added = false;

        private void OnEnable() {
            attachBlueprintItemOnRefProperty = serializedObject.FindProperty("attachBlueprintRefOnItem");
        }

        private void OnDestroy() {
            if(renderer != null) {
                renderer.Dispose();
            }
        }

        public override void OnInspectorGUI() {
            var assembly = target as PartAssembly;
            var root = assembly.Root;
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("Assembly Process", EditorStyles.boldLabel);
            var state = DrawBranch(root);
            if(state == State.Deleted) {
                root.collection = null;
                root.branches.Clear();
            }
            if(added) {
                serializedObject.Update();
                added = false;
                blueprint = assembly.CreateBlueprint();
                Repaint();
            }
            serializedObject.Update();
            EditorGUILayout.HelpBox("Parts will generate from root object in an hierarchy:\nRoot\n-Part 1 (parent: root)\n--Part 2 (parent: Part 1)\n-Part 3 (parent: root)", MessageType.Info, true);
            EditorGUILayout.LabelField("Meta", EditorStyles.boldLabel);
            attachBlueprintItemOnRefProperty.boolValue = EditorGUILayout.ToggleLeft(attachBlueprintItemOnRefProperty.displayName, attachBlueprintItemOnRefProperty.boolValue);

            if(blueprint == null) {
                blueprint = assembly.CreateBlueprint();
            }
            EditorGUILayout.LabelField("Example", EditorStyles.boldLabel);
            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("Generate", GUILayout.Width(200f))) {
                    blueprint = assembly.CreateBlueprint();
                }
            }
            DrawObject(root);

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawObject(PartAssembly.Branch branch) {
            if(renderer == null) {
                renderer = new PreviewRenderer();
                renderer.Bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(4, 4, 4));
            }
            var area = GUILayoutUtility.GetRect(512, 512);
            renderer.BeginRender(area);
            RenderBranch(branch, blueprint, Part.None, Pose.identity);
            renderer.EndRender(area);
        }

        private void RenderBranch(PartAssembly.Branch branch, PartBlueprint blueprint, Part parent, Pose offset) {
            if(branch == null || branch.collection == null) {
                return;
            }
            if(blueprint == null) {
                return;
            }
            var entry = branch.collection.GetEntry(blueprint.Id);
            if(entry == null) {
                return;
            }
            if(parent == Part.None) {
                // Add root offset
                offset = new Pose(offset.position + entry.Offset.position, offset.rotation * entry.Offset.rotation);
            }
            else {
                // Remove anchor offset
                for(int i = 0, length = entry.Connections.Count; i < length; i++) {
                    if(entry.Connections[i].OtherPart == parent) {
                        var anchor = entry.Connections[i].Anchor;
                        offset = new Pose(offset.position - anchor.position, offset.rotation * Quaternion.Inverse(anchor.rotation));
                        break;
                    }
                }
            }

            var prefab = entry?.Prefab ?? null;
            if(prefab != null) {
                var mr = prefab.GetComponent<MeshRenderer>();
                var mf = prefab.GetComponent<MeshFilter>();
                if(mr == null || mf == null) {

                }
                else {
                    // Needed to remove errors... unsure why
                    if(offset.rotation.Equals(new Quaternion(0, 0, 0, 0), Mathf.Epsilon)) {
                        offset.rotation = Quaternion.identity;
                    }
                    renderer.RenderCustom(offset.ToMatrix(), mf.sharedMesh, mr.sharedMaterial);
                }
            }
            for(int i = 0, length = entry.Connections.Count; i < length; i++) {
                var con = entry.Connections[i];
                // For each connection, find a branch of matching part type
                for(int brIndex = 0, brLength = branch.BranchCount; brIndex < brLength; brIndex++) {
                    var br = branch.GetBranch(brIndex);
                    if(br == null || br.collection == null) {
                        continue;
                    }
                    if(con.OtherPart == br.Collection.Part) {
                        var anchor = con.Anchor;
                        RenderBranch(br, blueprint.GetBranch(brIndex), branch.collection.Part, new Pose(offset.position + anchor.position, offset.rotation * anchor.rotation));
                        break;
                    }
                }
            }
        }

        private static State DrawBranch(PartAssembly.Branch branch) {
            using(new EditorGUILayout.VerticalScope("box")) {
                using(new EditorGUILayout.HorizontalScope()) {
                    var newCollection = EditorGUILayout.ObjectField(branch.collection, typeof(PartCollection), false) as PartCollection;
                    if(branch.collection != newCollection) {
                        branch.collection = newCollection;
                        added = true;
                    }
                    if(GUILayout.Button("-", GUILayout.Width(24))) {
                        return State.Deleted;
                    }
                }
                using(new EditorGUI.IndentLevelScope(1)) {
                    var branches = branch.branches;
                    for(int i = 0; i < branches.Count; i++) {
                        var br = branches[i];
                        var state = DrawBranch(br);
                        switch(state) {
                            case State.Deleted:
                                branches.RemoveAt(i);
                                i--;
                                continue;
                        }
                    }
                    using(new EditorGUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("+", GUILayout.Width(24))) {
                            branches.Add(new PartAssembly.Branch());
                        }
                    }
                }
            }
            return State.None;
        }

        public enum State
        {
            None,
            Deleted,
            Added,
        }

    }
}
