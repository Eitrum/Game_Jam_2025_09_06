using System;
using UnityEngine;

namespace Toolkit.Health {
    [AddComponentMenu("Toolkit/Health/Health")]
    public class Health : MonoBehaviour, IHealth, IDamageEvent, IHealEvent, IReadOnlyHealth {
        #region Variables

        [SerializeField] private Stat maxHealth = new Stat(100, 1, 1);
        [SerializeField] private float current = 0f;

        private DeterministicDelegates<PreDamageCallback> onPreDamage = new DeterministicDelegates<PreDamageCallback>();
        private DeterministicDelegates<PreHealCallback> onPreHeal = new DeterministicDelegates<PreHealCallback>();

        #endregion

        #region Properties

        public float Current {
            get => current;
            set {
                value = Mathf.Clamp(value, 0f, Full);
                if(value != current) {
                    var old = current;
                    current = value;
                    OnHealthChanged?.Invoke(this, old, current);
                }
            }
        }
        public float Full {
            get => maxHealth.Total;
            set {
                var multiplier = maxHealth.Increased;
                var newBase = value / multiplier;
                maxHealth.AddBase(newBase - maxHealth.Value);
            }
        }
        public float Percentage {
            get => current / maxHealth.Total;
            set => Current = (Mathf.Clamp01(value) * maxHealth.Total);
        }

        public bool IsAlive => current > 0f;
        public IHealth Root => this;

        #endregion

        #region Events

        public event HealthChangedCallback OnHealthChanged;
        public event DeathCallback OnDeath;
        public event ReviveCallback OnRevive;

        public event DamageCallback OnDamage;
        public event PreDamageCallback OnPreDamage {
            add => onPreDamage += value;
            remove => onPreDamage -= value;
        }

        public event HealCallback OnHeal;
        public event PreHealCallback OnPreHeal {
            add => onPreHeal += value;
            remove => onPreHeal -= value;
        }

        #endregion

        #region Initialize

        void Awake() {
            if(!IsAlive) {
                current = Full;
            }
        }

        #endregion

        #region IHealth Method Implementation

        public void Damage(DamageInstance data) {
            if(current <= 0f)
                return;
            onPreDamage.Delegate?.Invoke(data);
            if(data.HasDamage) {
                Current -= data.Damage;
                OnDamage?.Invoke(data);

                if(current <= 0f) {
                    OnDeath?.Invoke();
                }
            }
        }

        public void Heal(HealInstance data) {
            if(current <= 0f)
                return;
            onPreHeal.Delegate?.Invoke(data);
            if(data.HasHeal) {
                Current += data.Heal;
                OnHeal?.Invoke(data);
            }
        }

        public void Kill() {
            if(current <= 0f)
                return;
            Current = 0;
            OnDeath?.Invoke();
        }

        public void Revive(float health) {
            if(current > 0f)
                return;
            Current = health;
            OnRevive?.Invoke();
        }

        public void Restore(bool triggerEvent) {
            if(triggerEvent)
                Current = Full;
            else
                current = Full;
        }

        #endregion
    }
}
