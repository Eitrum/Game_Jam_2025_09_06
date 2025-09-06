using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RangeExAttribute : PropertyAttribute
    {
        public const float DEFAULT_STEP = 0.1f;

        #region Properties

        public float Min { get; private set; }
        public float Max { get; private set; }
        public float Step { get; private set; }

        #endregion

        #region Constructor

        public RangeExAttribute(float min, float max) {
            if(min > max) {
                Min = max;
                Max = min;
            }
            else {
                Min = min;
                Max = max;
            }

            Step = DEFAULT_STEP;
        }

        public RangeExAttribute(float min, float max, float steps) {
            if(min > max) {
                Min = max;
                Max = min;
            }
            else {
                Min = min;
                Max = max;
            }

            if(steps < 0f)
                Step = DEFAULT_STEP;
            else
                Step = steps;
        }

        #endregion
    }
}
