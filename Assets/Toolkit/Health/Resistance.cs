using System;
using UnityEngine;

namespace Toolkit.Health
{
    [AddComponentMenu("Toolkit/Health/Resistance")]
    public class Resistance : MonoBehaviour
    {
        #region Variables

        [SerializeField, DamageType(true)] private int damageTypeMask = 0;
        [SerializeField, RangeEx(0, 1f, 0.01f)] private float resistance = 0f;

        private IHealth health;
        private IDamageEvent damageEvent;

        #endregion

        #region Init

        private void Awake() {
            health = GetComponentInParent<IHealth>();
            health = health.Root;
            damageEvent = health as IDamageEvent;
        }

        private void OnEnable() {
            damageEvent.OnPreDamage += OnPreDamage;
        }

        private void OnDisable() {
            damageEvent.OnPreDamage -= OnPreDamage;
        }

        #endregion

        #region Callback

        [InvokeOrder(0)]
        private void OnPreDamage(DamageInstance data) {
            if(data.HasDamageTypeMask(damageTypeMask)) {
                data.ApplyResistance(resistance);
            }
        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        private void OnValidate() {
            health = GetComponentInParent<IHealth>();
            if(health == null)
                throw new MissingComponentException("No health component found!");
        }

#endif

        #endregion
    }
}
