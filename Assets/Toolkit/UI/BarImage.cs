using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI
{
    [AddComponentMenu("Toolkit/UI/Bar (Image)")]
    public sealed class BarImage : Bar, IBar
    {
        #region Variables

        [SerializeField] private float value = 0f;
        [SerializeField] private MinMax range = new MinMax(0f, 1f);
        [SerializeField] private UnityEngine.UI.Image image = null;
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
        public UnityEngine.UI.Image Image {
            get => image;
            set {
                image = value;
            }
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
            if(image)
                image.fillAmount = range.InverseEvaluate(this.value);
        }

        #endregion
    }
}
