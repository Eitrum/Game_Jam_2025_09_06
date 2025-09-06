using System;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.Health
{
    public partial class DamageInstance
    {
        #region Variables

        private float originalAmount = 0f;
        private float currentAmount = 0f;
        private int damageTypeMask = 0;

        #endregion

        #region Properties

        public bool HasDamage => currentAmount > Mathf.Epsilon;
        public float Damage => currentAmount;
        public float UnmodifiedDamage => originalAmount;
        public int DamageTypeMask => damageTypeMask;
        public float Efficiency => (currentAmount / originalAmount);

        public Source Source { get; set; } = null;

        #endregion

        #region Constructor

        public DamageInstance(float amount) {
            this.originalAmount = amount;
            this.currentAmount = amount;
        }

        public DamageInstance(float amount, int damageType) {
            this.originalAmount = amount;
            this.currentAmount = amount;
            this.damageTypeMask = damageType;
        }

        public DamageInstance(float amount, int damageType, Source source) {
            this.originalAmount = amount;
            this.currentAmount = amount;
            this.damageTypeMask = damageType;
            this.Source = source;
        }

        #endregion

        #region Inc / Decrease

        public void Increase(float amount) {
            this.currentAmount += amount;
        }

        public void Decrease(float amount) {
            this.currentAmount -= amount;
        }

        #endregion

        #region Resistances and multipliers

        /// <summary>
        /// Anything above 1 will make amount to 0.
        /// </summary>
        /// <param name="percentage"></param>
        public void ApplyResistance(float percentage) {
            currentAmount *= (1f - Mathf.Clamp01(percentage));
        }

        public void MultiplyDamage(float amount) {
            currentAmount *= amount;
        }

        #endregion

        #region Damage Type

        public bool HasDamageType(int damageType)
            => HasDamageTypeMask(1 << damageType);

        public bool HasDamageTypeMask(int damageTypeMask)
            => (this.damageTypeMask & damageTypeMask) == damageTypeMask;

        public bool IsDamageType(int damageType)
            => IsDamageTypeMask(1 << damageType);

        public bool IsDamageTypeMask(int damageTypeMask)
            => this.damageTypeMask == damageTypeMask;

        public void SetDamageType(int damageType)
            => SetDamageTypeMask(1 << damageType);

        public void SetDamageTypeMask(int damageTypeMask)
            => this.damageTypeMask = damageTypeMask;

        public void AddDamageType(int damageType)
            => AddDamageTypeMask(1 << damageType);

        public void AddDamageTypeMask(int damageTypeMask)
            => this.damageTypeMask |= damageTypeMask;

        public void RemoveDamageType(int damageType)
            => RemoveDamageTypeMask(1 << damageType);

        public void RemoveDamageTypeMask(int damageTypeMask)
            => this.damageTypeMask &= ~damageTypeMask;

        #endregion
    }
}
