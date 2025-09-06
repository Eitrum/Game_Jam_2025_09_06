using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Toolkit.CodeAnalysis {
    public class SyntaxTreeViewer : EditorWindow {

        #region Menu Items

        [MenuItem("Toolkit/Code Analysis/Syntax Tree Viewer")]
        public static void Open() {
            var w = GetWindow<SyntaxTreeViewer>("Syntax Tree Viewer");
            w?.Close();
            w = GetWindow<SyntaxTreeViewer>("Syntax Tree Viewer");
            w.Show();
            w.minSize = new Vector2(300, 200);
            w.position = new Rect(50, 50, 300, 200);
        }

        #endregion

        #region Variables

        private MonoScript mono;
        private Node node;
        private Vector2Int highlight = new Vector2Int(0, 0);
        internal static GUIStyle style;
        internal static GUIStyle foldoutStyle;
        private Vector2 scrollCode;
        private Vector2 scrollTree;
        private bool enable = true;

        #endregion

        #region Drawer

        private void OnGUI() {
            if(!enable) {
                return;
            }
            if(style == null) {
                style = new GUIStyle(GUI.skin.label);
                style.richText = true;
            }
            if(foldoutStyle == null) {
                foldoutStyle = new GUIStyle(EditorStyles.foldout);
                foldoutStyle.richText = true;
            }
            CalculateSize();
            DrawHeader();
            DrawBody();
            if(enable && focusedWindow == this)
                Repaint();
        }

        private void DrawHeader() {
            GUI.Box(Header, "");
            GUILayout.BeginArea(GetMinus5(Header));
            var newMono = (MonoScript)EditorGUILayout.ObjectField("Script:", mono, typeof(MonoScript), false);
            if(newMono != mono) {
                mono = newMono;
                node = null;
            }
            if(node == null && mono != null) {
                node = new Node(mono);
            }
            GUILayout.EndArea();
        }

        private void DrawBody() {
            GUI.Box(Body, "");
            DrawSyntaxTree();
            DrawCode();
        }

        private void DrawCode() {
            GUI.Box(CodeBody, "");
            GUILayout.BeginArea(GetMinus5(CodeBody));
            scrollCode = GUILayout.BeginScrollView(scrollCode);
            if(mono != null) {
                var text = mono.text;
                if((highlight.x != highlight.y))
                    text = text.Insert(highlight.y, "</color>").Insert(highlight.x, "<color=#ff00ff>");
                GUILayout.TextArea(text, style, GUILayout.ExpandHeight(true));
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawSyntaxTree() {
            GUI.Box(SyntaxBody, "Syntax Tree");
            GUILayout.BeginArea(GetMinus5(SyntaxBody));
            scrollTree = GUILayout.BeginScrollView(scrollTree);
            if(node != null) {
                highlight = new Vector2Int(0, 0);
                node.Draw(ref highlight);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region Size Calculation

        private void OnSizeChange(int width, int height) {

        }

        private int width = 0;
        private int height = 0;
        private int headerHeight = 26;

        private Vector2 Size => new Vector2(width, height);

        private Rect Header => new Rect(5, 5, width, headerHeight);

        private Rect Body => new Rect(5, headerHeight + 10, width, height - (headerHeight + 5));

        private Rect CodeBody => new Rect(10, headerHeight + 15, width / 2 - 7, height - (headerHeight + 15));

        private Rect SyntaxBody => new Rect(width / 2 + 8, headerHeight + 15, width / 2 - 7, height - (headerHeight + 15));

        private Rect GetMinus5(Rect rect) {
            rect.x += 5;
            rect.y += 5;
            rect.width -= 10;
            rect.height -= 10;
            return rect;
        }

        private void CalculateSize() {
            var newSize = position.size - new Vector2(10, 10);
            if(width != (int)(newSize.x) || height != (int)(newSize.y)) {
                width = (int)newSize.x;
                height = (int)newSize.y;
                OnSizeChange(width, height);
            }
        }

        #endregion
    }

    internal class Node {

        private enum NodeType {
            None,
            Root,
            Node,
            Token,
            Trivia
        }

        private NodeType type = NodeType.None;
        public string kind = "";
        public int spanStart = 0;
        public int spanEnd = 0;
        private bool foldout = true;

        public List<Node> nodes = new List<Node>();

        public Node(SyntaxNode node) {
            type = NodeType.Node;
            kind = node.Kind().ToString();
            var span = node.Span;
            spanStart = span.Start;
            spanEnd = span.End;
            foreach(var child in node.ChildNodesAndTokens()) {
                if(child.IsNode)
                    nodes.Add(new Node(child.AsNode()));
                else
                    nodes.Add(new Node(child.AsToken()));
            }
            
            foldout = false;
            Sort();
        }

        public Node(SyntaxToken token) {
            type = NodeType.Token;
            kind = token.Kind().ToString();
            var span = token.Span;
            spanStart = span.Start;
            spanEnd = span.End;
            foreach(var trivia in token.GetAllTrivia()){
                nodes.Add(new Node(trivia));
            }
        }

        public Node(SyntaxTrivia trivia) {
            type = NodeType.Trivia;
            kind = trivia.Kind().ToString();
            var span = trivia.Span;
            spanStart = span.Start;
            spanEnd = span.End;
        }

        public Node(MonoScript monoScript) {
            var syntaxTree = CSharpSyntaxTree.ParseText(monoScript.text);
            var root = syntaxTree.GetRoot();
            type = NodeType.Root;
            kind = root.Kind().ToString();
            var span = root.Span;
            spanStart = span.Start;
            spanEnd = span.End;
            foreach(var child in root.ChildNodesAndTokens()) {
                if(child.IsNode)
                    nodes.Add(new Node(child.AsNode()));
                else
                    nodes.Add(new Node(child.AsToken()));
            }
            Sort();
        }

        public void Sort() {
            nodes.Sort((a, b) => a.spanStart.CompareTo(b.spanStart));
        }

        public void Draw(ref Vector2Int hightlight) {
            if(type == NodeType.Node) {
                foldout = EditorGUILayout.Foldout(foldout, $"<color=#{GetColor(type).ToHex24()}>{kind} [{spanStart}...{spanEnd}]</color>", true, SyntaxTreeViewer.foldoutStyle);
            }
            else
                EditorGUILayout.LabelField($"<color=#{GetColor(type).ToHex24()}>{kind} [{spanStart}...{spanEnd}]</color>", SyntaxTreeViewer.style);
            var lastRect = GUILayoutUtility.GetLastRect();
            if(lastRect.Contains(Event.current.mousePosition)) {
                hightlight = new Vector2Int(spanStart, spanEnd);
            }
            if(!foldout) {
                return;
            }
            EditorGUI.indentLevel++;
            for(int i = 0, length = nodes.Count; i < length; i++) {
                nodes[i].Draw(ref hightlight);
            }
            EditorGUI.indentLevel--;
        }

        static Color GetColor(NodeType type) {
            switch(type) {
                case NodeType.Node: return Color.cyan;
                case NodeType.Token: return Color.green;
                case NodeType.Trivia: return Color.red;
                case NodeType.Root: return Color.magenta;
            }
            return GUI.contentColor;
        }
    }
}
