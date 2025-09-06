using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Toolkit {
    public static class ArrayExtensions {

        #region Probablity Cache

        public static (IReadOnlyList<float>, float) RandomProbablityCache<T>(this IReadOnlyList<T> array, System.Func<T, float> probability) {
            int length = array.Count;
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            for(int i = 0; i < length; i++) {
                totalProbability += probabilityCache[i] = probability(array[i]);
            }

            return (probabilityCache, totalProbability);
        }

        #endregion

        #region Random

        public static T RandomElement<T>(this IReadOnlyList<T> array) {
            if(array.Count == 0)
                return default;
            return array[Random.Range(0, array.Count)];
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, System.Random random) {
            if(array.Count == 0)
                return default;
            return array[random.Next(array.Count)];
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, System.Func<T, float> probability) {
            int length = array.Count;
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            for(int i = 0; i < length; i++) {
                totalProbability += probabilityCache[i] = probability(array[i]);
            }

            return RandomElement(array, probabilityCache, totalProbability);
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, System.Func<T, float> probability, System.Random random) {
            int length = array.Count;
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            for(int i = 0; i < length; i++) {
                totalProbability += probabilityCache[i] = probability(array[i]);
            }

            return RandomElement(array, probabilityCache, totalProbability, random);
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, IReadOnlyList<float> probabilityCache, float totalProbability) {
            var value = Random.Range(0, totalProbability);
            var length = array.Count;
            for(int i = 0; i < length; i++) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return array[i];
                }
            }
            return default;
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, IReadOnlyList<float> probabilityCache, float totalProbability, System.Random random) {
            var value = (float)random.NextDouble() * totalProbability;
            var length = array.Count;
            for(int i = 0; i < length; i++) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return array[i];
                }
            }
            return default;
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, System.Func<T, bool> available) {
            var temp = array.Where(x => available(x));
            var count = temp.Count();
            return temp.RandomElement(count);
        }

        public static T RandomElement<T>(this IReadOnlyList<T> array, System.Func<T, bool> available, System.Random random) {
            var temp = array.Where(x => available(x));
            var count = temp.Count();
            return temp.RandomElement(count, random);
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable) {
            int index = 0;
            foreach(var i in enumerable) {
                index++;
            }
            var value = Random.Range(0, index);
            index = 0;
            foreach(var i in enumerable) {
                if(value == index) {
                    return i;
                }
                index++;
            }
            return default;
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable, System.Random random) {
            int index = 0;
            foreach(var i in enumerable) {
                index++;
            }
            var value = random.Next(0, index);
            index = 0;
            foreach(var i in enumerable) {
                if(value == index) {
                    return i;
                }
                index++;
            }
            return default;
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable, int length) {
            int index = 0;
            var value = Random.Range(0, length);
            foreach(var i in enumerable) {
                if(value == index) {
                    return i;
                }
                index++;
            }
            return length == 0 ? default : enumerable.RandomElement();
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable, int length, System.Random random) {
            int index = 0;
            var value = random.Next(0, length);
            foreach(var i in enumerable) {
                if(value == index) {
                    return i;
                }
                index++;
            }
            return length == 0 ? default : enumerable.RandomElement();
        }

        public static T RandomElement<T>(this IEnumerable<T> array, System.Func<T, bool> available) {
            return array.Where(x => available(x)).RandomElement();
        }

        public static T RandomElement<T>(this IEnumerable<T> array, System.Func<T, bool> available, System.Random random) {
            return array.Where(x => available(x)).RandomElement(random);
        }

        public static T RandomElement<T>(this IEnumerable<T> array, System.Func<T, float> probability) {
            int length = array.Count();
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            array.Foreach((x, i) => totalProbability += probabilityCache[i] = probability(x));
            return RandomElement(array, probabilityCache, totalProbability);
        }

        public static T RandomElement<T>(this IEnumerable<T> array, System.Func<T, float> probability, System.Random random) {
            int length = array.Count();
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            array.Foreach((x, i) => totalProbability += probabilityCache[i] = probability(x));
            return RandomElement(array, probabilityCache, totalProbability, random);
        }

        public static T RandomElement<T>(this IEnumerable<T> array, IReadOnlyList<float> probabilityCache, float totalProbability) {
            var value = Random.Range(0, totalProbability);
            int i = 0;
            foreach(var x in array) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return x;
                }
                i++;
            }
            return default;
        }

        public static T RandomElement<T>(this IEnumerable<T> array, IReadOnlyList<float> probabilityCache, float totalProbability, System.Random random) {
            var value = (float)random.NextDouble() * totalProbability;
            int i = 0;
            foreach(var x in array) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return x;
                }
                i++;
            }
            return default;
        }

        #endregion

        #region RandomIndex

        public static int RandomIndex<T>(this IReadOnlyList<T> array) {
            return Random.Range(0, array.Count);
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, System.Random random) {
            return random.Next(array.Count);
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, System.Func<T, float> probability) {
            int length = array.Count;
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            for(int i = 0; i < length; i++) {
                totalProbability += probabilityCache[i] = probability(array[i]);
            }

            return RandomIndex(array, probabilityCache, totalProbability);
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, System.Func<T, float> probability, System.Random random) {
            int length = array.Count;
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            for(int i = 0; i < length; i++) {
                totalProbability += probabilityCache[i] = probability(array[i]);
            }

            return RandomIndex(array, probabilityCache, totalProbability, random);
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, IReadOnlyList<float> probabilityCache, float totalProbability) {
            var value = Random.Range(0, totalProbability);
            var length = array.Count;
            for(int i = 0; i < length; i++) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return i;
                }
            }
            return default;
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, IReadOnlyList<float> probabilityCache, float totalProbability, System.Random random) {
            var value = (float)random.NextDouble() * totalProbability;
            var length = array.Count;
            for(int i = 0; i < length; i++) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return i;
                }
            }
            return default;
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, System.Func<T, bool> available) {
            return array.Where(x => available(x)).RandomIndex(array.Count);
        }

        public static int RandomIndex<T>(this IReadOnlyList<T> array, System.Func<T, bool> available, System.Random random) {
            return array.Where(x => available(x)).RandomIndex(array.Count, random);
        }

        public static int RandomIndex<T>(this IEnumerable<T> enumerable) {
            int index = 0;
            foreach(var i in enumerable) {
                index++;
            }
            return Random.Range(0, index);
        }

        public static int RandomIndex<T>(this IEnumerable<T> enumerable, System.Random random) {
            int index = 0;
            foreach(var i in enumerable) {
                index++;
            }
            return random.Next(0, index);
        }

        public static int RandomIndex<T>(this IEnumerable<T> enumerable, int length) {
            return Random.Range(0, length);
        }

        public static int RandomIndex<T>(this IEnumerable<T> enumerable, int length, System.Random random) {
            return random.Next(0, length);
        }

        #endregion

        #region Random Multiple

        public static T[] RandomElements<T>(this IReadOnlyList<T> array, int amount)
            => RandomElements(array, amount, true);

        public static T[] RandomElements<T>(this IReadOnlyList<T> array, int amount, bool unique) {
            T[] output = new T[amount];
            if(unique) {
                List<T> unused = new List<T>(array);
                amount = Mathf.Min(amount, array.Count);
                for(int i = 0; i < amount; i++) {
                    var index = unused.RandomIndex();
                    output[i] = unused[index];
                    unused.RemoveAt(index);
                }
            }
            else {
                for(int i = 0; i < amount; i++)
                    output[i] = array.RandomElement();
            }
            return output;
        }

        public static T[] RandomElements<T>(this IReadOnlyList<T> array, int amount, bool unique, System.Random random) {
            T[] output = new T[amount];
            if(unique) {
                List<T> unused = new List<T>(array);
                amount = Mathf.Min(amount, array.Count);
                for(int i = 0; i < amount; i++) {
                    var index = unused.RandomIndex(random);
                    output[i] = unused[index];
                    unused.RemoveAt(index);
                }
            }
            else {
                for(int i = 0; i < amount; i++)
                    output[i] = array.RandomElement(random);
            }
            return output;
        }

        public static T[] RandomElements<T>(this IReadOnlyList<T> array, int amount, bool unique, System.Func<T, float> probability) {
            var temp = array.Select(x => (x, probability(x)));
            float total = temp.Sum(x => x.Item2);
            return RandomElements(temp, amount, unique, total);
        }

        public static T[] RandomElements<T>(this IReadOnlyList<T> array, int amount, bool unique, System.Func<T, float> probability, System.Random random) {
            var temp = array.Select(x => (x, probability(x)));
            float total = temp.Sum(x => x.Item2);
            return RandomElements(temp, amount, unique, total, random);
        }

        public static T[] RandomElements<T>(this IEnumerable<T> enu, int amount, bool unique = false) {
            if(unique) {
                List<T> unused = new List<T>(enu);
                amount = Mathf.Min(amount, unused.Count);
                T[] output = new T[amount];
                for(int i = 0; i < amount; i++) {
                    var index = unused.RandomIndex();
                    output[i] = unused[index];
                    unused.RemoveAt(index);
                }
                return output;
            }
            else {
                T[] output = new T[amount];
                for(int i = 0; i < amount; i++)
                    output[i] = enu.RandomElement();
                return output;
            }
        }

        private static T[] RandomElements<T>(this IEnumerable<(T, float)> enu, int amount, bool unique, float totalProbability) {
            T[] output = new T[amount];
            if(unique) {
                for(int i = 0; i < amount; i++) {
                    var item = enu.RandomElementsWithIndex(totalProbability);
                    output[i] = item.Item1;
                    totalProbability -= item.Item2;
                    enu = enu.SkipAt(item.Item3);
                }
            }
            else {
                for(int i = 0; i < amount; i++)
                    output[i] = enu.RandomElementsWithIndex(totalProbability).Item1;
            }

            return output;
        }

        private static T[] RandomElements<T>(this IEnumerable<(T, float)> enu, int amount, bool unique, float totalProbability, System.Random random) {
            T[] output = new T[amount];
            if(unique) {
                for(int i = 0; i < amount; i++) {
                    var item = enu.RandomElementsWithIndex(totalProbability, random);
                    output[i] = item.Item1;
                    totalProbability -= item.Item2;
                    enu = enu.SkipAt(item.Item3);
                }
            }
            else {
                for(int i = 0; i < amount; i++)
                    output[i] = enu.RandomElementsWithIndex(totalProbability, random).Item1;
            }

            return output;
        }

        private static (T, float, int) RandomElementsWithIndex<T>(this IEnumerable<(T, float)> enumerable, float totalProbability) {
            float value = Random.value * totalProbability;
            int index = 0;
            foreach(var i in enumerable) {
                value -= i.Item2;
                if(value <= 0f)
                    return (i.Item1, i.Item2, index);
                index++;
            }
            return default;
        }

        private static (T, float, int) RandomElementsWithIndex<T>(this IEnumerable<(T, float)> enumerable, float totalProbability, System.Random random) {
            float value = random.NextFloat() * totalProbability;
            int index = 0;
            foreach(var i in enumerable) {
                value -= i.Item2;
                if(value <= 0f)
                    return (i.Item1, i.Item2, index);
                index++;
            }
            return default;
        }

        #endregion

        #region Shuffle

        public static void Shuffle<T>(this IList<T> array) {
            int length = array.Count;
            for(int i = 0; i < length; i++) {
                var index = Random.Range(0, length);
                T temp = array[i];
                array[i] = array[index];
                array[index] = temp;
            }
        }

        public static void Shuffle<T>(this IList<T> array, int count) {
            for(int i = 0; i < count; i++) {
                Shuffle(array);
            }
        }

        #endregion

        #region To String Conversion

        public static string CombineToString<T>(this IReadOnlyList<T> array) {
            StringBuilder sb = new StringBuilder();
            for(int i = 0, length = array.Count; i < length; i++) {
                sb.AppendLine(array[i].ToString());
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IReadOnlyList<T> array, int count) {
            StringBuilder sb = new StringBuilder();
            for(int i = 0, length = Mathf.Min(count, array.Count); i < length; i++) {
                sb.AppendLine(array[i].ToString());
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IReadOnlyList<T> array, int index, int count) {
            StringBuilder sb = new StringBuilder();
            for(int i = index, length = Mathf.Min(count + index, array.Count); i < length; i++) {
                sb.AppendLine(array[i].ToString());
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IReadOnlyList<T> array, int index, int count, bool useNumbers) {
            if(!useNumbers)
                return CombineToString(array, index, count);
            StringBuilder sb = new StringBuilder();
            for(int i = index, length = Mathf.Min(count + index, array.Count); i < length; i++) {
                sb.AppendLine($"{i}. {array[i].ToString()}");
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IReadOnlyList<T> array, bool useNumbers) {
            if(useNumbers == false) {
                return CombineToString(array);
            }
            StringBuilder sb = new StringBuilder();
            for(int i = 0, length = array.Count; i < length; i++) {
                sb.AppendLine($"{i}. {array[i].ToString()}");
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IEnumerable<T> enumerable) {
            StringBuilder sb = new StringBuilder();
            foreach(var enu in enumerable)
                sb.AppendLine(enu.ToString());
            return sb.ToString();
        }

        public static string CombineToString<T>(this IEnumerable<T> enumerable, bool useNumbers) {
            if(!useNumbers)
                return CombineToString(enumerable);
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach(var enu in enumerable)
                sb.AppendLine($"{index++}. {enu.ToString()}");
            return sb.ToString();
        }

        public static string CombineToString<T>(this IEnumerable<T> enumerable, int maxElements) {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach(var enu in enumerable) {
                sb.AppendLine(enu.ToString());
                if(++index >= maxElements)
                    break;
            }
            return sb.ToString();
        }

        public static string CombineToString<T>(this IEnumerable<T> enumerable, bool useNumbers, int maxElements) {
            if(!useNumbers)
                return CombineToString(enumerable, maxElements);
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach(var enu in enumerable) {
                sb.AppendLine($"{index++}. {enu.ToString()}");
                if(index >= maxElements)
                    break;
            }
            return sb.ToString();
        }

        #endregion

        #region Replacing

        public static void ReplaceWithLast<T>(this IList<T> list, int index) {
            var last = list.Count - 1;
            var t = list[index];
            list[index] = list[last];
            list[last] = t;
        }

        public static void ReplaceWithLastAndRemove<T>(this IList<T> list, int index) {
            var last = list.Count - 1;
            list[index] = list[last];
            if(list.IsReadOnly == false)
                list.RemoveAt(last);
            else
                list[last] = default;
        }

        #endregion

        #region Foreach

        public delegate void ForeachCallback<T>(T item);
        public delegate void ForeachCallbackIndex<T>(T item, int index);
        public delegate void ForeachCallbackSource<TSource, TItem>(TSource source, TItem item, int index);
        public delegate TResult ForeachCallbackResult<TResult, TItem>(TItem item, int index);

        public static void Foreach<T>(this IList<T> list, ForeachCallback<T> callback) {
            for(int i = 0, length = list.Count; i < length; i++) {
                callback(list[i]);
            }
        }

        public static void Foreach<T>(this IList<T> list, ForeachCallbackIndex<T> callback) {
            for(int i = 0, length = list.Count; i < length; i++) {
                callback(list[i], i);
            }
        }

        public static void Foreach<TSource, TItem>(this IList<TItem> list, TSource source, ForeachCallbackSource<TSource, TItem> callback) {
            for(int i = 0, length = list.Count; i < length; i++) {
                callback(source, list[i], i);
            }
        }

        public static TResult[] Foreach<TResult, TItem>(this IList<TItem> list, ForeachCallbackResult<TResult, TItem> callback) {
            TResult[] results = new TResult[list.Count];
            for(int i = 0, length = list.Count; i < length; i++) {
                results[i] = callback(list[i], i);
            }
            return results;
        }

        public static int Foreach<TResult, TItem>(this IList<TItem> list, ForeachCallbackResult<TResult, TItem> callback, IList<TResult> results) {
            var length = Mathf.Min(list.Count, results.Count);
            for(int i = 0; i < length; i++) {
                results[i] = callback(list[i], i);
            }
            return length;
        }

        public static void Foreach<T>(this IEnumerable<T> enumerable, ForeachCallback<T> callback) {
            foreach(var item in enumerable) {
                callback(item);
            }
        }

        public static void Foreach<T>(this IEnumerable<T> enumerable, ForeachCallbackIndex<T> callback) {
            int index = 0;
            foreach(var item in enumerable) {
                callback(item, index++);
            }
        }

        #endregion

        #region Last

        public static T Last<T>(this IReadOnlyList<T> list) {
            if(list == null || list.Count == 0)
                return default;
            return list[list.Count - 1];
        }

        #endregion

        #region Adding

        public static void FillToMax<T>(this List<T> list, IEnumerable<T> elements, int maxSize) {
            foreach(var el in elements) {
                if(list.Count >= maxSize)
                    break;
                list.Add(el);
            }
        }

        public static void FillToMaxThenReplace<T>(this List<T> list, IEnumerable<T> elements, int maxSize) {
            int index = 0;
            foreach(var el in elements) {
                if(list.Count >= maxSize)
                    list[index++ % maxSize] = el;
                else
                    list.Add(el);
            }
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> enumerable, int index, T item) {
            int i = 0;
            if(index < 0)
                yield return item;
            foreach(var enu in enumerable) {
                if(i == index)
                    yield return item;
                yield return enu;
                i++;
            }
            if(i < index)
                yield return item;
        }

        public static IEnumerable<T> AddEnumerator<T>(this IEnumerable<T> enumerable, T item) {
            foreach(var enu in enumerable)
                yield return enu;
            yield return item;
        }

        #endregion

        #region Default Array

        public static T[] ToArray<T>(this T value, int size) where T : struct {
            T[] array = new T[size];
            for(int i = 0; i < size; i++) {
                array[i] = value;
            }
            return array;
        }

        public static List<T> ToList<T>(this T value, int size) where T : struct {
            List<T> array = new List<T>();
            for(int i = 0; i < size; i++) {
                array.Add(value);
            }
            return array;
        }

        #endregion

        #region Contains

        public static bool Contains<T>(this T[] array, T obj)
            => Contains(array, obj, 0, array.Length);

        public static bool Contains<T>(this T[] array, T obj, int startIndex, int endIndex) {
            endIndex = System.Math.Min(endIndex, array.Length);
            for(int i = startIndex; i < endIndex; i++) {
                if(array[i].Equals(obj))
                    return true;
            }
            return false;
        }

        public static bool Contains<T>(this T[] array, T obj, int startIndex, int endIndex, EqualityComparer<T> comparer) {
            endIndex = System.Math.Min(endIndex, array.Length);
            for(int i = startIndex; i < endIndex; i++) {
                if(comparer.Equals(array[i], obj))
                    return true;
            }
            return false;
        }

        #endregion

        #region Invert

        public static List<T> Invert<T>(this List<T> array) {
            for(int i = array.Count - 1, index = 0, halfLength = array.Count / 2; i >= halfLength; i--, index++) {
                var temp = array[index];
                array[index] = array[i];
                array[i] = temp;
            }
            return array;
        }

        #endregion

        #region IndexOf

        public static int IndexOf<T>(this IReadOnlyList<T> array, T element) {
            for(int i = 0, length = array.Count; i < length; i++) {
                if(array[i].Equals(element))
                    return i;
            }
            return -1;
        }

        #endregion

        #region SubArray

        public static T[] SubArray<T>(this IReadOnlyList<T> array, int index, int length) {
            index = Mathf.Clamp(index, 0, array.Count - 1);
            length = Mathf.Min(array.Count - index, length);
            T[] output = new T[length];
            for(int i = 0; i < length; i++) {
                output[i] = array[index + i];
            }
            return output;
        }

        #endregion

        #region Split

        public static T[][] Split<T>(this IReadOnlyList<T> array, int amount) {
            amount = Mathf.Max(1, amount);
            var output = new T[amount][];
            var splitLength = Mathf.Min(array.Count / amount + 1, array.Count);
            int index = 0;
            for(int i = 0; i < amount; i++) {
                output[i] = array.SubArray(index, splitLength);
                index += splitLength;
            }
            return output;
        }

        public static List<List<T>> Split<T>(this List<T> array, int amount) {
            amount = Mathf.Max(1, amount);
            var output = new List<List<T>>();
            var splitLength = Mathf.Min(array.Count / amount + 1, array.Count);
            int index = 0;
            for(int i = 0; i < amount; i++) {
                output.Add(new List<T>(array.SubArray(index, splitLength)));
                index += splitLength;
            }
            return output;
        }

        public static List<List<T>> SplitAt<T>(this List<T> array, int splitLength) {
            var amount = array.Count / splitLength + 1;
            if(amount == 1)
                return new List<List<T>>() { array };
            var output = new List<List<T>>();
            int index = 0;
            for(int i = 0; i < amount; i++) {
                output.Add(new List<T>(array.SubArray(index, splitLength)));
                index += splitLength;
            }
            return output;
        }

        #endregion

        #region List RemoveWhere

        public static void RemoveWhere<T>(this List<T> list, System.Func<T, bool> filter) {
            for(int i = list.Count - 1; i >= 0; i--) {
                if(filter(list[i]))
                    list.RemoveAt(i);
            }
        }

        #endregion

        #region RemoveAt Multiple 
        // Sorted and removed in order to not break array or accidentally remove unintended values.

        private static int[] removeAtCache = new int[8];

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 2);
            for(int i = 0; i < 2; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1, int index2) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            removeAtCache[2] = index2;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 3);
            for(int i = 0; i < 3; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1, int index2, int index3) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            removeAtCache[2] = index2;
            removeAtCache[3] = index3;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 4);
            for(int i = 0; i < 4; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1, int index2, int index3, int index4) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            removeAtCache[2] = index2;
            removeAtCache[3] = index3;
            removeAtCache[4] = index4;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 5);
            for(int i = 0; i < 5; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1, int index2, int index3, int index4, int index5) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            removeAtCache[2] = index2;
            removeAtCache[3] = index3;
            removeAtCache[4] = index4;
            removeAtCache[5] = index5;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 6);
            for(int i = 0; i < 6; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1, int index2, int index3, int index4, int index5, int index6) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            removeAtCache[2] = index2;
            removeAtCache[3] = index3;
            removeAtCache[4] = index4;
            removeAtCache[5] = index5;
            removeAtCache[6] = index6;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 7);
            for(int i = 0; i < 7; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index0, int index1, int index2, int index3, int index4, int index5, int index6, int index7) {
            removeAtCache[0] = index0;
            removeAtCache[1] = index1;
            removeAtCache[2] = index2;
            removeAtCache[3] = index3;
            removeAtCache[4] = index4;
            removeAtCache[5] = index5;
            removeAtCache[6] = index6;
            removeAtCache[7] = index7;
            Toolkit.Sort.Quick(removeAtCache, BackToFrontIndicesSort, 8);
            for(int i = 0; i < 8; i++) {
                list.RemoveAt(removeAtCache[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, params int[] indices) {
            Toolkit.Sort.Quick(indices, BackToFrontIndicesSort);
            for(int i = 0, length = indices.Length; i < length; i++) {
                list.RemoveAt(indices[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, IList<int> indices) {
            Toolkit.Sort.Quick(indices, BackToFrontIndicesSort);
            for(int i = 0, length = indices.Count; i < length; i++) {
                list.RemoveAt(indices[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, IList<int> indices, int length) {
            Toolkit.Sort.Quick(indices, BackToFrontIndicesSort, length);
            for(int i = 0; i < length; i++) {
                list.RemoveAt(indices[i]);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, IList<int> indices, int start, int end) {
            Toolkit.Sort.Quick(indices, BackToFrontIndicesSort, start, end);
            for(int i = start; i < end; i++) {
                list.RemoveAt(indices[i]);
            }
        }

        private static int BackToFrontIndicesSort(int a, int b) => -a.CompareTo(b);

        #endregion

        #region Remove Range

        public static void RemoveRange<T>(this IList<T> list, IReadOnlyList<T> otherList) {
            for(int i = otherList.Count - 1; i >= 0; i--) {
                list.Remove(otherList[i]);
            }
        }

        #endregion

        #region Equal Elements

        public static bool EqualElements<T>(this IReadOnlyList<T> array, IReadOnlyList<T> otherArray) {
            if(array == otherArray)
                return true;
            if(array == null || otherArray == null)
                return false;
            if(array.Count != otherArray.Count)
                return false;
            for(int i = 0, length = array.Count; i < length; i++) {
                if(!array[i].Equals(otherArray[i]))
                    return false;
            }
            return true;
        }

        #endregion

        #region Fill

        public static void Fill<T>(this IList<T> list, int amount) {
            for(int i = 0; i < amount; i++) {
                list.Add(default);
            }
        }

        public static void FillTo<T>(this IList<T> list, int amount) {
            for(int i = list.Count; i < amount; i++) {
                list.Add(default);
            }
        }

        #endregion

        #region RemoveLast

        public static void RemoveLast<T>(this IList<T> list) {
            if(list.Count > 0)
                list.RemoveAt(list.Count - 1);
        }

        #endregion

        #region ReplaceWithLastAndRemove

        public static bool ReplaceWithLastAndRemove<T>(this List<T> list, T item) {
            for(int i = 0, length = list.Count; i < length; i++) {
                if(list[i].Equals(item)) {
                    var lastIndex = length - 1;
                    list[i] = list[lastIndex];
                    list.RemoveAt(lastIndex);
                    return true;
                }
            }
            return false;
        }

        public static bool ReplaceWithLastAndRemove<T>(this List<T> list, int index) {
            if(index < 0 || index >= list.Count)
                return false;
            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
            return true;
        }

        #endregion

        #region Extract

        public static bool Extract<T>(this List<T> list, System.Func<T, bool> search, out T item) {
            for(int i = 0; i < list.Count; i++) {
                if(search(list[i])) {
                    item = list[i];
                    return true;
                }
            }
            item = default;
            return false;
        }

        #endregion

        #region MoveTo

        public static bool MoveTo<T>(this T[] array, int index, int newIndex) {
            if(index == newIndex)
                return true;
            if(index < 0 || newIndex < 0)
                return false;
            var len = array.Length;
            if(index >= len || newIndex >= len)
                return false;

            var item = array[index];
            if(index < newIndex) { // Shift Down
                for(int i = index; i < newIndex; i++)
                    array[i] = array[i + 1];
            }
            else { // Shift up
                for(int i = index; i > newIndex; i--)
                    array[i] = array[i - 1];
            }
            array[newIndex] = item;
            return true;
        }

        public static bool MoveTo<T>(this IList<T> array, int index, int newIndex) {
            if(index == newIndex)
                return true;
            if(index < 0 || newIndex < 0)
                return false;
            var len = array.Count;
            if(index >= len || newIndex >= len)
                return false;

            var item = array[index];
            if(index < newIndex) { // Shift Down
                for(int i = index; i < newIndex; i++)
                    array[i] = array[i + 1];
            }
            else { // Shift up
                for(int i = index; i > newIndex; i--)
                    array[i] = array[i - 1];
            }
            array[newIndex] = item;
            return true;
        }

        #endregion
    }
}
