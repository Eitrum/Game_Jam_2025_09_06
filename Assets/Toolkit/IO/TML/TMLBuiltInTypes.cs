using System;
using Debug = UnityEngine.Debug;

namespace Toolkit.IO.TML {
    public static class TMLBuiltInTypes {

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLBuiltInTypes]</color> - ";

        #region C#

        public const byte None = 0;

        public const byte Boolean = 3;
        public const byte Char = 4;

        public const byte SByte = 5;
        public const byte Byte = 6;

        public const byte Short = Int16;
        public const byte Int16 = 7;
        public const byte UShort = UInt16;
        public const byte UInt16 = 8;

        public const byte Int = Int32;
        public const byte Int32 = 9;
        public const byte UInt = UInt32;
        public const byte UInt32 = 10;

        public const byte Long = Int64;
        public const byte Int64 = 11;
        public const byte ULong = UInt64;
        public const byte UInt64 = 12;

        public const byte Float = Single;
        public const byte Single = 13;
        public const byte Double = 14;
        public const byte Decimal = 15;

        public const byte DateTime = 16;
        public const byte String = 18;

        // Utility to pack booleans into the type
        public const byte Boolean_False = 19;
        public const byte Boolean_True = 20;

        /// <summary>
        /// A 16 bit floating point number
        /// </summary>
        public const byte Half = 21;
        public const byte Float16 = Half;

        #endregion

        #region Unity

        // Unity
        public const byte Vector2 = 24;
        public const byte Vector3 = 25;
        public const byte Vector4 = 26;
        public const byte Quaternion = 27; // basically vector4 but some minor changes
        public const byte Pose = 28;
        public const byte Color = 29;
        
        // Optimized formats
        public const byte Rect = 30;
        public const byte Bounds = 31;
        public const byte AnimationCurve = 32;
        public const byte Gradient = 33;

        #endregion

        #region Masks

        /// <summary>
        /// 1000 0000. Most left bit represent it's using default value.
        /// </summary>
        public const byte Default_Value_Mask = 128;
        /// <summary>
        /// 0100 0000. Second most left bit represent it's an array variant.
        /// </summary>
        public const byte Array_Mask = 64;

        /// <summary>
        /// 0011 1111. Remaining mask for splitting out types.
        /// </summary>
        public const byte Type_Mask = 63;

        #endregion

        #region Type Code Conversion

        public static TypeCode GetTypeCode(byte typeId) {
            switch (typeId) {
                case Boolean: return TypeCode.Boolean;
                case Boolean_True: return TypeCode.Boolean;
                case Boolean_False: return TypeCode.Boolean;
                case Char: return TypeCode.Char;
                case SByte: return TypeCode.SByte;
                case Byte: return TypeCode.Byte;
                case Int16: return TypeCode.Int16;
                case UInt16: return TypeCode.UInt16;
                case Int32: return TypeCode.Int32;
                case UInt32: return TypeCode.UInt32;
                case Int64: return TypeCode.Int64;
                case UInt64: return TypeCode.UInt64;
                case Single: return TypeCode.Single;
                case Double: return TypeCode.Double;
                case Decimal: return TypeCode.Decimal;
                case DateTime: return TypeCode.DateTime;
                case String: return TypeCode.String;
                case None: return TypeCode.Empty;
            }
            Debug.LogWarning(TAG + $"No type code exists for '{typeId}'");
            return TypeCode.Empty;
        }

        #endregion
    }
}
