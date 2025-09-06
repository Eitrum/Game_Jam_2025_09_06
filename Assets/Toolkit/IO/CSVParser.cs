using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO {
    public static class CSVParser {

        #region Variables

        private const string TAG = "<color=#808000>[Toolkit.IO.CSVParser]</color> - ";

        #endregion

        #region Util

        private static List<string> GetAllEntries(string text, List<CSVTokenizer.Token> tokens, ref int index) {
            List<string> entries = new List<string>();
            for(int length = tokens.Count; index < length; index++) {
                var t = tokens[index];
                if(t.Type == CSVTokenizer.TokenType.Entry) {
                    entries.Add(t.GetSubstring(text));
                }
                else {
                    // Return 1 to not break other loops
                    index--;
                    break;
                }
            }
            return entries;
        }

        private static List<string> GetAllEntries(ReadOnlySpan<char> text, List<CSVTokenizer.Token> tokens, ref int index) {
            List<string> entries = new List<string>();
            for(int length = tokens.Count; index < length; index++) {
                var t = tokens[index];
                if(t.Type == CSVTokenizer.TokenType.Entry) {
                    entries.Add(t.GetSlice(text).ToString());
                }
                else {
                    // Return 1 to not break other loops
                    index--;
                    break;
                }
            }
            return entries;
        }

        #endregion

        #region KeyValuePair

        public static Dictionary<string, string> ToKeyValuePair(string text) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                Debug.LogError(TAG + "Failed to tokenize:\n" + text);
                return new Dictionary<string, string>();
            }
            try {
                var dict = new Dictionary<string, string>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Row) {
                        if(i + 2 < len) {
                            var key = tokens[i + 1];
                            var value = tokens[i + 2];

                            // Verify both key and value are entries
                            if(key.Type != CSVTokenizer.TokenType.Entry)
                                continue;
                            if(value.Type != CSVTokenizer.TokenType.Entry)
                                continue;

                            if(!dict.TryAdd(key.GetSubstring(text), value.GetSubstring(text)))
                                Debug.LogWarning(TAG + $"Duplicated key: '{key.GetSubstring(text)}'");
                        }
                        else
                            break;
                    }
                }
                return dict;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        public static Dictionary<string, string> ToKeyValuePair(ReadOnlySpan<char> text) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                Debug.LogError(TAG + "Failed to tokenize:\n" + text.ToString());
                return new Dictionary<string, string>();
            }
            try {
                var dict = new Dictionary<string, string>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Row) {
                        if(i + 2 < len) {
                            var key = tokens[i + 1];
                            var value = tokens[i + 2];

                            // Verify both key and value are entries
                            if(key.Type != CSVTokenizer.TokenType.Entry)
                                continue;
                            if(value.Type != CSVTokenizer.TokenType.Entry)
                                continue;

                            if(!dict.TryAdd(key.GetSlice(text).ToString(), value.GetSlice(text).ToString()))
                                Debug.LogWarning(TAG + $"Duplicated key: '{key.GetSlice(text).ToString()}'");
                        }
                        else
                            break;
                    }
                }
                return dict;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        #endregion

        #region KeyValueList

        /// <summary>
        /// Entry 1 as key.
        /// </summary>
        public static Dictionary<string, List<string>> ToKeyValueList(ReadOnlySpan<char> text) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                Debug.LogError(TAG + "Failed to tokenize:\n" + text.ToString());
                return new Dictionary<string, List<string>>();
            }
            try {
                var dict = new Dictionary<string, List<string>>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Row) {
                        if(i + 1 < len) {
                            var key = tokens[i + 1];
                            if(key.Type != CSVTokenizer.TokenType.Entry)
                                continue;
                            i++;
                            if(!dict.TryAdd(key.GetSlice(text).ToString(), GetAllEntries(text, tokens, ref i)))
                                Debug.LogWarning(TAG + $"Duplicated key: '{key.GetSlice(text).ToString()}'");
                        }
                        else
                            break;
                    }
                }
                return dict;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        /// <summary>
        /// Entry 1 as key.
        /// </summary>
        public static Dictionary<string, List<string>> ToKeyValueList(string text) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                Debug.LogError(TAG + "Failed to tokenize:\n" + text);
                return new Dictionary<string, List<string>>();
            }
            try {
                var dict = new Dictionary<string, List<string>>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Row) {
                        if(i + 1 < len) {
                            var key = tokens[i + 1];
                            if(key.Type != CSVTokenizer.TokenType.Entry)
                                continue;
                            i++;
                            if(!dict.TryAdd(key.GetSubstring(text), GetAllEntries(text, tokens, ref i)))
                                Debug.LogWarning(TAG + $"Duplicated key: '{key.GetSubstring(text)}'");
                        }
                        else
                            break;
                    }
                }
                return dict;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        #endregion

        #region List

        public static bool TryParse(string text, out List<List<string>> rows) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                //Debug.LogError(TAG + "Failed to tokenize:\n" + text.ToString());
                rows = null;
                return false;
            }
            try {
                rows = new List<List<string>>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Entry) {
                        rows.Add(GetAllEntries(text, tokens, ref i));
                    }
                }
                return true;
            }
            catch {
                rows = null;
                return false;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        public static List<List<string>> Parse(string text) {
            if(!TryParse(text, out var rows)) {
                Debug.LogError(TAG + "Failed to parse:\n" + text);
                return null;
            }
            return rows;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out List<List<string>> rows) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                //Debug.LogError(TAG + "Failed to tokenize:\n" + text.ToString());
                rows = null;
                return false;
            }
            try {
                rows = new List<List<string>>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Entry) {
                        rows.Add(GetAllEntries(text, tokens, ref i));
                    }
                }
                return true;
            }
            catch {
                rows = null;
                return false;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        public static List<List<string>> Parse(ReadOnlySpan<char> text) {
            if(!TryParse(text, out var rows)) {
                Debug.LogError(TAG + "Failed to parse:\n" + text.ToString());
                return null;
            }
            return rows;
        }

        #endregion

        #region Entries

        /// <summary>
        /// Linear list of entries only. This will ignore all row tokens.
        /// </summary>
        public static bool TryParseAsList(ReadOnlySpan<char> text, out List<string> entries) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                entries = null;
                return false;
            }
            try {
                entries = new List<string>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Entry)
                        entries.Add(t.GetSlice(text).ToString());
                }
                return true;
            }
            catch {
                entries = null;
                return false;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        /// <summary>
        /// Linear list of entries only. This will ignore all row tokens.
        /// </summary>
        public static List<string> ParseAsList(ReadOnlySpan<char> text) {
            if(!TryParseAsList(text, out var entries)) {
                Debug.LogError(TAG + "Failed to parse:\n" + text.ToString());
                return null;
            }
            return entries;
        }

        /// <summary>
        /// Linear list of entries only. This will ignore all row tokens.
        /// </summary>
        public static bool TryParseAsList(string text, out List<string> entries) {
            if(!CSVTokenizer.TryTokenize(text, out var tokens)) {
                entries = null;
                return false;
            }
            try {
                entries = new List<string>();
                for(int i = 0, len = tokens.Count; i < len; i++) {
                    var t = tokens[i];
                    if(t.Type == CSVTokenizer.TokenType.Entry)
                        entries.Add(t.GetSubstring(text));
                }
                return true;
            }
            catch {
                entries = null;
                return false;
            }
            finally {
                CSVTokenizer.Return(tokens);
            }
        }

        /// <summary>
        /// Linear list of entries only. This will ignore all row tokens.
        /// </summary>
        public static List<string> ParseAsList(string text) {
            if(!TryParseAsList(text, out var entries)) {
                Debug.LogError(TAG + "Failed to parse:\n" + text.ToString());
                return null;
            }
            return entries;
        }

        #endregion
    }
}
