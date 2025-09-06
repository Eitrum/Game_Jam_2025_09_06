using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit {
    public static class MenuItemsBeGone {

        private class Styles {
            public static readonly GUIStyle FoldoutBold = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            public static readonly Color StripeColor = new Color(1, 1, 1, 0.02f);
        }

        private class Removed {
            public List<string> removedMenuItems = new List<string>();
        }

        private const string FILE_PATH = "UserSettings/MenuItemsBeGone";
        private static Removed removed = new Removed();
        private static HashSet<string> hashed = new HashSet<string>();
        private static Vector2 scroll = new Vector2();
        private static bool isDirty;
        private static int line = 0;

        private static Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        private class Node {
            public string Name;
            public string Path;
            public bool Enabled = true;
            public bool Foldout = false;
            public List<Node> Children = new List<Node>();

            public Node(string name) {
                this.Name = name;
                this.Path = name;
            }

            private Node(string name, string previousPath) {
                this.Name = name;
                this.Path = $"{previousPath}/{name}";
            }

            public Node AddRange(IReadOnlyList<string> submenus) {
                foreach(var s in submenus) {
                    Add(s.Split("/"), 1);
                }
                return this;
            }

            private void Add(string[] split, int index) {
                if(index >= split.Length)
                    return;

                foreach(var c in Children) {
                    if(c.Name.Equals(split[index])) {
                        c.Add(split, index + 1);
                        return;
                    }
                }

                var cnode = new Node(split[index], Path);
                Children.Add(cnode);
                cnode.Add(split, index + 1);
            }

            public void Disable(HashSet<string> disabledHashes) {
                if(disabledHashes.Contains(Path))
                    Enabled = false;
                foreach(var c in Children)
                    c.Disable(disabledHashes);
            }

            public void RemoveDisabled(bool removeAll) {
                removeAll |= !Enabled;
                foreach(var c in Children)
                    c.RemoveDisabled(removeAll);

                if(removeAll) {
                    // Debug.Log("Removing: " + Path);
                    Menu.RemoveMenuItem(Path);
                }
            }
        }

        #region Init

        [InitializeOnLoadMethod]
        private static void Init() {
            EditorApplication.delayCall += DelayedInit;

            // Causes Errors...
            // RecompileUtility.OnRecompile += Menu.RebuildAllMenus;
        }

        private static void DelayedInit() {
            if(System.IO.File.Exists(FILE_PATH)) {
                var json = System.IO.File.ReadAllText(FILE_PATH);
                EditorJsonUtility.FromJsonOverwrite(json, removed);
                foreach(var r in removed.removedMenuItems)
                    hashed.Add(r);
            }
            LoadNodes();
        }

        private static void LoadNodes() {
            nodes.Clear();
            AddRoot("File");
            AddRoot("Edit");
            AddRoot("Assets");
            AddRoot("GameObject");
            AddRoot("Component");
            AddRoot("Services");
            AddRoot("Toolkit");
            AddRoot("Tools");
            AddRoot("Window");
            AddRoot("Help");

            Menu.AddMenuItem("Help/Restore Menu Items", Menu.RebuildAllMenus);
        }

        private static void Reapply() {
            foreach(var n in nodes) {
                n.Value.Disable(hashed);
                n.Value.RemoveDisabled(false);
            }
        }

        #endregion

        private static void AddRoot(string root) {
            Menu.ExtractSubmenus(root, out string[] submenus);
            // Debug.Log($"{root} Adding submenus: {submenus.Join(", ")}");
            var node =  new Node(root).AddRange(submenus);
            nodes.Add(root, node);
            node.Disable(hashed);
            node.RemoveDisabled(false);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Editor/Menu Items Be Gone", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string obj) {
            line = 0;
            using(var scrollScope = new EditorGUILayout.ScrollViewScope(scroll)) {
                using(new EditorGUILayout.HorizontalScope("box")) {
                    if(GUILayout.Button("Rebuild", GUILayout.Width(80))) {
                        Menu.RebuildAllMenus();
                    }
                    if(GUILayout.Button("Apply Changes", GUILayout.Width(100))) {
                        Reapply();
                    }
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("Reset", GUILayout.Width(80))) {
                        removed.removedMenuItems.Clear();
                        hashed.Clear();
                        Save();
                    }
                }
                scroll = scrollScope.scrollPosition;
                foreach(var node in nodes) {
                    DrawNode(node.Value);
                    EditorGUILayout.Space(4);
                }
            }
        }

        #region Save

        public static void Save() {
            if(isDirty)
                return;
            isDirty = true;
            EditorApplication.delayCall += Internal_Save;
        }

        private static void Internal_Save() {
            isDirty = false;
            try {
                var json = EditorJsonUtility.ToJson(removed, true);
                IO.IOUtility.File.TryWrite(FILE_PATH, json);
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion

        private static void DrawNode(Node node) {
            if(node.Children.Count == 0) {
                using(var area = new EditorGUILayout.HorizontalScope()) {
                    DrawStripes(area.rect);
                    EditorGUILayout.LabelField(node.Name);
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    node.Enabled = GUILayout.Toggle(node.Enabled, "");
                    GUILayout.Space(10);
                    if(EditorGUI.EndChangeCheck()) {
                        if(node.Enabled) {
                            hashed.Remove(node.Path);
                            removed.removedMenuItems.Remove(node.Path);
                        }
                        else {
                            hashed.Add(node.Path);
                            removed.removedMenuItems.Add(node.Path);
                        }
                        Save();
                    }
                }
            }
            else {
                using(var area = new EditorGUILayout.HorizontalScope()) {
                    DrawStripes(area.rect);
                    node.Foldout = EditorGUILayout.Foldout(node.Foldout, node.Name, true, Styles.FoldoutBold);
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    node.Enabled = GUILayout.Toggle(node.Enabled, "");
                    GUILayout.Space(10);
                    if(EditorGUI.EndChangeCheck()) {
                        if(node.Enabled) {
                            hashed.Remove(node.Path);
                            removed.removedMenuItems.Remove(node.Path);
                        }
                        else {
                            hashed.Add(node.Path);
                            removed.removedMenuItems.Add(node.Path);
                        }
                        Save();
                    }
                }
                if(node.Foldout) {
                    using(new EditorGUI.IndentLevelScope(2)) {
                        foreach(var c in node.Children) {
                            DrawNode(c);
                        }
                    }
                }
            }
        }

        public static void DrawStripes(Rect area) {
            if(line++ % 2 == 0)
                EditorGUI.DrawRect(area, Styles.StripeColor);
        }
    }
}
