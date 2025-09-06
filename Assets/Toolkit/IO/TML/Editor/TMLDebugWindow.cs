using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit.IO.TML {
    public class TMLDebugWindow : EditorWindow {

        #region Variables

        private const string TAG = ColorTable.RichTextTags.CYAN + "[Toolkit.TMLDebugWindow] - </color>";
        private const string PATH = "Toolkit/Editor/TML Debug Window";

        private string selected = "";
        private string searchNodeText;
        private Vector2 nodeListScroll;
        private Vector2 currentNodeScroll;


        private TMLNode parentSearchNode = null;
        private string previousSearched = "";
        private TMLNode searchedNode = null;

        #endregion

        #region Open

        [MenuItem(PATH)]
        public static void OpenWindow() {
            var w = GetWindow<TMLDebugWindow>("TML Debug Window");
            w.Show();
        }

        private void OnDisable() {
            TMLNodeDrawer.Unload();
        }

        #endregion

        #region Draw

        private void OnGUI() {
            var size = position.size;
            var area = new Rect(Vector2.zero, size);
            using(new GUILayout.AreaScope(area)) {
                using(new EditorGUILayout.HorizontalScope()) {
                    using(new EditorGUILayout.VerticalScope(GUILayout.Width(200))) {
                        DrawRegisteredNodes();
                    }
                    var line = GUILayoutUtility.GetRect(2, 2, 2, 2000, GUILayout.Width(2));
                    EditorGUI.DrawRect(line, Color.gray);
                    using(new EditorGUILayout.VerticalScope()) {
                        DrawSelectedNode();
                    }
                }
            }
        }

        private void DrawSelectedNode() {
            if(TMLUtility.DebugNodes.TryGetValue(selected, out TMLNode node)) {
                using(var scrollscope = new EditorGUILayout.ScrollViewScope(currentNodeScroll)) {
                    using(new EditorGUILayout.HorizontalScope("box")) {
                        searchNodeText = EditorGUILayout.TextField(searchNodeText, GUILayout.Width(140));
                        if(string.IsNullOrEmpty(searchNodeText)) {
                            var searchFieldArea = GUILayoutUtility.GetLastRect();
                            EditorGUI.LabelField(searchFieldArea, $"search", EditorStylesUtility.CenterAlignedMiniLabel);
                        }
                        using(new EditorGUI.DisabledScope(string.IsNullOrEmpty(searchNodeText))) {
                            if(GUILayout.Button("Search", GUILayout.Width(80))) {
                                if(searchNodeText.Contains('\\') || searchNodeText.Contains('/')) {
                                    if(!TMLUtility.FindNodeByPath(node, searchNodeText, out searchedNode))
                                        Debug.LogWarning(TAG + $"Didn't find node by path '{searchNodeText}' in {selected}");
                                    previousSearched = $"{node.Name}/{searchNodeText}";
                                }
                                else {
                                    if(!TMLUtility.FindNodeByDeepSearch(node, searchNodeText, out previousSearched, out searchedNode))
                                        Debug.LogWarning(TAG + $"Didn't find node by deep search '{searchNodeText}' in {selected}");
                                }
                                parentSearchNode = node;
                            }
                        }
                        if(GUILayout.Button("Clear", GUILayout.Width(80))) {
                            searchedNode = null;
                            previousSearched = string.Empty;
                            parentSearchNode = null;
                        }
                    }
                    currentNodeScroll = scrollscope.scrollPosition;
                    if(searchedNode != null) {
                        if(node != parentSearchNode)
                            searchedNode = null;
                        EditorGUILayout.LabelField("Current Filter:", previousSearched);
                        TMLNodeDrawer.DrawNodeLayout(searchedNode, true);
                    }
                    else
                        TMLNodeDrawer.DrawNodeLayout(node, true);
                }
            }
        }

        private void DrawRegisteredNodes() {
            using(var scrollscope = new EditorGUILayout.ScrollViewScope(nodeListScroll)) {
                nodeListScroll = scrollscope.scrollPosition;
                foreach(var list in TMLUtility.DebugNodes) {
                    var isSelected = list.Key.Equals(selected);
                    using(var scope = new EditorGUILayout.HorizontalScope()) {
                        if(isSelected)
                            EditorGUI.DrawRect(scope.rect, Color.gray);
                        GUILayout.Label(list.Key);
                        if(GUILayout.Button("x", GUILayout.Width(24))) {
                            EditorApplication.delayCall += () => TMLUtility.DebugNodes.Remove(list.Key);
                        }
                        var ev = Event.current;
                        if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && scope.rect.Contains(ev.mousePosition)) {
                            selected = list.Key;
                            ev.Use();
                        }
                    }

                }
            }
        }

        #endregion
    }
}
