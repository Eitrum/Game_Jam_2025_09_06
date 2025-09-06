using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.Health;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Cause/Trigger - (Health Change)")]
    public class TriggerOnHealthChanged : MonoBehaviour, ITrigger {
        public enum HealthChangedEventType {
            Damage = 0,
            Heal = 1,
            Any = 2,
        }

        #region Variables

        [SerializeField] private HealthChangedEventType healthChangedListener = HealthChangedEventType.Damage;
        [SerializeField] private bool repeatable = true;
        [SerializeField] private bool useRealtime = false;
        [SerializeField, Min(0f)] private float cooldown = 0f;

        private bool triggered = false;
        private float timeAtLastTrigger = 0f;
        private IHealth health;

        private OnTriggerDelegate onTrigger;

        #endregion

        #region Properties

        public event OnTriggerDelegate OnTrigger {
            add => onTrigger += value;
            remove => onTrigger -= value;
        }

        public bool Repeatable {
            get => repeatable;
            set => repeatable = value;
        }

        public bool UseRealtime {
            get => useRealtime;
            set => useRealtime = value;
        }

        public float Cooldown {
            get => cooldown;
            set => cooldown = Mathf.Max(0, value);
        }

        public bool HasTriggered {
            get => triggered;
            set => triggered = value;
        }

        public IHealth Health {
            get => health;
            set {
                if(health != null)
                    health.OnHealthChanged -= OnHealthChanged;
                health = value;
                if(health != null)
                    health.OnHealthChanged += OnHealthChanged;
            }
        }

        public HealthChangedEventType HealthChangedListener {
            get => healthChangedListener;
            set => healthChangedListener = value;
        }

        #endregion

        #region Init

        void Awake() {
            health = GetComponentInParent<IHealth>();
        }

        void OnEnable() {
            if(health != null)
                health.OnHealthChanged += OnHealthChanged;
        }

        void OnDisable() {
            if(health != null)
                health.OnHealthChanged -= OnHealthChanged;
        }

        #endregion

        #region Health Callback

        private void OnHealthChanged(IHealth source, float oldHealth, float newHealth) {
            switch(healthChangedListener) {
                case HealthChangedEventType.Damage:
                    if(newHealth < oldHealth)
                        CauseTrigger(Source.Create(source));
                    break;
                case HealthChangedEventType.Heal:
                    if(newHealth > oldHealth)
                        CauseTrigger(Source.Create(source));
                    break;
                case HealthChangedEventType.Any:
                    CauseTrigger(Source.Create(source));
                    break;
            }
        }

        #endregion

        #region ITrigger Impl

        [Button]
        public void CauseTrigger()
            => Handle(Source.Create(this as ITrigger));

        public void CauseTrigger(Source source)
            => Handle(source.AddChild(this as ITrigger));

        private void Handle(Source source) {
            if(!repeatable && triggered) {
                source?.Dispose();
                return;
            }

            var tsll = useRealtime ? Time.realtimeSinceStartup : Time.timeSinceLevelLoad;
            if(repeatable && cooldown > tsll - timeAtLastTrigger) {
                source?.Dispose();
                return;
            }

            timeAtLastTrigger = tsll;
            triggered = true;
            try {
                onTrigger?.Invoke(source);
            }
            finally {
                source.Dispose();
            }
        }

        #endregion
    }
}
