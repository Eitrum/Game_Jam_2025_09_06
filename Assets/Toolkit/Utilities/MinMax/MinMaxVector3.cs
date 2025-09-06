using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMaxVector3
    {
        public bool linear;
        public Vector3 min;
        public Vector3 max;

        public Vector3 Random => linear ? Vector3.Lerp(min, max, UnityEngine.Random.value) : new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));

        #region Constructor

        public MinMaxVector3(Vector3 min, Vector3 max) {
            this.min = min;
            this.max = max;
            this.linear = false;
        }

        public MinMaxVector3(float min, float max) {
            this.min = new Vector3(min, min, min);
            this.max = new Vector3(max, max, max);
            this.linear = false;
        }

        public MinMaxVector3(Vector3 min, Vector3 max, bool linear) {
            this.min = min;
            this.max = max;
            this.linear = linear;
        }

        public MinMaxVector3(float min, float max, bool linear) {
            this.min = new Vector3(min, min, min);
            this.max = new Vector3(max, max, max);
            this.linear = linear;
        }

        #endregion

        public Vector3 GetRandom() => Random;
        public Vector3 GetRandom(System.Random random) => linear ? Vector3.Lerp(min, max, random.NextFloat()) : new Vector3(Mathf.Lerp(min.x, max.x, random.NextFloat()), Mathf.Lerp(min.y, max.y, random.NextFloat()), Mathf.Lerp(min.z, max.z, random.NextFloat()));
        public Vector3 Clamp(Vector3 value) => new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
        public Vector3 EvaluateLinear(float t) => min + ((max - min) * t);
        public Vector3 LerpLinear(float t) => min + ((max - min) * t);
        public Vector3 Evaluate(float x, float y, float z) => new Vector3(min.x + ((max.x - min.x) * x), min.y + ((max.y - min.y) * y), min.z + ((max.z - min.z) * z));
        public Vector3 Lerp(float x, float y, float z) => new Vector3(min.x + ((max.x - min.x) * x), min.y + ((max.y - min.y) * y), min.z + ((max.z - min.z) * z));
        public bool Contains(Vector3 value) => min.x <= value.x && max.x >= value.x && min.y <= value.y && max.y >= value.y && min.z <= value.z && max.z >= value.z;

        public override string ToString() => $"([{min.x:#.##}, {min.y:#.##}, {min.z:#.##}] -> [{max.x:#.##}, {max.y:#.##}, {max.z:#.##}])";
    }
}
