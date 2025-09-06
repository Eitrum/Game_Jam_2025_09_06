using System;
using UnityEngine;

namespace Toolkit.Mathematics {
    public static class MathUtility {
        #region Const

        public const float SQR_2 = 1.414213562373095f;

        public const float PI = Mathf.PI;
        public const float Rad2Deg = Mathf.Rad2Deg;
        public const float Deg2Rad = Mathf.Deg2Rad;
        public const float Deg2Linear = 1f / 360f;

        public const float E = 2.7182818284590452353602874f;
        public const float PHI = 1.6180339887498948482f;

        public const float HexOut2In = 0.8660254f;
        public const float HexIn2Out = 1.1547005f;

        internal static readonly int[] FibonacciSeq = { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181 };

        public const int RANDOM_CONSTANT_VALUE = 0xA410 | 0b0100_0101;

        #endregion

        #region Fibonacci

        public static int Fibonacci(int index) {
            if(index < 1)
                return 0;
            if(index < 3)
                return 1;
            if(index < FibonacciSeq.Length) {
                return FibonacciSeq[index];
            }

            int val0 = 1;
            int val1 = 2;

            for(int i = 3; i < index; i++) {
                var res = val0 + val1;
                val0 = val1;
                val1 = res;
            }

            return val1;
        }

        #endregion

        #region Fract

        public static float Fract(float value) {
            return value - Mathf.Floor(value);
        }

        public static double Fract(double value) {
            return value - Math.Floor(value);
        }

        #endregion

        #region Hash

        public static float Hash21(float x, float y) {
            x = (x * 29.31f) % 1f;
            y = (y * 82.93f) % 1f;
            var res = Vector2.Dot(new Vector2(x, y), new Vector2(x + 33.1f, y + 81.7f));
            return ((x + res) * (y + res)) % 1f;
        }

        public static float Hash31(float x, float y, float z) {
            x = (x * 29.31f) % 1f;
            y = (y * 82.93f) % 1f;
            z = (z * 31.7f) % 1f;
            var res = Vector3.Dot(new Vector3(x, y, z), new Vector3(x + 12.3f, y + 48.1f, z + 93.8f));
            return ((x + res) * (y + res) * (z + res)) % 1f;
        }

        public static float Hash41(float x, float y, float z, float w) {
            x = (x * 29.31f) % 1f;
            y = (y * 82.93f) % 1f;
            z = (z * 31.7f) % 1f;
            w = (w * 19.73f) % 1f;
            var res = Vector4.Dot(new Vector4(x, y, z, w), new Vector4(x + 12.3f, y + 48.1f, z + 93.8f, w + 71.8f));
            return ((x + res) * (y + res) * (z + res) * (w + res)) % 1f;
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamps the rotation value between the minimum and maximum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float ClampRotation(float value, float min, float max) {
            if(min > max || max - min > 360f)
                return value;

            if(min > 180f) {
                min -= 360f;
                max -= 360f;
            }
            else if(max < -180f) {
                min += 360f;
                max += 360f;
            }
            value %= 360f;
            var diff = 180f - ((max - min) * 0.5f);

            var t = value < 0f ? value + 360f : value;
            if(t > max + diff)
                return Mathf.Clamp(t - min - 360f, 0f, max - min) + min;
            return Mathf.Clamp(t - min, 0f, max - min) + min;
        }

        public static float ClampRotation(float value, MinMax range)
            => ClampRotation(value, range.min, range.max);

        #endregion

        #region Loop

        public static float Loop(float value, float min, float max) {
            if(min >= max) {
                Debug.LogError($"Unable to loop value as min ({min}) is equal or larger than max ({max})");
                return min;
            }

            var diff = max - min;
            while(value < min)
                value += diff;
            while(value > max)
                value -= diff;
            return value;
        }

        #endregion

        #region Largest / Smallest

        public static float Smallest(float lhs, float rhs) {
            var alhs = Mathf.Abs(lhs);
            var arhs = Mathf.Abs(rhs);
            return alhs < arhs ? lhs : rhs;
        }

        public static float Largest(float lhs, float rhs) {
            var alhs = Mathf.Abs(lhs);
            var arhs = Mathf.Abs(rhs);
            return alhs > arhs ? lhs : rhs;
        }

        #endregion

        #region Min

        public static float Min(float value0, float value1) {
            return value0 < value1 ? value0 : value1;
        }

        public static float Min(float value0, float value1, float value2) {
            var res = value0 < value1 ? value0 : value1;
            return res < value2 ? res : value2;
        }

        public static float Min(float value0, float value1, float value2, float value3) {
            var res = value0 < value1 ? value0 : value1;
            var res2 = value2 < value3 ? value2 : value3;
            return res < res2 ? res : res2;
        }

        #endregion

        #region Max

        public static float Max(float value0, float value1) {
            return value0 > value1 ? value0 : value1;
        }

        public static float Max(float value0, float value1, float value2) {
            var res = value0 > value1 ? value0 : value1;
            return res > value2 ? res : value2;
        }

        public static float Max(float value0, float value1, float value2, float value3) {
            var res = value0 > value1 ? value0 : value1;
            var res2 = value2 > value3 ? value2 : value3;
            return res > res2 ? res : res2;
        }

        #endregion

        #region 3 to 1 for arrays

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int CoordToIndex(int x, int y, int z, int height, int depth) {
            return (x * height * depth) + (y * depth) + (z);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int CoordToIndex(Vector3Int coord, int height, int depth) {
            return (coord.x * height * depth) + (coord.y * depth) + (coord.z);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static (int x, int y, int z) IndexToCoord(int index, int height, int depth) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            return (x, y, z);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static (int x, int y, int z) IndexToCoord(uint index, int height, int depth) {
            return IndexToCoord((int)index, height, depth);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector3Int IndexToCoord_Vector3Int(int index, int height, int depth) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            return new(x, y, z);
        }

        #endregion
    }
}
