using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI
{
    [AddComponentMenu("Toolkit/UI/Bar (Image Delayed)")]
    public sealed class BarImageDelayed : Bar, IBar
    {
        #region Variables

        [SerializeField] private float value = 0f;
        [SerializeField] private MinMax range = new MinMax(0f, 1f);

        [SerializeField, Min(0f)] private float delay = 0.5f;
        [SerializeField, Min(0f)] private float duration = 0.5f;

        [SerializeField] private UnityEngine.UI.Image foreground = null;
        [SerializeField] private UnityEngine.UI.Image background = null;

        [SerializeField] private bool useTint = false;
        [SerializeField] private Color negativeTint = Color.white;
        [SerializeField] private Color positiveTint = Color.white;

        private float currentNormValue = 0f;
        private float targetValue = 0f;
        private Coroutine routine;

        public override event OnBarChangedCallback OnBarChanged;

        #endregion

        #region Properties

        public override MinMax Range {
            get => range;
            set {
                var norm = NormalizedValue;
                this.range = value;
                NormalizedValue = norm;
            }
        }

        public override float Value {
            get => value;
            set => SetValue(value);
        }

        public override float NormalizedValue { get => range.InverseEvaluate(value); set => SetValue(range.Evaluate(Mathf.Clamp01(value))); }
        public override bool Enabled { get => this.enabled; set => this.enabled = value; }
        public override Transform Parent => this.transform;

        public UnityEngine.UI.Image Foreground {
            get => foreground;
            set {
                foreground = value;
            }
        }

        public UnityEngine.UI.Image Background {
            get => background;
            set {
                background = value;
            }
        }

        #endregion

        #region Enable / Disable

        void OnEnable() {
            currentNormValue = range.InverseEvaluate(value);
            if(background)
                background.fillAmount = currentNormValue;
            if(foreground)
                foreground.fillAmount = currentNormValue;
        }

        void OnDisable() {
            Timer.Stop(routine);
            currentNormValue = range.InverseEvaluate(value);
            if(background)
                background.fillAmount = currentNormValue;
            if(foreground)
                foreground.fillAmount = currentNormValue;
        }

        #endregion

        #region SetValue

        public override void SetValue(float value) {
            if(this.value == value)
                return;
            var old = this.value;
            SetValueWithoutNotify(value);
            OnBarChanged?.Invoke(old, this.value);
        }

        public override void SetValueWithoutNotify(float value) {
            if(this.value == value)
                return;
            this.value = range.Clamp(value);
            this.targetValue = range.InverseEvaluate(this.value);

            if(Application.isPlaying)
                Timer.Once(delay, StartTransition, ref routine);
            else
                this.currentNormValue = this.targetValue;

            if(background) {
                background.fillAmount = Mathf.Max(this.currentNormValue, this.targetValue);
                if(useTint) {
                    background.color = this.targetValue > currentNormValue ? positiveTint : negativeTint;
                }
            }
            if(foreground)
                foreground.fillAmount = Mathf.Min(this.currentNormValue, this.targetValue);
        }

        #endregion

        #region Transition

        private void StartTransition() {
            Timer.Animate<float>(duration * Mathf.Abs(targetValue - currentNormValue), AnimateTransition, currentNormValue, ref routine);
        }

        private void AnimateTransition(float t, float currentValue) {
            var normVal = Mathf.Lerp(currentValue, targetValue, t);
            if(currentNormValue < targetValue) {
                if(foreground)
                    foreground.fillAmount = normVal;
            }
            else if(background)
                background.fillAmount = normVal;
            this.currentNormValue = normVal;
        }

        #endregion
    }
}
