using System;
using UnityEngine;

namespace Toolkit
{
    public static class Vector4Extensions
    {

        public static Quaternion To_Quaternion(this Vector4 v)
            => new Quaternion(v.x, v.y, v.z, v.w);

        public static Vector4 Multiply(this Vector4 vec, Vector4 otherVec) {
            return new Vector4(
                vec.x * otherVec.x,
                vec.y * otherVec.y,
                vec.z * otherVec.z,
                vec.w * otherVec.w);
        }

        public static Vector4 To_Vector4(this float value) => new Vector4(value, value, value, value);

        public static float Average(this Vector4 v) => (v.x + v.y + v.z + v.w) / 4f;

        public static Vector4 Average(this Vector4 v, Vector4 other) => new Vector4(
            (v.x + other.x) / 2f,
            (v.y + other.y) / 2f,
            (v.z + other.z) / 2f,
            (v.w + other.w) / 2f);
    }
}
