using System;
using UnityEngine;

namespace Toolkit.Currency
{
    [Serializable]
    public partial struct Currency
    {
        #region Variable / Properties

        [SerializeField] private int value;

        public int Value => value;
        public const CurrencySize SIZE = CurrencySize.Integer;
        public const CurrencyAccuracy ACCURACY = CurrencyAccuracy.Low;

        #endregion

        #region Constructor

        public Currency(int value) {
            this.value = value;
        }

        public Currency(float value) {
            this.value = (int)value;
        }

        public Currency(double value) {
            this.value = (int)value;
        }

        public Currency(long value) {
            this.value = (int)value;
        }

        public Currency(ulong value) {
            this.value = (int)value;
        }

        public Currency(short value) {
            this.value = (int)value;
        }

        public Currency(ushort value) {
            this.value = (int)value;
        }

        public Currency(CurrencyType type, int amount) {
            this.value = 0;
            Add(type, amount);
        }

        #endregion

        #region Add

        public void Add(Currency currency) {
            value += currency.value;
        }

        public void Add(int value) {
            this.value += value;
        }

        #endregion

        #region Remove

        public void Remove(Currency currency) {
            value -= currency.value;
        }

        #endregion

        #region Has

        public bool Has(Currency currency) {
            return value >= currency.value;
        }

        public bool Has(CurrencyType type, int amount) {
            switch(type) {
                case CurrencyType.Copper: return amount >= value;
                case CurrencyType.Silver: return amount >= (value / 100);
                case CurrencyType.Gold: return amount >= (value / 10000);
                case CurrencyType.Platinum: return amount >= (value / 1000000);
            }
            return false;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(Currency lhs, Currency rhs) => lhs.value == rhs.value;
        public static bool operator !=(Currency lhs, Currency rhs) => lhs.value != rhs.value;

        public static Currency operator ++(Currency c) => new Currency(c.value + 1);
        public static Currency operator --(Currency c) => new Currency(c.value - 1);

        public static Currency operator +(Currency lhs, Currency rhs) => new Currency() { value = lhs.value + rhs.value };
        public static Currency operator -(Currency lhs, Currency rhs) => new Currency() { value = lhs.value - rhs.value };
        public static Currency operator *(Currency lhs, Currency rhs) => new Currency() { value = lhs.value * rhs.value };
        public static Currency operator /(Currency lhs, Currency rhs) => new Currency() { value = lhs.value / rhs.value };

        public static Currency operator +(Currency lhs, int rhs) => new Currency(lhs.value + rhs);
        public static Currency operator +(Currency lhs, float rhs) => new Currency(lhs.value + (int)rhs);
        public static Currency operator +(Currency lhs, double rhs) => new Currency(lhs.value + (int)rhs);
        public static Currency operator +(Currency lhs, long rhs) => new Currency(lhs.value + (int)rhs);
        public static Currency operator +(Currency lhs, uint rhs) => new Currency(lhs.value + (int)rhs);
        public static Currency operator +(Currency lhs, ulong rhs) => new Currency(lhs.value + (int)rhs);
        public static Currency operator +(Currency lhs, short rhs) => new Currency(lhs.value + rhs);
        public static Currency operator +(Currency lhs, ushort rhs) => new Currency(lhs.value + rhs);
        public static Currency operator +(Currency lhs, byte rhs) => new Currency(lhs.value + rhs);
        public static Currency operator +(Currency lhs, sbyte rhs) => new Currency(lhs.value + rhs);

        public static Currency operator -(Currency lhs, int rhs) => new Currency(lhs.value - rhs);
        public static Currency operator -(Currency lhs, float rhs) => new Currency(lhs.value - (int)rhs);
        public static Currency operator -(Currency lhs, double rhs) => new Currency(lhs.value - (int)rhs);
        public static Currency operator -(Currency lhs, long rhs) => new Currency(lhs.value - (int)rhs);
        public static Currency operator -(Currency lhs, uint rhs) => new Currency(lhs.value - (int)rhs);
        public static Currency operator -(Currency lhs, ulong rhs) => new Currency(lhs.value - (int)rhs);
        public static Currency operator -(Currency lhs, short rhs) => new Currency(lhs.value - rhs);
        public static Currency operator -(Currency lhs, ushort rhs) => new Currency(lhs.value - rhs);
        public static Currency operator -(Currency lhs, byte rhs) => new Currency(lhs.value - rhs);
        public static Currency operator -(Currency lhs, sbyte rhs) => new Currency(lhs.value - rhs);

        #endregion

    }
}
