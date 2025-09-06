using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMax
    {
        #region Variables

        private const string TAG = "[MinMax] - ";

        public float min;
        public float max;

        #endregion

        #region Properties

        public float Random => UnityEngine.Random.Range(min, max);
        public float Mid => min + ((max - min) * 0.5f);

        public static MinMax EulerRotation => new MinMax(0, 360f);
        public static MinMax Zero => new MinMax(0f, 0f);
        public static MinMax MaximumRange => new MinMax(float.MinValue, float.MaxValue);
        public static MinMax InvertedMaximumRange => new MinMax(float.MinValue, float.MaxValue);

        #endregion

        #region Constructor

        public MinMax(float value) {
            this.min = value;
            this.max = value;
        }

        public MinMax(float min, float max) {
            this.min = min;
            this.max = max;
        }

        public MinMax(MinMaxInt range) {
            this.min = range.min;
            this.max = range.max;
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

        public static MinMax CombineMaximumRange(MinMax lhs, MinMax rhs) => new MinMax(Mathf.Min(lhs.min, rhs.min), Mathf.Max(lhs.max, rhs.max));
        public static MinMax CombineMinimumRange(MinMax lhs, MinMax rhs) => new MinMax(Mathf.Max(lhs.min, rhs.min), Mathf.Min(lhs.max, rhs.max));

        #endregion

        #region Clamp

        public float Clamp(float value) => Mathf.Clamp(value, min, max);
        public int Clamp(int value) => (value > max) ? (int)max : ((value < min) ? (int)min : value);

        #endregion

        #region Linear Interpolation

        /// <summary>
        /// Evaluates linear interpolated value based on min and max. (Unclamped)
        /// </summary>
        public float Evaluate(float t) => min + ((max - min) * t);
        public float Lerp(float t) => min + ((max - min) * Mathf.Clamp01(t));
        public float LerpUnclamped(float t) => min + ((max - min) * t);

        public static float Evaluate(float min, float max, float t) => min + ((max - min) * t);
        public static float Lerp(float min, float max, float t) => min + ((max - min) * Mathf.Clamp01(t));
        public static float LerpUnclamped(float min, float max, float t) => min + ((max - min) * t);

        #endregion

        #region Inverse Linear Interpolation

        public float InverseEvaluate(float value) => (value - min) / (max - min);
        public float InverseLerp(float value) => Mathf.Clamp01((value - min) / (max - min));
        public float InverseLerpUnclamped(float value) => (value - min) / (max - min);

        public static float InverseEvaluate(float min, float max, float value) => (value - min) / (max - min);
        public static float InverseLerp(float min, float max, float value) => Mathf.Clamp01((value - min) / (max - min));
        public static float InverseLerpUnclamped(float min, float max, float value) => (value - min) / (max - min);

        #endregion

        #region Remap

        public static float Remap(float value, MinMax from, MinMax to)
            => to.LerpUnclamped(from.InverseLerpUnclamped(value));

        #endregion

        #region Contains

        public bool Contains(float value) => min <= value && max >= value;
        public bool Contains(int value) => min <= value && max >= value;

        #endregion

        #region Conversion

        public MinMaxInt ToInt(Mathematics.RoundingMode mode = Mathematics.RoundingMode.Round) => new MinMaxInt(this, mode);

        /// <summary>
        /// Converts to a vector2 with (min as X) and (max as Y)
        /// </summary>
        public Vector2 ToVector2() => new Vector2(min, max);

        #endregion

        #region Overrides

        public override string ToString() => $"({min:#.##} -> {max:#.##})";

        #endregion

        #region Operators

        public static MinMax operator *(MinMax range, float value) => new MinMax(range.min * value, range.max * value);
        public static MinMax operator /(MinMax range, float value) => new MinMax(range.min / value, range.max / value);
        public static MinMax operator +(MinMax range, float value) => new MinMax(range.min + value, range.max + value);
        public static MinMax operator -(MinMax range, float value) => new MinMax(range.min - value, range.max - value);

        public static implicit operator Vector2(MinMax value) => value.ToVector2();

        #endregion
    }
}
