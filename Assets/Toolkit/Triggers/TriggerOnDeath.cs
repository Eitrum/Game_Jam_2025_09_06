using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.Health;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Cause/Trigger - (OnDeath)")]
    public class TriggerOnDeath : MonoBehaviour, ITrigger {
        #region Variables

        public event OnTriggerDelegate OnTrigger;

        private bool triggered = false;
        private IHealth health;

        #endregion

        #region Properties

        public IHealth HealthReference {
            get => health;
            set {
                if(health == value)
                    return;
                if(health != null)
                    health.OnDeath -= OnDeath;
                health = value;
                if(health != null)
                    health.OnDeath += OnDeath;
            }
        }

        public bool HasTriggered => triggered;

        public Source Source => Source.Create(this as ITrigger);

        #endregion

        #region Init

        void Awake() {
            health = GetComponentInParent<IHealth>();
        }

        void OnEnable() {
            if(health != null)
                health.OnDeath += OnDeath;
        }

        void OnDisable() {
            if(health != null)
                health.OnDeath -= OnDeath;
        }

        #endregion

        #region ITrigger Impl

        [Button]
        public void CauseTrigger()
            => OnDeath();

        public void CauseTrigger(Source source) {
            triggered = true;
            using(var t = source.AddChild(Source))
                OnTrigger?.Invoke(t);
        }

        private void OnDeath() {
            triggered = true;
            using(var t = Source.Create(health))
                OnTrigger?.Invoke(t);
        }

        #endregion
    }
}
