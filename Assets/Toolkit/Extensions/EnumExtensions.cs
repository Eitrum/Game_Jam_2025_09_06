using System;

namespace Toolkit
{
    public static class EnumExtensions
    {
        #region Conversion

        public static T ToEnum<T>(this int value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this byte value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this sbyte value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this short value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this ushort value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this uint value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this long value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        public static T ToEnum<T>(this ulong value) where T : Enum => (T)Convert.ChangeType(value, FastEnum.GetUnderlyingType<T>());
        
        public static T ToEnum<T>(this string value) where T : Enum => FastEnum.Parse<T>(value);
        public static T ToEnum<T>(this string value, T defaultValue) where T : Enum => FastEnum.TryParse<T>(value, out T res) ? res : defaultValue;
        public static bool ToEnum<T>(this string value, out T result) where T : struct, Enum => FastEnum.TryParse<T>(value, out result);
        public static T ToEnum<T>(this string value, bool ignoreCase) where T : Enum => FastEnum.Parse<T>(value, ignoreCase);
        public static bool ToEnum<T>(this string value, bool ignoreCase, out T result) where T : struct, Enum => FastEnum.TryParse<T>(value, ignoreCase, out result);

        public static int ToInt<T>(this T @enum) where T : Enum => (int)Convert.ChangeType(@enum, TypeCode.Int32);
        public static object ToInt<T>(this T @enum, IntegerType type) where T : Enum => Convert.ChangeType(@enum, type.ToTypeCode());

        /// <summary>
        /// This will attempt to change one enum type to an other with the same value.
        /// </summary>
        public static TOther ToEnum<T, TOther>(this T @enum) where T : Enum where TOther : Enum => (TOther)Convert.ChangeType(@enum, FastEnum.GetUnderlyingType<TOther>());

        #endregion

        #region Length

        public static int GetLength(this Enum @enum) {
            return FastEnum.GetValueCount(@enum.GetType());
        }

        public static int GetLength<T>() where T : Enum {
            return FastEnum.GetValueCount<T>();
        }

        #endregion

        #region Arrays

        public static T[] GetArray<T>(this T @enum) where T : Enum {
            var values = FastEnum.GetValues<T>();
            var length = values.Count;
            T[] array = new T[length];
            for(int i = 0; i < length; i++) {
                array[i] = values[i];
            }
            return array;
        }

        public static TValue[] GetArrayValues<TEnum, TValue>(this TEnum @enum) where TEnum : Enum {
            var values = FastEnum.GetValues<TEnum>();
            var length = values.Count;
            var t = typeof(TValue);
            TValue[] array = new TValue[length];
            for(int i = 0; i < length; i++) {
                array[i] = (TValue)Convert.ChangeType(values[i], t);
            }
            return array;
        }

        #endregion

        #region Flag <--> Index

        /// <summary>
        /// Returns position of the flag as it was integer between 0 -> 31 (including)
        /// Returns -1 if no flag is set.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static int GetFlagIndex(this int mask) {
            for(int i = 0; i < 32; i++) {
                if(((mask >> i) & 1) == 1) {
                    return i;
                }
            }
            return -1;
        }

        public static int GetHighestFlagIndex(this int mask) {
            for(int i = 31; i >= 0; i--) {
                if(((mask >> i) & 1) == 1) {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Any

        public static bool Any<T>(this T @enum, T val0) where T : Enum {
            return @enum.ToInt() == val0.ToInt();
        }

        public static bool Any<T>(this T @enum, T val0, T val1) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() || @base == val1.ToInt());
        }

        public static bool Any<T>(this T @enum, T val0, T val1, T val2) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() ||
                @base == val1.ToInt() ||
                @base == val2.ToInt());
        }

        public static bool Any<T>(this T @enum, T val0, T val1, T val2, T val3) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() ||
                @base == val1.ToInt() ||
                @base == val2.ToInt() ||
                @base == val3.ToInt());
        }

        public static bool Any<T>(this T @enum, T val0, T val1, T val2, T val3, T val4) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() ||
                @base == val1.ToInt() ||
                @base == val2.ToInt() ||
                @base == val3.ToInt() ||
                @base == val4.ToInt());
        }

        public static bool Any<T>(this T @enum, T val0, T val1, T val2, T val3, T val4, T val5) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() ||
                @base == val1.ToInt() ||
                @base == val2.ToInt() ||
                @base == val3.ToInt() ||
                @base == val4.ToInt() ||
                @base == val5.ToInt());
        }

        public static bool Any<T>(this T @enum, T val0, T val1, T val2, T val3, T val4, T val5, T val6) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() ||
                @base == val1.ToInt() ||
                @base == val2.ToInt() ||
                @base == val3.ToInt() ||
                @base == val4.ToInt() ||
                @base == val5.ToInt() ||
                @base == val6.ToInt());
        }

        public static bool Any<T>(this T @enum, T val0, T val1, T val2, T val3, T val4, T val5, T val6, T val7) where T : Enum {
            var @base = @enum.ToInt();
            return (@base == val0.ToInt() ||
                @base == val1.ToInt() ||
                @base == val2.ToInt() ||
                @base == val3.ToInt() ||
                @base == val4.ToInt() ||
                @base == val5.ToInt() ||
                @base == val6.ToInt() ||
                @base == val7.ToInt());
        }

        public static bool Any<T>(this T @enum, params T[] values) where T : Enum {
            var @base = @enum.ToInt();
            for(int i = 0, length = values.Length; i < length; i++) {
                if(values[i].ToInt() == @base)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
