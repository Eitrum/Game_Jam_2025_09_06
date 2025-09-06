using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Toolkit.CodeAnalysis {

    public static class ComplexityUtility {
        public static float TechDebtRating(IReadOnlyList<MonoScript> scripts) {
            return Calculate(scripts).Sum(x => x.CalculatedComplexity);
        }

        public static float TechDebtRating(IReadOnlyList<FileComplexity> files) {
            return files.Sum(x => x.CalculatedComplexity);
        }

        public static FileComplexity Calculate(MonoScript script) {
            return new FileComplexity(script);
        }

        public static FileComplexity[] Calculate(IReadOnlyList<MonoScript> scripts) {
            FileComplexity[] complexities = new FileComplexity[scripts.Count];
            for(int i = 0; i < complexities.Length; i++) {
                complexities[i] = new FileComplexity(scripts[i]);
            }
            return complexities;
        }
    }

    public class FileComplexity {

        private const float FieldsComplexityStrength = 0.25f;
        private const float MethodsComplexityStrength = 1f;
        private const float PropertiesComplexityStrength = 0.35f;
        private const float LoCComplexityStrength = 1f / 25f;

        private const float NonPredefinedStrength = 0.2f;

        private const float MethodCallMultiplier = 1.25f;
        private const float MethodsParameterMultiplier = 1.1f;
        private const float AttributeComplexityMultiplier = 1.15f;


        private MonoScript script;
        private float fieldsComplexity;
        private float methodsComplexity;
        private float propertiesComplexity;
        private float locComplexity;
        private float ccComplexity;

        public string Name => script.name;
        public MonoScript Script => script;
        public float FieldsComplexity => Mathf.Max(1f, FieldsComplexityStrength * fieldsComplexity);
        public float MethodsComplexity => Mathf.Max(1f, MethodsComplexityStrength * methodsComplexity);
        public float PropertiesComplexity => Mathf.Max(1f, PropertiesComplexityStrength * propertiesComplexity);
        public float LoCComplexity => Mathf.Max(1f, LoCComplexityStrength * locComplexity);
        public float CCComplexity => ccComplexity;

        public float CalculatedComplexity => (FieldsComplexity + MethodsComplexity + PropertiesComplexity + LoCComplexity) * CCComplexity;

        public FileComplexity(MonoScript script) {
            this.script = script;
            Microsoft.CodeAnalysis.SyntaxTree tree = CSharpSyntaxTree.ParseText(script.text);
            ccComplexity = CyclomaticComplexityUtility.CalculateAverage(script);
            fieldsComplexity = CalculateFieldsComplexity(tree);
            methodsComplexity = CalculateMethodsComplexity(tree);
            propertiesComplexity = CalculatePropertiesComplexity(tree);
            locComplexity = script.text.Split('\n').Length;
        }

        private static float CalculateFieldsComplexity(Microsoft.CodeAnalysis.SyntaxTree tree) {
            float result = 1f;
            var fields = tree.ChildNodesRecursive().Where(x => x.IsKind(SyntaxKind.FieldDeclaration));
            foreach(var field in fields) {
                result += AttributeCost(field);
                var variable = field.ChildNodes().Where(x => x.IsKind(SyntaxKind.VariableDeclaration)).First();
                var type = variable.ChildNodes().First();
                if(!type.IsKind(SyntaxKind.PredefinedType)) {
                    result += NonPredefinedStrength;
                }
            }
            return result;
        }

        private static float CalculateMethodsComplexity(Microsoft.CodeAnalysis.SyntaxTree tree) {
            float result = 1f;
            var methods = tree.ChildNodesRecursive().Where(x => x.IsKind(SyntaxKind.MethodDeclaration));
            foreach(var method in methods) {
                result += AttributeCost(method) * ParameterCost(method) + MethodCallCost(method);
            }
            return result / Mathf.Max(1f, methods.Count());
        }

        private static float CalculatePropertiesComplexity(Microsoft.CodeAnalysis.SyntaxTree tree) {
            float result = 1f;
            var properties = tree.ChildNodesRecursive().Where(x => x.IsKind(SyntaxKind.PropertyDeclaration));
            foreach(var prop in properties) {
                result += MethodCallCost(prop);
            }
            return result;
        }

        private static float AttributeCost(SyntaxNode node) {
            return Mathf.Pow(AttributeComplexityMultiplier, node.ChildNodesRecursive().Where(x => x.IsKind(SyntaxKind.Attribute)).Count());
        }

        private static float ParameterCost(SyntaxNode node) {
            return Mathf.Pow(MethodsParameterMultiplier, node.ChildNodesRecursive().Where(x => x.IsKind(SyntaxKind.Parameter)).SelectMany(x=>x.ChildNodes()).Where(x=>x.IsKind(SyntaxKind.IdentifierName)).Count());
        }

        private static float MethodCallCost(SyntaxNode node) {
            return Mathf.Max(1f, Mathf.Pow(node.ChildNodesRecursive().Where(x => x.IsKind(SyntaxKind.ExpressionStatement)).Count(), MethodCallMultiplier));
        }
    }
}
