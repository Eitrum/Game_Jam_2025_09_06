using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Short : ITMLProperty {
        short Short { get; }
    }

    public interface ITMLProperty_Short_Array : ITMLProperty {
        IReadOnlyList<short> Shorts { get; }
    }

    public sealed class TMLProperty_Short : TMLProperty_Base<short>, ITMLProperty_Int, ITMLProperty_Byte, ITMLProperty_Short {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Short]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Short;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > 0;
        public char Char => (char)value;

        public sbyte SByte => (sbyte)value;
        public byte Byte => (byte)value;

        public short Short => value;
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

        public TMLProperty_Short() { }

        public TMLProperty_Short(string name) : base(name) { }

        public TMLProperty_Short(string name, int value) : base(name, (byte)value) { }
        
        public TMLProperty_Short(string name, byte value) : base(name, value) { }
        
        public TMLProperty_Short(string name, short value) : base(name, value) { }

        public TMLProperty_Short(string name, ushort value) : base(name, (short)value) { }

        #endregion
    }

    public sealed class TMLProperty_Short_Array : TMLProperty_Base_Array<short>, ITMLProperty_Short, ITMLProperty_Short_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Short_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Short | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public short Short => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<short> Shorts => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Short_Array() : base() { }

        public TMLProperty_Short_Array(string name) : base(name) { }

        public TMLProperty_Short_Array(string name, short value) : base(name, value) { }

        public TMLProperty_Short_Array(string name, IReadOnlyList<short> values) : base(name, values) { }

        public TMLProperty_Short_Array(string name, IEnumerable<short> values) : base(name, values) { }

        public TMLProperty_Short_Array(string name, List<short> values) : base(name, values) { }

        public TMLProperty_Short_Array(string name, List<short> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Short_Array(string name, IReadOnlyList<short> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
