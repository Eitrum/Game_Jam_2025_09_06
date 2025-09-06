using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolkit.IO.TML {
    public static class TMLParser {
        #region Indentation

        private static StringUtil.RepeatingCharCache indentLookup;
        static TMLParser() {
            indentLookup = StringUtil.GetRepeatingCharCache('\t');
        }

        #endregion

        #region Print

        public static void Print(this TMLNode node) {
            Debug.Log(ToString(node, true));
        }

        #endregion

        #region ToString

        private static FastPool<StringBuilder> builders = new FastPool<StringBuilder>();

        public static string ToString(TMLNode node, bool beatify) {
            if(string.IsNullOrEmpty(node.Name))
                node.SetName("root");
            StringBuilder sb = builders.Pop();
            sb.Clear();
            try {
                if(beatify)
                    ToStringBeautify(sb, node, 0);
                else
                    ToStringUglify(sb, node);
                return sb.ToString();
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return string.Empty;
            }
            finally {
                sb.Clear();
                builders.Push(sb);
            }
        }

        private static void ToStringUglify(StringBuilder sb, TMLNode node) {
            if(node.HasNodes) {
                if(node.HasProperties)
                    sb.Append($"<{node.Name} {string.Join(" ", Formatted(node.Properties))}>");
                else
                    sb.Append($"<{node.Name}>");
                foreach(var n in node.Nodes)
                    ToStringUglify(sb, n);
                sb.Append($"</{node.Name}>");
            }
            else {
                if(node.HasProperties)
                    sb.Append($"<{node.Name} {string.Join(" ", Formatted(node.Properties))}/>");
                else
                    sb.Append($"<{node.Name}/>");
            }
        }

        private static void ToStringBeautify(StringBuilder sb, TMLNode node, int indent) {
            if(node.HasNodes) {
                if(node.HasProperties)
                    sb.AppendLine($"{indentLookup[indent]}<{node.Name} {string.Join(" ", Formatted(node.Properties))}>");
                else
                    sb.AppendLine($"{indentLookup[indent]}<{node.Name}>");

                foreach(var n in node.Nodes)
                    ToStringBeautify(sb, n, indent + 1);
                sb.AppendLine($"{indentLookup[indent]}</{node.Name}>");
            }
            else {
                if(node.HasProperties)
                    sb.AppendLine($"{indentLookup[indent]}<{node.Name} {string.Join(" ", Formatted(node.Properties))}/>");
                else
                    sb.AppendLine($"{indentLookup[indent]}<{node.Name}/>");
            }
        }

        private static IEnumerable<string> Formatted(IEnumerable<Properties.ITMLProperty> properties) {
            foreach(var p in properties)
                if(p is Properties.ITMLProperty_Xml xml)
                    yield return xml.GetFormattedXml();
                else
                    yield return p.ToString();
        }

        #endregion

        #region Parse

        public static TMLNode Parse(ReadOnlySpan<char> text) {
            var tokens = TMLTokenizer.Tokenize(text);
            try {
                Rebuild(text, out var node, tokens);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static TMLNode Parse(string text) {
            var tokens = TMLTokenizer.Tokenize(text);
            try {
                Rebuild(text, out var node, tokens);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static TMLNode Parse(ReadOnlySpan<char> text, Vector2Int range)
            => Parse(text, range.x, range.y);

        public static TMLNode Parse(string text, Vector2Int range)
            => Parse(text, range.x, range.y);

        public static TMLNode Parse(ReadOnlySpan<char> text, int start, int end) {
            var tokens = TMLTokenizer.Tokenize(text, start, end);
            try {
                Rebuild(text, out var node, tokens);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static TMLNode Parse(string text, int start, int end) {
            var tokens = TMLTokenizer.Tokenize(text, start, end);
            try {
                Rebuild(text, out var node, tokens);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        #endregion

        #region ParseSafe

        public static TMLNode ParseSafe(ReadOnlySpan<char> text) {
            if(!TMLTokenizer.TryTokenize(text, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                Rebuild(text, node, tokens);
                return node;
            }
            catch {
                return null;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static TMLNode ParseSafe(string text) {
            if(!TMLTokenizer.TryTokenize(text, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                Rebuild(text, node, tokens);
                return node;
            }
            catch {
                return null;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static TMLNode ParseSafe(ReadOnlySpan<char> text, Vector2Int range)
            => ParseSafe(text, range.x, range.y);

        public static TMLNode ParseSafe(string text, Vector2Int range)
            => ParseSafe(text, range.x, range.y);

        public static TMLNode ParseSafe(ReadOnlySpan<char> text, int start, int end) {
            if(!TMLTokenizer.TryTokenize(text, start, end, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                Rebuild(text, node, tokens);
                return node;
            }
            catch {
                return null;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static TMLNode ParseSafe(string text, int start, int end) {
            if(!TMLTokenizer.TryTokenize(text, start, end, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                Rebuild(text, node, tokens);
                return node;
            }
            catch {
                return null;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        #endregion

        #region TryParse

        public static bool TryParse(ReadOnlySpan<char> text, TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, out var tokens)) {
                return false;
            }
            try {
                Rebuild(text, node, tokens);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, out var tokens)) {
                return false;
            }
            try {
                Rebuild(text, node, tokens);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(ReadOnlySpan<char> text, out TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, out var tokens)) {
                node = null;
                return false;
            }
            try {
                Rebuild(text, out node, tokens);
                return node != null;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, out TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, out var tokens)) {
                node = null;
                return false;
            }
            try {
                Rebuild(text, out node, tokens);
                return node != null;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(ReadOnlySpan<char> text, Vector2Int range, TMLNode node)
            => TryParse(text, range.x, range.y, node);

        public static bool TryParse(string text, Vector2Int range, TMLNode node)
            => TryParse(text, range.x, range.y, node);

        public static bool TryParse(ReadOnlySpan<char> text, Vector2Int range, out TMLNode node)
            => TryParse(text, range.x, range.y, out node);

        public static bool TryParse(string text, Vector2Int range, out TMLNode node)
            => TryParse(text, range.x, range.y, out node);

        public static bool TryParse(ReadOnlySpan<char> text, int start, int end, TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, start, end, out var tokens)) {
                return false;
            }
            try {
                Rebuild(text, node, tokens);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, int start, int end, TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, start, end, out var tokens)) {
                return false;
            }
            try {
                Rebuild(text, node, tokens);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(ReadOnlySpan<char> text, int start, int end, out TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, start, end, out var tokens)) {
                node = null;
                return false;
            }
            try {
                Rebuild(text, out node, tokens);
                return node != null;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, int start, int end, out TMLNode node) {
            if(!TMLTokenizer.TryTokenize(text, start, end, out var tokens)) {
                node = null;
                return false;
            }
            try {
                Rebuild(text, out node, tokens);
                return node != null;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                TMLTokenizer.Return(tokens);
            }
        }

        private static TMLNode CreateInitialNode(string text, List<TMLTokenizer.Token> tokens) {
            if(tokens == null)
                return new TMLNode("root");
            if(tokens.Count < 2)
                return new TMLNode("root");
            if(tokens[0].Type != TMLTokenizer.TokenType.Block)
                return new TMLNode("root");
            return new TMLNode(tokens[1].GetSubstring(text));
        }

        private static void Rebuild(string text, out TMLNode node, List<TMLTokenizer.Token> tokens) {
            node = CreateInitialNode(text, tokens);
            int index = 2;
            Rebuild(text, node, tokens[0].End, tokens, ref index);
        }

        private static void Rebuild(string text, TMLNode node, List<TMLTokenizer.Token> tokens) {
            int index = 0;
            Rebuild(text, node, tokens[0].End, tokens, ref index);
        }

        private static void Rebuild(string text, TMLNode node, int end, List<TMLTokenizer.Token> tokens, ref int index) {
            while(index < tokens.Count) {
                if(tokens[index].Start > end)
                    return;
                switch(tokens[index].Type) {
                    case TMLTokenizer.TokenType.Block: {
                            var child = node.AddNode(tokens[index+1].GetSubstring(text));
                            index += 2;
                            Rebuild(text, child, tokens[index - 2].End, tokens, ref index);
                        }
                        break;
                    case TMLTokenizer.TokenType.Attribute:
                        node.AddProperty(new Toolkit.IO.TML.Properties.TMLProperty_String(tokens[index + 1].GetSubstring(text), tokens[index + 2].GetSubstring(text)));
                        index += 3;
                        break;
                    default:
                        Debug.LogError("Broken tokens");
                        return;
                }
            }
        }

        private static TMLNode CreateInitialNode(ReadOnlySpan<char> text, List<TMLTokenizer.Token> tokens) {
            if(tokens == null)
                return new TMLNode("root");
            if(tokens.Count < 2)
                return new TMLNode("root");
            if(tokens[0].Type != TMLTokenizer.TokenType.Block)
                return new TMLNode("root");
            return new TMLNode(tokens[1].GetSubstring(text));
        }

        private static void Rebuild(ReadOnlySpan<char> text, out TMLNode node, List<TMLTokenizer.Token> tokens) {
            node = CreateInitialNode(text, tokens);
            int index = 2;
            Rebuild(text, node, tokens[0].End, tokens, ref index);
        }

        private static void Rebuild(ReadOnlySpan<char> text, TMLNode node, List<TMLTokenizer.Token> tokens) {
            int index = 0;
            Rebuild(text, node, tokens[0].End, tokens, ref index);
        }

        private static void Rebuild(ReadOnlySpan<char> text, TMLNode node, int end, List<TMLTokenizer.Token> tokens, ref int index) {
            while(index < tokens.Count) {
                if(tokens[index].Start > end)
                    return;
                switch(tokens[index].Type) {
                    case TMLTokenizer.TokenType.Block: {
                            var child = node.AddNode(tokens[index+1].GetSubstring(text));
                            index += 2;
                            Rebuild(text, child, tokens[index - 2].End, tokens, ref index);
                        }
                        break;
                    case TMLTokenizer.TokenType.Attribute:
                        node.AddProperty(new Toolkit.IO.TML.Properties.TMLProperty_String(tokens[index + 1].GetSubstring(text), tokens[index + 2].GetSubstring(text)));
                        index += 3;
                        break;
                    default:
                        Debug.LogError("Broken tokens");
                        return;
                }
            }
        }

        #endregion
    }
}
