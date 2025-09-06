using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Decimal : ITMLProperty {
        decimal Decimal { get; }
    }

    public interface ITMLProperty_Decimal_Array : ITMLProperty {
        IReadOnlyList<decimal> Decimals { get; }
    }

    public sealed class TMLProperty_Decimal : TMLProperty_Base<decimal>, ITMLProperty_Decimal {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Decimal]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Decimal;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > decimal.Zero;
        public char Char => (char)value;

        public sbyte SByte => (sbyte)value;
        public byte Byte => (byte)value;

        public short Short => (short)value;
        public ushort UShort => (ushort)value;

        public int Int => (int)value;
        public uint UInt => (uint)value;

        public long Long => (long)value;
        public ulong ULong => (ulong)value;

        public float Float => (float)value;
        public double Double => (double)value;
        public decimal Decimal => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Decimal() { }

        public TMLProperty_Decimal(string name) : base(name) { }
        
        public TMLProperty_Decimal(string name, int value) : base(name, (decimal)value) { }

        public TMLProperty_Decimal(string name, float value) : base(name, (decimal)value) { }
        
        public TMLProperty_Decimal(string name, double value) : base(name, (decimal)value) { }

        public TMLProperty_Decimal(string name, decimal value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Decimal_Array : TMLProperty_Base_Array<decimal>, ITMLProperty_Decimal_Array, ITMLProperty_String, ITMLProperty_Decimal {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Decimal_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Decimal | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public decimal Decimal => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<decimal> Decimals => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Decimal_Array() : base() { }

        public TMLProperty_Decimal_Array(string name) : base(name) { }

        public TMLProperty_Decimal_Array(string name, decimal value) : base(name, value) { }

        public TMLProperty_Decimal_Array(string name, IReadOnlyList<decimal> values) : base(name, values) { }

        public TMLProperty_Decimal_Array(string name, IEnumerable<decimal> values) : base(name, values) { }

        public TMLProperty_Decimal_Array(string name, List<decimal> values) : base(name, values) { }

        public TMLProperty_Decimal_Array(string name, List<decimal> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Decimal_Array(string name, IReadOnlyList<decimal> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
