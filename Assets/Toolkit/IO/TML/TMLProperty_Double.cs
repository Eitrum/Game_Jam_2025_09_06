using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Double : ITMLProperty {
        double Double { get; }
    }

    public interface ITMLProperty_Double_Array : ITMLProperty {
        IReadOnlyList<double> Doubles { get; }
    }

    public sealed class TMLProperty_Double : TMLProperty_Base<double>, ITMLProperty_Double {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Double]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Double;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value > double.Epsilon;
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
        public double Double => value;
        public decimal Decimal => (decimal)value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Double() { }

        public TMLProperty_Double(string name) : base(name) { }
        
        public TMLProperty_Double(string name, int value) : base(name, (double)value) { }

        public TMLProperty_Double(string name, float value) : base(name, (double)value) { }

        public TMLProperty_Double(string name, double value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Double_Array : TMLProperty_Base_Array<double>, ITMLProperty_Double_Array, ITMLProperty_String, ITMLProperty_Double {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Double_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Double | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public double Double => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<double> Doubles => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Double_Array() : base() { }

        public TMLProperty_Double_Array(string name) : base(name) { }

        public TMLProperty_Double_Array(string name, double value) : base(name, value) { }

        public TMLProperty_Double_Array(string name, IReadOnlyList<double> values) : base(name, values) { }

        public TMLProperty_Double_Array(string name, IEnumerable<double> values) : base(name, values) { }

        public TMLProperty_Double_Array(string name, List<double> values) : base(name, values) { }

        public TMLProperty_Double_Array(string name, List<double> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Double_Array(string name, IReadOnlyList<double> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
