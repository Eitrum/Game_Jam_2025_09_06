using System;
using UnityEngine;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PowerOfTwoAttribute : PropertyAttribute
    {
        #region Variables

        public float min = 0;
        public float max = 31;

        #endregion

        #region Constructor

        public PowerOfTwoAttribute() {
            min = 0;
            max = 31;
        }

        public PowerOfTwoAttribute(float min, float max) {
            this.min = min;
            this.max = max;
        }

        public PowerOfTwoAttribute(int min, int max) {
            this.min = min;
            this.max = max;
        }

        #endregion
    }
}
