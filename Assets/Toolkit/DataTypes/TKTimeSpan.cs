using System;
using System.Globalization;

namespace Toolkit
{
    [System.Serializable]
    public struct TKTimeSpan : IComparable, IComparable<TKTimeSpan>, IEquatable<TKTimeSpan>, IFormattable
    {
        #region Consts

        public const long TicksPerMillisecond = 10000;
        public const long TicksPerSecond = 10000000;
        public const long TicksPerMinute = 600000000;
        public const long TicksPerHour = 36000000000;
        public const long TicksPerDay = 864000000000;

        #endregion

        #region Variable

        public static readonly TKTimeSpan MaxValue = new TKTimeSpan(long.MaxValue);
        public static readonly TKTimeSpan MinValue = new TKTimeSpan(long.MinValue);
        public static readonly TKTimeSpan Zero = new TKTimeSpan(0);
        [UnityEngine.SerializeField] private long ticks;

        #endregion

        #region Properties

        public long Ticks {
            get => ticks;
            set => ticks = value;
        }

        public int Milliseconds => (int)((ticks / TicksPerMillisecond) % 1000);
        public int Seconds => (int)((ticks / TicksPerSecond) % 60);
        public int Minutes => (int)((ticks / TicksPerMinute) % 60);
        public int Hours => (int)((ticks / TicksPerHour) % 24);
        public int Days => (int)(ticks / TicksPerDay);

        public double TotalMilliseconds => (ticks / (double)TicksPerMillisecond);
        public double TotalSeconds => (ticks / (double)TicksPerSecond);
        public double TotalMinutes => (ticks / (double)TicksPerMinute);
        public double TotalHours => (ticks / (double)TicksPerHour);
        public double TotalDays => (ticks / (double)TicksPerDay);

        #endregion

        #region Constructors

        private TKTimeSpan(double ticks) : this((long)ticks) { }

        public TKTimeSpan(long ticks) {
            this.ticks = ticks;
        }

        public TKTimeSpan(int hours, int minutes, int seconds) {
            ticks = hours * TicksPerHour + minutes * TicksPerMinute + seconds * TicksPerSecond;
        }

        public TKTimeSpan(int days, int hours, int minutes, int seconds) {
            ticks = days * TicksPerDay + hours * TicksPerHour + minutes * TicksPerMinute + seconds * TicksPerSecond;
        }

        public TKTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) {
            ticks = days * TicksPerDay + hours * TicksPerHour + minutes * TicksPerMinute + seconds * TicksPerSecond + milliseconds * TicksPerMillisecond;
        }

        public TKTimeSpan(System.TimeSpan timeSpan) {
            ticks = timeSpan.Ticks;
        }

        #endregion

        #region Local Methods

        public void Extract(out int days, out int hours, out int minutes, out int seconds, out int milliseconds) {
            days = Days;
            hours = Hours;
            minutes = Minutes;
            seconds = Seconds;
            milliseconds = Milliseconds;
        }

        public void Extract(out int days, out int hours, out int minutes, out int seconds, out int milliseconds, out int remainingTicks) {
            days = Days;
            hours = Hours;
            minutes = Minutes;
            seconds = Seconds;
            milliseconds = Milliseconds;
            remainingTicks = (int)(ticks % TicksPerMillisecond);
        }

        public void Extract(out int days, out int hours, out int minutes, out int seconds, out int milliseconds, out long remainingTicks) {
            days = Days;
            hours = Hours;
            minutes = Minutes;
            seconds = Seconds;
            milliseconds = Milliseconds;
            remainingTicks = ticks % TicksPerMillisecond;
        }

        public void Set(int days, int hours, int minutes, int seconds, int milliseconds, int ticks) {
            this.ticks = days * TicksPerDay + hours * TicksPerHour + minutes * TicksPerMinute + seconds * TicksPerSecond + milliseconds * TicksPerMillisecond + ticks;
        }

        public void Set(int days, int hours, int minutes, int seconds, int milliseconds, long ticks) {
            this.ticks = days * TicksPerDay + hours * TicksPerHour + minutes * TicksPerMinute + seconds * TicksPerSecond + milliseconds * TicksPerMillisecond + ticks;
        }

        public TKTimeSpan Negate() => new TKTimeSpan(-ticks);
        public TKTimeSpan Subtract(TKTimeSpan ts) => new TKTimeSpan(ticks - ts.ticks);
        public TKTimeSpan Duration() => new TKTimeSpan(Math.Abs(ticks));

        public override int GetHashCode() => ticks.GetHashCode();

        #endregion

        #region To String

        public override string ToString() => new System.TimeSpan(ticks).ToString();
        public string ToString(string format) => new System.TimeSpan(ticks).ToString(format);
        public string ToString(string format, IFormatProvider formatProvider) => new System.TimeSpan(ticks).ToString(format, formatProvider);

        #endregion

        #region From Values

        public static TKTimeSpan FromTicks(long value) => new TKTimeSpan(value);
        public static TKTimeSpan FromMilliseconds(double value) => new TKTimeSpan(value * TicksPerMillisecond);
        public static TKTimeSpan FromSeconds(double value) => new TKTimeSpan(value * TicksPerSecond);
        public static TKTimeSpan FromMinutes(double value) => new TKTimeSpan(value * TicksPerMinute);
        public static TKTimeSpan FromHours(double value) => new TKTimeSpan(value * TicksPerHour);
        public static TKTimeSpan FromDays(double value) => new TKTimeSpan(value * TicksPerDay);

        public static TKTimeSpan FromTicks(double value) => new TKTimeSpan(value);
        public static TKTimeSpan FromMilliseconds(int value) => new TKTimeSpan(value * TicksPerMillisecond);
        public static TKTimeSpan FromSeconds(int value) => new TKTimeSpan(value * TicksPerSecond);
        public static TKTimeSpan FromMinutes(int value) => new TKTimeSpan(value * TicksPerMinute);
        public static TKTimeSpan FromHours(int value) => new TKTimeSpan(value * TicksPerHour);
        public static TKTimeSpan FromDays(int value) => new TKTimeSpan(value * TicksPerDay);

        #endregion

        #region Comparison

        public override bool Equals(object obj) => (obj is TKTimeSpan ts) ? ticks == ts.ticks : false;
        public bool Equals(TKTimeSpan other) => ticks == other.ticks;
        public static bool Equals(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks.Equals(t2.ticks);

        public static int Compare(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks.CompareTo(t2.ticks);
        public int CompareTo(object value) => (value is TKTimeSpan ts) ? ticks.CompareTo(ts.ticks) : 0;
        public int CompareTo(TKTimeSpan value) => ticks.CompareTo(value.ticks);

        #endregion

        #region Parse

        public static TKTimeSpan Parse(string input, IFormatProvider formatProvider)
            => new TKTimeSpan(System.TimeSpan.Parse(input, formatProvider));
        public static TKTimeSpan Parse(string s)
            => new TKTimeSpan(System.TimeSpan.Parse(s));

        public static TKTimeSpan ParseExact(string input, string[] formats, IFormatProvider formatProvider)
            => new TKTimeSpan(System.TimeSpan.ParseExact(input, formats, formatProvider));
        public static TKTimeSpan ParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles)
            => new TKTimeSpan(System.TimeSpan.ParseExact(input, format, formatProvider, styles));
        public static TKTimeSpan ParseExact(string input, string format, IFormatProvider formatProvider)
            => new TKTimeSpan(System.TimeSpan.ParseExact(input, format, formatProvider));
        public static TKTimeSpan ParseExact(string input, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles)
            => new TKTimeSpan(System.TimeSpan.ParseExact(input, formats, formatProvider, styles));

        public static bool TryParse(string input, IFormatProvider formatProvider, out TKTimeSpan result) {
            if(System.TimeSpan.TryParse(input, formatProvider, out System.TimeSpan spResult)) {
                result = new TKTimeSpan(spResult);
                return true;
            }
            else {
                result = new TKTimeSpan();
                return false;
            }
        }
        public static bool TryParse(string s, out TKTimeSpan result) {
            if(System.TimeSpan.TryParse(s, out System.TimeSpan spResult)) {
                result = new TKTimeSpan(spResult);
                return true;
            }
            else {
                result = new TKTimeSpan();
                return false;
            }
        }

        public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out TKTimeSpan result) {
            if(System.TimeSpan.TryParseExact(input, format, formatProvider, styles, out System.TimeSpan spResult)) {
                result = new TKTimeSpan(spResult);
                return true;
            }
            else {
                result = new TKTimeSpan();
                return false;
            }
        }
        public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles, out TKTimeSpan result) {
            if(System.TimeSpan.TryParseExact(input, formats, formatProvider, styles, out System.TimeSpan spResult)) {
                result = new TKTimeSpan(spResult);
                return true;
            }
            else {
                result = new TKTimeSpan();
                return false;
            }
        }
        public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, out TKTimeSpan result) {
            if(System.TimeSpan.TryParseExact(input, formats, formatProvider, out System.TimeSpan spResult)) {
                result = new TKTimeSpan(spResult);
                return true;
            }
            else {
                result = new TKTimeSpan();
                return false;
            }
        }
        public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, out TKTimeSpan result) {
            if(System.TimeSpan.TryParseExact(input, format, formatProvider, out System.TimeSpan spResult)) {
                result = new TKTimeSpan(spResult);
                return true;
            }
            else {
                result = new TKTimeSpan();
                return false;
            }
        }

        #endregion

        #region Operators

        public static TKTimeSpan operator +(TKTimeSpan t) => new TKTimeSpan(t.ticks++);
        public static TKTimeSpan operator +(TKTimeSpan t1, TKTimeSpan t2) => new TKTimeSpan(t1.ticks + t2.ticks);
        public static TKTimeSpan operator -(TKTimeSpan t) => new TKTimeSpan(t.ticks--);
        public static TKTimeSpan operator -(TKTimeSpan t1, TKTimeSpan t2) => new TKTimeSpan(t1.ticks - t2.ticks);
        public static bool operator ==(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks == t2.ticks;
        public static bool operator !=(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks != t2.ticks;
        public static bool operator <(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks < t2.ticks;
        public static bool operator >(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks > t2.ticks;
        public static bool operator <=(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks <= t2.ticks;
        public static bool operator >=(TKTimeSpan t1, TKTimeSpan t2) => t1.ticks >= t2.ticks;

        public static implicit operator System.TimeSpan(TKTimeSpan ts) => new System.TimeSpan(ts.ticks);
        public static implicit operator TKTimeSpan(System.TimeSpan ts) => new TKTimeSpan(ts.Ticks);

        #endregion
    }
}
