using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit
{
    public static class DictionaryExtensions
    {
        #region Try Get Generic

        public static bool TryGetValue<TKey, TValue, TOutput>(this Dictionary<TKey, TValue> dict, TKey key, out TOutput output) where TOutput : TValue {
            var res = dict.TryGetValue(key, out TValue value);
            if(res) {
                output = (TOutput)value;
                return true;
            }
            output = default;
            return false;
        }

        public static bool TryGetValue<TKey, TValue, TOutput>(this Dictionary<TKey, TValue> dict, TKey key, out TOutput output, TOutput defaultValue) where TOutput : TValue {
            var res = dict.TryGetValue(key, out TValue value);
            if(res) {
                output = (TOutput)value;
                return true;
            }
            output = defaultValue;
            return false;
        }

        #endregion

        #region Copy

        public static Dictionary<TKey, TValue> CreateCopy<TKey, TValue>(this Dictionary<TKey, TValue> dict) {
            Dictionary<TKey, TValue> newCopy = new Dictionary<TKey, TValue>();
            foreach(var keyValue in dict)
                newCopy.Add(keyValue.Key, keyValue.Value);
            return newCopy;
        }

        #endregion

        #region SetOrAdd

        public static void SetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
            if(dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }

        #endregion

        #region AddSafe

        public static void AddSafe<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
            if(!dict.ContainsKey(key))
                dict.Add(key, value);
        }

        public static void AddSafe<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value, bool overrideValue) {
            if(!dict.ContainsKey(key))
                dict.Add(key, value);
            else if(overrideValue)
                dict[key] = value;
        }

        #endregion
    }
}
