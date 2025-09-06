using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class CommandParser {
        #region Variables

        private const string TAG = ColorTable.RichTextTags.OLIVE + "[Toolkit.Debugging.CommandParser]</color> - ";
        private const int TOKEN_CLASS = 1;
        private const int TOKEN_STRING = 2;
        private const int TOKEN_VALUE = 3;

        private static FastPool<List<Vector3Int>> pool = new FastPool<List<Vector3Int>>();

        #endregion

        #region TryParse

        public static bool TryParse(string text, out TMLNode node) {
            try {
                List<Vector3Int> tokens = pool.Pop();
                int length = text.Length;
                int type = -1;
                int recursiveBacktrack = -1;
                bool token_class_isInsideString = false;
                int token_class_depth = 0;

                for(int i = 0; i < length; i++) {
                    var c = text[i];
                    switch(type) {
                        case -1: {
                                if(char.IsWhiteSpace(c))
                                    continue;
                                switch(c) {
                                    case '{': {
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Vector3Int(i, -1, TOKEN_CLASS));
                                            token_class_depth = 1;
                                            type = TOKEN_CLASS;
                                            token_class_isInsideString = false;
                                        }
                                        break;
                                    case '\"': {
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Vector3Int(i + 1, -1, TOKEN_STRING));
                                            type = TOKEN_STRING;
                                        }
                                        break;
                                    default: {
                                            recursiveBacktrack = tokens.Count;
                                            tokens.Add(new Vector3Int(i, -1, TOKEN_VALUE));
                                            type = TOKEN_VALUE;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TOKEN_CLASS: {
                                switch(c) {
                                    case '\"':
                                        token_class_isInsideString = !token_class_isInsideString;
                                        break;
                                    case '{':
                                        if(!token_class_isInsideString)
                                            token_class_depth++;
                                        break;
                                    case '}':
                                        if(!token_class_isInsideString)
                                            token_class_depth--;
                                        if(token_class_depth <= 0) {
                                            var t = tokens[recursiveBacktrack];
                                            t.y = i;
                                            tokens[recursiveBacktrack] = t;
                                            type = -1;
                                            recursiveBacktrack = -1;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TOKEN_STRING: {
                                switch(c) {
                                    case '\\':
                                        i++;
                                        continue;
                                    case '\"': {
                                            var t = tokens[recursiveBacktrack];
                                            t.y = i;
                                            tokens[recursiveBacktrack] = t;
                                            type = -1;
                                            recursiveBacktrack = -1;
                                        }
                                        break;
                                }
                            }
                            break;
                        case TOKEN_VALUE: {
                                switch(c) {
                                    case '{':
                                    case ' ': {
                                            var t = tokens[recursiveBacktrack];
                                            t.y = i;
                                            tokens[recursiveBacktrack] = t;
                                            type = -1;
                                            recursiveBacktrack = -1;
                                            i--;
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }

                if(recursiveBacktrack != -1) {
                    node = default;
                    if(tokens.Count == 0)
                        return false;
                    switch(tokens.Last().z) {
                        case TOKEN_STRING:
                        case TOKEN_VALUE: {
                                var t = tokens[recursiveBacktrack];
                                t.y = text.Length;
                                tokens[recursiveBacktrack] = t;
                            }
                            break;
                        case TOKEN_CLASS:
                            return false;
                    }
                }

                node = new TMLNode("root");
                for(int index = 0, tokenLen = tokens.Count; index < tokenLen; index++) {
                    CreateListFromTokens(text, tokens, ref index, node);
                }

                tokens.Clear();
                pool.Push(tokens);

                return true;
            }
            catch {
                node = new TMLNode("root");
                return false;
            }
        }

        private static void CreateListFromTokens(string text, List<Vector3Int> tokens, ref int index, TMLNode node) {
            var t = tokens[index];
            switch(t.z) {
                case TOKEN_STRING:
                case TOKEN_VALUE: {
                        var snode = node.AddNode(node.HasChildren ? "parameter" : "command");

                        var valuetext = text.Substring(t.x, t.y - t.x);
                        if(valuetext.StartsWith("var.") && CommandsVarStorage.TryGetValue(valuetext.Remove(0, 4), out string varValue, out var varData)) { // Replace input commands with
                            snode.AddProperty("value", varValue);
                            if(varData != null)
                                snode.AddNode(varData);
                        }
                        else
                            snode.AddProperty("value", valuetext);

                        if(tokens.Count > index + 1 && tokens[index + 1].z == TOKEN_CLASS) {
                            var ctoken = tokens[++index];
                            IO.TML.TMLJsonExtendedParser.TryParse(text, ctoken.x, ctoken.y + 1, snode.AddNode("data"));
                        }
                    }
                    break;
            }
        }

        #endregion


        #region Chained Command Parser

        public static bool TryExtractChainedCommands(ReadOnlySpan<char> commandsInput, out string[] commandsList) {
            try {
                commandsInput = commandsInput.Trim();
                int length = commandsInput.Length;
                int depth = 0;
                bool isInString = false;
                List<string> commands = new List<string>();
                if(commandsInput[0] == '\"') {
                    commandsInput = commandsInput.Slice(1, length - 2);
                    length -= 2;
                }

                int index = -1;

                for(int i = 0; i < length; i++) {
                    var c = commandsInput[i];
                    if(char.IsWhiteSpace(c))
                        continue;
                    switch(c) {
                        case '\"': {
                                isInString = !isInString;
                            }
                            break;
                        case '{': {
                                if(!isInString)
                                    depth++;
                            }
                            break;
                        case '}': {
                                if(!isInString)
                                    depth--;
                            }
                            break;
                        case ';': {
                                if(isInString || depth > 0 || index == -1)
                                    continue;
                                commands.Add(commandsInput.Slice(index, i - index).Trim().ToString().Replace("\\\"", "\""));
                                index = -1;
                            }
                            break;
                        default: {
                                if(index < 0)
                                    index = i;
                            }
                            break;
                    }
                }
                if(index != -1)
                    commands.Add(commandsInput.Slice(index, length - index).Trim().ToString().Replace("\\\"", "\""));
                commandsList = commands.ToArray();
                return true;
            }
            catch {
                commandsList = new string[0];
                return false;
            }
        }

        #endregion
    }
}
