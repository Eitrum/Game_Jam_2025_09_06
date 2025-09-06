using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Toolkit.IO {
    public static class JsonTokenizer {
        #region TokenType

        public enum TokenType : byte {
            None = 0,
            Class = 1,
            String = 2,
            Value = 3,
            Array = 4,
        }

        #endregion

        #region Token Struct

        [System.Serializable]
        public struct Token {
            public int Start, End;
            public TokenType Type;

            /// <summary>
            /// Calculated length from (End - Start)
            /// </summary>
            public int Length => End - Start;

            public Token(int start, int end, TokenType type) {
                this.Start = start;
                this.End = end;
                this.Type = type;
            }

            public string GetSubstring(string text) => text.Substring(Start, Length);
            public string GetSubstring(ReadOnlySpan<char> text) => text.Slice(Start, Length).ToString();
            public Span<char> GetSlice(Span<char> text) => text.Slice(Start, Length);
            public ReadOnlySpan<char> GetSlice(ReadOnlySpan<char> text) => text.Slice(Start, Length);

            public override string ToString() {
                return $"[{Start}->{End}:{Type}]";
            }
        }

        #endregion

        #region Variables

        private const string TAG = ColorTable.RichTextTags.OLIVE + "[Toolkit.IO.JsonTokenizer]</color> - ";
        private static FastPool<List<Token>> pool = new FastPool<List<Token>>();

        #endregion

        #region Helper

        /// <summary>
        /// Returns the token list to the pool
        /// </summary>
        public static void Return(List<Token> tokens) {
            if(tokens == null)
                return;
            tokens.Clear();
            pool.Push(tokens);
        }

        #endregion

        #region Tokenize

        public static bool TryTokenize(ReadOnlySpan<char> text, out List<Token> outputTokens) {
            List<Token> tokens = pool.Pop();
            try {
                int length = text.Length;
                TokenType type = TokenType.None;
                int recursiveBacktrack = -1;

                for(int i = 0; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case TokenType.None: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '{': {  // Class begin
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(recursiveBacktrack + 1, -1, TokenType.Class));
                                        }
                                        break;
                                    case '}': { // Class end
                                            for(int b = recursiveBacktrack; b >= 0; b--) {
                                                if(tokens[b].End == -1) {
                                                    var t = tokens[b];
                                                    t.End = tokens.Count;
                                                    tokens[b] = t;
                                                    recursiveBacktrack = b - 1;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    case ',':
                                    case ':': { } // legacy, used to be splitter tokens
                                        break;
                                    case '\"': { // String found
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(i + 1, -1, TokenType.String));
                                            type = TokenType.String;
                                        }
                                        break;
                                    case '[': { // Array start
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(recursiveBacktrack + 1, -1, TokenType.Array));
                                        }
                                        break;
                                    case ']': { // Array end
                                            for(int b = recursiveBacktrack; b >= 0; b--) {
                                                if(tokens[b].End == -1) {
                                                    var t = tokens[b];
                                                    t.End = tokens.Count;
                                                    tokens[b] = t;
                                                    recursiveBacktrack = b - 1;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    default: { // Should be for all the remaining value found
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(i, -1, TokenType.Value));
                                            type = TokenType.Value;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.String: {
                                if(c == '\\') {
                                    i++;
                                    continue;
                                }
                                if(c == '\"') {
                                    var t = tokens[recursiveBacktrack];
                                    t.End = i;
                                    tokens[recursiveBacktrack] = t;
                                    type = TokenType.None;
                                    recursiveBacktrack--;
                                }
                            }
                            break;
                        case TokenType.Value: {
                                if(char.IsWhiteSpace(c)) {
                                    var t = tokens[recursiveBacktrack];
                                    t.End = i;
                                    tokens[recursiveBacktrack] = t;
                                    type = TokenType.None;
                                    recursiveBacktrack--;
                                }
                                switch(c) {
                                    case ',':
                                    case ']':
                                    case '}':
                                        var t = tokens[recursiveBacktrack];
                                        t.End = i;
                                        tokens[recursiveBacktrack] = t;
                                        type = TokenType.None;
                                        recursiveBacktrack--;
                                        i--;
                                        break;
                                }
                            }
                            break;
                    }
                }

                if(recursiveBacktrack != -1) {
                    tokens.Clear();
                    pool.Push(tokens);
                    outputTokens = null;
                    return false;
                }

                outputTokens = tokens;
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                tokens.Clear();
                pool.Push(tokens);
                outputTokens = null;
                return false;
            }
        }

        public static bool TryTokenize(string text, out List<Token> outputTokens)
            => TryTokenize(text.AsSpan(), out outputTokens);

        public static List<Token> Tokenize(ReadOnlySpan<char> text)
            => TryTokenize(text, out var list) ? list : throw new Exception();

        public static List<Token> Tokenize(string text)
            => TryTokenize(text, out var list) ? list : throw new Exception();

        public static List<Token> TokenizeSafe(ReadOnlySpan<char> text)
            => TryTokenize(text, out var list) ? list : null;

        public static List<Token> TokenizeSafe(string text)
            => TryTokenize(text, out var list) ? list : null;

        #endregion

        #region Tokenize (Range)

        public static bool TryTokenize(ReadOnlySpan<char> text, Vector2Int range, out List<Token> outputTokens)
            => TryTokenize(text, range.x, range.y, out outputTokens);

        public static bool TryTokenize(ReadOnlySpan<char> text, int start, int end, out List<Token> outputTokens) {
            List<Token> tokens = pool.Pop();
            try {
                int length = Mathf.Min(end, text.Length);
                TokenType type = TokenType.None;
                int recursiveBacktrack = -1;

                for(int i = start; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case TokenType.None: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '{': {  // Class begin
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(recursiveBacktrack + 1, -1, TokenType.Class));
                                        }
                                        break;
                                    case '}': { // Class end
                                            for(int b = recursiveBacktrack; b >= 0; b--) {
                                                if(tokens[b].End == -1) {
                                                    var t = tokens[b];
                                                    t.End = tokens.Count;
                                                    tokens[b] = t;
                                                    recursiveBacktrack = b - 1;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    case ',':
                                    case ':': { } // legacy, used to be splitter tokens
                                        break;
                                    case '\"': { // String found
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(i + 1, -1, TokenType.String));
                                            type = TokenType.String;
                                        }
                                        break;
                                    case '[': { // Array start
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(recursiveBacktrack + 1, -1, TokenType.Array));
                                        }
                                        break;
                                    case ']': { // Array end
                                            for(int b = recursiveBacktrack; b >= 0; b--) {
                                                if(tokens[b].End == -1) {
                                                    var t = tokens[b];
                                                    t.End = tokens.Count;
                                                    tokens[b] = t;
                                                    recursiveBacktrack = b - 1;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    default: { // Should be for all the remaining value found
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Token(i, -1, TokenType.Value));
                                            type = TokenType.Value;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.String: {
                                if(c == '\\') {
                                    i++;
                                    continue;
                                }
                                if(c == '\"') {
                                    var t = tokens[recursiveBacktrack];
                                    t.End = i;
                                    tokens[recursiveBacktrack] = t;
                                    type = TokenType.None;
                                    recursiveBacktrack--;
                                }
                            }
                            break;
                        case TokenType.Value: {
                                if(char.IsWhiteSpace(c)) {
                                    var t = tokens[recursiveBacktrack];
                                    t.End = i;
                                    tokens[recursiveBacktrack] = t;
                                    type = TokenType.None;
                                    recursiveBacktrack--;
                                }
                                switch(c) {
                                    case ',':
                                    case ']':
                                    case '}':
                                        var t = tokens[recursiveBacktrack];
                                        t.End = i;
                                        tokens[recursiveBacktrack] = t;
                                        type = TokenType.None;
                                        recursiveBacktrack--;
                                        i--;
                                        break;
                                }
                            }
                            break;
                    }
                }

                if(recursiveBacktrack != -1) {
                    tokens.Clear();
                    pool.Push(tokens);
                    outputTokens = null;
                    return false;
                }

                outputTokens = tokens;
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                tokens.Clear();
                pool.Push(tokens);
                outputTokens = null;
                return false;
            }
        }

        public static bool TryTokenize(string text, Vector2Int range, out List<Token> tokens)
            => TryTokenize(text, range.x, range.y, out tokens);

        public static bool TryTokenize(string text, int start, int end, out List<Token> outputTokens)
            => TryTokenize(text.AsSpan(), start, end, out outputTokens);


        public static List<Token> Tokenize(ReadOnlySpan<char> text, Vector2Int range)
            => Tokenize(text, range.x, range.y);

        public static List<Token> Tokenize(ReadOnlySpan<char> text, int start, int end)
            => TryTokenize(text, start, end, out var list) ? list : throw new Exception();

        public static List<Token> TokenizeSafe(ReadOnlySpan<char> text, Vector2Int range)
            => TokenizeSafe(text, range.x, range.y);

        public static List<Token> TokenizeSafe(ReadOnlySpan<char> text, int start, int end)
            => TryTokenize(text, start, end, out var list) ? list : null;


        public static List<Token> Tokenize(string text, Vector2Int range)
            => Tokenize(text, range.x, range.y);

        public static List<Token> Tokenize(string text, int start, int end)
            => TryTokenize(text, start, end, out var list) ? list : throw new Exception();

        public static List<Token> TokenizeSafe(string text, Vector2Int range)
            => TokenizeSafe(text, range.x, range.y);

        public static List<Token> TokenizeSafe(string text, int start, int end)
            => TryTokenize(text, start, end, out var list) ? list : null;

        #endregion
    }
}
