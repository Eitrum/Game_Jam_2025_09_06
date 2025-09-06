using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_SByte : ITMLProperty {
        sbyte SByte { get; }
    }

    public interface ITMLProperty_SByte_Array : ITMLProperty {
        IReadOnlyList<sbyte> SBytes { get; }
    }

    public sealed class TMLProperty_SByte : TMLProperty_Base<sbyte>, ITMLProperty_Int, ITMLProperty_Byte, ITMLProperty_SByte {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_SByte]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.SByte;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > 0;
        public char Char => (char)value;

        public sbyte SByte => value;
        public byte Byte => (byte)value;

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

        public TMLProperty_SByte() { }

        public TMLProperty_SByte(string name) : base(name) { }

        public TMLProperty_SByte(string name, int value) : base(name, (sbyte)value) { }

        public TMLProperty_SByte(string name, byte value) : base(name, (sbyte)value) { }

        public TMLProperty_SByte(string name, sbyte value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_SByte_Array : TMLProperty_Base_Array<sbyte>, ITMLProperty_SByte, ITMLProperty_SByte_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_SByte_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.SByte | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public sbyte SByte => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<sbyte> SBytes => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_SByte_Array() : base() { }

        public TMLProperty_SByte_Array(string name) : base(name) { }

        public TMLProperty_SByte_Array(string name, sbyte value) : base(name, value) { }

        public TMLProperty_SByte_Array(string name, IReadOnlyList<sbyte> values) : base(name, values) { }

        public TMLProperty_SByte_Array(string name, IEnumerable<sbyte> values) : base(name, values) { }

        public TMLProperty_SByte_Array(string name, List<sbyte> values) : base(name, values) { }

        public TMLProperty_SByte_Array(string name, List<sbyte> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_SByte_Array(string name, IReadOnlyList<sbyte> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
