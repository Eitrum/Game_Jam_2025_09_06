using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_TKDateTime : ITMLProperty {
        TKDateTime DateTime { get; }
    }

    public interface ITMLProperty_TKDateTime_Array : ITMLProperty {
        IReadOnlyList<TKDateTime> DateTimes { get; }
    }

    public interface ITMLProperty_DateTime : ITMLProperty {
        DateTime DateTime { get; }
    }

    public interface ITMLProperty_DateTime_Array : ITMLProperty {
        IEnumerable<DateTime> DateTimes { get; }
    }

    public sealed class TMLProperty_DateTime : TMLProperty_Base<TKDateTime>, ITMLProperty_TKDateTime, ITMLProperty_DateTime {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_DateTime]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.DateTime;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public long Long => (long)value.Ticks;
        public ulong ULong => (ulong)value.Ticks;

        public float Float => (float)value.Ticks;
        public double Double => (double)value.Ticks;
        public decimal Decimal => (decimal)value.Ticks;

        public TKDateTime DateTime => value;
        DateTime ITMLProperty_DateTime.DateTime => (DateTime)value;

        public string String => $"{value.Ticks}";

        #endregion

        #region Constructor

        public TMLProperty_DateTime() { }

        public TMLProperty_DateTime(string name) : base(name) { }

        public TMLProperty_DateTime(string name, int value) : base(name, new TKDateTime((long)value)) { }

        public TMLProperty_DateTime(string name, float value) : base(name, new TKDateTime((long)value)) { }

        public TMLProperty_DateTime(string name, double value) : base(name, new TKDateTime((long)value)) { }

        public TMLProperty_DateTime(string name, long value) : base(name, new TKDateTime(value)) { }

        public TMLProperty_DateTime(string name, TKDateTime value) : base(name, value) { }

        #endregion

        #region Overrides

        public override string ToString() {
            return String;
        }

        public override string GetFormattedXml() {
            return $"{name}=\"{value.Ticks}\"";
        }

        #endregion
    }

    public sealed class TMLProperty_DateTime_Array : TMLProperty_Base_Array<TKDateTime>, ITMLProperty_TKDateTime_Array, ITMLProperty_DateTime_Array, ITMLProperty_String, ITMLProperty_TKDateTime, ITMLProperty_DateTime {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_DateTime_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.DateTime | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public TKDateTime DateTime => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<TKDateTime> DateTimes => value;

        DateTime ITMLProperty_DateTime.DateTime => (DateTime)DateTime;
        IEnumerable<DateTime> ITMLProperty_DateTime_Array.DateTimes => value.Select(x => (DateTime)x);

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_DateTime_Array() : base() { }

        public TMLProperty_DateTime_Array(string name) : base(name) { }

        public TMLProperty_DateTime_Array(string name, TKDateTime value) : base(name, value) { }

        public TMLProperty_DateTime_Array(string name, IReadOnlyList<TKDateTime> values) : base(name, values) { }

        public TMLProperty_DateTime_Array(string name, IEnumerable<TKDateTime> values) : base(name, values) { }

        public TMLProperty_DateTime_Array(string name, List<TKDateTime> values) : base(name, values) { }

        public TMLProperty_DateTime_Array(string name, List<TKDateTime> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_DateTime_Array(string name, IReadOnlyList<TKDateTime> values, bool createCopy) : base(name, values, createCopy) { }



        public TMLProperty_DateTime_Array(string name, DateTime value) : base(name, value) { }

        public TMLProperty_DateTime_Array(string name, IReadOnlyList<DateTime> values) : base(name, values.Select(x => (TKDateTime)x)) { }

        public TMLProperty_DateTime_Array(string name, IEnumerable<DateTime> values) : base(name, values.Select(x => (TKDateTime)x)) { }

        public TMLProperty_DateTime_Array(string name, List<DateTime> values) : base(name, values.Select(x => (TKDateTime)x)) { }

        #endregion
    }
}
