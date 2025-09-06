using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public static class ColorExtensions
    {
        #region To Hex

        public static string ToHex24(this Color color) {
            return ToHex24((Color32)color);
        }

        public static string ToHex32(this Color color) {
            return ToHex32((Color32)color);
        }

        public static string ToHex24(this Color32 color) {
            return $"{color.r:X2}{color.g:X2}{color.b:X2}";
        }

        public static string ToHex32(this Color32 color) {
            return $"{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
        }

        #endregion

        #region Invert

        public static Color Invert(this Color color)
            => new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);

        public static Color32 Invert(this Color32 color)
                    => new Color32(
                        (byte)(byte.MaxValue - color.r),
                        (byte)(byte.MaxValue - color.g),
                        (byte)(byte.MaxValue - color.b),
                        color.a);

        public static void InvertRef(this ref Color color)
            => color = new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);

        public static void InvertRef(this ref Color32 color)
                    => color = new Color32(
                        (byte)(byte.MaxValue - color.r),
                        (byte)(byte.MaxValue - color.g),
                        (byte)(byte.MaxValue - color.b),
                        color.a);

        #endregion

        #region Int

        public static int ToInt(this Color color) {
            return ToInt((Color32)color);
        }

        public static int ToInt(this Color32 color) {
            return color.r << 24 | color.g << 16 | color.b << 8 | color.a;
        }

        public static uint ToUInt(this Color color) {
            return ToUInt((Color32)color);
        }

        public unsafe static uint ToUInt(this Color32 color) {
            uint result = 0;
            byte* p = (byte*)&result;
            if(System.BitConverter.IsLittleEndian) {
                *(p + 3) = color.r;
                *(p + 2) = color.g;
                *(p + 1) = color.b;
                *(p) = color.a;
            }
            else {
                *(p) = color.r;
                *(p + 1) = color.g;
                *(p + 2) = color.b;
                *(p + 3) = color.a;
            }
            return result;
        }

        public static Color ToColor(this int value) {
            return ToColor32(value);
        }

        public static Color32 ToColor32(this int value) {
            const int mask = 0xff;
            return new Color32(
                (byte)((value >> 24) & mask),
                (byte)((value >> 16) & mask),
                (byte)((value >> 8) & mask),
                (byte)(value & mask)
                );
        }

        public static Color ToColor(this uint value) {
            return ToColor32(value);
        }

        public unsafe static Color32 ToColor32(this uint value) {
            byte* p = (byte*)&value;
            if(System.BitConverter.IsLittleEndian) {
                return new Color32(
                 *(p + 3),
                 *(p + 2),
                 *(p + 1),
                 *(p));
            }
            else {
                return new Color32(
                *p,
                *(p + 1),
                *(p + 2),
                *(p + 3));
            }
        }

        #endregion

        #region Grayscale

        public static Color ToGrayscale(this Color color) { // (0.3, 0.59, 0.11)
            float g = color.r * 0.3f + color.g * 0.59f + color.b * 0.11f;
            return new Color(g, g, g, color.a);
        }

        #endregion

        #region Hue Shift

        public static Color HueShiftByRotation(this Color color, float rotation) {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return Color.HSVToRGB((h + (rotation / 360f)) % 1f, s, v);
        }

        #endregion

        #region Multiply

        public static Color MultiplyRGB(this Color color, float multiplier) {
            color.r *= multiplier;
            color.g *= multiplier;
            color.b *= multiplier;
            return color;
        }

        public static Color MultiplyAlpha(this Color color, float multiplier) {
            color.a *= multiplier;
            return color;
        }

        #endregion

        #region Equals

        public static bool Equals(this Color color, Color otherColor, float errorRange = 0.004f) {
            return color.r.Equals(otherColor.r, errorRange) &&
                color.g.Equals(otherColor.g, errorRange) &&
                color.b.Equals(otherColor.b, errorRange) &&
                color.a.Equals(otherColor.a, errorRange);
        }

        public static bool Equals(this Color32 color, Color32 otherColor) {
            return color.r == otherColor.r &&
                color.g == otherColor.g &&
                color.b == otherColor.b &&
                color.a == otherColor.a;
        }

        public static bool Equals(this Color32 color, Color32 otherColor, int errorRange) {
            return Mathf.Abs(color.r - otherColor.r) <= errorRange &&
                Mathf.Abs(color.g - otherColor.g) <= errorRange &&
                Mathf.Abs(color.b - otherColor.b) <= errorRange &&
                Mathf.Abs(color.a - otherColor.a) <= errorRange;
        }

        #endregion
    }

    public static class ColorUtilityEx
    {
        private const string TAG = "<color=cyan>[Color Utility]</color> - ";

        #region Color from index

        private static int Bit(int a, int b) => (a & (1 << b)) >> b;

        public static Color GetAreaColor(int i)
            => GetAreaColor(i, 1f);

        public static Color GetAreaColor(int i, float alpha) {
            if(i == 0)
                return new Color(0, 0.75f, 1.0f, alpha);
            int r = (Bit(i, 4) + Bit(i, 1) * 2 + 1) * 63;
            int g = (Bit(i, 3) + Bit(i, 2) * 2 + 1) * 63;
            int b = (Bit(i, 5) + Bit(i, 0) * 2 + 1) * 63;
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, alpha);
        }

        #endregion

        #region Index Strength From Color Value

        public static IndexStrength GetIndexAndStrength(int value, int width, int height)
            => Internal_GetIndexAndStrength(value / 255f, width * height);

        public static IndexStrength GetIndexAndStrength(byte value, int width, int height)
            => Internal_GetIndexAndStrength(value / 255f, width * height);

        public static IndexStrength GetIndexAndStrength(float value, int width, int height)
            => Internal_GetIndexAndStrength(value, width * height);


        private static IndexStrength Internal_GetIndexAndStrength(float value, int textures) {
#if UNITY_EDITOR
            if(textures > 256)
                throw new Exception(TAG + $"Too many textures in array!");
#endif
            int scale = 256 / textures;
            var t = (value * 255f) / scale;
            int index = (int)t;
            float strength = 1f - (t - index);
            return new IndexStrength(index, strength);
        }

        [System.Serializable]
        public struct IndexStrength
        {
            public int Index;
            public float Strength;

            public IndexStrength(int index, float strength) {
                this.Index = index;
                this.Strength = strength;
            }
        }

        #endregion
    }
}
