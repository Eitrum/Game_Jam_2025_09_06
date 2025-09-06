using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Color : ITMLProperty {
        Color Color { get; }
    }

    public interface ITMLProperty_Color_Array : ITMLProperty {
        IReadOnlyList<Color> Colors { get; }
    }

    public sealed class TMLProperty_Color : TMLProperty_Base<Color>, ITMLProperty_Int, ITMLProperty_Color {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Color]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Color;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public sbyte SByte => (sbyte)ColorToByte(value);
        public byte Byte => (byte)ColorToByte(value);

        public short Short => (short)ColorToShort(value);
        public ushort UShort => (ushort)ColorToShort(value);

        public int Int => ColorToInt(value);
        public uint UInt => (uint)ColorToInt(value);

        public long Long => (long)ColorToInt(value);
        public ulong ULong => (ulong)ColorToInt(value);

        public Color Color => value;
        public Color32 Color32 => value;

        public string String => $"[{Color32.r},{Color32.g},{Color32.b},{Color32.b}]";

        #endregion

        #region Constructor

        public TMLProperty_Color() { }

        public TMLProperty_Color(string name) : base(name) { }

        public TMLProperty_Color(string name, int value) : base(name, IntToColor(value)) { }

        public TMLProperty_Color(string name, Color value) : base(name, value) { }

        #endregion

        #region Utility

        public static Color IntToColor(int value)
            => IntToColor32(value);

        public static Color32 IntToColor32(int value) {
            const int mask = 0xff;
            return new Color32(
                (byte)((value >> 24) & mask),
                (byte)((value >> 16) & mask),
                (byte)((value >> 8) & mask),
                (byte)(value & mask)
                );
        }

        public static int ColorToInt(Color color)
            => ColorToInt((Color32)color);

        public static int ColorToInt(Color32 color) {
            return
                color.r << 24 |
                color.g << 16 |
                color.b << 8 |
                color.a;
        }

        public static short ColorToShort(Color color)
            => ColorToShort((Color32)color);

        public static short ColorToShort(Color32 color) {
            return (short)
                ((color.r / 32) << 12 |
                (color.g / 32) << 8 |
                (color.b / 32) << 4 |
                (color.a / 32));
        }

        public static byte ColorToByte(Color color)
            => ColorToByte((Color32)color);

        public static byte ColorToByte(Color32 color) {
            var c = (color.r + color.g + color.b) / 3;
            return (byte)c;
        }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        #endregion
    }

    public sealed class TMLProperty_Color_Array : TMLProperty_Base_Array<Color>, ITMLProperty_Color, ITMLProperty_Color_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Color_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Color | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Color Color => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Color> Colors => value;

        public string String => $"[{string.Join('|', value.Select(Stringify))}]";

        #endregion

        #region Constructor

        public TMLProperty_Color_Array() : base() { }

        public TMLProperty_Color_Array(string name) : base(name) { }

        public TMLProperty_Color_Array(string name, Color value) : base(name, value) { }

        public TMLProperty_Color_Array(string name, IReadOnlyList<Color> values) : base(name, values) { }

        public TMLProperty_Color_Array(string name, IEnumerable<Color> values) : base(name, values) { }

        public TMLProperty_Color_Array(string name, List<Color> values) : base(name, values) { }

        public TMLProperty_Color_Array(string name, List<Color> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Color_Array(string name, IReadOnlyList<Color> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        private static string Stringify(Color color) {
            var c32 = (Color32)color;
            return $"{c32.r},{c32.g},{c32.b},{c32.a}";
        }

        #endregion
    }
}
