using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMaxCurve
    {
        [SerializeField] AnimationCurve min;
        [SerializeField] AnimationCurve max;

        public float Random {
            get {
                var t = UnityEngine.Random.value;
                return Mathf.Lerp(min.Evaluate(t), max.Evaluate(t), UnityEngine.Random.value);
            }
        }

        public MinMaxCurve(AnimationCurve min, AnimationCurve max) {
            this.min = min;
            this.max = max;
        }

        public float RandomAt(float curveT) => Mathf.LerpUnclamped(min.Evaluate(curveT), max.Evaluate(curveT), UnityEngine.Random.value);
        public float Evaluate(float curveT, float t) => Mathf.LerpUnclamped(min.Evaluate(curveT), max.Evaluate(curveT), t);
        public bool Contains(float curveT, float value) {
            var min = this.min.Evaluate(curveT);
            var max = this.max.Evaluate(curveT);

            if(min > max) {
                var temp = max;
                max = min;
                min = temp;
            }
            return min <= value && max >= value;
        }
    }
}
