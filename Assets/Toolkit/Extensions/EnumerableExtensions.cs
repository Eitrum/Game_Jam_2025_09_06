using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Toolkit {
    public static class EnumerableExtensions {
        #region Better Linq

        public delegate TResult SelectIndexCallback<T, TResult>(T item, int index);
        public delegate bool WhereIndexCallback<T>(T item, int index);

        public static IEnumerable<TResult> SelectWithIndex<T, TResult>(this IEnumerable<T> enumerable, SelectIndexCallback<T, TResult> function) {
            int index = 0;
            foreach(var enu in enumerable)
                yield return function(enu, index++);
        }

        public static IEnumerable<T> WhereWithIndex<T>(this IEnumerable<T> enumerable, WhereIndexCallback<T> function) {
            int index = 0;
            foreach(var enu in enumerable)
                if(function(enu, index++))
                    yield return enu;
        }

        public static IEnumerable<TResult> WhereSelect<T, TResult>(this IEnumerable<T> enumerable, System.Func<T, (bool, TResult)> search) {
            foreach(var e in enumerable) {
                var t = search(e);
                if(t.Item1)
                    yield return t.Item2;
            }
        }

        public static T[] ToArray<T>(this IEnumerable<T> enumerable, int amount) {
            T[] array = new T[amount];
            int index = 0;
            foreach(var enu in enumerable) {
                array[index] = enu;
                if(++index >= amount)
                    break;
            }
            return array;
        }

        public static List<T> ToList<T>(this IEnumerable<T> enumerable, int amount) {
            List<T> list = new List<T>(amount);
            int index = 0;
            foreach(var enu in enumerable) {
                list.Add(enu);
                if(++index >= amount)
                    break;
            }
            return list;
        }

        public static (IEnumerable<T0>, IEnumerable<T1>) Split<T0, T1>(this IEnumerable<(T0, T1)> enumerable) {
            return (enumerable.Select(x => x.Item1), enumerable.Select(y => y.Item2));
        }

        public static (List<T0>, List<T1>) ToList<T0, T1>(this (IEnumerable<T0>, IEnumerable<T1>) value) {
            return (
                value.Item1.ToList(),
                value.Item2.ToList());
        }

        public static (T0[], T1[]) ToArray<T0, T1>(this (IEnumerable<T0>, IEnumerable<T1>) value) {
            return (
                value.Item1.ToArray(),
                value.Item2.ToArray());
        }

        #endregion

        #region Iteration Unique

        public static IEnumerable<T> Unique<T>(this IEnumerable<T> enumerable) {
            HashSet<T> hash = new HashSet<T>();
            foreach(var enu in enumerable) {
                if(!hash.Contains(enu)) {
                    hash.Add(enu);
                }
            }
            foreach(var h in hash)
                yield return h;
        }

        public delegate TComp CompareFunctionCallback<T, TComp>(T item);

        public static IEnumerable<T> Unique<T, TComp>(this IEnumerable<T> enumerable, CompareFunctionCallback<T, TComp> toCompareFunction) {
            HashSet<TComp> hash = new HashSet<TComp>();
            List<T> list = new List<T>();
            foreach(var enu in enumerable) {
                var item = toCompareFunction(enu);
                if(!hash.Contains(item)) {
                    hash.Add(item);
                    list.Add(enu);
                }
            }
            foreach(var i in list)
                yield return i;
        }

        public static bool IsAllUnique<T>(this IEnumerable<T> enumerable) {
            HashSet<T> hash = new HashSet<T>();
            foreach(var enu in enumerable) {
                if(hash.Contains(enu))
                    return false;
                hash.Add(enu);
            }
            return true;
        }

        public static bool IsAllUnique<T, TComp>(this IEnumerable<T> enumerable, CompareFunctionCallback<T, TComp> toCompareFunction) {
            HashSet<TComp> hash = new HashSet<TComp>();
            foreach(var enu in enumerable) {
                var res = toCompareFunction(enu);
                if(hash.Contains(res))
                    return false;
                hash.Add(res);
            }
            return true;
        }

        #endregion

        #region Multiply

        public static float Multiply<T>(this IEnumerable<T> enumerable, System.Func<T, float> function) {
            var result = 1f;
            foreach(var enu in enumerable) {
                result *= function(enu);
            }
            return result;
        }

        public static double Multiply<T>(this IEnumerable<T> enumerable, System.Func<T, double> function) {
            var result = 1d;
            foreach(var enu in enumerable) {
                result *= function(enu);
            }
            return result;
        }

        public static int Multiply<T>(this IEnumerable<T> enumerable, System.Func<T, int> function) {
            var result = 1d;
            foreach(var enu in enumerable) {
                result *= function(enu);
            }
            return (int)System.Math.Round(result);
        }

        #endregion

        #region Accumulate

        public static IEnumerable<float> Accumulate<T>(this IEnumerable<T> enumerable, System.Func<T, float> function) {
            var result = 0f;
            foreach(var enu in enumerable)
                yield return (result += function(enu));
        }

        public static IEnumerable<double> Accumulate<T>(this IEnumerable<T> enumerable, System.Func<T, double> function) {
            var result = 0d;
            foreach(var enu in enumerable)
                yield return (result += function(enu));
        }

        public static IEnumerable<int> Accumulate<T>(this IEnumerable<T> enumerable, System.Func<T, int> function) {
            var result = 0;
            foreach(var enu in enumerable)
                yield return (result += function(enu));
        }

        #endregion

        #region Combine To String

        public static string CombineToString(this IEnumerator<string> enumerator) {
            StringBuilder sb = new StringBuilder();
            while(enumerator.MoveNext()) {
                sb.AppendLine(enumerator.Current);
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IEnumerator<T> enumerator) {
            StringBuilder sb = new StringBuilder();
            while(enumerator.MoveNext()) {
                sb.AppendLine(enumerator.Current.ToString());
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IEnumerator<T> enumerator, bool appendNewLine) {
            if(appendNewLine)
                return enumerator.CombineToString();
            StringBuilder sb = new StringBuilder();
            while(enumerator.MoveNext()) {
                sb.Append(enumerator.Current.ToString());
            }
            return sb.ToString();
        }

        #endregion

        #region Skip

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> enu, T item) {
            foreach(var e in enu)
                if(!e.Equals(item))
                    yield return e;
        }

        public static IEnumerable<T> SkipAt<T>(this IEnumerable<T> enu, int index) {
            int tempIndex = -1;
            foreach(var e in enu)
                if((++tempIndex) != index)
                    yield return e;
        }

        #endregion

        #region Extract

        private class ExtractContainer<T> {
            public T objectToExtract;
        }

        private static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, System.Func<T, bool> search, ExtractContainer<T> container) {
            bool isExtracted = false;
            foreach(var e in enu) {
                if(!isExtracted && search(e)) {
                    container.objectToExtract = e;
                    isExtracted = false;
                }
                else
                    yield return e;
            }
        }

        private static IEnumerable<T> ExtractAll<T>(this IEnumerable<T> enu, System.Func<T, bool> search, ExtractContainer<T> container) {
            bool isExtracted = false;
            foreach(var e in enu) {
                if(search(e)) {
                    if(!isExtracted) {
                        container.objectToExtract = e;
                        isExtracted = true;
                    }
                }
                else
                    yield return e;
            }
        }

        public static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, Func<T, bool> search, out T output) {
            ExtractContainer<T> container = new ExtractContainer<T>();
            enu = enu.Extract(search, container);
            foreach(var e in enu) { }
            output = container.objectToExtract;
            return enu;
        }

        public static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, Func<T, bool> search, out T output, bool clearAllMatches) {
            ExtractContainer<T> container = new ExtractContainer<T>();
            enu = clearAllMatches ? enu.ExtractAll(search, container) : enu.Extract(search, container);
            foreach(var e in enu) { }
            output = container.objectToExtract;
            return enu;
        }

        public static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, Func<T, bool> search, List<T> output) {
            output.Clear();
            foreach(var e in enu) {
                if(search(e))
                    output.Add(e);
                else
                    yield return e;
            }
        }

        public static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, Func<T, bool> search, out T[] output) {
            List<T> temp = new List<T>();
            enu = enu.Extract(search, temp);
            foreach(var e in enu) { }
            output = temp.ToArray();
            return enu;
        }

        private static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, Func<T, bool> search, T[] output, ExtractContainer<int> container) {
            int index = 0;
            int max = output.Length;

            foreach(var e in enu) {
                if(index < max && search(e))
                    output[index++] = e;
                else
                    yield return e;
            }
            container.objectToExtract = index;
        }

        public static IEnumerable<T> Extract<T>(this IEnumerable<T> enu, Func<T, bool> search, T[] output, out int length) {
            ExtractContainer<int> container = new ExtractContainer<int>();
            enu = enu.Extract(search, output, container);
            foreach(var e in enu) { }
            length = container.objectToExtract;
            return enu;
        }

        #endregion

        #region Add Range

        public static IEnumerable<T> AddRange<T>(this IEnumerable<T> enu, IReadOnlyList<T> array) {
            foreach(var e in enu)
                yield return e;
            for(int i = 0, length = array.Count; i < length; i++)
                yield return array[i];
        }

        public static IEnumerable<T> AddRange<T>(this IEnumerable<T> enu, IEnumerable<T> otherEnu) {
            foreach(var e in enu)
                yield return e;
            foreach(var e in otherEnu)
                yield return e;
        }

        public static IEnumerable<T> AddRangeBefore<T>(this IEnumerable<T> enu, IReadOnlyList<T> array) {
            for(int i = 0, length = array.Count; i < length; i++)
                yield return array[i];
            foreach(var e in enu)
                yield return e;
        }

        public static IEnumerable<T> AddRangeBefore<T>(this IEnumerable<T> enu, IEnumerable<T> otherEnu) {
            foreach(var e in otherEnu)
                yield return e;
            foreach(var e in enu)
                yield return e;
        }

        #endregion

        #region Highest Lowest

        public static T Highest<T>(this IEnumerable<T> enu, System.Func<T, float> function) {
            var highest = float.MinValue;
            T temp = default;
            foreach(var e in enu) {
                var f = function(e);
                if(f > highest) {
                    highest = f;
                    temp = e;
                }
            }
            return temp;
        }

        public static T Lowest<T>(this IEnumerable<T> enu, System.Func<T, float> function) {
            var lowest = float.MaxValue;
            T temp = default;
            foreach(var e in enu) {
                var f = function(e);
                if(f < lowest) {
                    lowest = f;
                    temp = e;
                }
            }
            return temp;
        }



        public static T Lowest<T>(this IEnumerable<T> enu, System.Func<T, float> function, out int index) {
            var lowest = float.MaxValue;
            T temp = default;
            int counter = 0;
            index = 0;
            foreach(var e in enu) {
                var f = function(e);
                if(f < lowest) {
                    lowest = f;
                    temp = e;
                    index = counter;
                }
                counter++;
            }
            return temp;
        }

        public static T Highest<T>(this IEnumerable<T> enu, System.Func<T, int> function) {
            var highest = int.MinValue;
            T temp = default;
            foreach(var e in enu) {
                var f = function(e);
                if(f > highest) {
                    highest = f;
                    temp = e;
                }
            }
            return temp;
        }

        public static T Lowest<T>(this IEnumerable<T> enu, System.Func<T, int> function) {
            var lowest = int.MaxValue;
            T temp = default;
            foreach(var e in enu) {
                var f = function(e);
                if(f < lowest) {
                    lowest = f;
                    temp = e;
                }
            }
            return temp;
        }



        public static T Highest<T>(this IEnumerable<T> enu, System.Func<T, double> function) {
            var highest = double.MinValue;
            T temp = default;
            foreach(var e in enu) {
                var f = function(e);
                if(f > highest) {
                    highest = f;
                    temp = e;
                }
            }
            return temp;
        }

        public static T Lowest<T>(this IEnumerable<T> enu, System.Func<T, double> function) {
            var lowest = double.MaxValue;
            T temp = default;
            foreach(var e in enu) {
                var f = function(e);
                if(f < lowest) {
                    lowest = f;
                    temp = e;
                }
            }
            return temp;
        }

        #endregion

        #region MinMax

        public static MinMaxInt MinMax<T>(this IEnumerable<T> enu, System.Func<T, int> minFunction, System.Func<T, int> maxFunction) {
            MinMaxInt range = MinMaxInt.InvertedMaximumRange;
            foreach(var e in enu) {
                range.min = Math.Min(range.min, minFunction(e));
                range.max = Math.Max(range.max, maxFunction(e));
            }
            return range;
        }

        #endregion

        #region Sum Unity

        public static Vector3 Sum(this IEnumerable<Vector3> enums) {
            Vector3 result = Vector3.zero;
            foreach(var e in enums)
                result += e;
            return result;
        }

        public static Vector3 Sum<T>(this IEnumerable<T> enums, Func<T, Vector3> func) {
            Vector3 result = Vector3.zero;
            foreach(var e in enums)
                result += func(e);
            return result;
        }

        public static Vector2 Sum(this IEnumerable<Vector2> enums) {
            Vector2 result = Vector2.zero;
            foreach(var e in enums)
                result += e;
            return result;
        }

        public static Vector2 Sum<T>(this IEnumerable<T> enums, Func<T, Vector2> func) {
            Vector2 result = Vector2.zero;
            foreach(var e in enums)
                result += func(e);
            return result;
        }

        #endregion

        #region Min

        static float[] minValueCache;
        // Seeb, DO NOT TRY OPTIMIZE THIS SHIT!

        public static T[] Min<T>(this IEnumerable<T> enu, int amount, Func<T, float> func) {
            if(minValueCache == null || minValueCache.Length < amount)
                minValueCache = new float[Mathf.NextPowerOfTwo(amount)];
            float[] value = minValueCache;
            T[] result = new T[amount];
            for(int i = 0; i < amount; i++)
                value[i] = float.MaxValue;

            int index = 0;
            foreach(var e in enu) {
                var val = func(e);
                bool inserted = false;
                for(int i = index - 1; i >= 0; i--) {
                    if(val < value[i]) {
                        if(i + 1 < amount) {
                            value[i + 1] = value[i];
                            result[i + 1] = result[i];
                        }
                        if(i == 0) {
                            value[i] = val;
                            result[i] = e;
                            inserted = true;
                        }
                    }
                    else {
                        if(i + 1 < index) {
                            value[i + 1] = val;
                            result[i + 1] = e;
                            inserted = true;
                            break;
                        }
                        break;
                    }
                }
                if(!inserted && index < amount) {
                    value[index] = val;
                    result[index] = e;
                    index++;
                }
            }

            return result;
        }

        public static T[] Min<T>(this IEnumerable<T> enu, int amount, Func<T, float> func, int elementToSkip) {
            if(minValueCache == null || minValueCache.Length < amount)
                minValueCache = new float[Mathf.NextPowerOfTwo(amount)];
            float[] value = minValueCache;
            T[] result = new T[amount];
            for(int i = 0; i < amount; i++)
                value[i] = float.MaxValue;

            int index = 0;
            int t = 0;

            foreach(var e in enu) {
                if(elementToSkip == t++)
                    continue;
                var val = func(e);
                bool inserted = false;
                for(int i = index - 1; i >= 0; i--) {
                    if(val < value[i]) {
                        if(i + 1 < amount) {
                            value[i + 1] = value[i];
                            result[i + 1] = result[i];
                        }
                        if(i == 0) {
                            value[i] = val;
                            result[i] = e;
                            inserted = true;
                        }
                    }
                    else {
                        if(i + 1 < index) {
                            value[i + 1] = val;
                            result[i + 1] = e;
                            inserted = true;
                            break;
                        }
                        break;
                    }
                }
                if(!inserted && index < amount) {
                    value[index] = val;
                    result[index] = e;
                    index++;
                }
            }

            return result;
        }

        public static T[] Min<T>(this IEnumerable<T> enu, int amount, Func<T, float> func, IReadOnlyList<int> elementsToSkip) {
            if(minValueCache == null || minValueCache.Length < amount)
                minValueCache = new float[Mathf.NextPowerOfTwo(amount)];
            float[] value = minValueCache;
            T[] result = new T[amount];
            for(int i = 0; i < amount; i++)
                value[i] = float.MaxValue;

            int index = 0;
            int t = 0;

            foreach(var e in enu) {
                if(elementsToSkip.Contains(t++))
                    continue;
                var val = func(e);
                bool inserted = false;
                for(int i = index - 1; i >= 0; i--) {
                    if(val < value[i]) {
                        if(i + 1 < amount) {
                            value[i + 1] = value[i];
                            result[i + 1] = result[i];
                        }
                        if(i == 0) {
                            value[i] = val;
                            result[i] = e;
                            inserted = true;
                        }
                    }
                    else {
                        if(i + 1 < index) {
                            value[i + 1] = val;
                            result[i + 1] = e;
                            inserted = true;
                            break;
                        }
                        break;
                    }
                }
                if(!inserted && index < amount) {
                    value[index] = val;
                    result[index] = e;
                    index++;
                }
            }

            return result;
        }

        public static int[] MinAsIndex<T>(this IEnumerable<T> enu, int amount, Func<T, float> func) {
            if(minValueCache == null || minValueCache.Length < amount)
                minValueCache = new float[Mathf.NextPowerOfTwo(amount)];
            float[] value = minValueCache;
            int[] result = new int[amount];
            for(int i = 0; i < amount; i++)
                value[i] = float.MaxValue;

            int index = 0;
            int t = 0;

            foreach(var e in enu) {
                var val = func(e);
                bool inserted = false;
                for(int i = index - 1; i >= 0; i--) {
                    if(val < value[i]) {
                        if(i + 1 < amount) {
                            value[i + 1] = value[i];
                            result[i + 1] = result[i];
                        }
                        if(i == 0) {
                            value[i] = val;
                            result[i] = t;
                            inserted = true;
                        }
                    }
                    else {
                        if(i + 1 < index) {
                            value[i + 1] = val;
                            result[i + 1] = t;
                            inserted = true;
                            break;
                        }
                        break;
                    }
                }
                if(!inserted && index < amount) {
                    value[index] = val;
                    result[index] = t;
                    index++;
                }
                t++;
            }

            return result;
        }

        public static int[] MinAsIndex<T>(this IEnumerable<T> enu, int amount, Func<T, float> func, int elementToSkip) {
            if(minValueCache == null || minValueCache.Length < amount)
                minValueCache = new float[Mathf.NextPowerOfTwo(amount)];
            float[] value = minValueCache;
            int[] result = new int[amount];
            for(int i = 0; i < amount; i++)
                value[i] = float.MaxValue;

            int index = 0;
            int t = 0;

            foreach(var e in enu) {
                if(elementToSkip == t) {
                    t++;
                    continue;
                }
                var val = func(e);
                bool inserted = false;
                for(int i = index - 1; i >= 0; i--) {
                    if(val < value[i]) {
                        if(i + 1 < amount) {
                            value[i + 1] = value[i];
                            result[i + 1] = result[i];
                        }
                        if(i == 0) {
                            value[i] = val;
                            result[i] = t;
                            inserted = true;
                        }
                    }
                    else {
                        if(i + 1 < index) {
                            value[i + 1] = val;
                            result[i + 1] = t;
                            inserted = true;
                            break;
                        }
                        break;
                    }
                }
                if(!inserted && index < amount) {
                    value[index] = val;
                    result[index] = t;
                    index++;
                }
                t++;
            }

            return result;
        }

        public static int[] MinAsIndex<T>(this IEnumerable<T> enu, int amount, Func<T, float> func, IReadOnlyList<int> elementsToSkip) {
            if(minValueCache == null || minValueCache.Length < amount)
                minValueCache = new float[Mathf.NextPowerOfTwo(amount)];
            float[] value = minValueCache;
            int[] result = new int[amount];
            for(int i = 0; i < amount; i++)
                value[i] = float.MaxValue;

            int index = 0;
            int t = 0;

            foreach(var e in enu) {
                if(elementsToSkip.Contains(t)) {
                    t++;
                    continue;
                }
                var val = func(e);
                bool inserted = false;
                for(int i = index - 1; i >= 0; i--) {
                    if(val < value[i]) {
                        if(i + 1 < amount) {
                            value[i + 1] = value[i];
                            result[i + 1] = result[i];
                        }
                        if(i == 0) {
                            value[i] = val;
                            result[i] = t;
                            inserted = true;
                        }
                    }
                    else {
                        if(i + 1 < index) {
                            value[i + 1] = val;
                            result[i + 1] = t;
                            inserted = true;
                            break;
                        }
                        break;
                    }
                }
                if(!inserted && index < amount) {
                    value[index] = val;
                    result[index] = t;
                    index++;
                }
                t++;
            }

            return result;
        }

        #endregion

        #region Average Vector

        public static Vector3 Average<T>(this IEnumerable<T> enu, System.Func<T, Vector3> conversion) {
            Vector3 result = new Vector3();
            int count = 0;
            foreach(var e in enu) {
                result += conversion(e);
                count++;
            }
            return result / count;
        }

        public static Vector3 Average(this IEnumerable<Vector3> enu) {
            Vector3 result = new Vector3();
            int count = 0;
            foreach(var e in enu) {
                result += e;
                count++;
            }
            return result / count;
        }

        public static Vector2 Average<T>(this IEnumerable<T> enu, System.Func<T, Vector2> conversion) {
            Vector2 result = new Vector2();
            int count = 0;
            foreach(var e in enu) {
                result += conversion(e);
                count++;
            }
            return result / count;
        }

        public static Vector2 Average(this IEnumerable<Vector2> enu) {
            Vector2 result = new Vector2();
            int count = 0;
            foreach(var e in enu) {
                result += e;
                count++;
            }
            return result / count;
        }

        public static Vector4 Average<T>(this IEnumerable<T> enu, System.Func<T, Vector4> conversion) {
            Vector4 result = new Vector4();
            int count = 0;
            foreach(var e in enu) {
                result += conversion(e);
                count++;
            }
            return result / count;
        }

        public static Vector4 Average(this IEnumerable<Vector4> enu) {
            Vector4 result = new Vector4();
            int count = 0;
            foreach(var e in enu) {
                result += e;
                count++;
            }
            return result / count;
        }

        #endregion
    }
}
