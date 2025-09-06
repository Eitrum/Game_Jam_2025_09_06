using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMaxInt
    {
        #region Variables

        private const string TAG = "[MinMaxInt] - ";

        public int min;
        public int max;

        #endregion

        #region Properties

        /// <summary>
        /// Random value between min and max inclusive. (Range(min, max+1))
        /// </summary>
        public int Random => UnityEngine.Random.Range(min, max + 1);
        public int RandomExclusive => UnityEngine.Random.Range(min, max);
        public static MinMaxInt Zero => new MinMaxInt(0, 0);
        public static MinMaxInt MaximumRange => new MinMaxInt(int.MinValue, int.MaxValue);
        public static MinMaxInt InvertedMaximumRange => new MinMaxInt() { min = int.MaxValue, max = int.MinValue };

        #endregion

        #region Constructor

        public MinMaxInt(int value) {
            this.min = value;
            this.max = value;
#if UNITY_EDITOR
            if(this.min > this.max)
                Debug.LogWarning(TAG + $"Minimum value is greater than maximum");
#endif
        }

        public MinMaxInt(int min, int max) {
            this.min = min;
            this.max = max;
#if UNITY_EDITOR
            if(this.min > this.max)
                Debug.LogWarning(TAG + $"Minimum value is greater than maximum");
#endif
        }

        public MinMaxInt(float value, Mathematics.RoundingMode roundingMode = Mathematics.RoundingMode.Round) {
            switch(roundingMode) {
                case Mathematics.RoundingMode.Floor:
                    min = Mathf.FloorToInt(value);
                    max = Mathf.FloorToInt(value);
                    break;
                case Mathematics.RoundingMode.Ceil:
                    min = Mathf.CeilToInt(value);
                    max = Mathf.CeilToInt(value);
                    break;
                default:
                    min = Mathf.RoundToInt(value);
                    max = Mathf.RoundToInt(value);
                    break;
            }
#if UNITY_EDITOR
            if(this.min > this.max)
                Debug.LogWarning(TAG + $"Minimum value is greater than maximum");
#endif
        }

        public MinMaxInt(MinMax range, Mathematics.RoundingMode roundingMode = Mathematics.RoundingMode.Round) {
            switch(roundingMode) {
                case Mathematics.RoundingMode.Floor:
                    min = Mathf.FloorToInt(range.min);
                    max = Mathf.FloorToInt(range.max);
                    break;
                case Mathematics.RoundingMode.Ceil:
                    min = Mathf.CeilToInt(range.min);
                    max = Mathf.CeilToInt(range.max);
                    break;
                default:
                    min = Mathf.RoundToInt(range.min);
                    max = Mathf.RoundToInt(range.max);
                    break;
            }
#if UNITY_EDITOR
            if(this.min > this.max)
                Debug.LogWarning(TAG + $"Minimum value is greater than maximum");
#endif
        }

        #endregion

        #region Utility

        /// <summary>
        /// Verifies and fixes so the min value is less than max.
        /// </summary>
        public void Verify() {
            if(max < min) {
                var t = min;
                min = max;
                max = t;
            }
        }

        public static MinMaxInt CombineMaximumRange(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(Mathf.Min(lhs.min, rhs.min), Mathf.Max(lhs.max, rhs.max));
        public static MinMaxInt CombineMinimumRange(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(Mathf.Max(lhs.min, rhs.min), Mathf.Min(lhs.max, rhs.max));

        #endregion

        #region Clamp

        public int Clamp(int value) => Mathf.Clamp(value, min, max);
        public float Clamp(float value) => (value > max) ? (float)max : ((value < min) ? (float)min : value);
        public static int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);

        #endregion

        #region Linear Interpolation

        public int Evaluate(float t) => min + Mathf.RoundToInt((max - min) * t);
        public int Lerp(float t) => min + Mathf.RoundToInt((max - min) * t);
        public static int Evaluate(int min, int max, float t) => min + Mathf.RoundToInt((max - min) * t);
        public static int Lerp(int min, int max, float t) => min + Mathf.RoundToInt((max - min) * t);
        public float InverseEvaluate(int value) => (value - min) / (float)(max - min);
        public float InverseLerp(int value) => (value - min) / (float)(max - min);
        public static float InverseEvaluate(int value, int min, int max) => (value - min) / (float)(max - min);
        public static float InverseLerp(int value, int min, int max) => (value - min) / (float)(max - min);

        #endregion

        #region Range Check

        public bool Contains(int value) => min <= value && max >= value;
        public bool Contains(float value) => min <= value && max >= value;

        #endregion

        #region Conversion

        public MinMax ToFloat() => new MinMax(min, max);
        public static implicit operator MinMax(MinMaxInt val) => new MinMax(val.min, val.max);

        #endregion

        #region ToString

        public override string ToString() => $"({min} -> {max})";
        /// <summary>
        /// Returns a result similar to '4-6'
        /// </summary>
        /// <returns></returns>
        public string ToStringShort() => $"{min}-{max}";
        public string ToStringFormat(string format) => string.Format(format, min, max);

        #endregion

        #region Multiply

        public static MinMaxInt Multiply(MinMaxInt lhs, float rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min * rhs), Mathf.RoundToInt(lhs.max * rhs));
        public static MinMaxInt Multiply(MinMaxInt lhs, float rhs, Toolkit.Mathematics.RoundingMode roundingMode) {
            switch(roundingMode) {
                case Mathematics.RoundingMode.Ceil: return new MinMaxInt(Mathf.CeilToInt(lhs.min * rhs), Mathf.CeilToInt(lhs.max * rhs));
                case Mathematics.RoundingMode.Floor: return new MinMaxInt(Mathf.FloorToInt(lhs.min * rhs), Mathf.FloorToInt(lhs.max * rhs));
                case Mathematics.RoundingMode.Round: return new MinMaxInt(Mathf.RoundToInt(lhs.min * rhs), Mathf.RoundToInt(lhs.max * rhs));
            }
            return lhs;
        }
        public static MinMaxInt Multiply(MinMaxInt lhs, float rhs, Toolkit.Mathematics.RoundingMode roundingMin, Toolkit.Mathematics.RoundingMode roundingMax) {
            int max = lhs.max;
            int min = lhs.min;
            switch(roundingMin) {
                case Mathematics.RoundingMode.Ceil: min = Mathf.CeilToInt(min * rhs); break;
                case Mathematics.RoundingMode.Floor: min = Mathf.FloorToInt(min * rhs); break;
                case Mathematics.RoundingMode.Round: min = Mathf.RoundToInt(min * rhs); break;
            }
            switch(roundingMax) {
                case Mathematics.RoundingMode.Ceil: max = Mathf.CeilToInt(max * rhs); break;
                case Mathematics.RoundingMode.Floor: max = Mathf.FloorToInt(max * rhs); break;
                case Mathematics.RoundingMode.Round: max = Mathf.RoundToInt(max * rhs); break;
            }
            return new MinMaxInt(min, max);
        }
        public static MinMaxInt MultiplyFloorMinCeilMax(MinMaxInt lhs, float rhs) => new MinMaxInt(Mathf.FloorToInt(lhs.min * rhs), Mathf.CeilToInt(lhs.max * rhs));

        #endregion

        #region Average

        public static MinMaxInt Average(MinMaxInt lhs, MinMaxInt rhs) {
            return new MinMaxInt((lhs.min + rhs.min) / 2, (lhs.max + rhs.max) / 2);
        }

        public static MinMaxInt Average(MinMaxInt lhs, MinMaxInt rhs, Toolkit.Mathematics.RoundingMode rounding) {
            switch(rounding) {
                case Mathematics.RoundingMode.Floor:
                case Mathematics.RoundingMode.Round:
                    return new MinMaxInt((lhs.min + rhs.min) / 2, (lhs.max + rhs.max) / 2);
                case Mathematics.RoundingMode.Ceil:
                    return new MinMaxInt(Mathf.CeilToInt((lhs.min + rhs.min) / 2f), Mathf.CeilToInt((lhs.max + rhs.max) / 2f));
            }
            throw new System.Exception($"[MinMaxInt] - missing rounding mode!");
        }

        #endregion

        #region Operators

        public static MinMaxInt operator *(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(lhs.min * rhs.min, lhs.max * rhs.max);
        public static MinMaxInt operator /(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(lhs.min / rhs.min, lhs.max / rhs.max);
        public static MinMaxInt operator +(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(lhs.min + rhs.min, lhs.max + rhs.max);
        public static MinMaxInt operator -(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(lhs.min - rhs.min, lhs.max - rhs.max);
        public static MinMaxInt operator %(MinMaxInt lhs, MinMaxInt rhs) => new MinMaxInt(lhs.min % rhs.min, lhs.max % rhs.max);

        public static MinMaxInt operator *(MinMaxInt lhs, MinMax rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min * rhs.min), Mathf.RoundToInt(lhs.max * rhs.max));
        public static MinMaxInt operator /(MinMaxInt lhs, MinMax rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min / rhs.min), Mathf.RoundToInt(lhs.max / rhs.max));
        public static MinMaxInt operator +(MinMaxInt lhs, MinMax rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min + rhs.min), Mathf.RoundToInt(lhs.max + rhs.max));
        public static MinMaxInt operator -(MinMaxInt lhs, MinMax rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min - rhs.min), Mathf.RoundToInt(lhs.max - rhs.max));

        public static MinMaxInt operator *(MinMaxInt lhs, int rhs) => new MinMaxInt(lhs.min * rhs, lhs.max * rhs);
        public static MinMaxInt operator /(MinMaxInt lhs, int rhs) => new MinMaxInt(lhs.min / rhs, lhs.max / rhs);
        public static MinMaxInt operator +(MinMaxInt lhs, int rhs) => new MinMaxInt(lhs.min + rhs, lhs.max + rhs);
        public static MinMaxInt operator -(MinMaxInt lhs, int rhs) => new MinMaxInt(lhs.min - rhs, lhs.max - rhs);
        public static MinMaxInt operator %(MinMaxInt lhs, int rhs) => new MinMaxInt(lhs.min % rhs, lhs.max % rhs);

        public static MinMaxInt operator *(MinMaxInt lhs, float rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min * rhs), Mathf.RoundToInt(lhs.max * rhs));
        public static MinMaxInt operator /(MinMaxInt lhs, float rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min / rhs), Mathf.RoundToInt(lhs.max / rhs));
        public static MinMaxInt operator +(MinMaxInt lhs, float rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min + rhs), Mathf.RoundToInt(lhs.max + rhs));
        public static MinMaxInt operator -(MinMaxInt lhs, float rhs) => new MinMaxInt(Mathf.RoundToInt(lhs.min - rhs), Mathf.RoundToInt(lhs.max - rhs));

        #endregion
    }
}
