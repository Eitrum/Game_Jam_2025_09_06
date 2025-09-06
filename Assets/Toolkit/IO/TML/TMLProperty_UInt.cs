using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_UInt : ITMLProperty {
        uint UInt { get; }
    }

    public interface ITMLProperty_UInt_Array : ITMLProperty {
        IReadOnlyList<uint> UInts { get; }
    }

    public sealed class TMLProperty_UInt : TMLProperty_Base<uint>, ITMLProperty_UInt {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_UInt]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.UInt;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > 0;
        public char Char => (char)value;

        public sbyte SByte => (sbyte)value;
        public byte Byte => (byte)value;

        public short Short => (short)value;
        public ushort UShort => (ushort)value;

        public int Int => (int)value;
        public uint UInt => value;

        public long Long => (long)value;
        public ulong ULong => (ulong)value;

        public float Float => value;
        public double Double => value;
        public decimal Decimal => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_UInt() { }

        public TMLProperty_UInt(string name) : base(name) { }

        public TMLProperty_UInt(string name, int value) : base(name, (uint)value) { }

        public TMLProperty_UInt(string name, uint value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_UInt_Array : TMLProperty_Base_Array<uint>, ITMLProperty_UInt_Array, ITMLProperty_String, ITMLProperty_UInt {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_UInt_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.UInt | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public uint UInt => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<uint> UInts => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_UInt_Array() : base() { }

        public TMLProperty_UInt_Array(string name) : base(name) { }

        public TMLProperty_UInt_Array(string name, uint value) : base(name, value) { }

        public TMLProperty_UInt_Array(string name, IReadOnlyList<uint> values) : base(name, values) { }

        public TMLProperty_UInt_Array(string name, IEnumerable<uint> values) : base(name, values) { }

        public TMLProperty_UInt_Array(string name, List<uint> values) : base(name, values) { }

        public TMLProperty_UInt_Array(string name, List<uint> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_UInt_Array(string name, IReadOnlyList<uint> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
