using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_UShort : ITMLProperty {
        ushort UShort { get; }
    }

    public interface ITMLProperty_UShort_Array : ITMLProperty {
        IReadOnlyList<ushort> UShorts { get; }
    }

    public sealed class TMLProperty_UShort : TMLProperty_Base<ushort>, ITMLProperty_Int, ITMLProperty_Byte, ITMLProperty_Short, ITMLProperty_UShort {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_UShort]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.UShort;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > 0;
        public char Char => (char)value;

        public sbyte SByte => (sbyte)value;
        public byte Byte => (byte)value;

        public short Short => (short)value;
        public ushort UShort => value;

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

        public TMLProperty_UShort() { }

        public TMLProperty_UShort(string name) : base(name) { }

        public TMLProperty_UShort(string name, int value) : base(name, (byte)value) { }
        
        public TMLProperty_UShort(string name, byte value) : base(name, value) { }
        
        public TMLProperty_UShort(string name, short value) : base(name, (ushort)value) { }

        public TMLProperty_UShort(string name, ushort value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_UShort_Array : TMLProperty_Base_Array<ushort>, ITMLProperty_UShort, ITMLProperty_UShort_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_UShort_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.UShort | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public ushort UShort => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<ushort> UShorts => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_UShort_Array() : base() { }

        public TMLProperty_UShort_Array(string name) : base(name) { }

        public TMLProperty_UShort_Array(string name, ushort value) : base(name, value) { }

        public TMLProperty_UShort_Array(string name, IReadOnlyList<ushort> values) : base(name, values) { }

        public TMLProperty_UShort_Array(string name, IEnumerable<ushort> values) : base(name, values) { }

        public TMLProperty_UShort_Array(string name, List<ushort> values) : base(name, values) { }

        public TMLProperty_UShort_Array(string name, List<ushort> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_UShort_Array(string name, IReadOnlyList<ushort> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
