using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMaxGradient
    {
        public Gradient min;
        public Gradient max;

        public Color Random {
            get {
                var t = UnityEngine.Random.value;
                return Color.Lerp(min.Evaluate(t), max.Evaluate(t), UnityEngine.Random.value);
            }
        }

        public MinMaxGradient(Gradient min, Gradient max) {
            this.min = min;
            this.max = max;
        }

        public Color Evaluate(float gradientT, float t) => Color.LerpUnclamped(min.Evaluate(gradientT), max.Evaluate(gradientT), t);
    }
}
