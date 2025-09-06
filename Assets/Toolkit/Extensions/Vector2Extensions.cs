using System;
using UnityEngine;

namespace Toolkit
{
    public static class Vector2Extensions
    {

        #region Convert

        public static Vector2 To_Vector2(this float v) => new Vector2(v, v);
        public static Vector2Int To_Vector2(this int v) => new Vector2Int(v, v);

        public static Vector3 ToVector3_XY(this Vector2 v) => new Vector3(v.x, v.y, 0f);
        public static Vector3 ToVector3_XZ(this Vector2 v) => new Vector3(v.x, 0f, v.y);
        public static Vector3 ToVector3_YZ(this Vector2 v) => new Vector3(0f, v.x, v.y);

        public static Vector3 ToVector3_XY(this Vector2 v, float z) => new Vector3(v.x, v.y, z);
        public static Vector3 ToVector3_XZ(this Vector2 v, float y) => new Vector3(v.x, y, v.y);
        public static Vector3 ToVector3_YZ(this Vector2 v, float x) => new Vector3(x, v.x, v.y);

        public static Vector2Int To_Int(this Vector2 v) => new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        #endregion

        #region Clamp

        public static Vector2 ClampMagnitude(this Vector2 vec, float maxLength) {
            return Vector2.ClampMagnitude(vec, maxLength);
        }

        public static Vector2 ClampMagnitude(this Vector2 vec, float minLength, float maxLength) {
            var length = vec.magnitude;
            if(length < minLength || length > maxLength)
                return (vec / length) * Mathf.Clamp(length, minLength, maxLength);
            return vec;
        }

        #endregion

        #region Snap

        public static Vector2 Snap(this Vector2 v)
            => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));

        public static Vector2 Snap(this Vector2 v, float snapValue) {
            if(snapValue == 0f) {
                return v;
            }
            return Snap(v / snapValue) * snapValue;
        }

        #endregion

        #region Multiply

        public static Vector2 Multiply(this Vector2 vec, Vector2 other) {
            return new Vector2(vec.x * other.x, vec.y * other.y);
        }

        public static Vector2 Multiply(this Vector2 vec, float x, float y) {
            return new Vector2(vec.x * x, vec.y * y);
        }

        #endregion

        #region Min

        public static Vector2 Min(this Vector2 vec, Vector2 otherVec) {
            vec.x = Mathf.Min(vec.x, otherVec.x);
            vec.y = Mathf.Min(vec.y, otherVec.y);
            return vec;
        }

        public static Vector2 Min(this Vector2 vec, float x, float y) {
            vec.x = Mathf.Min(vec.x, x);
            vec.y = Mathf.Min(vec.y, y);
            return vec;
        }

        public static void MinRef(this ref Vector2 vec, Vector2 otherVec) {
            vec.x = Mathf.Min(vec.x, otherVec.x);
            vec.y = Mathf.Min(vec.y, otherVec.y);
        }

        public static void MinRef(this ref Vector2 vec, float x, float y) {
            vec.x = Mathf.Min(vec.x, x);
            vec.y = Mathf.Min(vec.y, y);
        }

        #endregion

        #region Max

        public static Vector2 Max(this Vector2 vec, Vector2 otherVec) {
            vec.x = Mathf.Max(vec.x, otherVec.x);
            vec.y = Mathf.Max(vec.y, otherVec.y);
            return vec;
        }

        public static Vector2 Max(this Vector2 vec, float x, float y) {
            vec.x = Mathf.Max(vec.x, x);
            vec.y = Mathf.Max(vec.y, y);
            return vec;
        }

        public static void MaxRef(this ref Vector2 vec, Vector2 otherVec) {
            vec.x = Mathf.Max(vec.x, otherVec.x);
            vec.y = Mathf.Max(vec.y, otherVec.y);
        }

        public static void MaxRef(this ref Vector2 vec, float x, float y) {
            vec.x = Mathf.Max(vec.x, x);
            vec.y = Mathf.Max(vec.y, y);
        }

        #endregion

        #region Clamp

        public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max) {
            vec.x = Mathf.Clamp(vec.x, min.x, max.x);
            vec.y = Mathf.Clamp(vec.y, min.y, max.y);
            return vec;
        }

        public static Vector2 Clamp(this Vector2 vec, float min, float max) {
            vec.x = Mathf.Clamp(vec.x, min, max);
            vec.y = Mathf.Clamp(vec.y, min, max);
            return vec;
        }

        public static Vector2 Clamp(this Vector2 vec, float minX, float maxX, float minY, float maxY) {
            vec.x = Mathf.Clamp(vec.x, minX, maxX);
            vec.y = Mathf.Clamp(vec.y, minY, maxY);
            return vec;
        }

        #endregion

        #region Equals

        public static bool Equals(this Vector2 value, Vector2 otherValue, float proximity) {
            return value.x + proximity >= otherValue.x && value.x - proximity <= otherValue.x &&
                value.y + proximity >= otherValue.y && value.y - proximity <= otherValue.y;
        }

        #endregion

        #region Abs

        public static Vector2 Abs(this Vector2 vec) => new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));

        #endregion

        #region Rounding

        public static Vector2 Round(this Vector2 vec) => new Vector2(Mathf.Round(vec.x), Mathf.Round(vec.y));
        public static Vector2Int RoundToInt(this Vector2 vec) => new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));

        public static Vector2 Floor(this Vector2 vec) => new Vector2(Mathf.Floor(vec.x), Mathf.Floor(vec.y));
        public static Vector2Int FloorToInt(this Vector2 vec) => new Vector2Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));

        public static Vector2 Ceil(this Vector2 vec) => new Vector2(Mathf.Ceil(vec.x), Mathf.Ceil(vec.y));
        public static Vector2Int CeilToInt(this Vector2 vec) => new Vector2Int(Mathf.CeilToInt(vec.x), Mathf.CeilToInt(vec.y));

        #endregion

        #region Average

        public static float Average(this Vector2 v) => (v.x + v.y) / 2f;

        public static Vector2 Average(this Vector2 v, Vector2 other) => new Vector2(
            (v.x + other.x) / 2f,
            (v.y + other.x) / 2f);

        #endregion
    }
}
