using System;
using System.Globalization;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct TKDateTime : IComparable, IComparable<TKDateTime>, IEquatable<TKDateTime>, IFormattable
    {
        #region Consts

        public const long TicksPerMillisecond = 10000;
        public const long TicksPerSecond = 10000000;
        public const long TicksPerMinute = 600000000;
        public const long TicksPerHour = 36000000000;
        public const long TicksPerDay = 864000000000;

        #endregion

        #region Variables

        public static readonly TKDateTime MaxValue = new TKDateTime(System.DateTime.MaxValue.Ticks);
        public static readonly TKDateTime MinValue = new TKDateTime(System.DateTime.MinValue.Ticks);

        [SerializeField] private DateTimeKind kind;
        [SerializeField] private long ticks;

        #endregion

        #region Properties

        public static TKDateTime Now => System.DateTime.Now;
        public static TKDateTime Today => System.DateTime.Today;
        public static TKDateTime UtcNow => System.DateTime.UtcNow;
        public long Ticks => ticks;
        public int Millisecond => (int)((ticks / TicksPerMillisecond) % 1000);
        public int Second => (int)((ticks / TicksPerSecond) % 60);
        public int Minute => (int)((ticks / TicksPerMinute) % 60);
        public int Hour => (int)((ticks / TicksPerHour) % 24);
        public int Day => new System.DateTime(ticks).Day;
        public int Month => new System.DateTime(ticks).Month;
        public int Year => new System.DateTime(ticks).Year;

        public int DayOfYear => new System.DateTime(ticks).Year;
        public TKDateTime Date => new System.DateTime(ticks).Date;
        public DateTimeKind Kind => new System.DateTime(ticks).Kind;
        public DayOfWeek DayOfWeek => new System.DateTime(ticks).DayOfWeek;
        public TKTimeSpan TimeOfDay => new System.DateTime(ticks).TimeOfDay;

        #endregion

        #region Constructor

        private TKDateTime(double ticks) {
            this.ticks = (long)ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        private TKDateTime(double ticks, DateTimeKind kind) {
            this.ticks = (long)ticks;
            this.kind = kind;
        }
        public TKDateTime(long ticks) {
            this.ticks = ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        public TKDateTime(long ticks, DateTimeKind kind) {
            this.ticks = ticks;
            this.kind = kind;
        }
        public TKDateTime(int year, int month, int day) {
            this.ticks = new System.DateTime(year, month, day).Ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        public TKDateTime(int year, int month, int day, Calendar calendar) {
            var dt = new System.DateTime(year, month, day, calendar);
            this.ticks = dt.Ticks;
            this.kind = dt.Kind;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second) {
            var dt = new System.DateTime(year, month, day, hour, minute, second);
            this.ticks = dt.Ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind) {
            var dt = new System.DateTime(year, month, day, hour, minute, second, kind);
            this.ticks = dt.Ticks;
            this.kind = kind;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar) {
            var dt = new System.DateTime(year, month, day, hour, minute, second);
            this.ticks = dt.Ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond) {
            var dt = new System.DateTime(year, month, day, hour, minute, second, millisecond);
            this.ticks = dt.Ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind) {
            var dt = new System.DateTime(year, month, day, hour, minute, second, millisecond, kind);
            this.ticks = dt.Ticks;
            this.kind = kind;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar) {
            var dt = new System.DateTime(year, month, day, hour, minute, second, millisecond, calendar);
            this.ticks = dt.Ticks;
            this.kind = DateTimeKind.Unspecified;
        }
        public TKDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, DateTimeKind kind) {
            var dt = new System.DateTime(year, month, day, hour, minute, second, millisecond, calendar, kind);
            this.ticks = dt.Ticks;
            this.kind = kind;
        }

        #endregion

        #region Add

        public TKDateTime Add(TKTimeSpan value) => new TKDateTime(ticks + value.Ticks, kind);

        public TKDateTime AddTicks(int value) => new System.DateTime(ticks, kind).AddTicks(value);
        public TKDateTime AddTicks(long value) => new System.DateTime(ticks, kind).AddTicks(value);
        public TKDateTime AddTicks(double value) => new System.DateTime(ticks, kind).AddTicks((long)value);

        public TKDateTime AddMilliseconds(double value) => new TKDateTime(ticks + value * TicksPerMillisecond, kind);
        public TKDateTime AddMilliseconds(float value) => new TKDateTime(ticks + value * TicksPerMillisecond, kind);
        public TKDateTime AddMilliseconds(int value) => new TKDateTime(ticks + value * TicksPerMillisecond, kind);
        public TKDateTime AddMilliseconds(long value) => new TKDateTime(ticks + value * TicksPerMillisecond, kind);

        public TKDateTime AddSeconds(double value) => new TKDateTime(ticks + value * TicksPerSecond, kind);
        public TKDateTime AddSeconds(float value) => new TKDateTime(ticks + value * TicksPerSecond, kind);
        public TKDateTime AddSeconds(int value) => new TKDateTime(ticks + value * TicksPerSecond, kind);
        public TKDateTime AddSeconds(long value) => new TKDateTime(ticks + value * TicksPerSecond, kind);

        public TKDateTime AddMinutes(double value) => new TKDateTime(ticks + value * TicksPerMinute, kind);
        public TKDateTime AddMinutes(float value) => new TKDateTime(ticks + value * TicksPerMinute, kind);
        public TKDateTime AddMinutes(int value) => new TKDateTime(ticks + value * TicksPerMinute, kind);
        public TKDateTime AddMinutes(long value) => new TKDateTime(ticks + value * TicksPerMinute, kind);

        public TKDateTime AddHours(double value) => new TKDateTime(ticks + value * TicksPerHour, kind);
        public TKDateTime AddHours(float value) => new TKDateTime(ticks + value * TicksPerHour, kind);
        public TKDateTime AddHours(int value) => new TKDateTime(ticks + value * TicksPerHour, kind);
        public TKDateTime AddHours(long value) => new TKDateTime(ticks + value * TicksPerHour, kind);

        public TKDateTime AddDays(double value) => new TKDateTime(ticks + value * TicksPerDay, kind);
        public TKDateTime AddDays(float value) => new TKDateTime(ticks + value * TicksPerDay, kind);
        public TKDateTime AddDays(int value) => new TKDateTime(ticks + value * TicksPerDay, kind);
        public TKDateTime AddDays(long value) => new TKDateTime(ticks + value * TicksPerDay, kind);

        public TKDateTime AddMonths(double value) => AddMonths((int)value);
        public TKDateTime AddMonths(float value) => AddMonths((int)value);
        public TKDateTime AddMonths(int value) => new System.DateTime(ticks, kind).AddMonths(value);
        public TKDateTime AddMonths(long value) => AddMonths((int)value);

        public TKDateTime AddYears(double value) => AddYears((int)value);
        public TKDateTime AddYears(float value) => AddYears((int)value);
        public TKDateTime AddYears(int value) => new System.DateTime(ticks, kind).AddYears(value);
        public TKDateTime AddYears(long value) => AddYears((int)value);

        #endregion

        #region Subtract

        public TKTimeSpan Subtract(TKDateTime value) => this - value;
        public TKDateTime Subtract(TKTimeSpan value) => this - value;

        #endregion

        #region Parse

        public static TKDateTime Parse(string s)
            => System.DateTime.Parse(s);
        public static TKDateTime Parse(string s, IFormatProvider provider)
            => System.DateTime.Parse(s, provider);
        public static TKDateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
            => System.DateTime.Parse(s, provider, styles);

        public static TKDateTime ParseExact(string s, string format, IFormatProvider provider)
            => System.DateTime.ParseExact(s, format, provider);
        public static TKDateTime ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
            => System.DateTime.ParseExact(s, format, provider, style);
        public static TKDateTime ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
            => System.DateTime.ParseExact(s, formats, provider, style);

        public static bool TryParse(string s, out TKDateTime result) {
            if(System.DateTime.TryParse(s, out System.DateTime dt)) {
                result = dt;
                return true;
            }
            else {
                result = default;
                return false;
            }
        }
        public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out TKDateTime result) {
            if(System.DateTime.TryParse(s, provider, styles, out System.DateTime dt)) {
                result = dt;
                return true;
            }
            else {
                result = default;
                return false;
            }
        }

        public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out TKDateTime result) {
            if(System.DateTime.TryParseExact(s, format, provider, style, out System.DateTime dt)) {
                result = dt;
                return true;
            }
            else {
                result = default;
                return false;
            }
        }
        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out TKDateTime result) {
            if(System.DateTime.TryParseExact(s, formats, provider, style, out System.DateTime dt)) {
                result = dt;
                return true;
            }
            else {
                result = default;
                return false;
            }
        }

        public static TKDateTime FromBinary(long dateData) => System.DateTime.FromBinary(dateData);
        public static TKDateTime FromFileTime(long fileTime) => System.DateTime.FromFileTime(fileTime);
        public static TKDateTime FromFileTimeUtc(long fileTime) => System.DateTime.FromFileTimeUtc(fileTime);
        public static TKDateTime FromOADate(double d) => System.DateTime.FromOADate(d);

        #endregion

        #region Utility

        public static bool IsLeapYear(int year) => System.DateTime.IsLeapYear(year);
        public static int DaysInMonth(int year, int month) => System.DateTime.DaysInMonth(year, month);
        public bool IsDaylightSavingTime() => new System.DateTime(ticks, kind).IsDaylightSavingTime();
        public TypeCode GetTypeCode() => TypeCode.DateTime;
        public override int GetHashCode() => new System.DateTime(ticks, kind).GetHashCode();

        #endregion

        #region Comparison

        public static int Compare(TKDateTime t1, TKDateTime t2) => t1.ticks.CompareTo(t2.ticks);
        public int CompareTo(TKDateTime value) => ticks.CompareTo(value.ticks);
        public int CompareTo(object value) => (value is TKDateTime dt) ? ticks.CompareTo(dt.ticks) : 0;
        public static bool Equals(TKDateTime t1, TKDateTime t2) => t1.ticks == t2.ticks;
        public override bool Equals(object value) => (value is TKDateTime dt) ? ticks.Equals(dt.ticks) : false;
        public bool Equals(TKDateTime value) => ticks.Equals(value.ticks);

        #endregion

        #region Conversion

        public long ToBinary() => new System.DateTime(ticks, kind).ToBinary();
        public long ToFileTime() => new System.DateTime(ticks, kind).ToFileTime();
        public long ToFileTimeUtc() => new System.DateTime(ticks, kind).ToFileTimeUtc();
        public TKDateTime ToUniversalTime() => new System.DateTime(ticks, kind).ToUniversalTime();
        public TKDateTime ToLocalTime() => new System.DateTime(ticks, kind).ToLocalTime();
        public double ToOADate() => new System.DateTime(ticks, kind).ToOADate();

        #endregion

        #region ToString

        public string ToLongDateString() => new System.DateTime(ticks, kind).ToLongDateString();
        public string ToLongTimeString() => new System.DateTime(ticks, kind).ToLongTimeString();
        public string ToShortDateString() => new System.DateTime(ticks, kind).ToShortDateString();
        public string ToShortTimeString() => new System.DateTime(ticks, kind).ToShortTimeString();
        public string ToString(string format) => new System.DateTime(ticks, kind).ToString(format);
        public string ToString(IFormatProvider provider) => new System.DateTime(ticks, kind).ToString(provider);
        public override string ToString() => new System.DateTime(ticks, kind).ToString();
        public string ToString(string format, IFormatProvider provider) => new System.DateTime(ticks, kind).ToString(format, provider);

        public string[] GetDateTimeFormats() => new System.DateTime(ticks, kind).GetDateTimeFormats();
        public string[] GetDateTimeFormats(char format) => new System.DateTime(ticks, kind).GetDateTimeFormats(format);
        public string[] GetDateTimeFormats(IFormatProvider provider) => new System.DateTime(ticks, kind).GetDateTimeFormats(provider);
        public string[] GetDateTimeFormats(char format, IFormatProvider provider) => new System.DateTime(ticks, kind).GetDateTimeFormats(format, provider);

        #endregion

        #region Operators

        public static TKDateTime operator +(TKDateTime d, TKTimeSpan t) => new TKDateTime(d.ticks + t.Ticks, d.kind);
        public static TKTimeSpan operator -(TKDateTime d1, TKDateTime d2) => new TKTimeSpan(d1.ticks - d2.ticks);
        public static TKDateTime operator -(TKDateTime d, TKTimeSpan t) => new TKDateTime(d.ticks - t.Ticks, d.kind);
        public static bool operator ==(TKDateTime d1, TKDateTime d2) => d1.ticks == d2.ticks;
        public static bool operator !=(TKDateTime d1, TKDateTime d2) => d1.ticks != d2.ticks;
        public static bool operator <(TKDateTime t1, TKDateTime t2) => t1.ticks < t2.ticks;
        public static bool operator >(TKDateTime t1, TKDateTime t2) => t1.ticks > t2.ticks;
        public static bool operator <=(TKDateTime t1, TKDateTime t2) => t1.ticks <= t2.ticks;
        public static bool operator >=(TKDateTime t1, TKDateTime t2) => t1.ticks >= t2.ticks;

        public static implicit operator System.DateTime(TKDateTime ts) => new System.DateTime(ts.ticks, ts.kind);
        public static implicit operator TKDateTime(System.DateTime ts) => new TKDateTime(ts.Ticks, ts.Kind);

        #endregion
    }
}
