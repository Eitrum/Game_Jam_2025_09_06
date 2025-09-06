using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Byte : ITMLProperty {
        byte Byte { get; }
    }

    public interface ITMLProperty_Byte_Array : ITMLProperty {
        IReadOnlyList<byte> Bytes { get; }
    }

    public sealed class TMLProperty_Byte : TMLProperty_Base<byte>, ITMLProperty_Int, ITMLProperty_Byte {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Byte]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Byte;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > 0;
        public char Char => (char)value;

        public sbyte SByte => (sbyte)value;
        public byte Byte => value;

        public short Short => (short)value;
        public ushort UShort => (ushort)value;

        public int Int => value;
        public uint UInt => (uint)value;

        public long Long => (long)value;
        public ulong ULong => (ulong)value;

        public float Float => value;
        public double Double => value;
        public decimal Decimal => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Byte() { }

        public TMLProperty_Byte(string name) : base(name) { }

        public TMLProperty_Byte(string name, int value) : base(name, (byte)value) { }

        public TMLProperty_Byte(string name, byte value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Byte_Array : TMLProperty_Base_Array<byte>, ITMLProperty_Byte, ITMLProperty_Byte_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Byte_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Byte | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public byte Byte => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<byte> Bytes => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Byte_Array() : base() { }

        public TMLProperty_Byte_Array(string name) : base(name) { }

        public TMLProperty_Byte_Array(string name, byte value) : base(name, value) { }

        public TMLProperty_Byte_Array(string name, IReadOnlyList<byte> values) : base(name, values) { }

        public TMLProperty_Byte_Array(string name, IEnumerable<byte> values) : base(name, values) { }

        public TMLProperty_Byte_Array(string name, List<byte> values) : base(name, values) { }

        public TMLProperty_Byte_Array(string name, List<byte> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Byte_Array(string name, IReadOnlyList<byte> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
