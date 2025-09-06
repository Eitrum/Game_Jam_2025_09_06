using System.Collections;
using System.Collections.Generic;
using Toolkit.IO.TML.Properties;
using UnityEditor;
using UnityEngine;

namespace Toolkit.IO.TML {
    public static class TMLNodeDrawer {

        #region Variables

        private const int MAX_CHILDREN_NODES = 10;
        private static Dictionary<TMLNode, bool> foldouts = new Dictionary<TMLNode, bool>();
        private static Dictionary<TMLNode, int> drawIndex = new Dictionary<TMLNode, int>();

        #endregion

        #region Unload

        public static void Unload() {
            foldouts.Clear();
            drawIndex.Clear();
        }

        #endregion

        #region Draw

        public static void DrawNodeLayout(TMLNode node, bool includeChildren) {
            using(new EditorGUI.IndentLevelScope(1)) {
                foldouts.TryGetValue(node, out var foldout);
                EditorGUI.BeginChangeCheck();
                var foldoutres = EditorGUILayout.Foldout(foldout, node.Name, true);
                if(EditorGUI.EndChangeCheck()) {
                    foldouts[node] = foldoutres;
                }
                if(foldout) {
                    using(new EditorGUI.IndentLevelScope(1)) {
                        foreach(var p in node.Properties)
                            DrawPropertyLayout(p);
                    }

                    var children = node.Children;
                    int start = 0;
                    int end = children.Count;
                    if(children.Count > 16) {
                        drawIndex.TryGetValue(node, out start);
                        end = Mathf.Min(children.Count, start + 10);
                        using(new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField($"{start} ...", GUILayout.Width(120));
                            if(start > 0) {
                                if(GUILayout.Button("previous", GUILayout.Width(80))) {
                                    drawIndex[node] = Mathf.Max(0, (start - 10));
                                }
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                    if(includeChildren) {
                        for(int i = start; i < end; i++)
                            DrawNodeLayout(children[i], includeChildren);
                    }
                    else {
                        for(int i = start; i < end; i++)
                            EditorGUILayout.LabelField($"->{children[i].Name}");
                    }
                    if(end < children.Count) {
                        using(new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField($"... {children.Count}", GUILayout.Width(120));
                            if(start + 10 < children.Count) {
                                if(GUILayout.Button("next", GUILayout.Width(80))) {
                                    drawIndex[node] = Mathf.Min(children.Count - 10, (start + 10));
                                }
                            }
                            EditorGUI.BeginChangeCheck();
                            var newStart = EditorGUILayout.DelayedIntField(start, GUILayout.Width(140));
                            var floatArea = GUILayoutUtility.GetLastRect();
                            EditorGUI.LabelField(floatArea, "jump to", EditorStylesUtility.RightAlignedGrayMiniLabel);
                            if(EditorGUI.EndChangeCheck()) {
                                drawIndex[node] = Mathf.Clamp(newStart, 0, children.Count - 10);
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }
        }

        public static void DrawPropertyLayout(ITMLProperty property) {
            if(property is ITMLProperty_Xml xml) {
                EditorGUILayout.LabelField(xml.GetFormattedXml());
            }
            else {
                EditorGUILayout.LabelField("unable to read propery");
            }
        }

        #endregion
    }
}
