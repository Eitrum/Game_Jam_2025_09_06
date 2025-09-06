using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Mathematics {
    public static class Random {
        #region Const

        private const string TAG = "[Toolkit.Random] - ";
        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int ALPHABET_COUNT = 26;
        private const string CHARACTER_ARRAY = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int CHARACTER_ARRAY_COUNT = 62;

        #endregion

        #region Initialization

        static Random() {
            seed = TKDateTime.UtcNow.Millisecond;
            random = new System.Random(seed);
        }

        #endregion

        #region Variables

        private static System.Random random;
        private static int seed;

        #endregion

        #region Properties

        public static int Seed {
            get => seed;
            set => SetSeed(value);
        }

        public static int Int => random.Next();
        public static double Double => random.NextDouble();
        public static float Float => (float)random.NextDouble();
        public static byte Byte => (byte)(Int & 0xff);
        public static short Short => (short)(Int & 0xffff);
        public static bool Bool => random.Next(2) == 0;

        public static char Character => ALPHABET[Int % ALPHABET_COUNT];


        public static Quaternion Rotation2D => Quaternion.Euler(0, 0, Float * 360f);
        public static Quaternion Rotation => Quaternion.Euler(Float * 360f, Float * 360f, Float * 360f);

        public static Vector2 OnUnitCircle {
            get {
                var value = Float * Mathf.PI * 2f;
                return new Vector2(Mathf.Sin(value), Mathf.Cos(value));
            }
        }
        public static Vector2 InsideUnitCircle {
            get {
                return Rotation2D * new Vector2(0, Mathf.Sqrt(Float));
            }
        }

        public static Vector3 OnUnitSphere => Rotation * new Vector3(0, 0, 1f);
        public static Vector3 InsideUnitSphere {
            get {
                var value = Float;
                return Rotation * new Vector3(0, 0, (1f - (value * value * value)));
            }
        }

        #endregion

        #region Set Seed

        public static void SetSeed(int seed) {
            Random.seed = seed;
            random = new System.Random(seed);
        }

        #endregion

        #region Array

        public static void Suffle<T>(IList<T> array) {
            int length = array.Count;
            for(int i = 0; i < length; i++) {
                var index = Range(length);
                T temp = array[i];
                array[i] = array[index];
                array[index] = temp;
            }
        }

        public static void Suffle<T>(IList<T> array, int count) {
            for(int i = 0; i < count; i++) {
                Suffle(array);
            }
        }

        public static T Element<T>(IReadOnlyList<T> array) => array[Int % array.Count];

        public static T Element<T>(IReadOnlyList<T> array, Func<T, float> probability) {
            int length = array.Count;
            float totalProbability = 0f;
            float[] probabilityCache = new float[length];
            for(int i = 0; i < length; i++) {
                totalProbability += probabilityCache[i] = probability(array[i]);
            }

            return Element(array, probabilityCache, totalProbability);
        }

        public static T Element<T>(IReadOnlyList<T> array, IReadOnlyList<float> probabilityCache, float totalProbability = 1f) {
            var value = Range(totalProbability);
            var length = array.Count;
            for(int i = 0; i < length; i++) {
                value -= probabilityCache[i];
                if(value <= 0f) {
                    return array[i];
                }
            }
            return default;
        }

        public static T Element<T>(IReadOnlyList<T> array, Func<T, bool> available) {
            return Element(array, (t) => available(t) ? 1f : 0f);
        }

        #endregion

        #region Range

        public static int Range(int max) => random.Next(max);
        public static int Range(int min, int max) => random.Next(min, max);

        public static float Range(float max) => max * Float;
        public static float Range(float min, float max) => min + (max - min) * Float;

        public static double Range(double max) => max * Double;
        public static double Range(double min, float max) => min + (max - min) * Double;

        public static byte Range(byte max) => (byte)(Int % max);
        public static byte Range(byte min, byte max) => (byte)(min + (byte)(Int % (max - min)));

        public static byte RangeInc(byte max) => (byte)(Int % (max + 1));
        public static byte RangeInc(byte min, byte max) => (byte)(min + (byte)(Int % ((max + 1) - min)));

        #endregion

        #region Color

        public static Color ColorHSV() => Color.HSVToRGB(Float, Float, Float);
        public static Color ColorHSV(float minHue, float maxHue, float minSat, float maxSat, float minBrightness, float maxBrightness)
            => Color.HSVToRGB(Range(minHue, maxHue), Range(minSat, maxSat), Range(minBrightness, maxBrightness));
        public static Color ColorHSV(float minHue, float maxHue, float minSat, float maxSat, float minBrightness, float maxBrightness, bool hdr)
            => Color.HSVToRGB(Range(minHue, maxHue), Range(minSat, maxSat), Range(minBrightness, maxBrightness), hdr);

        public static Color ColorRGBA() => new Color32(Byte, Byte, Byte, Byte);
        public static Color ColorRGBA(byte minRed, byte maxRed, byte minGreen, byte maxGreen, byte minBlue, byte maxBlue)
            => ColorRGBA(minRed, maxRed, minGreen, maxGreen, minBlue, maxBlue, 0, 255);
        public static Color ColorRGBA(byte minRed, byte maxRed, byte minGreen, byte maxGreen, byte minBlue, byte maxBlue, byte minAlpha, byte maxAlpha)
            => new Color32(RangeInc(minRed, maxRed), RangeInc(minGreen, maxGreen), RangeInc(minBlue, maxBlue), RangeInc(minAlpha, maxAlpha));

        public static Color ColorRGB() => new Color32(Byte, Byte, Byte, 255);
        public static Color ColorRGB(byte alpha) => new Color32(Byte, Byte, Byte, alpha);

        #endregion

        #region String

        public static string String(int length) {
            char[] array = new char[length];
            for(int i = 0; i < length; i++) {
                array[i] = CHARACTER_ARRAY[Range(CHARACTER_ARRAY_COUNT)];
            }
            return new string(array);
        }

        #endregion

        #region ByLength

        public static int ByLength(int length) {
            int result = 0;
            if(length > 9) {
                Debug.LogWarning(TAG + "Int maximum digits is 9");
                length = 9;
            }
            if(length < 1) {
                Debug.LogWarning(TAG + "Int minimum digits is 1");
                length = 1;
            }

            for(int i = 0; i < length; i++) {
                result *= 10;
                result += Range(0, 10);
            }

            return result;
        }

        #endregion
    }
}
