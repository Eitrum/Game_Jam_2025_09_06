using System;
using UnityEngine;

namespace Toolkit
{
    [Serializable]
    public struct MinMaxVector4
    {
        public bool linear;
        public Vector4 min;
        public Vector4 max;

        public Vector4 Random => linear ? Vector4.Lerp(min, max, UnityEngine.Random.value) : new Vector4(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z), UnityEngine.Random.Range(min.w, max.w));

        #region Constructor

        public MinMaxVector4(Vector4 min, Vector4 max) {
            this.min = min;
            this.max = max;
            this.linear = false;
        }

        public MinMaxVector4(float min, float max) {
            this.min = new Vector4(min, min, min, min);
            this.max = new Vector4(max, max, max, max);
            this.linear = false;
        }

        public MinMaxVector4(Vector4 min, Vector4 max, bool linear) {
            this.min = min;
            this.max = max;
            this.linear = linear;
        }

        public MinMaxVector4(float min, float max, bool linear) {
            this.min = new Vector4(min, min, min, min);
            this.max = new Vector4(max, max, max, max);
            this.linear = linear;
        }

        #endregion

        public Vector4 GetRandom() => Random;
        public Vector4 GetRandom(System.Random random) => linear ? Vector4.Lerp(min, max, random.NextFloat()) : new Vector4(Mathf.Lerp(min.x, max.x, random.NextFloat()), Mathf.Lerp(min.y, max.y, random.NextFloat()), Mathf.Lerp(min.z, max.z, random.NextFloat()), Mathf.Lerp(min.w, max.w, random.NextFloat()));
        public Vector4 Clamp(Vector4 value) => new Vector4(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z), Mathf.Clamp(value.w, min.w, max.w));
        public Vector4 EvaluateLinear(float t) => min + ((max - min) * t);
        public Vector4 LerpLinear(float t) => min + ((max - min) * t);
        public Vector4 Evaluate(float x, float y, float z, float w) => new Vector4(min.x + ((max.x - min.x) * x), min.y + ((max.y - min.y) * y), min.z + ((max.z - min.z) * z), min.w + ((max.w - min.w) * w));
        public Vector4 Lerp(float x, float y, float z, float w) => new Vector4(min.x + ((max.x - min.x) * x), min.y + ((max.y - min.y) * y), min.z + ((max.z - min.z) * z), min.w + ((max.w - min.w) * w));
        public bool Contains(Vector4 value) => min.x <= value.x && max.x >= value.x && min.y <= value.y && max.y >= value.y && min.z <= value.z && max.z >= value.z && min.w <= value.w && max.w >= value.w;

        public override string ToString() => $"([{min.x:#.##}, {min.y:#.##}, {min.z:#.##}, {min.w:#.##}] -> [{max.x:#.##}, {max.y:#.##}, {max.z:#.##}, {max.w:#.##}])";
    }
}
