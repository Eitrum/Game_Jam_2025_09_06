using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Health.Utility {
    [AddComponentMenu("Toolkit/Health/Utility/Damage State (OnHealthChanged)")]
    public class DamageStateOnHealthChanged : MonoBehaviour {
        public delegate void OnDamageStateChangedDelegate(int tier);

        #region Variables

        [SerializeField] private bool usePercentageHealth = true;
        [SerializeField] private Data[] data = { };

        private int currentInstance = -1;
        private IHealth health;
        private OnDamageStateChangedDelegate onDamageStateChanged;

        #endregion

        #region Properties

        public bool UsePercentages => usePercentageHealth;

        public int StateCount => data.Length;
        public IReadOnlyList<Data> States => data;

        public IHealth Health {
            get => health;
            set {
                if(enabled) {
                    OnDisable();
                    health = value;
                    OnEnable();
                }
                else
                    health = value;
            }
        }

        public event OnDamageStateChangedDelegate OnDamageStateChanged {
            add {
                onDamageStateChanged += value;
                if(currentInstance != -1)
                    value?.Invoke(currentInstance);
            }
            remove => onDamageStateChanged -= value;
        }

        #endregion

        #region Init

        private void Awake() {
            health = GetComponentInParent<IHealth>();
            foreach(var d in data)
                d.SetActive(false);
        }

        private void OnEnable() {
            if(health != null) {
                health.OnHealthChanged += OnHealthChanged;
                OnHealthChanged(health, health.Current, health.Current);
            }
        }

        private void OnDisable() {
            if(health != null)
                health.OnHealthChanged -= OnHealthChanged;
        }

        #endregion

        #region Callbacks

        private void OnHealthChanged(IHealth source, float oldHealth, float newHealth) {
            var t = usePercentageHealth ? (newHealth / source.Full) : newHealth;

            for(int i = data.Length - 1; i >= 0; i--) {
                if(data[i].IsInRange(t)) {
                    SetStateActive(i);
                    return;
                }
            }
            SetStateActive(0);
        }

        #endregion

        #region Utility

        public void SetStateActive(int i) {
            i = Mathf.Clamp(i, 0, data.Length);
            if(i == currentInstance)
                return; // Already active

            if(currentInstance != -1)
                data[currentInstance].SetActive(false);

            currentInstance = i;
            data[currentInstance].SetActive(true);
            onDamageStateChanged?.Invoke(i);
        }

        [ContextMenu("Balance Thresholds")]
        public void BalanceThresholds() {
            if(usePercentageHealth) {
                var lm1 = data.Length - 0f;

                for(int i = data.Length - 1; i >= 0; i--)
                    data[i].Threshold = 1f - (i / lm1);
            }
            else {
                var h = GetComponentInParent<IHealth>();
                var f = h.Full;
                var lm1 = data.Length - 0f;
                for(int i = data.Length - 1; i >= 0; i--)
                    data[i].Threshold = (1f - (i / lm1)) * f;
            }
        }

        #endregion

        [System.Serializable]
        public class Data {
            #region Variables

            [SerializeField] private float threshold = 0.5f;
            [SerializeField] private Transform container = null;

            #endregion

            #region Properties

            public float Threshold {
                get => threshold;
                set => threshold = value;
            }

            public Transform Container => container;

            #endregion

            #region Constructor

            public Data() { }

            public Data(float threshold, Transform container) {
                this.threshold = threshold;
                this.container = container;
            }

            #endregion

            #region Utility

            public bool IsInRange(float t) => t <= threshold;

            public void SetActive(bool value) {
                if(container == null)
                    return;
                if(value != container.gameObject.activeSelf)
                    container.gameObject.SetActive(value);
            }

            #endregion
        }
    }
}
