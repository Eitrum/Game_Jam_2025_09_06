using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolkit.Mathematics
{
    public static class MathematicalExpressionBuilder
    {
        /* Cases
         * 
         *   (x-(mod(x,1)*-5))*.2-x.2
         *   (x - (mod(x,1) * -5)) * 0.2 - x * 0.2
         */

        public static IMathExpression Build(string expression)
            => Build(expression, "x");

        public static IMathExpression Build(string expression, string variables) {
            if(string.IsNullOrEmpty(variables)) {
                variables = "x";
            }

            if(!VerifyParenthesis(expression)) {
                Debug.LogError("Uneven amount of parenthesis!");
            }

            var tokens = GetTokens(expression);
            //Debug.Log(tokens.CombineToString(true));
            return RecursiveBuildFromTokens(variables, tokens, 0, tokens.Count - 1);
        }

        // END IS INCLUSIVE
        private static IMathExpression RecursiveBuildFromTokens(string variables, List<string> tokens, int start, int end) {
            //Debug.Log($"Building: {start} -> {end}");
            if(start == end) {
                var t = tokens[start];
                if(MathematicalExpressionData.CONSTANTS.Contains(t)) {
                    if(MathematicalExpressionData.CONSTANTS_TO_EXPRESSION.TryGetValue(t, out Func<IMathExpression> func)) {
                        return func.Invoke();
                    }
                    else {
                        throw new Exception("Constant value not found in dictionary!");
                    }
                }
                var varIndex = variables.IndexOf(t[0]);
                if(varIndex >= 0) {
                    return new VariableExpression(varIndex);
                }
                if(float.TryParse(t, out float val)) {
                    return new NumberExpression(val);
                }
            }

            // Handle groups
            if(tokens[start] == "(" && tokens[end] == ")" && VerifyParenthesis(tokens, start, end))
                return RecursiveBuildFromTokens(variables, tokens, start + 1, end - 1);


            // Find operators by order
            if(HandleOperatorSearch("+-", tokens, start, end, out MathematicalExpressionData.GetOperatorExpressionDelegate func0, out int center0))
                return func0.Invoke(
                    RecursiveBuildFromTokens(variables, tokens, start, center0 - 1),
                    RecursiveBuildFromTokens(variables, tokens, center0 + 1, end));
            if(HandleOperatorSearch("*/%", tokens, start, end, out MathematicalExpressionData.GetOperatorExpressionDelegate func1, out int center1))
                return func1.Invoke(
                    RecursiveBuildFromTokens(variables, tokens, start, center1 - 1),
                    RecursiveBuildFromTokens(variables, tokens, center1 + 1, end));
            if(HandleOperatorSearch("^", tokens, start, end, out MathematicalExpressionData.GetOperatorExpressionDelegate func2, out int center2))
                return func2.Invoke(
                    RecursiveBuildFromTokens(variables, tokens, start, center2 - 1),
                    RecursiveBuildFromTokens(variables, tokens, center2 + 1, end));

            if(HandleMethodSearch(tokens, start, end, out MathematicalExpressionData.GetMethodExpressionDelegate method, out int startIndex) &&
                FindClosingParanthesis(tokens, startIndex, out int endIndex)) {
                startIndex++;
                endIndex--;
                var splits = GetSplitIndicies(tokens, startIndex, endIndex, out int s0, out int s1, out int s2);
                switch(splits) {
                    case 0: return method?.Invoke(RecursiveBuildFromTokens(variables, tokens, startIndex + 1, endIndex), null, null, null);
                    case 1:
                        return method?.Invoke(
                            RecursiveBuildFromTokens(variables, tokens, startIndex + 1, s0 - 1),
                            RecursiveBuildFromTokens(variables, tokens, s0 + 1, endIndex), null, null);
                    case 2:
                        return method?.Invoke(
                            RecursiveBuildFromTokens(variables, tokens, startIndex + 1, s0 - 1),
                            RecursiveBuildFromTokens(variables, tokens, s0 + 1, s1),
                            RecursiveBuildFromTokens(variables, tokens, s1 + 1, endIndex), null);
                    case 3:
                        return method?.Invoke(
                            RecursiveBuildFromTokens(variables, tokens, startIndex + 1, s0),
                            RecursiveBuildFromTokens(variables, tokens, s0 + 1, s1),
                            RecursiveBuildFromTokens(variables, tokens, s1 + 1, s2),
                            RecursiveBuildFromTokens(variables, tokens, s2 + 1, endIndex));
                }
            }

            Debug.LogError($"Unable to properly parse! {start} -> {end}");
            return null;
        }

        #region Utility

        private static int GetSplitIndicies(List<string> tokens, int start, int end, out int s0, out int s1, out int s2) {
            int result = 0;
            s0 = -1;
            s1 = -1;
            s2 = -1;
            for(int i = start, length = end; i <= length; i++) {
                if(tokens[i] == ",") {
                    switch(result) {
                        case 0:
                            s0 = i;
                            break;
                        case 1:
                            s1 = i;
                            break;
                        case 2:
                            s2 = i;
                            break;
                    }
                    result++;
                }
            }

            return result;
        }

        private static bool FindClosingParanthesis(List<string> tokens, int start, out int end) {
            int depth = 0;
            for(int i = start, length = tokens.Count; i < length; i++) {
                var t = tokens[i];
                if(t == "(")
                    depth++;
                if(t == ")") {
                    depth--;
                    if(depth <= 0) {
                        end = i;
                        return true;
                    }
                }
            }
            end = -1;
            return false;
        }

        private static bool HandleMethodSearch(List<string> tokens, int start, int end, out MathematicalExpressionData.GetMethodExpressionDelegate method, out int startIndex) {
            int depth = 0;
            for(int i = end; i >= start; i--) {
                var t = tokens[i];
                switch(t) {
                    case ")":
                        depth++;
                        break;
                    case "(":
                        depth--;
                        break;
                }
                if(depth > 0 || t.Length < 3)
                    continue;

                if(MathematicalExpressionData.METHODS_TO_EXPRESSION.TryGetValue(t, out method)) {
                    startIndex = i;
                    return true;
                }
            }
            method = null;
            startIndex = 0;
            return false;
        }

        private static bool HandleOperatorSearch(string operators, List<string> tokens, int start, int end, out MathematicalExpressionData.GetOperatorExpressionDelegate func, out int index) {
            int depth = 0;
            for(int i = end; i >= start; i--) {
                var t = tokens[i];
                if(t.Length > 1)
                    continue;
                switch(t) {
                    case ")":
                        depth++;
                        break;
                    case "(":
                        depth--;
                        break;
                }
                if(depth == 0 && operators.Contains(t)) {
                    index = i;
                    if(MathematicalExpressionData.OPERATORS_TO_EXPRESSION.TryGetValue(t, out func)) {
                        return true;
                    }
                    else {
                        throw new Exception($"Could not find operator function '{t}'");
                    }
                }
            }
            index = 0;
            func = null;
            return false;
        }

        #endregion

        public static bool VerifyParenthesis(List<string> tokens, int start, int end) {
            int depth = 0;
            for(int i = end; i >= start; i--) {
                var t = tokens[i];
                if(t.Length > 1)
                    continue;
                switch(t) {
                    case ")":
                        depth++;
                        break;
                    case "(":
                        depth--;
                        break;
                }
            }
            return depth == 0;
        }

        public static bool VerifyParenthesis(string expression) {
            int l = 0;
            int r = 0;
            if(string.IsNullOrEmpty(expression))
                return true;

            for(int i = 0, length = expression.Length; i < length; i++) {
                var c = expression[i];
                if(c == '(')
                    l++;
                else if(c == ')')
                    r++;
            }

            return l == r;
        }

        #region Token 

        public static List<string> GetTokens(string expression) {
            const string operators = "()^*/+-%,";
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach(char c in expression.Replace(" ", string.Empty)) {
                if(operators.IndexOf(c) >= 0) {
                    if((sb.Length > 0)) {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    if(c == '-') {
                        if(tokens.Count > 0) {
                            var t = tokens[tokens.Count - 1];
                            if(t.Length == 1 && t[0] != ')' && operators.Contains(t)) {
                                sb.Append(c);
                                continue;
                            }
                        }
                        else {
                            sb.Append(c);
                            continue;
                        }
                    }
                    tokens.Add(c.ToString());
                    continue;
                }
                sb.Append(c);
            }

            if((sb.Length > 0)) {
                tokens.Add(sb.ToString());
            }

            // Multiplication pass
            for(int i = 0; i < tokens.Count; i++) {
                var t = tokens[i];

                // Safety check tokens
                if(tokens.Count > 1000)
                    break;
                if(string.IsNullOrEmpty(t)) {
                    tokens.RemoveAt(i);
                    i--;
                    continue;
                }

                // Verify its not a single character
                if(t.Length == 1)
                    continue;

                // Check if a magic number
                if(float.TryParse(t, out float r))
                    continue;

                // Check if a method
                if(MathematicalExpressionData.METHODS.Contains(t))
                    continue;

                // Check if constant
                if(MathematicalExpressionData.CONSTANTS.Contains(t))
                    continue;

                // Check if first character is letter
                if(char.IsLetter(t[0])) {
                    t.SplitAt(1, out t, out string toInsert);
                    tokens[i] = t;
                    tokens.Insert(i + 1, "*");
                    tokens.Insert(i + 2, toInsert);
                    continue;
                }

                for(int x = 1, length = t.Length; x < length; x++) {
                    if(char.IsLetter(t[x])) {
                        t.SplitAt(x, out string newToken, out string toInsert);
                        tokens[i] = newToken;
                        tokens.Insert(i + 1, "*");
                        tokens.Insert(i + 2, toInsert);
                        break;
                    }
                }
            }

            return tokens;
        }

        #endregion
    }

    public static class MathematicalExpressionData
    {
        public const string OPERATORS = "+-*/%^";

        public static readonly string[] METHODS = {
            "log",
            "logn",
            "pow",
            "sqr",
            "sqrt",
            "exp",
            "mod",
            "min",
            "max",
            "clamp",
            "floor",
            "round",
            "ceil",
            "sin",
            "cos",
            "tan",
        };

        public delegate IMathExpression GetMethodExpressionDelegate(IMathExpression p0, IMathExpression p1, IMathExpression p2, IMathExpression p3);
        public static readonly Dictionary<string, GetMethodExpressionDelegate> METHODS_TO_EXPRESSION = new Dictionary<string, GetMethodExpressionDelegate>() {
            {"pow", (p0, p1, p2, p3) => new PowerExpression(p0, p1) },
            {"log", (p0, p1, p2, p3) => new Method1Expression(Mathf.Log, p0) },
            {"logn", (p0, p1, p2, p3) => new Method2Expression(Mathf.Log, p0, p1) },
            {"sqr", (p0, p1, p2, p3) => new Method1Expression(Mathf.Sqrt, p0) },
            {"sqrt", (p0, p1, p2, p3) => new Method1Expression(Mathf.Sqrt, p0) },

            {"exp", (p0, p1, p2, p3) => new Method1Expression(Mathf.Exp, p0) },
            {"mod", (p0, p1, p2, p3) => new ModuloExpression(p0, p1) },

            {"min", (p0, p1, p2, p3) => new Method2Expression(Mathf.Min, p0, p1) },
            {"max", (p0, p1, p2, p3) => new Method2Expression(Mathf.Max, p0, p1) },
            {"clamp", (p0, p1, p2, p3) => new Method3Expression(Mathf.Clamp, p0, p1, p2) },

            {"floor", (p0, p1, p2, p3) => new Method1Expression(Mathf.Floor, p0) },
            {"ceil", (p0, p1, p2, p3) => new Method1Expression(Mathf.Ceil, p0) },
            {"round", (p0, p1, p2, p3) => new Method1Expression(Mathf.Round, p0) },

            {"sin", (p0, p1, p2, p3) => new Method1Expression(Mathf.Sin, p0) },
            {"cos", (p0, p1, p2, p3) => new Method1Expression(Mathf.Cos, p0) },
            {"tan", (p0, p1, p2, p3) => new Method1Expression(Mathf.Tan, p0) },
        };

        public static readonly string[] CONSTANTS ={
            "e",
            "phi",
            "sqr_2",
            "pi",
            "r2d", // Radians to Degrees
            "d2r", // Degrees to Radians
            "d2l", // Degrees to percentage (0 -> 1)
        };

        public delegate IMathExpression GetOperatorExpressionDelegate(IMathExpression lhs, IMathExpression rhs);
        public static readonly Dictionary<string, GetOperatorExpressionDelegate> OPERATORS_TO_EXPRESSION = new Dictionary<string, GetOperatorExpressionDelegate>() {
            {"+", (l, r) => new AdditionExpression(l, r) },
            {"-", (l, r) => new SubtractExpression(l, r) },
            {"*", (l, r) => new MultiplyExpression(l, r) },
            {"/", (l, r) => new DivideExpression(l, r) },
            {"%", (l, r) => new ModuloExpression(l, r) },
            {"^", (l, r) => new PowerExpression(l, r) },
        };

        public static readonly Dictionary<string, Func<IMathExpression>> CONSTANTS_TO_EXPRESSION = new Dictionary<string, Func<IMathExpression>>() {
            {"e", ()=> new NumberExpression(MathUtility.E) },
            {"phi", ()=>new NumberExpression(MathUtility.PHI) },
            {"pi", ()=>new NumberExpression(MathUtility.PI) },
            {"sqr_2", ()=>new NumberExpression(MathUtility.SQR_2) },
            {"r2d", ()=>new NumberExpression(MathUtility.Rad2Deg) },
            {"d2r", ()=>new NumberExpression(MathUtility.Deg2Rad) },
            {"d2l", ()=>new NumberExpression(MathUtility.Deg2Linear) },
        };

        internal static readonly GUIContent[] OPERATOR_DESCRIPTIONS = {
            new GUIContent("[+] Addition"),
            new GUIContent("[-] Subtract"),
            new GUIContent("[*] Multiply"),
            new GUIContent("[/] Divide"),
            new GUIContent("[%] Modulo"),
            new GUIContent("[^] Power"),
        };
    }

    public class StoredVariables : IDisposable
    {
        #region Variables

        private float value0;
        private float value1;
        private float value2;
        private float value3;
        private float value4;
        private float value5;
        private float value6;
        private float value7;

        #endregion

        #region Properties

        public unsafe float this[int index] {
            get {
                fixed(float* p = &value0)
                    return *(p + index);
            }
        }

        #endregion

        #region Pooling

        public static StoredVariables Create(float v0)
            => Create(v0, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

        public static StoredVariables Create(float v0, float v1)
            => Create(v0, v1, 0f, 0f, 0f, 0f, 0f, 0f);

        public static StoredVariables Create(float v0, float v1, float v2)
            => Create(v0, v1, v2, 0f, 0f, 0f, 0f, 0f);

        public static StoredVariables Create(float v0, float v1, float v2, float v3)
            => Create(v0, v1, v2, v3, 0f, 0f, 0f, 0f);

        public static StoredVariables Create(float v0, float v1, float v2, float v3, float v4)
            => Create(v0, v1, v2, v3, v4, 0f, 0f, 0f);

        public static StoredVariables Create(float v0, float v1, float v2, float v3, float v4, float v5)
            => Create(v0, v1, v2, v3, v4, v5, 0f, 0f);

        public static StoredVariables Create(float v0, float v1, float v2, float v3, float v4, float v5, float v6)
            => Create(v0, v1, v2, v3, v4, v5, v6, 0f);

        public static StoredVariables Create(float v0, float v1, float v2, float v3, float v4, float v5, float v6, float v7) {
            var sv = FastPool<StoredVariables>.Global.Pop();
            sv.value0 = v0;
            sv.value1 = v1;
            sv.value2 = v2;
            sv.value3 = v3;
            sv.value4 = v4;
            sv.value5 = v5;
            sv.value6 = v6;
            sv.value7 = v7;
            return sv;
        }

        public static StoredVariables Create() {
            return FastPool<StoredVariables>.Global.Pop();
        }

        public void Dispose() {
            FastPool<StoredVariables>.Global.Push(this);
        }

        #endregion
    }

    public sealed class Method1Expression : IMathExpression
    {
        private Func<float, float> func;
        public IMathExpression Block { get; private set; }

        public float Compute(StoredVariables sv) {
            return func(Block.Compute(sv));
        }

        public Method1Expression(Func<float, float> func, IMathExpression block) {
            this.func = func;
            this.Block = block;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("method1");
            Block.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var blockFunc = Block.BakeFunction();
            return (sv) => func.Invoke(blockFunc.Invoke(sv));
        }
    }

    public sealed class Method2Expression : IMathExpression
    {
        private Func<float, float, float> func;
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return func(Lhs?.Compute(sv) ?? 0, Rhs?.Compute(sv) ?? 0);
        }

        public Method2Expression(Func<float, float, float> func, IMathExpression lhs, IMathExpression rhs) {
            this.func = func;
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("method2");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => func.Invoke(lhsFunc.Invoke(sv), rhsFunc.Invoke(sv));
        }
    }

    public sealed class Method3Expression : IMathExpression
    {
        private Func<float, float, float, float> func;
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Chs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return func(Lhs?.Compute(sv) ?? 0, Chs?.Compute(sv) ?? 0, Rhs?.Compute(sv) ?? 0);
        }

        public Method3Expression(Func<float, float, float, float> func, IMathExpression lhs, IMathExpression chs, IMathExpression rhs) {
            this.func = func;
            this.Lhs = lhs;
            this.Chs = chs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("method3");
            Lhs.CreateLogTree(sb, depth + 1);
            Chs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var chsFunc = Chs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => func.Invoke(lhsFunc.Invoke(sv), chsFunc.Invoke(sv), rhsFunc.Invoke(sv));
        }
    }

    public sealed class VariableExpression : IMathExpression
    {
        public int Index { get; private set; } = 0;

        public float Compute(StoredVariables sv) {
            return sv[Index];
        }

        public VariableExpression() { }
        public VariableExpression(int index) => this.Index = index;

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("variable : " + Index);
        }

        public Func<StoredVariables, float> BakeFunction() {
            return (sv) => sv[Index];
        }
    }

    public sealed class NumberExpression : IMathExpression
    {
        public float Value { get; private set; } = 0f;

        public float Compute(StoredVariables sv) {
            return Value;
        }

        public NumberExpression() { }
        public NumberExpression(float value) => this.Value = value;

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("number : " + Value);
        }

        public Func<StoredVariables, float> BakeFunction() {
            return (sv) => Value;
        }
    }

    public sealed class AdditionExpression : IMathExpression
    {
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return (Lhs?.Compute(sv) ?? 0f) + (Rhs?.Compute(sv) ?? 0f);
        }

        public AdditionExpression() { }
        public AdditionExpression(IMathExpression lhs, IMathExpression rhs) {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("addition");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => lhsFunc.Invoke(sv) + rhsFunc.Invoke(sv);
        }
    }

    public sealed class SubtractExpression : IMathExpression
    {
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return (Lhs?.Compute(sv) ?? 0f) - (Rhs?.Compute(sv) ?? 0f);
        }

        public SubtractExpression() { }
        public SubtractExpression(IMathExpression lhs, IMathExpression rhs) {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("subtract");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => lhsFunc.Invoke(sv) - rhsFunc.Invoke(sv);
        }
    }

    public sealed class MultiplyExpression : IMathExpression
    {
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return (Lhs?.Compute(sv) ?? 0f) * (Rhs?.Compute(sv) ?? 0f);
        }

        public MultiplyExpression() { }
        public MultiplyExpression(IMathExpression lhs, IMathExpression rhs) {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("multiply");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => lhsFunc.Invoke(sv) * rhsFunc.Invoke(sv);
        }
    }

    public sealed class DivideExpression : IMathExpression
    {
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return (Lhs?.Compute(sv) ?? 0f) / (Rhs?.Compute(sv) ?? 0f);
        }

        public DivideExpression() { }
        public DivideExpression(IMathExpression lhs, IMathExpression rhs) {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("divide");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => lhsFunc.Invoke(sv) / rhsFunc.Invoke(sv);
        }
    }

    public sealed class ModuloExpression : IMathExpression
    {
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return (Lhs?.Compute(sv) ?? 0f) % (Rhs?.Compute(sv) ?? 0f);
        }

        public ModuloExpression() { }
        public ModuloExpression(IMathExpression lhs, IMathExpression rhs) {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("modulo");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => lhsFunc.Invoke(sv) % rhsFunc.Invoke(sv);
        }
    }

    public sealed class PowerExpression : IMathExpression
    {
        public IMathExpression Lhs { get; private set; }
        public IMathExpression Rhs { get; private set; }

        public float Compute(StoredVariables sv) {
            return Mathf.Pow((Lhs?.Compute(sv) ?? 0f), (Rhs?.Compute(sv) ?? 0f));
        }

        public PowerExpression() { }
        public PowerExpression(IMathExpression lhs, IMathExpression rhs) {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public void CreateLogTree(StringBuilder sb, int depth) {
            sb.Indent(depth, "--");
            sb.AppendLine("power");
            Lhs.CreateLogTree(sb, depth + 1);
            Rhs.CreateLogTree(sb, depth + 1);
        }

        public Func<StoredVariables, float> BakeFunction() {
            var lhsFunc = Lhs.BakeFunction();
            var rhsFunc = Rhs.BakeFunction();
            return (sv) => Mathf.Pow(lhsFunc.Invoke(sv), rhsFunc.Invoke(sv));
        }
    }

    public interface IMathExpression
    {
        float Compute(StoredVariables sv);
        void CreateLogTree(StringBuilder sb, int depth);
        Func<StoredVariables, float> BakeFunction();
    }
}
