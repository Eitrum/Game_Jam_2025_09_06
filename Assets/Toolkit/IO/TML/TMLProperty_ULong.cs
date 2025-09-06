using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_ULong :ITMLProperty {
        ulong ULong { get; }
    }
    
    public interface ITMLProperty_ULong_Array : ITMLProperty {
        IReadOnlyList<ulong> ULongs { get; }
    }

    public sealed class TMLProperty_ULong : TMLProperty_Base<ulong>, ITMLProperty_Int, ITMLProperty_Byte, ITMLProperty_Short, ITMLProperty_UShort, ITMLProperty_ULong {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_ULong]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.ULong;

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
        public uint UInt => (uint)value;

        public long Long => (long)value;
        public ulong ULong => value;

        public float Float => value;
        public double Double => value;
        public decimal Decimal => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_ULong() { }

        public TMLProperty_ULong(string name) : base(name) { }

        public TMLProperty_ULong(string name, int value) : base(name, (byte)value) { }
        
        public TMLProperty_ULong(string name, byte value) : base(name, value) { }
        
        public TMLProperty_ULong(string name, short value) : base(name, (ushort)value) { }
        
        public TMLProperty_ULong(string name, ushort value) : base(name, value) { }

        public TMLProperty_ULong(string name, ulong value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_ULong_Array : TMLProperty_Base_Array<ulong>, ITMLProperty_ULong, ITMLProperty_ULong_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_ULong_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.ULong | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public ulong ULong => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<ulong> ULongs => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_ULong_Array() : base() { }

        public TMLProperty_ULong_Array(string name) : base(name) { }

        public TMLProperty_ULong_Array(string name, ulong value) : base(name, value) { }

        public TMLProperty_ULong_Array(string name, IReadOnlyList<ulong> values) : base(name, values) { }

        public TMLProperty_ULong_Array(string name, IEnumerable<ulong> values) : base(name, values) { }

        public TMLProperty_ULong_Array(string name, List<ulong> values) : base(name, values) { }

        public TMLProperty_ULong_Array(string name, List<ulong> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_ULong_Array(string name, IReadOnlyList<ulong> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
