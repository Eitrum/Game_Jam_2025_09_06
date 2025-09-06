using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toolkit {
    public static class StringExtensions {
        #region Modify

        public static string ReplaceAt(this string str, int index, char c) {
            if(str == null) {
                throw new ArgumentNullException("str");
            }
            char[] chars = str.ToCharArray();
            chars[index] = c;
            return new string(chars);
        }

        public static string ToUpper(this string str, int index) => ReplaceAt(str, index, char.ToUpper(str[index]));
        public static string ToLower(this string str, int index) => ReplaceAt(str, index, char.ToLower(str[index]));

        public static string SplitPascalCase(this string str)
            => Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");

        public static string ToTitleCase(this string str)
            => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);

        #endregion

        #region Hashing

        // https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        // https://referencesource.microsoft.com/#mscorlib/system/string.cs
        public static int GetHash32(this string str) {
            unchecked {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        public static uint GetHashU32(this string str) {
            unchecked {
                uint hash1 = (5381 << 16) + 5381;
                uint hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        public static byte GetHash8(this string str) {
            unchecked {
                int hash1 = (531 << 3) + 531;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return (byte)((hash1 + (hash2 * 1566083941)) >> (3));
            }
        }

        public static short GetHash16(this string str) {
            unchecked {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return (short)((hash1 + (hash2 * 1566083941)));
            }
        }

        public static ushort GetHashU16(this string str) {
            unchecked {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return (ushort)((hash1 + (hash2 * 1566083941)));
            }
        }

        // No idea if it works or not, basically the 32 version with a larger number
        public static long GetHash64(this string str) {
            unchecked {
                long hash1 = (5381 << 16) + 5381;
                long hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941566083941L);
            }
        }

        #endregion

        #region Hex

        /// <summary>
        /// "ff032f"
        /// "ff00ff00"
        /// "=39ff19
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color32 HexToColor(this string str) {
            var l = str.Length;
            if(l == 6) {
                var result = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                return new Color32((byte)((result >> 16) & 0xff), (byte)((result >> 8) & 0xff), (byte)((result) & 0xff), 255);
            }
            else if(l == 8) {
                var result = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                return new Color32((byte)((result >> 24) & 0xff), (byte)((result >> 16) & 0xff), (byte)((result >> 8) & 0xff), (byte)((result) & 0xff));
            }
            else {
                var equalIndex = str.IndexOf('=');
                if(equalIndex >= 0) { // handle if equal sign exists.
                    if(equalIndex + 9 <= l && IsHex(str.Substring(equalIndex + 7, 2))) {
                        // check if its a 4 bytes hex or only 3
                        return HexToColor(str, equalIndex + 1, 8);
                    }
                    return HexToColor(str, equalIndex + 1, 6);
                }
            }
            return default;
        }

        public static Color32 HexToColor(this string str, int index, int length) {
            return HexToColor(str.Substring(index, length));
        }

        public static byte HexToByte(this string str) {
            return byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        public static byte HexToByte(this string str, int index) {
            return byte.Parse(str.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
        }

        public static bool IsHex(this string test) {
            return Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        #endregion

        #region Finding

        public static int FindNextWhiteSpace(this string str, int index) {
            if(index < 0)
                index = 0;
            while(index < str.Length) {
                if(char.IsWhiteSpace(str[index])) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static int FindNextWhiteSpaceOrPunctuation(this string str, int index) {
            if(index < 0)
                index = 0;
            while(index < str.Length) {
                if(char.IsWhiteSpace(str[index]) || char.IsPunctuation(str[index]))
                    return index;
                index++;
            }
            return -1;
        }

        public static string FindNextWord(this string str) => FindNextWord(str, 0);
        public static string FindNextWord(this string str, int index) => FindNextWord(str, ref index, out int length);
        public static string FindNextWord(this string str, ref int index) => FindNextWord(str, ref index, out int length);
        public static string FindNextWord(this string str, ref int index, out int length) {
            if(index < 0)
                index = 0;
            if(index >= str.Length || string.IsNullOrEmpty(str)) {
                // Invalid string
                Debug.LogWarning("Invalid string 'FindNextWord'");
                length = 0;
                return "";
            }
            if(index > 0)
                index = FindNextWhiteSpaceOrPunctuation(str, index);
            if(index == -1) {
                // Invalid string
                Debug.LogWarning("Invalid string 'FindNextWord'");
                length = 0;
                return "";
            }
            do {
                if(!(char.IsWhiteSpace(str[index]) || char.IsPunctuation(str[index])))
                    break;
            } while(++index < str.Length);
            var endOfWord = str.FindNextWhiteSpaceOrPunctuation(index);
            if(endOfWord == -1)
                endOfWord = str.Length;
            length = endOfWord - index;
            return str.Substring(index, length);
        }

        public static string[] FindWords(this string str) {
            List<string> words = new List<string>();
            int index = 0;
            while(index < str.Length) {
                words.Add(str.FindNextWord(ref index, out int length));
                index += length;
            }
            return words.ToArray();
        }

        public static int FindWords(this string str, IList<string> array) {
            if(array.IsReadOnly) {
                int index = 0;
                int element = 0;
                while(index < str.Length && element < array.Count) {
                    var word = str.FindNextWord(ref index, out int length);
                    if(!string.IsNullOrEmpty(word))
                        array[element++] = word;
                    index += length;
                }
                return element;
            }
            else {
                array.Clear();
                int index = 0;
                while(index < str.Length) {
                    var word = str.FindNextWord(ref index, out int length);
                    if(!string.IsNullOrEmpty(word))
                        array.Add(word);
                    index += length;
                }
                return array.Count;
            }
        }

        #endregion

        #region Limit

        public static string Limit(this string input, int amount) {
            if(input.Length <= amount)
                return input;
            return input.Substring(0, amount);
        }

        public static string Limit(this string input, int amount, string ending) {
            if(input.Length <= amount)
                return input;
            return input.Substring(0, amount - ending.Length) + ending;
        }

        #endregion

        #region SplitAt

        public static void SplitAt(this string input, int index, out string lhs, out string rhs) {
            if(index < 1) {
                lhs = string.Empty;
                rhs = input;
            }
            else if(index >= input.Length - 1) {
                lhs = input;
                rhs = string.Empty;
            }
            else {
                lhs = input.Substring(0, index);
                rhs = input.Substring(index + 1, input.Length - (index + 1));
            }
        }

        #endregion

        #region StringBuilder Indent

        public static void Indent(this StringBuilder sb, int amount)
            => Indent(sb, amount, "\t");

        public static void Indent(this StringBuilder sb, int amount, string m) {
            for(int i = 0; i < amount; i++)
                sb.Append(m);
        }

        #endregion

        #region Join

        public static string Join<T>(this IReadOnlyList<T> array, string seperator) {
            return string.Join(seperator, array);
        }

        public static string Join<T>(this IEnumerable<T> array, string seperator) {
            return string.Join(seperator, array);
        }

        public static string Join<T>(this IReadOnlyList<T> array, char seperator) {
            return string.Join(seperator, array);
        }

        public static string Join<T>(this IEnumerable<T> array, char seperator) {
            return string.Join(seperator, array);
        }

        #endregion
    }
}
