using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.Emit;
using System.CodeDom.Compiler;
using System.CodeDom;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEditor;
using System.Linq;
using System;

namespace Toolkit.CodeAnalysis {
    public static class SyntaxTreeUtility {

        public static IEnumerable<SyntaxNode> ChildNodesRecursive(this Microsoft.CodeAnalysis.SyntaxTree tree) {
            List<SyntaxNode> nodes = new List<SyntaxNode>();
            ChildNodesRecursive(tree.GetRoot(), nodes);
            return nodes;
        }

        public static IEnumerable<SyntaxNode> ChildNodesRecursive(this SyntaxNode node) {
            List<SyntaxNode> nodes = new List<SyntaxNode>();
            ChildNodesRecursive(node, nodes);
            return nodes;
        }

        private static void ChildNodesRecursive(SyntaxNode node, List<SyntaxNode> nodes) {
            foreach(var child in node.ChildNodes()) {
                nodes.Add(child);
                ChildNodesRecursive(child, nodes);
            }
        }

        public static void Print(this IEnumerable<SyntaxNode> nodes, MonoScript script) {
            var text = script.text;
            foreach(var node in nodes) {
                var span = node.Span;
                UnityEngine.Debug.Log(text.Substring(span.Start, span.End - span.Start));
            }
        }
    }
}
