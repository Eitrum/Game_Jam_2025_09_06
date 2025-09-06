using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DegToRadAttribute : PropertyAttribute
    {
        public enum Mode
        {
            Value,
            Slider,
            SliderStep,
        }

        #region Variables

        public float min;
        public float max;
        public float step;
        public Mode mode;

        #endregion

        #region Constructor

        public DegToRadAttribute() { mode = Mode.Value; }
        public DegToRadAttribute(float min, float max) {
            mode = Mode.Slider;
            this.min = min;
            this.max = max;
        }

        public DegToRadAttribute(float min, float max, float step) {
            mode = Mode.SliderStep;
            this.min = min;
            this.max = max;
            this.step = step;
        }

        public DegToRadAttribute(int min, int max) {
            mode = Mode.SliderStep;
            this.min = min;
            this.max = max;
            this.step = 1f;
        }

        #endregion
    }
}
