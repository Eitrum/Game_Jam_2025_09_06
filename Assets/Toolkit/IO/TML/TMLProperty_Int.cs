using System;
using System.Collections.Generic;
using System.Text;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Int : ITMLProperty {
        int Int { get; }
    }

    public interface ITMLProperty_Int_Array : ITMLProperty {
        IReadOnlyList<int> Ints { get; }
    }

    public sealed class TMLProperty_Int : TMLProperty_Base<int>, ITMLProperty_Int, ITMLProperty_Byte, ITMLProperty_UInt, ITMLProperty_SByte, ITMLProperty_Short, ITMLProperty_UShort, ITMLProperty_Char, ITMLProperty_Boolean {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Int]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Int;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > 0;
        public char Char => (char)value;

        public sbyte SByte => (sbyte)value;
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

        public TMLProperty_Int() { }

        public TMLProperty_Int(string name) : base(name) { }

        public TMLProperty_Int(string name, int value) : base(name, value) { }

        public TMLProperty_Int(string name, byte value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Int_Array : TMLProperty_Base_Array<int>, ITMLProperty_Int, ITMLProperty_Int_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Int_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Int | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public int Int => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<int> Ints => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Int_Array() : base() { }

        public TMLProperty_Int_Array(string name) : base(name) { }

        public TMLProperty_Int_Array(string name, int value) : base(name, value) { }

        public TMLProperty_Int_Array(string name, IReadOnlyList<int> values) : base(name, values) { }

        public TMLProperty_Int_Array(string name, IEnumerable<int> values) : base(name, values) { }

        public TMLProperty_Int_Array(string name, List<int> values) : base(name, values) { }

        public TMLProperty_Int_Array(string name, List<int> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Int_Array(string name, IReadOnlyList<int> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
