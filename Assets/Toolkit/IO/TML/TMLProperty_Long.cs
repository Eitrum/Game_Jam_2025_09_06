using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Long : ITMLProperty{
        long Long { get; }
    }
    
    public interface ITMLProperty_Long_Array : ITMLProperty{
        IReadOnlyList<long> Longs { get; }
    }

    public sealed class TMLProperty_Long : TMLProperty_Base<long>, ITMLProperty_Int, ITMLProperty_Byte, ITMLProperty_Short, ITMLProperty_UShort, ITMLProperty_ULong, ITMLProperty_Long {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Long]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Long;

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

        public long Long => value;
        public ulong ULong => (ulong)value;

        public float Float => value;
        public double Double => value;
        public decimal Decimal => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Long() { }

        public TMLProperty_Long(string name) : base(name) { }

        public TMLProperty_Long(string name, int value) : base(name, (byte)value) { }
        
        public TMLProperty_Long(string name, byte value) : base(name, value) { }
        
        public TMLProperty_Long(string name, short value) : base(name, (ushort)value) { }
        
        public TMLProperty_Long(string name, ushort value) : base(name, value) { }
        
        public TMLProperty_Long(string name, ulong value) : base(name, (long)value) { }

        public TMLProperty_Long(string name, long value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Long_Array : TMLProperty_Base_Array<long>, ITMLProperty_Long, ITMLProperty_Long_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Long_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Long | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public long Long => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<long> Longs => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Long_Array() : base() { }

        public TMLProperty_Long_Array(string name) : base(name) { }

        public TMLProperty_Long_Array(string name, long value) : base(name, value) { }

        public TMLProperty_Long_Array(string name, IReadOnlyList<long> values) : base(name, values) { }

        public TMLProperty_Long_Array(string name, IEnumerable<long> values) : base(name, values) { }

        public TMLProperty_Long_Array(string name, List<long> values) : base(name, values) { }

        public TMLProperty_Long_Array(string name, List<long> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Long_Array(string name, IReadOnlyList<long> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
