using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace Toolkit.CodeAnalysis {
    public static class CyclomaticComplexityUtility {

        public static float CalculateAverage(IReadOnlyList<MonoScript> scripts) {
            var value = 0f;
            EditorUtility.DisplayProgressBar("Cyclomatic Complexity", "", 0f);
            for(int i = 0, length = scripts.Count; i < length; i++) {
                EditorUtility.DisplayProgressBar("Cyclomatic Complexity", scripts[i].name, (float)i / length);
                value += CalculateAverage(scripts[i]);
            }
            EditorUtility.ClearProgressBar();
            return value / scripts.Count;
        }

        public static float CalculateAverage(MonoScript monoScript) {
            int ccCount = 0;
            int methodCount = 0;
            int classCount = 0;
            Calculate(monoScript, out ccCount, out classCount, out methodCount);
            return methodCount > 0 ? (float)ccCount / methodCount : 1f;
        }

        public static int CalculateTotal(MonoScript script) {
            int ccCount = 0;
            int methodCount = 0;
            int classCount = 0;
            Calculate(script, out ccCount, out classCount, out methodCount);
            return ccCount;
        }

        public static void Calculate(MonoScript script, out int ccCount, out int classCount, out int methodCount) {
            ccCount = 0;
            classCount = 0;
            methodCount = 0;
            var tree = CSharpSyntaxTree.ParseText(script.text);
            var classes = tree.ChildNodesRecursive().Where(x => x.Kind() == SyntaxKind.ClassDeclaration);
            foreach(var @class in classes) {
                var methods = @class.ChildNodesRecursive().Where(x => x.Kind() == SyntaxKind.MethodDeclaration);
                foreach(var method in methods) {
                    methodCount++;
                    var branches = method.ChildNodesRecursive().Where(x => {
                        var kind = x.Kind();
                        return
                        kind == SyntaxKind.IfStatement ||
                        kind == SyntaxKind.ForStatement ||
                        kind == SyntaxKind.ForEachStatement ||
                        kind == SyntaxKind.WhileStatement ||
                        kind == SyntaxKind.CaseKeyword ||
                        kind == SyntaxKind.CatchDeclaration ||
                        kind == SyntaxKind.ConditionalExpression ||
                        kind == SyntaxKind.LogicalAndExpression ||
                        kind == SyntaxKind.LogicalOrExpression;
                    });
                    var localCC = branches.Count() + 1;
                    ccCount += localCC;
                }
            }
        }
    }
}
