using System;
using UnityEngine;

namespace Toolkit
{
    public static class MathExtensions
    {

        #region Rounding

        public static float Round(this float val) => Mathf.Round(val);
        public static int RoundToInt(this float val) => Mathf.RoundToInt(val);

        #endregion

        #region Snap

        public static float Snap(this float val) => Mathf.Round(val);
        public static float Snap(this float val, float scale) => scale == 0f ? Mathf.Round(val) : Mathf.Round(val / scale) * scale;
        public static int SnapInt(this float val) => Mathf.RoundToInt(val);
        public static int SnapInt(this float val, float scale) => scale == 0f ? (int)Mathf.RoundToInt(val) : (int)(Mathf.Round(val / scale) * scale);

        public static double Snap(this double val) => Math.Round(val);
        public static double Snap(this double val, double scale) => val == 0d ? Math.Round(val) : Math.Round(val / scale) * scale;

        #endregion

        #region Easing

        public static float Ease(this float time, Mathematics.Ease.Function function, Mathematics.Ease.Type type)
            => Mathematics.Ease.GetEaseFunction(function, type)(time);

        #endregion

        #region Simple Math

        public static float Sqr(this float value) => value * value;
        public static float Cube(this float value) => value * value * value;
        public static float Pow(this float value, float power) => Mathf.Pow(value, power);
        public static float Sqrt(this float value) => Mathf.Sqrt(value);

        public static double Sqr(this double value) => value * value;
        public static double Cube(this double value) => value * value * value;
        public static double Pow(this double value, double power) => Math.Pow(value, power);
        public static double Sqrt(this double value) => Math.Sqrt(value);

        public static int Sqr(this int value) => value * value;
        public static int Cube(this int value) => value * value * value;

        #endregion

        #region Remap
        
        public static float Remap(this float value, float min0, float max0, float min1, float max1){
            var il = Mathf.InverseLerp(min0, max0, value);
            return Mathf.Lerp(min1, max1, Mathf.Clamp01(il));
        }

        public static float RemapUnclamped(this float value, float min0, float max0, float min1, float max1){
            var il = Mathf.InverseLerp(min0, max0, value);
            return Mathf.Lerp(min1, max1, il);
        }

        #endregion

        #region Clamping

        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
        public static void ClampRef(ref this float value, float min, float max) => value = Mathf.Clamp(value, min, max);

        public static float Clamp01(this float value) => Mathf.Clamp01(value);
        public static void Clamp01Ref(ref this float value) => value = Mathf.Clamp01(value);

        public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
        public static void ClampRef(ref this int value, int min, int max) => value = Mathf.Clamp(value, min, max);

        public static float Max(this float value, float maxValue, out float remainder) {
            if(value > maxValue) {
                remainder = value - maxValue;
                return maxValue;
            }
            remainder = 0f;
            return value;
        }

        public static float Min(this float value, float minValue, out float remainder) {
            if(value < minValue) {
                remainder = minValue - value;
                return minValue;
            }
            remainder = 0f;
            return value;
        }

        public static float Clamp(this float value, float min, float max, out float remainder) {
            if(value > max) {
                remainder = value - max;
                return max;
            }
            if(value < min) {
                remainder = min - value;
                return min;
            }
            remainder = 0f;
            return value;
        }

        #endregion

        #region Inverse Lerp

        public static float InverseLerp(this int val, int min, int max) => (val - min) / (float)(max - min);
        public static float InverseLerp(this float val, float min, float max) => (val - min) / (max - min);

        #endregion

        #region Converting

        public static Color ToColor(this double value) => ToColor((float)value);
        public static Color ToColor(this float value) {
            return new Color(value, value, value, 1f);
        }

        public static Color ToColorInvert(this double value) => ToColorInvert((float)value);
        public static Color ToColorInvert(this float value) {
            value = 1f - value;
            return new Color(value, value, value, 1f);
        }

        #endregion

        #region Equals

        public static bool Equals(this float value, float otherValue, float proximity) {
            return value + proximity >= otherValue && value - proximity <= otherValue;
        }

        #endregion
    }
}
