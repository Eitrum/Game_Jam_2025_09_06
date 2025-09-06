using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct BoolStack
    {
        #region Variables

        [SerializeField] private sbyte value;

        #endregion

        #region Properties

        public bool Value {
            get => value > 0;
            set {
                if(value)
                    this.value++;
                else
                    this.value--;
            }
        }

        public int Stacks { get => value; set => this.value = (sbyte)value; }

        #endregion

        #region Constructor

        public BoolStack(bool value) {
            this.value = value ? (sbyte)1 : (sbyte)0;
        }

        public BoolStack(sbyte value) {
            this.value = value;
        }

        public BoolStack(int value) {
            this.value = (sbyte)value;
        }

        #endregion

        #region Add

        public bool Add() {
            value++;
            return value > 0;
        }

        public bool Add(int value) {
            this.value += (sbyte)value;
            return value > 0;
        }

        public bool Add(sbyte value) {
            this.value += value;
            return value > 0;
        }

        #endregion

        #region Remove

        public bool Remove() {
            value--;
            return value > 0;
        }

        public bool Remove(int value) {
            this.value -= (sbyte)value;
            return value > 0;
        }

        public bool Remove(sbyte value) {
            this.value -= value;
            return value > 0;
        }

        #endregion

        #region Utility

        public void Set(bool value) => this.value = (sbyte)(value ? 1 : 0);
        public void Set(int value) => this.value = (sbyte)value;
        public void Reset() => value = 0;

        #endregion

        #region Conversion

        public static implicit operator bool(BoolStack s) => s.Value;
        public static implicit operator int(BoolStack s) => s.value;
        public static explicit operator BoolStack(bool b) => new BoolStack(b);

        #endregion

        #region Operators

        public static BoolStack operator ++(BoolStack s) => new BoolStack(s.value + 1);
        public static BoolStack operator --(BoolStack s) => new BoolStack(s.value - 1);

        public static BoolStack operator +(BoolStack s) => new BoolStack(s.value + 1);
        public static BoolStack operator -(BoolStack s) => new BoolStack(s.value - 1);

        public static BoolStack operator +(BoolStack lhs, int rhs) => new BoolStack(lhs.value + rhs);
        public static BoolStack operator -(BoolStack lhs, int rhs) => new BoolStack(lhs.value - rhs);

        public static bool operator ==(BoolStack lhs, bool value) => lhs.Value == value;
        public static bool operator !=(BoolStack lhs, bool value) => lhs.Value != value;

        public static bool operator >(BoolStack lhs, int value) => lhs.value > value;
        public static bool operator <(BoolStack lhs, int value) => lhs.value < value;
        public static bool operator >=(BoolStack lhs, int value) => lhs.value >= value;
        public static bool operator <=(BoolStack lhs, int value) => lhs.value <= value;

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is bool b)
                return b == Value;
            else if(obj is BoolStack bs)
                return bs.Value == Value;
            return base.Equals(obj);
        }

        public override string ToString() {
            return $"{(value > 0)} [{value}]";
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }

        #endregion
    }
}
