using System;
using System.Linq;
using UnityEngine;

namespace Toolkit {
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Stat {

        public enum ValueType {
            Base,
            Increase,
            More,
        }

        #region Variables

        private const string TAG = "[Stat] - ";

        [SerializeField] private float value;
        [SerializeField] private float increased;
        [SerializeField] private float more;

        #endregion

        #region Properties

        public float Value => value;
        public float Increased => increased;
        public float More => more;

        public float this[int index] {
            get {
                switch(index) {
                    case 0: return value;
                    case 1: return increased;
                    case 2: return more;
                    default:
                        throw new IndexOutOfRangeException(TAG + "range between 0 and 2 inclusive.");
                }
            }
        }

        public float this[ValueType type] {
            get {
                switch(type) {
                    case ValueType.Base: return value;
                    case ValueType.Increase: return increased;
                    case ValueType.More: return more;
                    default:
                        throw new IndexOutOfRangeException(TAG + "Unsupported ValueType.");
                }
            }
        }

        public float Total => value * (1f + increased) * (1f + more);

        public static Stat Default => new Stat(0f, 0f, 0f);

        #endregion

        #region Constructor

        public Stat(float value) {
            this.value = value;
            this.increased = 0f;
            this.more = 0f;
        }

        public Stat(float value, float increased) {
            this.value = value;
            this.increased = increased;
            this.more = 0f;
        }

        public Stat(float value, float increased, float more) {
            this.value = value;
            this.increased = increased;
            this.more = more;
        }

        #endregion

        #region Util

        public void Reset() {
            value = 0f;
            increased = 0f;
            more = 0f;
        }

        #endregion

        #region Modifications

        public void Add(FieldValue value) {
            Add(value.Type, value.Value);
        }

        public void Add(ValueType field, float value) {
            switch(field) {
                case ValueType.Base:
                    AddBase(value);
                    break;
                case ValueType.Increase:
                    Increase(value);
                    break;
                case ValueType.More:
                    AddMore(value);
                    break;
            }
        }

        public void Remove(FieldValue value) {
            Remove(value.Type, value.Value);
        }

        public void Remove(ValueType field, float value) {
            switch(field) {
                case ValueType.Base:
                    RemoveBase(value);
                    break;
                case ValueType.Increase:
                    Decrease(value);
                    break;
                case ValueType.More:
                    AddLess(value);
                    break;
            }
        }

        public void AddBase(float amount) {
            value += amount;
        }

        public void RemoveBase(float amount) {
            value -= amount;
        }

        public void Increase(float value) {
            increased += value;
        }

        public void Decrease(float value) {
            increased -= value;
        }

        /// <summary>
        /// Add More of a stat
        ///   30% More, would be calulated with 0.3f
        /// </summary>
        public void AddMore(float value) {
            if(value < 0f) {
                AddLess(-value);
                return;
            }
            // Does this to keep the values 0 based instead of having to do 1.x for every value.
            more = (1f + more) * (1f + value) - 1f;
        }

        /// <summary>
        /// Shortcut for AddLess
        /// </summary>
        public void RemoveMore(float value)
            => AddLess(value);

        /// <summary>
        /// Opposite of "AddMore" --- Add Less of a stat
        ///   30% Less, would be calulated with 0.3f
        /// </summary>
        public void AddLess(float value) {
            if(value < 0f) {
                AddMore(-value);
                return;
            }
            // Does this to keep the values 0 based instead of having to do 1.x for every value.
            more = (1f + more) / (1f + value) - 1f;
        }

        #endregion

        #region Operators

        public static Stat operator ++(Stat stat) {
            return new Stat(stat.value + 1f, stat.increased, stat.more);
        }

        public static Stat operator --(Stat stat) {
            return new Stat(stat.value - 1f, stat.increased, stat.more);
        }

        public static Stat operator +(Stat stat, float value) {
            return new Stat(stat.value + value, stat.increased, stat.more);
        }

        public static Stat operator -(Stat stat, float value) {
            return new Stat(stat.value - value, stat.increased, stat.more);
        }

        public static Stat operator +(Stat lhs, Stat rhs) => new Stat(lhs.value + rhs.value, lhs.increased + rhs.increased, (1f + lhs.more) * (1f + rhs.more) - 1f); // Multiplier stat should be multiplicative
        public static Stat operator -(Stat lhs, Stat rhs) => new Stat(lhs.value - rhs.value, lhs.increased - rhs.increased, (1f + lhs.more) / (1f + rhs.more) - 1f); // Multiplier stat should be multiplicative

        public static implicit operator int(Stat stat) => Mathf.FloorToInt(stat.Total);
        public static implicit operator float(Stat stat) => stat.Total;

        public static bool operator <(Stat stat0, Stat stat1) => stat0.Total < stat1.Total;
        public static bool operator >(Stat stat0, Stat stat1) => stat0.Total > stat1.Total;

        public static bool operator <=(Stat stat0, Stat stat1) => stat0.Total <= stat1.Total;
        public static bool operator >=(Stat stat0, Stat stat1) => stat0.Total >= stat1.Total;

        public static bool operator <(Stat stat, float val) => stat.Total < val;
        public static bool operator >(Stat stat, float val) => stat.Total > val;

        public static bool operator <=(Stat stat, float val) => stat.Total <= val;
        public static bool operator >=(Stat stat, float val) => stat.Total >= val;

        public static bool operator <(Stat stat, int val) => stat.Total < val;
        public static bool operator >(Stat stat, int val) => stat.Total > val;

        public static bool operator <=(Stat stat, int val) => stat.Total <= val;
        public static bool operator >=(Stat stat, int val) => stat.Total >= val;

        public static bool operator <(float val, Stat stat) => val < stat.Total;
        public static bool operator >(float val, Stat stat) => val > stat.Total;

        public static bool operator <=(float val, Stat stat) => val <= stat.Total;
        public static bool operator >=(float val, Stat stat) => val >= stat.Total;

        public static bool operator <(int val, Stat stat) => val < stat.Total;
        public static bool operator >(int val, Stat stat) => val > stat.Total;

        public static bool operator <=(int val, Stat stat) => val <= stat.Total;
        public static bool operator >=(int val, Stat stat) => val >= stat.Total;

        #endregion

        #region String and parsing

        public override string ToString() {
            return $"[{value}, {increased}, {more}]";
        }

        public static Stat Parse(string input) {
            TryParse(input, out Stat stat);
            return stat;
        }

        public static bool TryParse(string input, out Stat stat) {
            if(input.Length > 5 && input[0] == '[' && input[input.Length - 1] == ']') {
                var split = input.Substring(1, input.Length - 2).Split(',');
                if(split.Length == 3 && float.TryParse(split[0].Trim(), out float baseValue) && float.TryParse(split[1].Trim(), out float baseMulti) && float.TryParse(split[2].Trim(), out float multiplier)) {
                    stat = new Stat(baseValue, baseMulti, multiplier);
                    return true;
                }
            }
            stat = default;
            return false;
        }

        #endregion

        [Serializable]
        public struct FieldValue {

            #region Variables

            [SerializeField] private ValueType type;
            [SerializeField] private float value;

            #endregion

            #region Properties

            public ValueType Type => type;
            public float Value => value;

            #endregion

            #region Constructor

            public FieldValue(ValueType type) {
                this.type = type;
                this.value = 0f;
            }

            public FieldValue(float value) {
                this.type = ValueType.Base;
                this.value = value;
            }

            public FieldValue(ValueType type, float value) {
                this.type = type;
                this.value = value;
            }

            #endregion
        }
    }
}
