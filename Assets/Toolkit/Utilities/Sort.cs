using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Toolkit {
    public static class Sort {

        #region Merge

        public static IEnumerable<T> Merge<T>(IEnumerable<T> enu, Comparison<T> comparison)
            => Sort<T>.Merge(enu, comparison);

        public static void Merge<T>(IList<T> array, Comparison<T> comparison)
            => Merge(array, comparison, 0, array.Count);

        public static void Merge<T>(IList<T> array, Comparison<T> comparison, int length)
            => Merge(array, comparison, 0, length);

        public static void Merge<T>(IList<T> array, Comparison<T> comparison, int start, int end)
            => Sort<T>.Merge(array, comparison, start, end);

        #endregion

        #region Quick

        public static void Quick<T>(IList<T> array, Comparison<T> comparison)
            => Quick(array, comparison, 0, array.Count);

        public static void Quick<T>(IList<T> array, Comparison<T> comparison, int length)
            => Quick(array, comparison, 0, length);

        public static void Quick<T>(IList<T> array, Comparison<T> comparison, int start, int end)
            => Sort<T>.Quick(array, comparison, start, end);

        #endregion

        #region Comparisons

        // Int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByLargest(int item0, int item1) {
            return item1.CompareTo(item0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BySmallest(int item0, int item1) {
            return item0.CompareTo(item1);
        }

        // Float
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByLargest(float item0, float item1) {
            return item1.CompareTo(item0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BySmallest(float item0, float item1) {
            return item0.CompareTo(item1);
        }

        // Tuples
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByLargest<T>((int, T) item0, (int, T) item1) {
            return item1.Item1.CompareTo(item0.Item1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BySmallest<T>((int, T) item0, (int, T) item1) {
            return item0.Item1.CompareTo(item1.Item1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByLargest<T>((T, int) item0, (T, int) item1) {
            return item1.Item2.CompareTo(item0.Item2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BySmallest<T>((T, int) item0, (T, int) item1) {
            return item0.Item2.CompareTo(item1.Item2);
        }

        #endregion
    }

    internal static class Sort<T> {
        private static T[] cache = new T[32];
        private static List<T> enumeratorCache = new List<T>();

        #region Merge

        public static IEnumerable<T> Merge(IEnumerable<T> enu, Comparison<T> comparison) {
            enumeratorCache.Clear();
            enu.Foreach(x => enumeratorCache.Add(x));
            Merge(enumeratorCache, comparison, 0, enumeratorCache.Count);
            foreach(var e in enumeratorCache)
                yield return e;
        }

        public static void Merge(IList<T> array, Comparison<T> comparison, int start, int end) {
            if(array.Count > cache.Length) {
                cache = new T[UnityEngine.Mathf.NextPowerOfTwo(array.Count)];
            }
            RecursiveMerge(array, comparison, start, end);
        }

        private static void RecursiveMerge(IList<T> array, Comparison<T> comparison, int start, int end) {
            var diff = end - start;
            if(diff < 2) return; // Too small scope (already sorted)

            var mid = start + diff / 2;
            if(diff == 2) {
                // Only 2 units to compare with.
                // if comparison returns 1, switch out the elements.
                if(comparison(array[start], array[mid]) == 1) {
                    var temp = array[start];
                    array[start] = array[mid];
                    array[mid] = temp;
                    return;
                }
            }

            // Run recursive merge until reaching diff == 2 or less...
            RecursiveMerge(array, comparison, start, mid);
            RecursiveMerge(array, comparison, mid, end);

            // Unpack recursive merge through continues splitting.
            var index0 = start;
            var index1 = mid;

            for(int i = start; i < end; i++) {
                var res = comparison(array[index0], array[index1]);
                if(res < 1) {
                    // comparison already ordered, continue.
                    cache[i] = array[index0++];
                    if(index0 == index1)
                        break;
                }
                else {
                    // comparison should switch place...
                    cache[i] = array[index1++];
                    if(index1 >= end) {
                        // if ending, add all elements into the cache...
                        while(++i < end) {
                            cache[i] = array[index0++];
                        }
                        break;
                    }
                }
            }
            // set array to the cache values. 
            for(int i = start; i < index1; i++) {
                array[i] = cache[i];
            }
        }

        #endregion

        #region Quicksort

        public static void Quick(IList<T> array, Comparison<T> comparison, int start, int end) {
            // remove 1 from end as its being used in array accessing
            QuickRecursive(array, comparison, start, end - 1);
        }

        private static void QuickRecursive(IList<T> array, Comparison<T> comparison, int start, int end) {
            if(start < end) {
                int partition = QuickPartition(array, comparison, start, end);
                QuickRecursive(array, comparison, start, partition - 1);
                QuickRecursive(array, comparison, partition + 1, end);
            }
        }

        private static int QuickPartition(IList<T> array, Comparison<T> comparison, int start, int end) {
            var pivot = array[end];
            var index = start - 1;
            for(int i = start; i < end; i++) {
                // if comparison returns 1, then they need to switch place.
                if(comparison(pivot, array[i]) == 1) {
                    index++;
                    var temp = array[index];
                    array[index] = array[i];
                    array[i] = temp;
                }
            }
            // end with switching the last elements.
            index++;
            var t = array[index];
            array[index] = array[end];
            array[end] = t;

            return index;
        }

        #endregion
    }
}
