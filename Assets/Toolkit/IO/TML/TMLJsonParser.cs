using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolkit.IO.TML {
    public static class TMLJsonParser {

        #region Variables

        private const string TAG = ColorTable.RichTextTags.OLIVE + "[Toolkit.IO.TMLJsonParser]</color> - ";

        #endregion

        #region Parse

        public static TMLNode Parse(ReadOnlySpan<char> text) {
            var tokens = JsonTokenizer.Tokenize(text);
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static TMLNode Parse(string text) {
            var tokens = JsonTokenizer.Tokenize(text);
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static TMLNode Parse(ReadOnlySpan<char> text, Vector2Int range)
            => Parse(text, range.x, range.y);

        public static TMLNode Parse(string text, Vector2Int range)
            => Parse(text, range.x, range.y);

        public static TMLNode Parse(ReadOnlySpan<char> text, int start, int end) {
            var tokens = JsonTokenizer.Tokenize(text, start, end);
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static TMLNode Parse(string text, int start, int end) {
            var tokens = JsonTokenizer.Tokenize(text, start, end);
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        #endregion

        #region ParseSafe

        public static TMLNode ParseSafe(ReadOnlySpan<char> text) {
            if(!JsonTokenizer.TryTokenize(text, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch {
                return null;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static TMLNode ParseSafe(string text) {
            if(!JsonTokenizer.TryTokenize(text, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch {
                return null;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static TMLNode ParseSafe(ReadOnlySpan<char> text, Vector2Int range)
            => ParseSafe(text, range.x, range.y);

        public static TMLNode ParseSafe(string text, Vector2Int range)
            => ParseSafe(text, range.x, range.y);

        public static TMLNode ParseSafe(ReadOnlySpan<char> text, int start, int end) {
            if(!JsonTokenizer.TryTokenize(text, start, end, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch {
                return null;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static TMLNode ParseSafe(string text, int start, int end) {
            if(!JsonTokenizer.TryTokenize(text, start, end, out var tokens))
                return null;
            try {
                var node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return node;
            }
            catch {
                return null;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        #endregion

        #region TryParse

        public static bool TryParse(ReadOnlySpan<char> text, TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, out var tokens)) {
                return false;
            }
            try {
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, out var tokens)) {
                return false;
            }
            try {
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(ReadOnlySpan<char> text, out TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, out var tokens)) {
                node = null;
                return false;
            }
            try {
                node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, out TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, out var tokens)) {
                node = null;
                return false;
            }
            try {
                node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
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
            if(!JsonTokenizer.TryTokenize(text, start, end, out var tokens)) {
                return false;
            }
            try {
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, int start, int end, TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, start, end, out var tokens)) {
                return false;
            }
            try {
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(ReadOnlySpan<char> text, int start, int end, out TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, start, end, out var tokens)) {
                node = null;
                return false;
            }
            try {
                node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        public static bool TryParse(string text, int start, int end, out TMLNode node) {
            if(!JsonTokenizer.TryTokenize(text, start, end, out var tokens)) {
                node = null;
                return false;
            }
            try {
                node = new TMLNode("root");
                int index = 0;
                RecursiveCombine(text, tokens, ref index, node, false);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                node = new TMLNode("root");
                return false;
            }
            finally {
                JsonTokenizer.Return(tokens);
            }
        }

        #endregion

        #region Node generation

        private static void RecursiveCombine(ReadOnlySpan<char> text, List<JsonTokenizer.Token> tokens, ref int index, TMLNode node, bool isInArray) {
            var t = tokens[index];
            switch(t.Type) {
                case JsonTokenizer.TokenType.String: {
                        var s = new string(text.Slice(t.Start, t.End - t.Start));
                        if(isInArray) {
                            node.AddNode(s);
                            return;
                        }
                        var nextT = tokens[index + 1];
                        switch(nextT.Type) {
                            case JsonTokenizer.TokenType.Array: {
                                    var n = node.AddNode(s);
                                    index += 1;
                                    RecursiveCombine(text, tokens, ref index, n, false);
                                }
                                break;
                            case JsonTokenizer.TokenType.Class: {
                                    var n = node.AddNode(s);
                                    index += 1;
                                    RecursiveCombine(text, tokens, ref index, n, false);
                                }
                                break;
                            case JsonTokenizer.TokenType.Value:
                            case JsonTokenizer.TokenType.String:
                                node.AddProperty(s, new string(text.Slice(tokens[index + 1].Start, tokens[index + 1].End - tokens[index + 1].Start)));
                                index++;
                                break;
                        }
                    }
                    break;
                case JsonTokenizer.TokenType.Array: {
                        if(isInArray)
                            node = node.AddNode($"{node.Children.Count}");
                        while(index + 1 < t.End) {
                            index++;
                            RecursiveCombine(text, tokens, ref index, node, true);
                        }
                    }
                    break;
                case JsonTokenizer.TokenType.Class: {
                        if(isInArray)
                            node = node.AddNode($"{node.Children.Count}");
                        while(index + 1 < t.End) {
                            index++;
                            RecursiveCombine(text, tokens, ref index, node, false);
                        }
                    }
                    break;
                case JsonTokenizer.TokenType.Value: {
                        node.AddNode(new string(text.Slice(t.Start, t.End - t.Start)));
                    }
                    break;
            }
        }

        private static void RecursiveCombine(string text, List<JsonTokenizer.Token> tokens, ref int index, TMLNode node, bool isInArray) {
            var t = tokens[index];
            switch(t.Type) {
                case JsonTokenizer.TokenType.String: {
                        var s = text.Substring(t.Start, t.End - t.Start);
                        if(isInArray) {
                            node.AddNode(s);
                            return;
                        }
                        var nextT = tokens[index + 1];
                        switch(nextT.Type) {
                            case JsonTokenizer.TokenType.Array: {
                                    var n = node.AddNode(s);
                                    index += 1;
                                    RecursiveCombine(text, tokens, ref index, n, false);
                                }
                                break;
                            case JsonTokenizer.TokenType.Class: {
                                    var n = node.AddNode(s);
                                    index += 1;
                                    RecursiveCombine(text, tokens, ref index, n, false);
                                }
                                break;
                            case JsonTokenizer.TokenType.Value:
                            case JsonTokenizer.TokenType.String:
                                node.AddProperty(s, text.Substring(tokens[index + 1].Start, tokens[index + 1].End - tokens[index + 1].Start));
                                index++;
                                break;
                        }
                    }
                    break;
                case JsonTokenizer.TokenType.Array: {
                        if(isInArray)
                            node = node.AddNode($"{node.Children.Count}");
                        while(index + 1 < t.End) {
                            index++;
                            RecursiveCombine(text, tokens, ref index, node, true);
                        }
                    }
                    break;
                case JsonTokenizer.TokenType.Class: {
                        if(isInArray)
                            node = node.AddNode($"{node.Children.Count}");
                        while(index + 1 < t.End) {
                            index++;
                            RecursiveCombine(text, tokens, ref index, node, false);
                        }
                    }
                    break;
                case JsonTokenizer.TokenType.Value: {
                        node.AddNode(text.Substring(t.Start, t.End - t.Start));
                    }
                    break;
            }
        }

        #endregion


        #region Write

        public static void Write(TMLNode node) {
            StringBuilder sb = new StringBuilder();
        }

        private static void Write(TMLNode node, StringBuilder sb, int depth) {
            //sb.Append(StringUtil.Get('\t', depth));
            sb.AppendLine("{");

            var props = node.Properties;
            foreach(var p in props) {
                
            }

            var children = node.Children;
            foreach(var c in children) {

            }

            sb.Append(StringUtil.Get('\t', depth));
            sb.AppendLine("}");
        }

        #endregion
    }
}
