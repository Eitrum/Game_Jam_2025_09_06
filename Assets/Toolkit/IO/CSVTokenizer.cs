// Uses Toolkit.FastPool
#define USE_POOLING

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO {
    public static class CSVTokenizer {
        #region Enum

        public enum TokenType {
            None,
            Row,
            Entry,
        }

        #endregion

        #region Token

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

            public string GetSubstring(string text) => Length <= 0 ? string.Empty : text.Substring(Start, Length);
            public ReadOnlySpan<char> GetSlice(Span<char> text) => Length <= 0 ? string.Empty.AsSpan() : text.Slice(Start, Length);
            public ReadOnlySpan<char> GetSlice(ReadOnlySpan<char> text) => text.Slice(Start, Length);

            public override string ToString() {
                return $"[{Start}->{End}:{Type}]";
            }
        }

        #endregion

        #region Variables

        private const string TAG = "<color=#808000>[Toolkit.IO.CSVTokenizer]</color> - ";
#if USE_POOLING
        private static FastPool<List<Token>> pool = new FastPool<List<Token>>();
#endif

        #endregion

        #region Helper

        /// <summary>
        /// Returns the token list to the pool
        /// </summary>
        public static void Return(List<Token> tokens) {
            if(tokens == null)
                return;
            tokens.Clear();
#if USE_POOLING
            pool.Push(tokens);
#endif
        }

        #endregion

        #region TryTokenize

        public static bool TryTokenize(ReadOnlySpan<char> text, out List<Token> tokens) {
#if USE_POOLING
            tokens = pool.Pop();
#else
            tokens = new List<Token>();
#endif
            try {
                int length = text.Length;
                TokenType type = TokenType.None;
                bool isstring = false;
                bool skipnextcomma = false;

                int lastRow = 0;
                tokens.Add(new Token(0, -1, TokenType.Row));

                lastRow = 0;
                for(int i = 0; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case TokenType.None: {
                                switch(c) {
                                    case '\r': { // Handle Mac Line Endings
                                            if(text[i + 1] == '\n') {
                                                var t = tokens[lastRow];
                                                t.End = i - 1;
                                                tokens[lastRow] = t;
                                                if(i + 2 < length) {
                                                    lastRow = tokens.Count;
                                                    tokens.Add(new Token(i + 2, -1, TokenType.Row));
                                                }
                                            }
                                            else {
                                                // Should not happen
                                                Debug.LogError(TAG + "\\r was in the text without the \\n character");
                                            }
                                        }
                                        break;
                                    case '\n': { // Windows Line Endings
                                            var t = tokens[lastRow];
                                            t.End = i - 1;
                                            tokens[lastRow] = t;
                                            if(i + 1 < length) {
                                                lastRow = tokens.Count;
                                                tokens.Add(new Token(i + 1, -1, TokenType.Row));
                                            }
                                        }
                                        break;
                                    case ',': {
                                            if(skipnextcomma)
                                                skipnextcomma = false;
                                            else
                                                tokens.Add(new Token(i, i, TokenType.Entry));
                                        }
                                        break;
                                    case '\"': {
                                            isstring = true;
                                            skipnextcomma = true;
                                            tokens.Add(new Token(i + 1, -1, TokenType.Entry));
                                            type = TokenType.Entry;
                                        }
                                        break;
                                    default: {
                                            // Ignore all remaining whitespaces
                                            if(char.IsWhiteSpace(c))
                                                break;
                                            isstring = false;
                                            tokens.Add(new Token(i, -1, TokenType.Entry));
                                            type = TokenType.Entry;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.Entry: {
                                switch(c) {
                                    case '\r': { // Handle Mac Line Endings
                                            if(isstring)
                                                break;
                                            if(text[i + 1] == '\n') {
                                                var et = tokens[tokens.Count-1];
                                                et.End = i;
                                                tokens[tokens.Count - 1] = et;
                                                var t = tokens[lastRow];
                                                t.End = i;
                                                tokens[lastRow] = t;
                                                //Debug.Log($"Adding token: '{et.GetSlice(text).ToString()}' len({et.Length})");
                                                if(i + 2 < length) {
                                                    lastRow = tokens.Count;
                                                    tokens.Add(new Token(i + 2, -1, TokenType.Row));

                                                }
                                                type = TokenType.None;
                                                i++;
                                            }
                                            else {
                                                // Should not happen
                                                Debug.LogError(TAG + "\\r was in the text without the \\n character");
                                            }
                                        }
                                        break;
                                    case '\n': { // Windows Line Endings
                                            if(isstring)
                                                break;
                                            var et = tokens[tokens.Count-1];
                                            et.End = i;
                                            tokens[tokens.Count - 1] = et;
                                            var t = tokens[lastRow];
                                            t.End = i;
                                            tokens[lastRow] = t;
                                            if(i + 1 < length) {
                                                lastRow = tokens.Count;
                                                tokens.Add(new Token(i + 1, -1, TokenType.Row));
                                            }
                                            type = TokenType.None;
                                        }
                                        break;
                                    case ',': {
                                            if(isstring)
                                                break;
                                            var et = tokens[tokens.Count-1];
                                            et.End = i;
                                            tokens[tokens.Count - 1] = et;
                                            type = TokenType.None;
                                        }
                                        break;
                                    case '\\':
                                        if(isstring)
                                            i++;
                                        break;
                                    case '\"': {
                                            var et = tokens[tokens.Count-1];
                                            et.End = i;
                                            tokens[tokens.Count - 1] = et;
                                            isstring = false;
                                            type = TokenType.None;
                                        }
                                        break;
                                    default:
                                        //if(!isstring && char.IsWhiteSpace(c)) {
                                        //    var et = tokens[tokens.Count-1];
                                        //    et.End = i;
                                        //    tokens[tokens.Count - 1] = et;
                                        //    type = TokenType.None;
                                        //}
                                        break;
                                }
                            }
                            break;
                    }
                }

                {
                    // Close last row token
                    var t = tokens[lastRow];
                    t.End = length;
                    tokens[lastRow] = t;
                }

                if(!isstring && text[length - 1] != '\"' && type != TokenType.None) {
                    var et = tokens[tokens.Count - 1];
                    et.End = length;
                    tokens[tokens.Count - 1] = et;
                    type = TokenType.None;
                }

                // Check if inside entry
                if(type != TokenType.None) {
                    tokens.Clear();
#if USE_POOLING
                    pool.Push(tokens);
#endif
                    tokens = null;
                    return false;
                }

                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                tokens.Clear();
#if USE_POOLING
                pool.Push(tokens);
#endif
                tokens = null;
                return false;
            }
        }

        #endregion
    }
}
