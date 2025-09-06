using System;
using UnityEngine;

namespace Toolkit {
    [Serializable]
    public struct StatInt {
        #region Variables

        [SerializeField] private int baseStat;
        [SerializeField] private float baseMultiplierStat;
        [SerializeField] private float multiplierStat;

        #endregion

        #region Properties

        public int BaseValue => baseStat;
        public float Multiplier => baseMultiplierStat * multiplierStat;
        public int Total => (int)(baseStat * baseMultiplierStat * multiplierStat);

        public static StatInt Default => new StatInt(100, 1, 1);

        #endregion

        #region Constructor

        public StatInt(int baseValue) {
            this.baseStat = baseValue;
            this.baseMultiplierStat = 1;
            this.multiplierStat = 1;
        }

        public StatInt(int baseValue, float baseMultiplier, float multiply) {
            this.baseStat = baseValue;
            this.baseMultiplierStat = baseMultiplier;
            this.multiplierStat = multiply;
        }

        #endregion

        #region Math operations

        public void AddBase(int amount) {
            baseStat += amount;
        }

        public void RemoveBase(int amount) {
            baseStat -= amount;
        }

        public void AddMultiplierBase(float value) {
            baseMultiplierStat += value;
        }

        public void RemoveMultiplierBase(float value) {
            baseMultiplierStat -= value;
        }

        public void Multiply(float value) {
            multiplierStat *= value;
        }

        public void Divide(float value) {
            multiplierStat /= value;
        }

        #endregion

        #region Operators

        public static StatInt operator ++(StatInt stat) {
            return new StatInt(stat.baseStat + 1, stat.baseMultiplierStat, stat.multiplierStat);
        }

        public static StatInt operator --(StatInt stat) {
            return new StatInt(stat.baseStat - 1, stat.baseMultiplierStat, stat.multiplierStat);
        }

        public static StatInt operator +(StatInt stat, int value) {
            return new StatInt(stat.baseStat + value, stat.baseMultiplierStat, stat.multiplierStat);
        }

        public static StatInt operator -(StatInt stat, int value) {
            return new StatInt(stat.baseStat - value, stat.baseMultiplierStat, stat.multiplierStat);
        }

        public static StatInt operator *(StatInt stat, float value) {
            return new StatInt(stat.baseStat, stat.baseMultiplierStat, stat.multiplierStat * value);
        }

        public static StatInt operator /(StatInt stat, float value) {
            return new StatInt(stat.baseStat, stat.baseMultiplierStat, stat.multiplierStat / value);
        }

        #endregion
    }
}
