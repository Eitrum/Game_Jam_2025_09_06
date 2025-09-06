using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Cause/Trigger - (Impact)")]
    public class TriggerOnImpact : MonoBehaviour, ITrigger {
        #region Variables

        [SerializeField] private bool repeatable = false;
        [SerializeField, Min(0f)] private float minimumImpactVelocity = 0f;
        [SerializeField, Layer(true)] private int mask = ~0;

        private bool triggered = false;
        private Rigidbody body;

        public event OnTriggerDelegate OnTrigger;

        #endregion

        #region Init

        void Awake() {
            body = GetComponentInParent<Rigidbody>();
        }

        #endregion

        #region Properties

        public bool Repeatable {
            get => repeatable;
            set => repeatable = value;
        }

        public bool HasTriggered {
            get => triggered;
            set => triggered = value;
        }

        public float MinimumImpactVelocity {
            get => minimumImpactVelocity;
            set => minimumImpactVelocity = Mathf.Max(0, value);
        }

        public int Mask {
            get => mask;
            set => mask = value;
        }

        public Rigidbody Body {
            get => body;
            set => body = value;
        }

        #endregion

        #region Collision

        private void OnCollisionEnter(Collision collision) {
            if((minimumImpactVelocity < Mathf.Epsilon || collision.relativeVelocity.magnitude > minimumImpactVelocity) && mask.HasFlag(1 << collision.gameObject.layer))
                CauseTrigger();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if((minimumImpactVelocity < Mathf.Epsilon || collision.relativeVelocity.magnitude > minimumImpactVelocity) && mask.HasFlag(1 << collision.gameObject.layer))
                CauseTrigger();
        }

        #endregion

        #region ITrigger Impl

        private void HandleImpact(Source source) {
            if(!repeatable && triggered) {
                source?.Dispose();
                return;
            }
            try {
                triggered = true;
                OnTrigger?.Invoke(source);
            }
            finally {
                source.Dispose();
            }
        }

        public void CauseTrigger(Source source) {
            using(var s = source.AddChild(body))
                HandleImpact(s);
        }

        [Button]
        public void CauseTrigger() {
            using(var s = Source.Create(body))
                HandleImpact(s);
        }

        #endregion
    }
}
