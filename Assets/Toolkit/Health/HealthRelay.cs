using System.Collections;
using System.Collections.Generic;
using Toolkit.Health;
using UnityEngine;

namespace Toolkit.Health
{
    [AddComponentMenu("Toolkit/Health/Relay/Default")]
    public class HealthRelay : MonoBehaviour, IHealth, IReadOnlyHealth
    {
        #region Variables

        [SerializeField, RangeEx(0f, 10f, 0.01f)] private float damageMultiplier = 1f;
        private IHealth parent;

        #endregion

        #region Init

        void Awake() {
            parent = transform.parent.GetComponentInParent<IHealth>();
        }

        #endregion

        #region Properties

        public float Current { get => parent.Current; set => parent.Current = value; }
        public float Full { get => parent.Full; set => parent.Full = value; }
        public float Percentage { get => parent.Percentage; set => parent.Percentage = value; }
        public bool IsAlive => parent.IsAlive;
        public IHealth Root => parent.Root;

        #endregion

        #region Events

        public event HealthChangedCallback OnHealthChanged {
            add => parent.OnHealthChanged += value;
            remove => parent.OnHealthChanged -= value;
        }

        public event DeathCallback OnDeath {
            add => parent.OnDeath += value;
            remove => parent.OnDeath -= value;
        }

        public event ReviveCallback OnRevive {
            add => parent.OnRevive += value;
            remove => parent.OnRevive -= value;
        }

        #endregion

        #region IHealth Method Impl

        public void Damage(DamageInstance data) {
            data.MultiplyDamage(damageMultiplier);
            parent.Damage(data);
        }
        public void Heal(HealInstance data) => parent.Heal(data);
        public void Kill() => parent.Kill();
        public void Revive(float health) => parent.Revive(health);
        public void Restore(bool triggerEvent) => parent.Restore(triggerEvent);

        #endregion
    }
}
