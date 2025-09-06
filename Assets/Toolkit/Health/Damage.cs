using System;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.Health {
    [Serializable]
    public struct Damage {
        #region Variables

        [SerializeField, DamageType(false)] private int damageType;
        [SerializeField, Min(0f)] private float amount;

        #endregion

        #region Properties

        public int DamageTypeMask {
            get => damageType;
            set => damageType = value;
        }

        public float Amount { get => amount; set => amount = Mathf.Max(0f, value); }

        public DamageInstance DamageData => new DamageInstance(Amount, DamageTypeMask);

        #endregion

        #region Constructor

        public Damage(float amount) {
            this.damageType = 0;
            this.amount = amount;
        }

        public Damage(float amount, int damageType) {
            this.damageType = damageType;
            this.amount = amount;
        }

        #endregion

        #region Methods

        public DamageInstance CreateDamageData(Source source) {
            return new DamageInstance(Amount, DamageTypeMask, source);
        }

        #endregion
    }

    [Serializable]
    public struct DamageRange {
        #region Variables

        [SerializeField, DamageType(false)] private int damageType;
        [SerializeField] private MinMax amount;

        #endregion

        #region Properties

        public int DamageTypeMask {
            get => damageType;
            set => damageType = value;
        }

        public float Amount { get => amount.Random; }
        public MinMax Range => amount;

        public DamageInstance DamageData => new DamageInstance(Amount, DamageTypeMask);

        #endregion

        #region Constructor

        public DamageRange(float amount) {
            this.damageType = 0;
            this.amount = new MinMax(amount, amount);
        }

        public DamageRange(float amount, int damageType) {
            this.damageType = damageType;
            this.amount = new MinMax(amount, amount);
        }

        public DamageRange(MinMax amount) {
            this.damageType = 0;
            this.amount = amount;
        }

        public DamageRange(MinMax amount, int damageType) {
            this.damageType = damageType;
            this.amount = amount;
        }

        #endregion

        #region Methods

        public DamageInstance CreateDamageData(Source source) {
            return new DamageInstance(Amount, DamageTypeMask, source);
        }

        #endregion
    }
}
