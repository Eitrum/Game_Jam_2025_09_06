using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO {
    public static class TMLTokenizer {
        #region TokenType

        public enum TokenType {
            None = 0,
            /// <summary>
            /// Includes the start and end block. < --- />
            /// </summary>
            Block = 1,

            /// <summary>
            /// The name of the block or attribute. Does not include "" if the name is a string.
            /// </summary>
            Name = 2,

            /// <summary>
            /// Includes the name=value range.
            /// </summary>
            Attribute = 3,

            /// <summary>
            /// Includes the value part of an attribute. Does not include "" if the value is a string.
            /// </summary>
            Value = 4,

            /// <summary>
            /// Used during tokenization.
            /// </summary>
            BlockEnd = 5,
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
        private static FastPool<Stack<int>> stackpool = new FastPool<Stack<int>>();

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

        public static bool TryTokenize(string text, out List<Token> tokens) {
            tokens = pool.Pop();
            var stack = stackpool.Pop();
            try {
                int length = text.Length;
                TokenType type = TokenType.None;
                bool isstring = false;

                for(int i = 0; i < length; i++) {
                    var c = text[i];
                    if(c == '\\') {
                        i++;
                        continue;
                    }
                    switch(type) {
                        case TokenType.None: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '<': // Open new block or enter close block
                                        if(i + 1 < length && text[i + 1] == '/') {
                                            type = TokenType.BlockEnd;
                                        }
                                        else {
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Block));
                                            type = TokenType.Block;
                                        }
                                        break;
                                    case '/': // Close block
                                        if(i + 1 < length && text[i + 1] == '>') {
                                            var index = stack.Pop();
                                            var t = tokens[index];
                                            t.End = i + 2;
                                            tokens[index] = t;
                                            type = TokenType.None;
                                            i++;
                                        }
                                        else
                                            Debug.LogError(TAG + "tokenization can't happen");
                                        break;
                                    case '\"':

                                        break;
                                    case '>':

                                        break;
                                    default: {
                                            // Should be attribute blocks
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Attribute));
                                            tokens.Add(new Token(i, -1, TokenType.Name));
                                            type = TokenType.Name;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.Block: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                if(c == '\"') {
                                    tokens.Add(new Token(i + 1, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                                else {
                                    tokens.Add(new Token(i, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                            }
                            break;
                        case TokenType.BlockEnd: {
                                if(c != '>')
                                    continue;
                                var index = stack.Pop();
                                var t = tokens[index];
                                t.End = i + 1;
                                tokens[index] = t;
                                type = TokenType.None;
                            }
                            break;
                        case TokenType.Name: {
                                if(char.IsWhiteSpace(c)) {
                                    var index = tokens.Count - 1;
                                    var t = tokens[index];
                                    t.End = i;
                                    tokens[index] = t;
                                    type = TokenType.None;
                                }
                                else {
                                    switch(c) {
                                        case '/': {
                                                if(i + 1 < length && text[i + 1] == '>') {
                                                    {// Close name token
                                                        var index = tokens.Count - 1;
                                                        var t = tokens[index];
                                                        t.End = i;
                                                        tokens[index] = t;
                                                    }
                                                    {//close block token
                                                        var index = stack.Pop();
                                                        var t = tokens[index];
                                                        t.End = i + 2;
                                                        tokens[index] = t;
                                                    }
                                                    type = TokenType.None;
                                                    i++;
                                                }
                                            }
                                            break;
                                        case '>':
                                        case '\"': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.None;
                                            }
                                            break;
                                        case '=':
                                        case ':': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.Value;
                                                if(isstring = (text[i + 1] == '\"')) // Skip first string marker if any
                                                    i++;
                                                tokens.Add(new Token(i + 1, -1, TokenType.Value));
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case TokenType.Value: {
                                if(isstring) {
                                    if(c == '\"') {
                                        var index = tokens.Count - 1;
                                        var t = tokens[index];
                                        t.End = i;
                                        tokens[index] = t;
                                        type = TokenType.None;

                                        // Release attribute marker
                                        var attIndex = stack.Pop();
                                        var t2 = tokens[attIndex];
                                        t2.End = i;
                                        tokens[attIndex] = t2;
                                    }
                                }
                            }
                            break;
                    }
                }

                if(stack.Count > 0) {
                    tokens.Clear();
                    pool.Push(tokens);
                    stack.Clear();
                    stackpool.Push(stack);
                    tokens = null;
                    return false;
                }

                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                Debug.LogError(TAG + text);
                tokens.Clear();
                pool.Push(tokens);
                tokens = null;
                return false;
            }
            finally {
                stack.Clear();
                stackpool.Push(stack);
            }
        }

        public static bool TryTokenize(ReadOnlySpan<char> text, out List<Token> tokens) {
            tokens = pool.Pop();
            var stack = stackpool.Pop();
            try {
                int length = text.Length;
                TokenType type = TokenType.None;
                bool isstring = false;

                for(int i = 0; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case TokenType.None: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '<': // Open new block or enter close block
                                        if(i + 1 < length && text[i + 1] == '/') {
                                            type = TokenType.BlockEnd;
                                        }
                                        else {
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Block));
                                            type = TokenType.Block;
                                        }
                                        break;
                                    case '/': // Close block
                                        if(i + 1 < length && text[i + 1] == '>') {
                                            var index = stack.Pop();
                                            var t = tokens[index];
                                            t.End = i + 2;
                                            tokens[index] = t;
                                            type = TokenType.None;
                                            i++;
                                        }
                                        else
                                            Debug.LogError(TAG + "tokenization can't happen");
                                        break;
                                    case '\"':

                                        break;
                                    case '>':

                                        break;
                                    default: {
                                            // Should be attribute blocks
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Attribute));
                                            tokens.Add(new Token(i, -1, TokenType.Name));
                                            type = TokenType.Name;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.Block: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                if(c == '\"') {
                                    tokens.Add(new Token(i + 1, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                                else {
                                    tokens.Add(new Token(i, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                            }
                            break;
                        case TokenType.BlockEnd: {
                                if(c != '>')
                                    continue;
                                var index = stack.Pop();
                                var t = tokens[index];
                                t.End = i + 1;
                                tokens[index] = t;
                                type = TokenType.None;
                            }
                            break;
                        case TokenType.Name: {
                                if(char.IsWhiteSpace(c)) {
                                    var index = tokens.Count - 1;
                                    var t = tokens[index];
                                    t.End = i;
                                    tokens[index] = t;
                                    type = TokenType.None;
                                }
                                else {
                                    switch(c) {
                                        case '/': {
                                                if(i + 1 < length && text[i + 1] == '>') {
                                                    {// Close name token
                                                        var index = tokens.Count - 1;
                                                        var t = tokens[index];
                                                        t.End = i;
                                                        tokens[index] = t;
                                                    }
                                                    {//close block token
                                                        var index = stack.Pop();
                                                        var t = tokens[index];
                                                        t.End = i + 2;
                                                        tokens[index] = t;
                                                    }
                                                    type = TokenType.None;
                                                    i++;
                                                }
                                            }
                                            break;
                                        case '>':
                                        case '\"': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.None;
                                            }
                                            break;
                                        case '=':
                                        case ':': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.Value;
                                                if(isstring = (text[i + 1] == '\"')) // Skip first string marker if any
                                                    i++;
                                                tokens.Add(new Token(i + 1, -1, TokenType.Value));
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case TokenType.Value: {
                                if(isstring) {
                                    if(c == '\"') {
                                        var index = tokens.Count - 1;
                                        var t = tokens[index];
                                        t.End = i;
                                        tokens[index] = t;
                                        type = TokenType.None;

                                        // Release attribute marker
                                        var attIndex = stack.Pop();
                                        var t2 = tokens[attIndex];
                                        t2.End = i;
                                        tokens[attIndex] = t2;
                                    }
                                }
                            }
                            break;
                    }
                }

                if(stack.Count > 0) {
                    tokens.Clear();
                    pool.Push(tokens);
                    stack.Clear();
                    stackpool.Push(stack);
                    tokens = null;
                    return false;
                }

                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                tokens.Clear();
                pool.Push(tokens);
                tokens = null;
                return false;
            }
            finally {
                stack.Clear();
                stackpool.Push(stack);
            }
        }

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

        public static bool TryTokenize(ReadOnlySpan<char> text, int start, int end, out List<Token> tokens) {
            tokens = pool.Pop();
            var stack = stackpool.Pop();
            try {
                int length = Mathf.Min(end, text.Length);
                TokenType type = TokenType.None;
                bool isstring = false;

                for(int i = start; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case TokenType.None: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '<': // Open new block or enter close block
                                        if(i + 1 < length && text[i + 1] == '/') {
                                            type = TokenType.BlockEnd;
                                        }
                                        else {
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Block));
                                            type = TokenType.Block;
                                        }
                                        break;
                                    case '/': // Close block
                                        if(i + 1 < length && text[i + 1] == '>') {
                                            var index = stack.Pop();
                                            var t = tokens[index];
                                            t.End = i + 2;
                                            tokens[index] = t;
                                            type = TokenType.None;
                                            i++;
                                        }
                                        else
                                            Debug.LogError(TAG + "tokenization can't happen");
                                        break;
                                    case '\"':

                                        break;
                                    case '>':

                                        break;
                                    default: {
                                            // Should be attribute blocks
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Attribute));
                                            tokens.Add(new Token(i, -1, TokenType.Name));
                                            type = TokenType.Name;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.Block: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                if(c == '\"') {
                                    tokens.Add(new Token(i + 1, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                                else {
                                    tokens.Add(new Token(i, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                            }
                            break;
                        case TokenType.BlockEnd: {
                                if(c != '>')
                                    continue;
                                var index = stack.Pop();
                                var t = tokens[index];
                                t.End = i + 1;
                                tokens[index] = t;
                                type = TokenType.None;
                            }
                            break;
                        case TokenType.Name: {
                                if(char.IsWhiteSpace(c)) {
                                    var index = tokens.Count - 1;
                                    var t = tokens[index];
                                    t.End = i;
                                    tokens[index] = t;
                                    type = TokenType.None;
                                }
                                else {
                                    switch(c) {
                                        case '/': {
                                                if(i + 1 < length && text[i + 1] == '>') {
                                                    {// Close name token
                                                        var index = tokens.Count - 1;
                                                        var t = tokens[index];
                                                        t.End = i;
                                                        tokens[index] = t;
                                                    }
                                                    {//close block token
                                                        var index = stack.Pop();
                                                        var t = tokens[index];
                                                        t.End = i + 2;
                                                        tokens[index] = t;
                                                    }
                                                    type = TokenType.None;
                                                    i++;
                                                }
                                            }
                                            break;
                                        case '>':
                                        case '\"': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.None;
                                            }
                                            break;
                                        case '=':
                                        case ':': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.Value;
                                                if(isstring = (text[i + 1] == '\"')) // Skip first string marker if any
                                                    i++;
                                                tokens.Add(new Token(i + 1, -1, TokenType.Value));
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case TokenType.Value: {
                                if(isstring) {
                                    if(c == '\"') {
                                        var index = tokens.Count - 1;
                                        var t = tokens[index];
                                        t.End = i;
                                        tokens[index] = t;
                                        type = TokenType.None;

                                        // Release attribute marker
                                        var attIndex = stack.Pop();
                                        var t2 = tokens[attIndex];
                                        t2.End = i;
                                        tokens[attIndex] = t2;
                                    }
                                }
                            }
                            break;
                    }
                }

                if(stack.Count > 0) {
                    tokens.Clear();
                    pool.Push(tokens);
                    stack.Clear();
                    stackpool.Push(stack);
                    tokens = null;
                    return false;
                }

                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                tokens.Clear();
                pool.Push(tokens);
                tokens = null;
                return false;
            }
            finally {
                stack.Clear();
                stackpool.Push(stack);
            }
        }

        public static bool TryTokenize(string text, Vector2Int range, out List<Token> tokens)
            => TryTokenize(text, range.x, range.y, out tokens);

        public static bool TryTokenize(string text, int start, int end, out List<Token> tokens) {
            tokens = pool.Pop();
            var stack = stackpool.Pop();
            try {
                int length = Mathf.Min(end, text.Length);
                TokenType type = TokenType.None;
                bool isstring = false;

                for(int i = start; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case TokenType.None: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '<': // Open new block or enter close block
                                        if(i + 1 < length && text[i + 1] == '/') {
                                            type = TokenType.BlockEnd;
                                        }
                                        else {
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Block));
                                            type = TokenType.Block;
                                        }
                                        break;
                                    case '/': // Close block
                                        if(i + 1 < length && text[i + 1] == '>') {
                                            var index = stack.Pop();
                                            var t = tokens[index];
                                            t.End = i + 2;
                                            tokens[index] = t;
                                            type = TokenType.None;
                                            i++;
                                        }
                                        else
                                            Debug.LogError(TAG + "tokenization can't happen");
                                        break;
                                    case '\"':

                                        break;
                                    case '>':

                                        break;
                                    default: {
                                            // Should be attribute blocks
                                            stack.Push(tokens.Count);
                                            tokens.Add(new Token(i, -1, TokenType.Attribute));
                                            tokens.Add(new Token(i, -1, TokenType.Name));
                                            type = TokenType.Name;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TokenType.Block: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                if(c == '\"') {
                                    tokens.Add(new Token(i + 1, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                                else {
                                    tokens.Add(new Token(i, -1, TokenType.Name));
                                    type = TokenType.Name;
                                }
                            }
                            break;
                        case TokenType.BlockEnd: {
                                if(c != '>')
                                    continue;
                                var index = stack.Pop();
                                var t = tokens[index];
                                t.End = i + 1;
                                tokens[index] = t;
                                type = TokenType.None;
                            }
                            break;
                        case TokenType.Name: {
                                if(char.IsWhiteSpace(c)) {
                                    var index = tokens.Count - 1;
                                    var t = tokens[index];
                                    t.End = i;
                                    tokens[index] = t;
                                    type = TokenType.None;
                                }
                                else {
                                    switch(c) {
                                        case '/': {
                                                if(i + 1 < length && text[i + 1] == '>') {
                                                    {// Close name token
                                                        var index = tokens.Count - 1;
                                                        var t = tokens[index];
                                                        t.End = i;
                                                        tokens[index] = t;
                                                    }
                                                    {//close block token
                                                        var index = stack.Pop();
                                                        var t = tokens[index];
                                                        t.End = i + 2;
                                                        tokens[index] = t;
                                                    }
                                                    type = TokenType.None;
                                                    i++;
                                                }
                                            }
                                            break;
                                        case '>':
                                        case '\"': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.None;
                                            }
                                            break;
                                        case '=':
                                        case ':': {
                                                var index = tokens.Count - 1;
                                                var t = tokens[index];
                                                t.End = i;
                                                tokens[index] = t;
                                                type = TokenType.Value;
                                                if(isstring = (text[i + 1] == '\"')) // Skip first string marker if any
                                                    i++;
                                                tokens.Add(new Token(i + 1, -1, TokenType.Value));
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case TokenType.Value: {
                                if(isstring) {
                                    if(c == '\"') {
                                        var index = tokens.Count - 1;
                                        var t = tokens[index];
                                        t.End = i;
                                        tokens[index] = t;
                                        type = TokenType.None;

                                        // Release attribute marker
                                        var attIndex = stack.Pop();
                                        var t2 = tokens[attIndex];
                                        t2.End = i;
                                        tokens[attIndex] = t2;
                                    }
                                }
                            }
                            break;
                    }
                }

                if(stack.Count > 0) {
                    tokens.Clear();
                    pool.Push(tokens);
                    stack.Clear();
                    stackpool.Push(stack);
                    tokens = null;
                    return false;
                }

                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                tokens.Clear();
                pool.Push(tokens);
                tokens = null;
                return false;
            }
            finally {
                stack.Clear();
                stackpool.Push(stack);
            }
        }


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
