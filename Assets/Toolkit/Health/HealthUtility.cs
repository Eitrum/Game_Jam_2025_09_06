using System.Collections;
using System.Collections.Generic;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.Health
{
    public static class HealthUtility
    {
        #region Damage Extension

        public static DamageInstance Damage(this IHealth health, float amount) {
            var dd = new DamageInstance(amount);
            health.Damage(dd);
            return dd;
        }

        public static DamageInstance Damage(this IHealth health, float amount, int damageType) {
            var dd = new DamageInstance(amount, damageType);
            health.Damage(dd);
            return dd;
        }

        public static DamageInstance Damage(this IHealth health, float amount, int damageType, Source source) {
            var dd = new DamageInstance(amount, damageType, source);
            health.Damage(dd);
            return dd;
        }

        #endregion

        #region Heal Extension

        public static HealInstance Heal(this IHealth health, float amount) {
            var hd = new HealInstance(amount);
            health.Heal(hd);
            return hd;
        }

        public static HealInstance Heal(this IHealth health, float amount, int damageType) {
            var hd = new HealInstance(amount, damageType);
            health.Heal(hd);
            return hd;
        }

        public static HealInstance Heal(this IHealth health, float amount, int damageType, Source source) {
            var hd = new HealInstance(amount, damageType, source);
            health.Heal(hd);
            return hd;
        }

        #endregion

        #region Revive Extensions

        public static void Revive(this IHealth health, float value, bool percentage)
            => health.Revive(percentage ? value * health.Full : value);

        #endregion

        #region Event

        public static IDamageEvent GetDamageEvent(this IHealth health) => health as IDamageEvent;
        public static IHealEvent GetHealEvent(this IHealth health) => health as IHealEvent;

        #endregion

        #region Health Extraction

        public static float GetValue(this IHealth health, HealthReference reference){
            switch(reference) {
                case HealthReference.Percentage: return health.Percentage;
                case HealthReference.Current: return health.Current;
                case HealthReference.Full: return health.Full;
            }
            return 0f;
        }

        public static bool Is(IHealth lhs, IHealth rhs, HealthReference reference, Toolkit.Mathematics.ComparitorOperatorType comparitor){
            return Toolkit.Mathematics.ComparitorOperatorUtility.Is(GetValue(lhs, reference), GetValue(rhs, reference), comparitor);
        }

        public static bool Is(IHealth lhs, float rhs, HealthReference reference, Toolkit.Mathematics.ComparitorOperatorType comparitor){
            return Toolkit.Mathematics.ComparitorOperatorUtility.Is(GetValue(lhs, reference), rhs, comparitor);
        }

        public static float GetAmount(this IHealth health, float inputValue, HealthDamageMode mode) {
            switch(mode) {
                case HealthDamageMode.Normal: return inputValue;
                case HealthDamageMode.MaxHealthPercentage: return health.Full * inputValue;
                case HealthDamageMode.CurrentHealthPercentage: return health.Current * inputValue;
            }
            return 0f;
        }

        #endregion
    }
}
