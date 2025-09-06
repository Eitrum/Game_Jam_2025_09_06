using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMaxVector2
    {
        public bool linear;
        public Vector2 min;
        public Vector2 max;

        public Vector2 Random => linear ? Vector2.Lerp(min, max, UnityEngine.Random.value) : new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));

        #region Constructor

        public MinMaxVector2(Vector2 min, Vector2 max) {
            this.min = min;
            this.max = max;
            this.linear = false;
        }

        public MinMaxVector2(float min, float max) {
            this.min = new Vector2(min, min);
            this.max = new Vector2(max, max);
            this.linear = false;
        }

        public MinMaxVector2(Vector2 min, Vector2 max, bool linear) {
            this.min = min;
            this.max = max;
            this.linear = linear;
        }

        public MinMaxVector2(float min, float max, bool linear) {
            this.min = new Vector2(min, min);
            this.max = new Vector2(max, max);
            this.linear = linear;
        }

        #endregion

        public Vector2 GetRandom() => Random;
        public Vector2 GetRandom(System.Random random) => linear ? Vector2.Lerp(min, max, random.NextFloat()) : new Vector2(Mathf.Lerp(min.x, max.x, random.NextFloat()), Mathf.Lerp(min.y, max.y, random.NextFloat()));
        public Vector2 Clamp(Vector2 value) => new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
        public Vector2 EvaluateLinear(float t) => min + ((max - min) * t);
        public Vector2 LerpLinear(float t) => min + ((max - min) * t);
        public Vector2 Evaluate(float x, float y) => new Vector2(min.x + ((max.x - min.x) * x), min.y + ((max.y - min.y) * y));
        public Vector2 Lerp(float x, float y) => new Vector2(min.x + ((max.x - min.x) * x), min.y + ((max.y - min.y) * y));
        public bool Contains(Vector2 value) => min.x <= value.x && max.x >= value.x && min.y <= value.y && max.y >= value.y;

        public override string ToString() => $"([{min.x:#.##}, {min.y:#.##}] -> [{max.x:#.##}, {max.y:#.##}])";
    }
}
