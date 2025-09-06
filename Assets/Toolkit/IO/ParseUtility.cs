using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Toolkit.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Toolkit {
    public static class ParseUtility {

        #region Format Rules

        /// <summary>
        /// 
        ///  Arrays
        ///     Open Close  = [ ]
        ///     Split       = |
        ///     Example:
        ///         [ "hello, world" | "mynameis" | 0.3 | 182 ]
        /// 
        ///  Complex Datatypes
        ///     Open close  = ( )
        ///     Split       = |
        ///     Split+Alt   = | or ,
        ///         Used for vector2,3,4 and quaternions.
        ///     Example:
        ///         ( 0.32 | -8.12 | "shol" )
        ///         ( 18 | ( "name" | "name2" ) | 17289.13 )
        /// 
        /// </summary>

        #endregion

        #region Consts

        private const string TAG =  "[ParseUtility] - ";

        private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Number | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.Float;
        private static readonly CultureInfo DEFAULT_CULTUREINFO = CultureInfo.InvariantCulture;

        public const char ARRAY_OPEN = '[';
        public const char ARRAY_CLOSE = ']';
        public const char ARRAY_SPLIT = '|';

        public const char COMPLEX_OPEN = '(';
        public const char COMPLEX_CLOSE = ')';
        public const char COMPLEX_SPLIT = '|';
        public const char COMPLEX_SPLIT_ALT = ',';

        #endregion

        #region Array Util

        public static bool IsArray(string input)
            => IsArray(input, 0, input.Length - 1);

        public static bool IsArray(string input, int startIndex, int endIndex) {
            if(input[startIndex] != ARRAY_OPEN)
                return false;
            if(input[endIndex] != ARRAY_CLOSE)
                return false;
            return true;
        }

        public static bool IsArray(ReadOnlySpan<char> text) {
            if(text[0] != ARRAY_OPEN)
                return false;
            if(text[text.Length - 1] != ARRAY_CLOSE)
                return false;
            return true;
        }

        #endregion

        #region Complex Util

        public static bool IsComplex(string input)
            => IsComplex(input, 0, input.Length - 1);

        public static bool IsComplex(string input, int startIndex, int endIndex) {
            if(input[startIndex] != COMPLEX_OPEN)
                return false;
            if(input[endIndex] != COMPLEX_CLOSE)
                return false;
            return true;
        }

        public static bool IsComplex(ReadOnlySpan<char> text) {
            if(text[0] != COMPLEX_OPEN)
                return false;
            if(text[text.Length - 1] != COMPLEX_CLOSE)
                return false;
            return true;
        }

        private static bool FindNextComplexValue(ReadOnlySpan<char> input, ref int index, out ReadOnlySpan<char> result) {
            if(input.IsEmpty || index > input.Length) {
                result = default;
                return false;
            }
            var len = input.Length;
            int start = -1;
            int end = -1;
            bool nonComplex = true;
            for(int i = index; i < len; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN: {
                            start = i + 1;
                            nonComplex = false;
                        }
                        break;
                    case COMPLEX_SPLIT:
                    case COMPLEX_CLOSE: {
                            if(start == -1) {
                                start = i + 1;
                            }
                            else {
                                if(nonComplex) {
                                    end = i - 1;
                                    index = i;
                                    i = len;
                                }
                                else
                                    start = i + 1;
                            }
                            nonComplex = false;
                        }
                        break;
                    default:
                        if(!char.IsWhiteSpace(input[i]))
                            nonComplex = true;
                        break;
                }
            }
            if(start == -1 || end == -1) {
                result = default;
                return false;
            }
            result = input.Slice(start, end - start + 1); // Requires +1 to calculate correctly
            return true;
        }

        private static bool FindNextComplexValueWithAlt(ReadOnlySpan<char> input, ref int index, out ReadOnlySpan<char> result) {
            if(input.IsEmpty || index > input.Length) {
                result = default;
                return false;
            }
            var len = input.Length;
            int start = -1;
            int end = -1;
            bool nonComplex = true;
            for(int i = index; i < len; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN: {
                            start = i + 1;
                            nonComplex = false;
                        }
                        break;
                    case COMPLEX_SPLIT:
                    case COMPLEX_CLOSE:
                    case COMPLEX_SPLIT_ALT: {
                            if(start == -1) {
                                start = i + 1;
                            }
                            else {
                                if(nonComplex) {
                                    end = i - 1;
                                    index = i;
                                    i = len;
                                }
                                else
                                    start = i + 1;
                            }
                            nonComplex = false;
                        }
                        break;
                    default:
                        if(!char.IsWhiteSpace(input[i]))
                            nonComplex = true;
                        break;
                }
            }
            if(start == -1 || end == -1) {
                result = default;
                return false;
            }
            result = input.Slice(start, end - start + 1); // Requires +1 to calculate correctly
            return true;
        }

        #endregion


        #region Bool

        public enum BoolParseMode {
            Default = 0,
            Uppercase = 1,

            PlusMinus = 2,
            YesNo = 3,
            YesNoShort = 4,

            Number = 5,
        }


        public static bool TryParse(string input, out bool[] results)
            => TryParse(input, false, out results);

        public static bool TryParse(string input, bool extendedList, out bool[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }
            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new bool[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), extendedList, out bool value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), extendedList, out bool lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out bool[] results)
                    => TryParse(input, false, out results);

        public static bool TryParse(ReadOnlySpan<char> input, bool extendedList, out bool[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }
            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new bool[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), extendedList, out bool value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), extendedList, out bool lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }


        public static bool TryParse(string input, out List<bool> results)
            => TryParse(input, false, out results);

        public static bool TryParse(string input, List<bool> results)
            => TryParse(input, false, results);

        public static bool TryParse(string input, bool extendedList, out List<bool> results) {
            results = new List<bool>();
            return TryParse(input, extendedList, results);
        }

        public static bool TryParse(string input, bool extendedList, List<bool> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), extendedList, out bool value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(false);
                    }

                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), extendedList, out bool lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(false);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<bool> results)
                    => TryParse(input, false, out results);

        public static bool TryParse(ReadOnlySpan<char> input, List<bool> results)
                    => TryParse(input, false, results);

        public static bool TryParse(ReadOnlySpan<char> input, bool extendedList, out List<bool> results) {
            results = new List<bool>();
            return TryParse(input, extendedList, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, bool extendedList, List<bool> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), extendedList, out bool value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(false);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), extendedList, out bool lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(false);
            }

            return true;
        }


        public static bool TryParse(string input, out bool result) {
            return bool.TryParse(input, out result);
        }

        public static bool TryParse(string input, bool extendedList, out bool result) {
            if(bool.TryParse(input, out result))
                return true;
            if(!extendedList || string.IsNullOrEmpty(input))
                return false;

            // if any text starts with y or n, 1 or 0, no matter the length, will count as true or false for optimization reasons.
            switch(input[0]) {
                case '+':
                case 'y':
                case '1': {
                        result = true;
                        return true;
                    }
                case '-':
                case 'n':
                case '0': {
                        result = false;
                        return true;
                    }
            }

            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out bool result) {
            return bool.TryParse(input, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, bool extendedList, out bool result) {
            if(bool.TryParse(input, out result))
                return true;
            if(!extendedList || input.IsEmpty)
                return false;

            // if any text starts with y or n, 1 or 0, no matter the length, will count as true or false for optimization reasons.
            switch(input[0]) {
                case '+':
                case 'y':
                case '1': {
                        result = true;
                        return true;
                    }
                case '-':
                case 'n':
                case '0': {
                        result = false;
                        return true;
                    }
            }

            return false;
        }


        public static string ToString(bool value) => value ? bool.TrueString : bool.FalseString;

        public static string ToString(bool value, BoolParseMode mode) {
            switch(mode) {
                case BoolParseMode.Default: return ToString(value);
                case BoolParseMode.Uppercase: return value ? "TRUE" : "FALSE";
                case BoolParseMode.PlusMinus: return value ? "+" : "-";
                case BoolParseMode.YesNo: return value ? "yes" : "no";
                case BoolParseMode.YesNoShort: return value ? "y" : "n";
                case BoolParseMode.Number: return value ? "1" : "0";
            }
            return ToString(value);
        }

        public static string ToString(bool value, string @true, string @false) => value ? @true : @false;

        #endregion

        #region Char // TODO: Implement string to char for arrays

        public static bool TryParse(string input, out char[] results) {
            if(!IsArray(input)) {
                if(string.IsNullOrEmpty(input)) {
                    results = null;
                    return false;
                }
                // Fallback string to char if not an array.
                results = input.ToCharArray();
                return true;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new char[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out char value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out char lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<char> results) {
            results = new List<char>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<char> results) {
            results.Clear();
            if(!IsArray(input)) {
                if(string.IsNullOrEmpty(input)) {
                    return false;
                }
                // Fallback string to char if not an array.
                results.AddRange(input);
                return true;
            }

            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out char value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add('\0');
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out char lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add('\0');
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out char[] results) {
            if(!IsArray(input)) {
                if(input.IsEmpty) {
                    results = null;
                    return false;
                }
                // Fallback string to char if not an array.
                results = input.ToArray();
                return true;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new char[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out char value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out char lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<char> results) {
            results = new List<char>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<char> results) {
            results.Clear();
            if(!IsArray(input)) {
                if(input.IsEmpty) {
                    return false;
                }
                // Fallback string to char if not an array.
                for(int i = 0, len = input.Length; i < len; i++)
                    results.Add(input[i]);
                return true;
            }

            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out char value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add('\0');
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out char lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add('\0');
            }

            return true;
        }


        public static bool TryParse(string input, out char result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }

            if(input.Length > 1 && input[0] == '\"') {
                if(input[1] == '\"')
                    result = '\0';
                else
                    result = input[1];
            }
            else
                result = input[0];
            return true;
        }

        public static bool TryParse(string input, int index, out char result) {
            if(string.IsNullOrEmpty(input) || index < 0 || index >= input.Length) {
                result = default;
                return false;
            }

            if(input.Length > 1 && input[index] == '\"' && index + 1 < input.Length) {
                if(input[index + 1] == '\"')
                    result = '\0';
                else
                    result = input[index + 1];
            }
            else
                result = input[index];
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out char result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }

            if(input.Length > 1 && input[0] == '\"') {
                if(input[1] == '\"')
                    result = '\0';
                else
                    result = input[1];
            }
            else
                result = input[0];
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, int index, out char result) {
            if(input.IsEmpty || index < 0 || index >= input.Length) {
                result = default;
                return false;
            }

            if(input.Length > 1 && input[index] == '\"' && index + 1 < input.Length) {
                if(input[index + 1] == '\"')
                    result = '\0';
                else
                    result = input[index + 1];
            }
            else
                result = input[index];
            return true;
        }

        #endregion

        #region Byte

        // Array
        public static bool TryParse(string input, out byte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new byte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out byte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out byte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<byte> results) {
            results = new List<byte>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<byte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out byte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out byte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out byte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new byte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out byte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out byte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<byte> results) {
            results = new List<byte>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<byte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out byte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out byte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out byte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new byte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out byte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out byte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<byte> results) {
            results = new List<byte>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<byte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out byte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out byte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out byte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new byte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out byte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out byte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<byte> results) {
            results = new List<byte>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<byte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out byte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out byte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out byte result) {
            return byte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out byte result) {
            return byte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out byte result) {
            if(byte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out byte result) {
            if(byte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(byte value) => value.ToString();

        #endregion

        #region SByte

        // Array
        public static bool TryParse(string input, out sbyte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new sbyte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out sbyte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out sbyte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<sbyte> results) {
            results = new List<sbyte>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<sbyte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out sbyte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out sbyte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out sbyte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new sbyte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out sbyte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out sbyte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<sbyte> results) {
            results = new List<sbyte>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<sbyte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out sbyte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out sbyte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out sbyte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new sbyte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out sbyte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out sbyte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<sbyte> results) {
            results = new List<sbyte>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<sbyte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out sbyte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out sbyte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out sbyte[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new sbyte[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out sbyte value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out sbyte lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<sbyte> results) {
            results = new List<sbyte>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<sbyte> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out sbyte value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out sbyte lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out sbyte result) {
            return sbyte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out sbyte result) {
            return sbyte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out sbyte result) {
            if(sbyte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out sbyte result) {
            if(sbyte.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(sbyte value) => value.ToString();

        #endregion

        #region Short

        // Array
        public static bool TryParse(string input, out short[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new short[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out short value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out short lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<short> results) {
            results = new List<short>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<short> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out short value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out short lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out short[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new short[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out short value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out short lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<short> results) {
            results = new List<short>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<short> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out short value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out short lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out short[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new short[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out short value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out short lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<short> results) {
            results = new List<short>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<short> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out short value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out short lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out short[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new short[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out short value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out short lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<short> results) {
            results = new List<short>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<short> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out short value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out short lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out short result) {
            return short.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out short result) {
            return short.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out short result) {
            if(short.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out short result) {
            if(short.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(short value) => value.ToString();

        #endregion

        #region UShort

        // Array
        public static bool TryParse(string input, out ushort[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ushort[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out ushort value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out ushort lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<ushort> results) {
            results = new List<ushort>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<ushort> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out ushort value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out ushort lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out ushort[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ushort[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out ushort value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out ushort lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<ushort> results) {
            results = new List<ushort>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<ushort> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out ushort value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out ushort lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out ushort[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ushort[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ushort value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ushort lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<ushort> results) {
            results = new List<ushort>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<ushort> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ushort value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ushort lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out ushort[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ushort[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ushort value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ushort lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<ushort> results) {
            results = new List<ushort>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<ushort> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ushort value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ushort lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out ushort result) {
            return ushort.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out ushort result) {
            return ushort.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out ushort result) {
            if(ushort.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out ushort result) {
            if(ushort.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(ushort value) => value.ToString();

        #endregion

        #region Int

        // Array
        public static bool TryParse(string input, out int[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new int[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out int value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out int lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<int> results) {
            results = new List<int>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<int> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out int value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out int lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out int[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new int[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out int value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out int lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<int> results) {
            results = new List<int>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<int> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out int value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out int lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out int[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new int[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out int value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out int lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<int> results) {
            results = new List<int>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<int> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out int value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out int lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out int[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new int[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out int value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out int lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<int> results) {
            results = new List<int>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<int> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out int value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out int lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out int result) {
            return int.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out int result) {
            return int.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out int result) {
            if(int.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out int result) {
            if(int.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(int value) => value.ToString();

        #endregion

        #region UInt

        // Array
        public static bool TryParse(string input, out uint[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new uint[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out uint value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out uint lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<uint> results) {
            results = new List<uint>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<uint> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out uint value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out uint lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out uint[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new uint[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out uint value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out uint lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<uint> results) {
            results = new List<uint>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<uint> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out uint value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out uint lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out uint[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new uint[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out uint value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out uint lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<uint> results) {
            results = new List<uint>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<uint> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out uint value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out uint lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out uint[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new uint[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out uint value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out uint lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<uint> results) {
            results = new List<uint>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<uint> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out uint value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out uint lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out uint result) {
            return uint.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out uint result) {
            return uint.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out uint result) {
            if(uint.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out uint result) {
            if(uint.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(uint value) => value.ToString();

        #endregion

        #region Long

        // Array
        public static bool TryParse(string input, out long[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new long[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out long value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out long lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<long> results) {
            results = new List<long>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<long> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out long value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out long lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out long[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new long[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out long value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out long lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<long> results) {
            results = new List<long>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<long> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out long value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out long lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out long[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new long[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out long value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out long lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<long> results) {
            results = new List<long>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<long> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out long value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out long lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out long[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new long[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out long value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out long lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<long> results) {
            results = new List<long>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<long> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out long value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out long lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out long result) {
            return long.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out long result) {
            return long.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out long result) {
            if(long.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out long result) {
            if(long.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(long value) => value.ToString();

        #endregion

        #region ULong

        // Array
        public static bool TryParse(string input, out ulong[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ulong[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out ulong value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out ulong lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<ulong> results) {
            results = new List<ulong>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<ulong> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out ulong value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out ulong lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out ulong[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ulong[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out ulong value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out ulong lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<ulong> results) {
            results = new List<ulong>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<ulong> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out ulong value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out ulong lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Array w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out ulong[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ulong[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ulong value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ulong lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, RoundingMode roundingMode, out List<ulong> results) {
            results = new List<ulong>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(string input, RoundingMode roundingMode, List<ulong> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ulong value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ulong lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out ulong[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new ulong[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ulong value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ulong lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out List<ulong> results) {
            results = new List<ulong>();
            return TryParse(input, roundingMode, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, List<ulong> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), roundingMode, out ulong value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), roundingMode, out ulong lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out ulong result) {
            return ulong.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out ulong result) {
            return ulong.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        // Singular w/rounding
        public static bool TryParse(string input, RoundingMode roundingMode, out ulong result) {
            if(ulong.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, RoundingMode roundingMode, out ulong result) {
            if(ulong.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result))
                return true;
            if(float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out float resFloat) && roundingMode.Convert(resFloat, out result))
                return true;
            return false;
        }


        public static string ToString(ulong value) => value.ToString();

        #endregion

        #region Float

        // Array
        public static bool TryParse(string input, out float[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new float[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out float value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out float lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<float> results) {
            results = new List<float>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<float> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out float value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out float lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out float[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new float[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out float value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out float lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<float> results) {
            results = new List<float>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<float> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out float value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out float lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out float result) {
            return float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out float result) {
            return float.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }


        public static string ToString(float value) => value.ToString();

        #endregion

        #region Double

        // Array
        public static bool TryParse(string input, out double[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new double[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out double value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out double lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<double> results) {
            results = new List<double>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<double> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out double value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out double lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out double[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new double[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out double value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out double lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<double> results) {
            results = new List<double>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<double> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out double value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out double lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out double result) {
            return double.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out double result) {
            return double.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }


        public static string ToString(double value) => value.ToString();

        #endregion

        #region Decimal

        // Array
        public static bool TryParse(string input, out decimal[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new decimal[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out decimal value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out decimal lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<decimal> results) {
            results = new List<decimal>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<decimal> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out decimal value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out decimal lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(0);
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out decimal[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;

            for(int i = 0; i < length; i++) {
                if(input[i] == ARRAY_SPLIT)
                    count++;
            }

            results = new decimal[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out decimal value)) {
                        results[resultIndex] = value;
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                    }
                    resultIndex++;
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out decimal lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<decimal> results) {
            results = new List<decimal>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<decimal> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }
            int length = input.Length;
            int startIndex = 1;

            for(int i = 1; i < length - 1; i++) {
                if(input[i] == ARRAY_SPLIT) {
                    if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out decimal value)) {
                        results.Add(value);
                    }
                    else {
                        Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                        results.Add(0);
                    }
                    startIndex = i + 1;
                    while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                        startIndex++;
                    }
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out decimal lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(0);
            }

            return true;
        }

        // Singular
        public static bool TryParse(string input, out decimal result) {
            return decimal.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out decimal result) {
            return decimal.TryParse(input, DEFAULT_NUMBER_STYLE, DEFAULT_CULTUREINFO, out result);
        }


        public static string ToString(decimal value) => value.ToString();

        #endregion

        #region DateTime // TODO: Implement it

        public static bool TryParse(string input, out TKDateTime[] results) {
            Debug.LogError(TAG + "DateTime[] parser not implemented");
            results = default;
            return false;
        }
        public static bool TryParse(ReadOnlySpan<char> input, out TKDateTime[] results) {
            Debug.LogError(TAG + "DateTime[] parser not implemented");
            results = default;
            return false;
        }

        public static bool TryParse(string input, out TKDateTime result) {
            if(TryParse(input, out long tick)) {
                result = new TKDateTime(tick);
                return true;
            }
            Debug.LogError(TAG + "DateTime parser not implemented");
            result = default;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out TKDateTime result) {
            if(TryParse(input, out long tick)) {
                result = new TKDateTime(tick);
                return true;
            }
            Debug.LogError(TAG + "DateTime parser not implemented");
            result = default;
            return false;
        }

        #endregion

        #region String

        public static bool TryParse(string input, out string[] results) {
            if(!IsArray(input)) {
                if(string.IsNullOrEmpty(input)) {
                    results = null;
                    return false;
                }
                // Fallback string to char if not an array.
                results = new string[] { input };
                return true;
            }

            int length = input.Length;
            int count = 0;
            bool inside = false;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case '\\':
                        if(inside) // To skip next element if \" or \\
                            i++;
                        break;
                    case '\"': // Toggle inside string or not.
                        inside = !inside;
                        break;
                    case ARRAY_SPLIT:
                        if(!inside)
                            count++;
                        break;
                }
            }

            results = new string[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            inside = false;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length - 1; i++) {
                switch(input[i]) {
                    case '\\':
                        if(inside)
                            i++;
                        break;
                    case '\"':
                        inside = !inside;
                        break;
                    case ARRAY_SPLIT:
                        if(!inside) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out string value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex)}");
                            }
                            resultIndex++;
                            startIndex = i + 1;
                            while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                                startIndex++;
                            }
                        }
                        break;
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out string lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
            }

            return true;
        }

        public static bool TryParse(string input, out List<string> results) {
            results = new List<string>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<string> results) {
            results.Clear();
            if(!IsArray(input)) {
                if(string.IsNullOrEmpty(input)) {
                    results = null;
                    return false;
                }
                // Fallback string to char if not an array.
                results.Add(input);
                return true;
            }

            int length = input.Length;
            bool inside = false;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length - 1; i++) {
                switch(input[i]) {
                    case '\\':
                        if(inside)
                            i++;
                        break;
                    case '\"':
                        inside = !inside;
                        break;
                    case ARRAY_SPLIT:
                        if(!inside) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex).Trim(), out string value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex)}");
                                results.Add(string.Empty);
                            }
                            startIndex = i + 1;
                            while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                                startIndex++;
                            }
                        }
                        break;
                }
            }

            if(TryParse(inputSpan.Slice(startIndex, length - startIndex - 1).Trim(), out string lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Substring(startIndex, length - startIndex - 1)}");
                results.Add(string.Empty);
            }

            return true;
        }


        public static bool TryParse(ReadOnlySpan<char> input, out string[] results) {
            if(!IsArray(input)) {
                if(input.IsEmpty) {
                    results = null;
                    return false;
                }
                // Fallback string to char if not an array.
                results = new string[] { input.ToString() };
                return true;
            }

            int length = input.Length;
            int count = 0;
            bool inside = false;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case '\\':
                        if(inside) // To skip next element if \" or \\
                            i++;
                        break;
                    case '\"': // Toggle inside string or not.
                        inside = !inside;
                        break;
                    case ARRAY_SPLIT:
                        if(!inside)
                            count++;
                        break;
                }
            }

            results = new string[count + 1];
            int resultIndex = 0;
            int startIndex = 1;
            inside = false;

            for(int i = 0; i < length - 1; i++) {
                switch(input[i]) {
                    case '\\':
                        if(inside)
                            i++;
                        break;
                    case '\"':
                        inside = !inside;
                        break;
                    case ARRAY_SPLIT:
                        if(!inside) {
                            if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out string value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex).ToString()}");
                            }
                            resultIndex++;
                            startIndex = i + 1;
                            while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                                startIndex++;
                            }
                        }
                        break;
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out string lastValue)) {
                results[resultIndex] = lastValue;
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<string> results) {
            results = new List<string>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<string> results) {
            results.Clear();
            if(!IsArray(input)) {
                if(input.IsEmpty) {
                    results = null;
                    return false;
                }
                // Fallback string to char if not an array.
                results.Add(input.ToString());
                return true;
            }

            int length = input.Length;
            bool inside = false;
            int startIndex = 1;

            for(int i = 0; i < length - 1; i++) {
                switch(input[i]) {
                    case '\\':
                        if(inside)
                            i++;
                        break;
                    case '\"':
                        inside = !inside;
                        break;
                    case ARRAY_SPLIT:
                        if(!inside) {
                            if(TryParse(input.Slice(startIndex, i - startIndex).Trim(), out string value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex).ToString()}");
                                results.Add(string.Empty);
                            }
                            startIndex = i + 1;
                            while(startIndex < length - 1 && char.IsWhiteSpace(input[startIndex])) {
                                startIndex++;
                            }
                        }
                        break;
                }
            }

            if(TryParse(input.Slice(startIndex, length - startIndex - 1).Trim(), out string lastValue)) {
                results.Add(lastValue);
            }
            else {
                Debug.LogWarning(TAG + $"Failed to parse last element: {input.Slice(startIndex, length - startIndex - 1).ToString()}");
                results.Add(string.Empty);
            }

            return true;
        }


        /// <summary>
        /// Wrapper to trim/remove "" from input strings.
        /// </summary>
        public static bool TryParse(ReadOnlySpan<char> input, out string result) {
            if(input.IsEmpty) {
                result = string.Empty;
                return false;
            }
            result = input.Trim('\"').ToString();
            return true;
        }

        /// <summary>
        /// Wrapper to trim/remove "" from input strings.
        /// </summary>
        public static bool TryParse(string input, out string result) {
            if(string.IsNullOrEmpty(input)) {
                result = input;
                return false;
            }
            result = input.Trim('\"');
            return true;
        }


        public static string ToString(IReadOnlyList<string> value) => ParseUtility.ARRAY_OPEN + $"{string.Join(ARRAY_SPLIT, value)}" + ParseUtility.ARRAY_CLOSE;

        #endregion


        #region Vector2

        /// 
        ///     Array only supports depth 0 for complex parsing.
        /// 

        public static bool TryParse(string input, out Vector2[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Vector2[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Vector2 value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex + 1)}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(string input, out List<Vector2> results) {
            results = new List<Vector2>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Vector2> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Vector2 value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex + 1)}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Vector2[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Vector2[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Vector2 value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Vector2> results) {
            results = new List<Vector2>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Vector2> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Vector2 value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Vector2 result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1))) {
                result = default;
                return false;
            }

            result = new Vector2(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f);
            return true;
        }

        public static bool TryParse(string input, ref int index, out Vector2 result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1))) {
                result = default;
                return false;
            }

            result = new Vector2(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Vector2 result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1))) {
                result = default;
                return false;
            }

            result = new Vector2(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, ref int index, out Vector2 result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1))) {
                result = default;
                return false;
            }

            result = new Vector2(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f);
            return true;
        }


        public static string ToString(Vector2 value) => $"({value.x}|{value.y})";
        public static string ToString(Vector2 value, bool useAlt) => useAlt ? $"({value.x}, {value.y})" : $"({value.x}|{value.y})";
        public static string ToString(Vector2 value, int decimals, bool useAlt) => useAlt ?
            $"({Math.Round(value.x, decimals)}, {Math.Round(value.y, decimals)})" :
            $"({Math.Round(value.x, decimals)}|{Math.Round(value.y, decimals)})";

        #endregion

        #region Vector3

        /// 
        ///     Array only supports depth 0 for complex parsing.
        /// 

        public static bool TryParse(string input, out Vector3[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Vector3[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Vector3 value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex + 1)}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(string input, out List<Vector3> results) {
            results = new List<Vector3>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Vector3> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Vector3 value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex + 1)}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Vector3[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Vector3[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Vector3 value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Vector3> results) {
            results = new List<Vector3>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Vector3> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Vector3 value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Vector3 result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2))) {
                result = default;
                return false;
            }

            result = new Vector3(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f);
            return true;
        }

        public static bool TryParse(string input, ref int index, out Vector3 result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2))) {
                result = default;
                return false;
            }

            result = new Vector3(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Vector3 result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2))) {
                result = default;
                return false;
            }

            result = new Vector3(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, ref int index, out Vector3 result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2))) {
                result = default;
                return false;
            }

            result = new Vector3(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f);
            return true;
        }


        public static string ToString(Vector3 value) => $"({value.x}|{value.y}|{value.z})";
        public static string ToString(Vector3 value, bool useAlt) => useAlt ? $"({value.x}, {value.y}, {value.z})" : $"({value.x}|{value.y}|{value.z})";
        public static string ToString(Vector3 value, int decimals, bool useAlt) => useAlt ?
            $"({Math.Round(value.x, decimals)}, {Math.Round(value.y, decimals)}, {Math.Round(value.z, decimals)})" :
            $"({Math.Round(value.x, decimals)}|{Math.Round(value.y, decimals)}|{Math.Round(value.z, decimals)})";

        #endregion

        #region Vector4 

        /// 
        ///     Array only supports depth 0 for complex parsing.
        /// 

        public static bool TryParse(string input, out Vector4[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Vector4[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Vector4 value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex + 1)}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(string input, out List<Vector4> results) {
            results = new List<Vector4>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Vector4> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Vector4 value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex + 1)}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Vector4[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Vector4[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Vector4 value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Vector4> results) {
            results = new List<Vector4>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Vector4> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Vector4 value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Vector4 result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Vector4(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static bool TryParse(string input, ref int index, out Vector4 result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Vector4(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Vector4 result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Vector4(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, ref int index, out Vector4 result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Vector4(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }


        public static string ToString(Vector4 value) => $"({value.x}|{value.y}|{value.z}|{value.w})";
        public static string ToString(Vector4 value, bool useAlt) => useAlt ? $"({value.x}, {value.y}, {value.z}, {value.w})" : $"({value.x}|{value.y}|{value.z}|{value.w})";
        public static string ToString(Vector4 value, int decimals, bool useAlt) => useAlt ?
            $"({Math.Round(value.x, decimals)}, {Math.Round(value.y, decimals)}, {Math.Round(value.z, decimals)}, {Math.Round(value.w, decimals)})" :
            $"({Math.Round(value.x, decimals)}|{Math.Round(value.y, decimals)}|{Math.Round(value.z, decimals)}|{Math.Round(value.w, decimals)})";

        #endregion

        #region Quaternion 

        /// 
        ///     Array only supports depth 0 for complex parsing.
        /// 

        public static bool TryParse(string input, out Quaternion[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Quaternion[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Quaternion value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex + 1)}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(string input, out List<Quaternion> results) {
            results = new List<Quaternion>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Quaternion> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Quaternion value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex + 1)}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Quaternion[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Quaternion[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Quaternion value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Quaternion> results) {
            results = new List<Quaternion>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Quaternion> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Quaternion value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Quaternion result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Quaternion(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static bool TryParse(string input, ref int index, out Quaternion result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Quaternion(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Quaternion result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Quaternion(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, ref int index, out Quaternion result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Quaternion(
                TryParse(value0, out float res0) ? res0 : 0.0f,
                TryParse(value1, out float res1) ? res1 : 0.0f,
                TryParse(value2, out float res2) ? res2 : 0.0f,
                TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }


        public static string ToString(Quaternion value) => $"({value.x}|{value.y}|{value.z}|{value.w})";
        public static string ToString(Quaternion value, bool useAlt) => useAlt ? $"({value.x}, {value.y}, {value.z}, {value.w})" : $"({value.x}|{value.y}|{value.z}|{value.w})";

        #endregion


        #region Pose 

        /// 
        ///     Array only supports depth 0 for complex parsing.
        /// 

        public static bool TryParse(string input, out Pose[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Pose[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Pose value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Substring(startIndex, i - startIndex + 1)}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(string input, out List<Pose> results) {
            results = new List<Pose>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Pose> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(inputSpan.Slice(startIndex, i - startIndex + 1).Trim(), out Pose value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Substring(startIndex, i - startIndex + 1)}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Pose[] results) {
            if(!IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Pose[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Pose value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Pose> results) {
            results = new List<Pose>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Pose> results) {
            results.Clear();
            if(!IsArray(input)) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Pose value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Pose result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value4) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value5) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value6))) {
                result = default;
                return false;
            }

            result = new Pose(
                new Vector3(
                    TryParse(value0, out float res0) ? res0 : 0.0f,
                    TryParse(value1, out float res1) ? res1 : 0.0f,
                    TryParse(value2, out float res2) ? res2 : 0.0f),
                new Quaternion(
                    TryParse(value3, out float res3) ? res3 : 0.0f,
                    TryParse(value4, out float res4) ? res4 : 0.0f,
                    TryParse(value5, out float res5) ? res5 : 0.0f,
                    TryParse(value6, out float res6) ? res6 : 0.0f));
            return true;
        }

        public static bool TryParse(string input, ref int index, out Pose result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value4) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value5) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value6))) {
                result = default;
                return false;
            }

            result = new Pose(
                new Vector3(
                    TryParse(value0, out float res0) ? res0 : 0.0f,
                    TryParse(value1, out float res1) ? res1 : 0.0f,
                    TryParse(value2, out float res2) ? res2 : 0.0f),
                new Quaternion(
                    TryParse(value3, out float res3) ? res3 : 0.0f,
                    TryParse(value4, out float res4) ? res4 : 0.0f,
                    TryParse(value5, out float res5) ? res5 : 0.0f,
                    TryParse(value6, out float res6) ? res6 : 0.0f));
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Pose result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value4) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value5) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value6))) {
                result = default;
                return false;
            }

            result = new Pose(
                new Vector3(
                    TryParse(value0, out float res0) ? res0 : 0.0f,
                    TryParse(value1, out float res1) ? res1 : 0.0f,
                    TryParse(value2, out float res2) ? res2 : 0.0f),
                new Quaternion(
                    TryParse(value3, out float res3) ? res3 : 0.0f,
                    TryParse(value4, out float res4) ? res4 : 0.0f,
                    TryParse(value5, out float res5) ? res5 : 0.0f,
                    TryParse(value6, out float res6) ? res6 : 0.0f));
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, ref int index, out Pose result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value4) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value5) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value6))) {
                result = default;
                return false;
            }

            result = new Pose(
                new Vector3(
                    TryParse(value0, out float res0) ? res0 : 0.0f,
                    TryParse(value1, out float res1) ? res1 : 0.0f,
                    TryParse(value2, out float res2) ? res2 : 0.0f),
                new Quaternion(
                    TryParse(value3, out float res3) ? res3 : 0.0f,
                    TryParse(value4, out float res4) ? res4 : 0.0f,
                    TryParse(value5, out float res5) ? res5 : 0.0f,
                    TryParse(value6, out float res6) ? res6 : 0.0f));
            return true;
        }


        public static string ToString(Pose value) => $"({value.position.x}|{value.position.y}|{value.position.z}|{value.rotation.x}|{value.rotation.y}|{value.rotation.z}|{value.rotation.w})";
        public static string ToString(Pose value, bool useAlt) => useAlt ? $"({value.position.x}, {value.position.y}, {value.position.z}, {value.rotation.x}, {value.rotation.y}, {value.rotation.z}, {value.rotation.w})" : ToString(value);

        #endregion

        #region Colors

        public static bool TryParse(string input, out Color[] results)
            => TryParse(input, false, out results);

        public static bool TryParse(string input, bool allowColorTable, out Color[] results) {
            return TryParse(input.AsSpan(), allowColorTable, out results);
        }

        public static bool TryParse(string input, out List<Color> results) {
            results = new List<Color>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, bool allowColorTable, out List<Color> results) {
            results = new List<Color>();
            return TryParse(input, allowColorTable, results);
        }

        public static bool TryParse(string input, List<Color> results)
            => TryParse(input, false, results);

        public static bool TryParse(string input, bool allowColorTable, List<Color> results) {
            return TryParse(input.AsSpan(), allowColorTable, results);
        }


        public static bool TryParse(ReadOnlySpan<char> input, out Color[] results)
            => TryParse(input, out results);

        public static bool TryParse(ReadOnlySpan<char> input, bool allowColorTable, out Color[] results) {
            if(input.IsEmpty) {
                results = default;
                return false;
            }

            int count = 0;
            int depth = 0;
            int length = input.Length;
            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        break;
                    case ARRAY_SPLIT:
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Color[count + 1];
            int resultIndex = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        if(depth == 0)
                            startIndex = i + 1;
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        break;
                    case ARRAY_SPLIT:
                        if(depth == 0) {
                            ReadOnlySpan<char> temporary = input[i - 1] == COMPLEX_CLOSE ?
                                input.Slice(startIndex, i - startIndex - 1) :
                                input.Slice(startIndex, i - startIndex);
                            if(TryParse(temporary, allowColorTable, out Color res)) {
                                results[resultIndex] = res;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {temporary.ToString()}");
                            }
                            resultIndex++;
                            startIndex = i + 1;
                            while(startIndex < length && char.IsWhiteSpace(input[startIndex])) {
                                startIndex++;
                            }
                        }
                        break;
                }
            }

            if(depth == 0) {
                ReadOnlySpan<char> temporary = input[length-2] == COMPLEX_CLOSE ?
                                input.Slice(startIndex, length - startIndex - 2) :
                                input.Slice(startIndex, length - startIndex - 1);
                if(TryParse(temporary, allowColorTable, out Color res)) {
                    results[resultIndex] = res;
                }
                else {
                    Debug.LogWarning(TAG + $"Failed to parse element last element: {temporary.ToString()}");
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Color> results) {
            results = new List<Color>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, bool allowColorTable, out List<Color> results) {
            results = new List<Color>();
            return TryParse(input, allowColorTable, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Color> results)
            => TryParse(input, false, results);

        public static bool TryParse(ReadOnlySpan<char> input, bool allowColorTable, List<Color> results) {
            results.Clear();
            if(input.IsEmpty) {
                return false;
            }
            int depth = 0;
            int length = input.Length;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        if(depth == 0)
                            startIndex = i + 1;
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        break;
                    case ARRAY_SPLIT:
                        if(depth == 0) {
                            ReadOnlySpan<char> temporary = input[i - 1] == COMPLEX_CLOSE ?
                                input.Slice(startIndex, i - startIndex - 1) :
                                input.Slice(startIndex, i - startIndex);
                            if(TryParse(temporary, allowColorTable, out Color res)) {
                                results.Add(res);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {temporary.ToString()}");
                                results.Add(res);
                            }
                            startIndex = i + 1;
                            while(startIndex < length && char.IsWhiteSpace(input[startIndex])) {
                                startIndex++;
                            }
                        }
                        break;
                }
            }

            if(depth == 0) {
                ReadOnlySpan<char> temporary = input[length-2] == COMPLEX_CLOSE ?
                                input.Slice(startIndex, length - startIndex - 2) :
                                input.Slice(startIndex, length - startIndex - 1);
                if(TryParse(temporary, allowColorTable, out Color res)) {
                    results.Add(res);
                }
                else {
                    Debug.LogWarning(TAG + $"Failed to parse element last element: {temporary.ToString()}");
                    results.Add(res);
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Color result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            /// Unity build-in is incredibly fast
            if(ColorUtility.TryParseHtmlString(input, out result))
                return true;

            if(!uint.TryParse(input, out uint res)) {
                result = default;
                return false;
            }

            result = res.ToColor();
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Color result)
            => TryParse(input, false, out result);

        public static bool TryParse(ReadOnlySpan<char> input, bool allowColorTable, out Color result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }

            if(input[0] == COMPLEX_OPEN) {

            }

            switch(input.Length) {
                case 6: // RGB
                    {
                        if(TryParseHexValue(input.Slice(0, 2), out byte r) &&
                            TryParseHexValue(input.Slice(2, 2), out byte g) &&
                            TryParseHexValue(input.Slice(4, 2), out byte b)) {
                            result = new Color32(r, g, b, 255);
                            return true;
                        }
                    }
                    break;
                case 7: // #RGB
                    {
                        if(TryParseHexValue(input.Slice(1, 2), out byte r) &&
                           TryParseHexValue(input.Slice(3, 2), out byte g) &&
                            TryParseHexValue(input.Slice(5, 2), out byte b)) {
                            result = new Color32(r, g, b, 255);
                            return true;
                        }
                    }
                    break;
                case 8: // RGBA
                    {
                        if(TryParseHexValue(input.Slice(0, 2), out byte r) &&
                            TryParseHexValue(input.Slice(2, 2), out byte g) &&
                            TryParseHexValue(input.Slice(4, 2), out byte b) &&
                            TryParseHexValue(input.Slice(6, 2), out byte a)) {
                            result = new Color32(r, g, b, a);
                            return true;
                        }
                    }
                    break;
                case 9: // #RGBA
                    {
                        if(TryParseHexValue(input.Slice(1, 2), out byte r) &&
                            TryParseHexValue(input.Slice(3, 2), out byte g) &&
                            TryParseHexValue(input.Slice(5, 2), out byte b) &&
                            TryParseHexValue(input.Slice(7, 2), out byte a)) {
                            result = new Color32(r, g, b, a);
                            return true;
                        }
                    }
                    break;
            }


            if(allowColorTable) {
                var type = FastStringParse_ColorTableType.GetColorTableType(input);
                result = ColorTable.ToColor(type);
                return type != ColorTableType.None;
            }

            result = default;
            return false;
        }

        private static bool TryParseHexValue(string input, out byte result) {
            if(string.IsNullOrEmpty(input) || input.Length < 2) {
                result = 0;
                return false;
            }
            var lv = ((byte)input[0]);
            var rv = ((byte)input[1]);

            if(lv > 47 && lv < 58)
                lv -= 48;
            else if(lv > 64 && lv < 71)
                lv -= 55;
            else if(lv > 96 && lv < 103)
                lv -= 87;
            else {
                result = 0;
                return false;
            }


            if(rv > 47 && rv < 58)
                rv -= 48;
            else if(rv > 64 && rv < 71)
                rv -= 55;
            else if(rv > 96 && rv < 103)
                rv -= 87;
            else {
                result = 0;
                return false;
            }

            result = (byte)(lv * 16 + rv);
            return true;
        }

        private static bool TryParseHexValue(ReadOnlySpan<char> input, out byte result) {
            if(input.IsEmpty || input.Length < 2) {
                result = 0;
                return false;
            }
            var lv = ((byte)input[0]);
            var rv = ((byte)input[1]);

            if(lv > 47 && lv < 58)
                lv -= 48;
            else if(lv > 64 && lv < 71)
                lv -= 55;
            else if(lv > 96 && lv < 103)
                lv -= 87;
            else {
                result = 0;
                return false;
            }


            if(rv > 47 && rv < 58)
                rv -= 48;
            else if(rv > 64 && rv < 71)
                rv -= 55;
            else if(rv > 96 && rv < 103)
                rv -= 87;
            else {
                result = 0;
                return false;
            }

            result = (byte)(lv * 16 + rv);
            return true;
        }

        public static string ToString(Color color) => ColorUtility.ToHtmlStringRGBA(color);
        public static string ToString(Color32 color) => ColorUtility.ToHtmlStringRGBA(color);

        public static string ToString(Color color, bool useAlt) => useAlt ? $"({Mathf.RoundToInt(color.r * 255f)}, {Mathf.RoundToInt(color.g * 255f)}, {Mathf.RoundToInt(color.b * 255f)}, {Mathf.RoundToInt(color.a * 255f)})" : ToString(color);

        #endregion

        #region Rect

        public static bool TryParse(string input, out Rect[] results) {
            if(string.IsNullOrEmpty(input)) {
                results = default;
                return false;
            }
            return TryParse(input.AsSpan(), out results);
        }

        public static bool TryParse(string input, out List<Rect> results) {
            results = new List<Rect>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Rect> results) {
            results.Clear();
            if(string.IsNullOrEmpty(input))
                return false;
            return TryParse(input.AsSpan(), results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Rect[] results) {
            if(input.IsEmpty || !IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Rect[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Rect value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Rect> results) {
            results = new List<Rect>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Rect> results) {
            results.Clear();
            if(input.IsEmpty) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Rect value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Rect result) {
            return TryParse(input.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Rect result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }

            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3))) {
                result = default;
                return false;
            }

            result = new Rect(
                    TryParse(value0, out float res0) ? res0 : 0.0f,
                    TryParse(value1, out float res1) ? res1 : 0.0f,
                    TryParse(value2, out float res2) ? res2 : 0.0f,
                    TryParse(value3, out float res3) ? res3 : 0.0f);
            return true;
        }

        public static string ToString(Rect value) => $"{COMPLEX_OPEN}{value.x}{COMPLEX_SPLIT}{value.y}{COMPLEX_SPLIT}{value.width}{COMPLEX_SPLIT}{value.height}{COMPLEX_CLOSE}";
        public static string ToString(Rect value, bool useAlt) => useAlt ? $"{COMPLEX_OPEN}{value.x}{COMPLEX_SPLIT_ALT}{value.y}{COMPLEX_SPLIT_ALT}{value.width}{COMPLEX_SPLIT_ALT}{value.height}{COMPLEX_CLOSE}" : ToString(value);
        private static string ToStringAltWrapper(Rect value) => ToString(value, true);

        public static string ToString(IReadOnlyList<Rect> values) => $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToString))}{ARRAY_CLOSE}";
        public static string ToString(IReadOnlyList<Rect> values, bool useAlt) => $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToStringAltWrapper))}{ARRAY_CLOSE}";

        #endregion

        #region Bounds

        public static bool TryParse(string input, out Bounds[] results) {
            if(string.IsNullOrEmpty(input)) {
                results = default;
                return false;
            }
            return TryParse(input.AsSpan(), out results);
        }

        public static bool TryParse(string input, out List<Bounds> results) {
            results = new List<Bounds>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Bounds> results) {
            results.Clear();
            if(string.IsNullOrEmpty(input))
                return false;
            return TryParse(input.AsSpan(), results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Bounds[] results) {
            if(input.IsEmpty || !IsArray(input)) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Bounds[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Bounds value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Bounds> results) {
            results = new List<Bounds>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Bounds> results) {
            results.Clear();
            if(input.IsEmpty) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Bounds value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Bounds result) {
            return TryParse(input.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Bounds result) {
            if(input.IsEmpty) {
                result = default;
                return false;
            }

            int index = 0;
            if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value4) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value5))) {
                result = default;
                return false;
            }

            result = new Bounds(
                    new Vector3(TryParse(value0, out float res0) ? res0 : 0.0f,
                                TryParse(value1, out float res1) ? res1 : 0.0f,
                                TryParse(value2, out float res2) ? res2 : 0.0f),
                    new Vector3(TryParse(value3, out float res3) ? res3 : 0.0f,
                                TryParse(value4, out float res4) ? res4 : 0.0f,
                                TryParse(value5, out float res5) ? res5 : 0.0f));
            return true;
        }

        public static string ToString(Bounds value) => $"{COMPLEX_OPEN}{value.center.x}{COMPLEX_SPLIT}{value.center.y}{COMPLEX_SPLIT}{value.center.z}{COMPLEX_SPLIT}{value.size.x}{COMPLEX_SPLIT}{value.size.y}{COMPLEX_SPLIT}{value.size.z}{COMPLEX_CLOSE}";
        public static string ToString(Bounds value, bool useAlt) => useAlt ? $"{COMPLEX_OPEN}{value.center.x}{COMPLEX_SPLIT_ALT}{value.center.y}{COMPLEX_SPLIT_ALT}{value.center.z}{COMPLEX_SPLIT_ALT}{value.size.x}{COMPLEX_SPLIT_ALT}{value.size.y}{COMPLEX_SPLIT_ALT}{value.size.z}{COMPLEX_CLOSE}" : ToString(value);
        private static string ToStringAltWrapper(Bounds value) => ToString(value, true);

        public static string ToString(IReadOnlyList<Bounds> values) => $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToString))}{ARRAY_CLOSE}";
        public static string ToString(IReadOnlyList<Bounds> values, bool useAlt) => $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToStringAltWrapper))}{ARRAY_CLOSE}";

        #endregion

        #region AnimationCurve

        public static bool TryParse(string input, out AnimationCurve[] results) {
            if(string.IsNullOrEmpty(input)) {
                results = default;
                return false;
            }
            return TryParse(input.AsSpan(), out results);
        }

        public static bool TryParse(string input, out List<AnimationCurve> results) {
            results = new List<AnimationCurve>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<AnimationCurve> results) {
            return TryParse(input.AsSpan(), results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out AnimationCurve[] results) {
            if(input.IsEmpty) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new AnimationCurve[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out AnimationCurve value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<AnimationCurve> results) {
            results = new List<AnimationCurve>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<AnimationCurve> results) {
            results.Clear();
            if(input.IsEmpty) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out AnimationCurve value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out AnimationCurve result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            return TryParse(input.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out AnimationCurve result) {
            if(input.IsEmpty) {
                result = default;
                return true;
            }
            int index = 0;
            if(!FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> len)) {
                result = default;
                return false;
            }

            if(!TryParse(len, out int length)) {
                result = default;
                return false;
            }

            Keyframe[] keyframes = new Keyframe[length];
            for(int i = 0; i < length; i++) {
                if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value0) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value1) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value2) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value3) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value4) &&
                FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> value5))) {
                    result = default;
                    return false;
                }
                keyframes[i] = new Keyframe(
                                TryParse(value0, out float res0) ? res0 : 0.0f,
                                TryParse(value1, out float res1) ? res1 : 0.0f,
                                TryParse(value2, out float res2) ? res2 : 0.0f,
                                TryParse(value3, out float res3) ? res3 : 0.0f,
                                TryParse(value4, out float res4) ? res4 : 0.0f,
                                TryParse(value5, out float res5) ? res5 : 0.0f);
            }
            result = new AnimationCurve(keyframes);
            return true;
        }


        public static string ToString(AnimationCurve value) {
            var keys = value.keys;
            StringBuilder sb = new StringBuilder();
            sb.Append(COMPLEX_OPEN);
            sb.Append(keys.Length);
            for(int i = 0, length = keys.Length; i < length; i++) {
                sb.Append(COMPLEX_SPLIT);
                var k = keys[i];
                sb.Append(k.time);
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.value);
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.inTangent);
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.outTangent);
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.inWeight);
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.outWeight);
            }
            sb.Append(COMPLEX_CLOSE);
            return sb.ToString();
        }

        public static string ToString(AnimationCurve value, bool useAlt) {
            if(!useAlt)
                return ToString(value);
            var keys = value.keys;
            StringBuilder sb = new StringBuilder();
            sb.Append(COMPLEX_OPEN);
            sb.Append(keys.Length);
            for(int i = 0, length = keys.Length; i < length; i++) {
                sb.Append(COMPLEX_SPLIT_ALT);
                var k = keys[i];
                sb.Append(k.time);
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.value);
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.inTangent);
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.outTangent);
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.inWeight);
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.outWeight);
            }
            sb.Append(COMPLEX_CLOSE);
            return sb.ToString();
        }
        private static string ToStringAltWrapper(AnimationCurve value) => ToString(value, true);

        public static string ToString(IReadOnlyList<AnimationCurve> values) => $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToString))}{ARRAY_CLOSE}";
        public static string ToString(IReadOnlyList<AnimationCurve> values, bool useAlt) => useAlt ? $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToStringAltWrapper))}{ARRAY_CLOSE}" : ToString(values);

        #endregion

        #region Gradient

        public static bool TryParse(string input, out Gradient[] results) {
            if(string.IsNullOrEmpty(input)) {
                results = default;
                return false;
            }
            return TryParse(input.AsSpan(), out results);
        }

        public static bool TryParse(string input, out List<Gradient> results) {
            results = new List<Gradient>();
            return TryParse(input, results);
        }

        public static bool TryParse(string input, List<Gradient> results) {
            return TryParse(input.AsSpan(), results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Gradient[] results) {
            if(input.IsEmpty) {
                results = default;
                return false;
            }

            int length = input.Length;
            int count = 0;
            int depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0)
                            count++;
                        break;
                }
            }

            results = new Gradient[count];
            int resultIndex = 0;
            int startIndex = 1;
            depth = 0;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Gradient value)) {
                                results[resultIndex] = value;
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {resultIndex}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                            }
                            resultIndex++;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> input, out List<Gradient> results) {
            results = new List<Gradient>();
            return TryParse(input, results);
        }

        public static bool TryParse(ReadOnlySpan<char> input, List<Gradient> results) {
            results.Clear();
            if(input.IsEmpty) {
                return false;
            }

            int length = input.Length;
            int depth = 0;
            int startIndex = 1;

            for(int i = 0; i < length; i++) {
                switch(input[i]) {
                    case COMPLEX_OPEN:
                        depth++;
                        startIndex = i;
                        break;
                    case COMPLEX_CLOSE:
                        depth--;
                        if(depth == 0) {
                            if(TryParse(input.Slice(startIndex, i - startIndex + 1).Trim(), out Gradient value)) {
                                results.Add(value);
                            }
                            else {
                                Debug.LogWarning(TAG + $"Failed to parse element at index {results.Count}: {input.Slice(startIndex, i - startIndex + 1).ToString()}");
                                results.Add(default);
                            }
                        }
                        break;
                }
            }

            return true;
        }


        public static bool TryParse(string input, out Gradient result) {
            if(string.IsNullOrEmpty(input)) {
                result = default;
                return false;
            }
            return TryParse(input.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> input, out Gradient result) {
            if(input.IsEmpty) {
                result = default;
                return true;
            }
            int index = 0;
            if(!FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> mod)) {
                result = default;
                return false;
            }

            if(!TryParse(mod, out int mode)) {
                result = default;
                return false;
            }

            if(!FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> len)) {
                result = default;
                return false;
            }

            if(!TryParse(len, out int length)) {
                result = default;
                return false;
            }

            if(!FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> len2)) {
                result = default;
                return false;
            }

            if(!TryParse(len2, out int length2)) {
                result = default;
                return false;
            }

            GradientColorKey[] colorKeys = new GradientColorKey[length];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[length2];

            for(int i = 0; i < length; i++) {
                if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> col) &&
                     FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> ti))) {
                    result = default;
                    return false;
                }

                if(!TryParse(col, true, out Color color)) {
                    result = default;
                    return false;
                }
                if(!TryParse(ti, out float time)) {
                    result = default;
                    return false;
                }

                colorKeys[i] = new GradientColorKey(color, time);
            }


            for(int i = 0; i < length2; i++) {
                if(!(FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> alp) &&
                     FindNextComplexValueWithAlt(input, ref index, out ReadOnlySpan<char> ti))) {
                    result = default;
                    return false;
                }

                if(!TryParse(alp, out float alpha)) {
                    result = default;
                    return false;
                }
                if(!TryParse(ti, out float time)) {
                    result = default;
                    return false;
                }

                alphaKeys[i] = new GradientAlphaKey(alpha, time);
            }

            result = new Gradient();
            result.mode = (GradientMode)mode;
            result.SetKeys(colorKeys, alphaKeys);

            return true;

        }


        public static string ToString(Gradient gradient) {
            if(gradient == null) {
                return "(null)";
            }
            var mode = gradient.mode;
            var ckeys = gradient.colorKeys;
            var akeys = gradient.alphaKeys;
            StringBuilder sb = new StringBuilder();
            sb.Append(COMPLEX_OPEN);
            sb.Append((int)mode);
            sb.Append(COMPLEX_SPLIT);
            sb.Append(ckeys.Length);
            sb.Append(COMPLEX_SPLIT);
            sb.Append(akeys.Length);
            for(int i = 0, length = ckeys.Length; i < length; i++) {
                sb.Append(COMPLEX_SPLIT);
                var k = ckeys[i];
                sb.Append(ToString(k.color));
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.time);
            }
            for(int i = 0, length = akeys.Length; i < length; i++) {
                sb.Append(COMPLEX_SPLIT);
                var k = akeys[i];
                sb.Append(k.alpha);
                sb.Append(COMPLEX_SPLIT);
                sb.Append(k.time);
            }
            sb.Append(COMPLEX_CLOSE);
            return sb.ToString();
        }

        public static string ToString(Gradient gradient, bool useAlt) {
            if(gradient == null) {
                return "(null)";
            }
            if(!useAlt)
                return ToString(gradient);
            var mode = gradient.mode;
            var ckeys = gradient.colorKeys;
            var akeys = gradient.alphaKeys;
            StringBuilder sb = new StringBuilder();
            sb.Append(COMPLEX_OPEN);
            sb.Append((int)mode);
            sb.Append(COMPLEX_SPLIT_ALT);
            sb.Append(ckeys.Length);
            sb.Append(COMPLEX_SPLIT_ALT);
            sb.Append(akeys.Length);
            for(int i = 0, length = ckeys.Length; i < length; i++) {
                sb.Append(COMPLEX_SPLIT_ALT);
                var k = ckeys[i];
                sb.Append(ToString(k.color));
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.time);
            }
            for(int i = 0, length = akeys.Length; i < length; i++) {
                sb.Append(COMPLEX_SPLIT_ALT);
                var k = akeys[i];
                sb.Append(k.alpha);
                sb.Append(COMPLEX_SPLIT_ALT);
                sb.Append(k.time);
            }
            sb.Append(COMPLEX_CLOSE);
            return sb.ToString();
        }
        private static string ToStringAltWrapper(Gradient gradient) => ToString(gradient, true);

        public static string ToString(IReadOnlyList<Gradient> values) => $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToString))}{ARRAY_CLOSE}";
        public static string ToString(IReadOnlyList<Gradient> values, bool useAlt) => useAlt ? $"{ARRAY_OPEN}{string.Join(ARRAY_SPLIT, values.Select(ToStringAltWrapper))}{ARRAY_CLOSE}" : ToString(values);

        #endregion
    }
}
