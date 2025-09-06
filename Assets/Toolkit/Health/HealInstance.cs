using System;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.Health
{
    public partial class HealInstance
    {
        #region Variables

        private float originalAmount = 0f;
        private float currentAmount = 0f;
        private int type = 0;

        #endregion

        #region Properties

        public bool HasHeal => currentAmount > Mathf.Epsilon;
        public float Heal => currentAmount;
        public float Unmodified => originalAmount;
        public int HealType => type;
        public float Efficiency => (currentAmount / originalAmount);

        public Source Source { get; set; } = null;

        #endregion

        #region Constructor

        public HealInstance(float amount) {
            originalAmount = amount;
            currentAmount = amount;
        }

        public HealInstance(float amount, int healType) {
            originalAmount = amount;
            currentAmount = amount;
            this.type = healType;
        }

        public HealInstance(float amount, int healType, Source source) {
            originalAmount = amount;
            currentAmount = amount;
            this.type = healType;
            this.Source = source;
        }

        #endregion

        #region Increase Decrease

        public void Increase(float amount) {
            this.currentAmount += amount;
        }

        public void Decrease(float amount) {
            this.currentAmount -= amount;
        }

        #endregion

        #region Efficiency

        public void ApplyEfficiency(float percentage) {
            this.currentAmount *= percentage;
        }

        #endregion

        #region Utility

        public void Reset() {
            this.currentAmount = originalAmount;
        }

        public void Combine(HealInstance other) {
            this.originalAmount += other.originalAmount;
            this.currentAmount += other.currentAmount;
            this.type = this.type | other.type;
        }

        #endregion

        #region Heal Type

        public bool HasHealType(int damageType) {
            return (this.type & damageType) == damageType;
        }

        public bool IsHealType(int damageType) {
            return this.type == damageType;
        }

        public void SetHealType(int type) {
            this.type = type;
        }

        public void AddHealType(int type) {
            this.type |= type;
        }

        public void RemoveHealType(int type) {
            this.type &= ~type;
        }

        #endregion
    }
}
