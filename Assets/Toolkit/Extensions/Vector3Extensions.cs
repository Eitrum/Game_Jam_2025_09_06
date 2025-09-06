using System;
using UnityEngine;

namespace Toolkit {
    public static class Vector3Extensions {

        #region Convert

        public static Vector3 To_Vector3(this float value) => new Vector3(value, value, value);
        public static Vector3Int To_Vector3(this int value) => new Vector3Int(value, value, value);

        public static Vector3 To_XZ(this Vector3 vec) => new Vector3(vec.x, 0f, vec.z);
        public static Vector3 To_XZ(this Vector3 vec, out float y) {
            y = vec.y;
            return new Vector3(vec.x, 0f, vec.z);
        }

        public static Vector3 To_XY(this Vector3 vec) => new Vector3(vec.x, vec.y, 0f);
        public static Vector3 To_XY(this Vector3 vec, out float z) {
            z = vec.z;
            return new Vector3(vec.x, vec.y, 0f);
        }

        public static Vector3 To_YZ(this Vector3 vec) => new Vector3(0f, vec.y, vec.z);
        public static Vector3 To_YZ(this Vector3 vec, out float x) {
            x = vec.x;
            return new Vector3(0f, vec.y, vec.z);
        }

        public static Vector3Int To_Int(this Vector3 vec) => new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));

        public static Vector2 To_Vector2_XY(this Vector3 vec) => new Vector2(vec.x, vec.y);
        public static Vector2 To_Vector2_XZ(this Vector3 vec) => new Vector2(vec.x, vec.z);
        public static Vector2 To_Vector2_YZ(this Vector3 vec) => new Vector2(vec.y, vec.z);

        public static Vector2 To_Vector2_XY(this Vector3 vec, out float z) {
            z = vec.z;
            return new Vector2(vec.x, vec.y);
        }
        public static Vector2 To_Vector2_XZ(this Vector3 vec, out float y) {
            y = vec.y;
            return new Vector2(vec.x, vec.z);
        }
        public static Vector2 To_Vector2_YZ(this Vector3 vec, out float x) {
            x = vec.x;
            return new Vector2(vec.y, vec.z);
        }

        public static Vector4 To_Vector4(this Vector3 vec) => new Vector4(vec.x, vec.y, vec.z);
        public static Vector4 To_Vector4(this Vector3 vec, float w) => new Vector4(vec.x, vec.y, vec.z, w);

        #endregion

        #region Normalizing Clamping

        public static Vector3 ClampMagnitude(this Vector3 vec, float maxLength) {
            return Vector3.ClampMagnitude(vec, maxLength);
        }

        public static Vector3 ClampMagnitude(this Vector3 vec, float minLength, float maxLength) {
            var length = vec.magnitude;
            if(length < minLength || length > maxLength)
                return vec / length * Mathf.Clamp(length, minLength, maxLength);
            return vec;
        }

        public static Vector3 ClampMagnitude_XZ(this Vector3 vec, float maxLength) {
            var y = vec.y;
            vec.y = 0f;
            vec = Vector3.ClampMagnitude(vec, maxLength);
            vec.y = y;
            return vec;
        }

        #endregion

        #region Snap

        public static Vector3 Snap(this Vector3 vector) {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }

        public static Vector3 Snap(this Vector3 vector, float snapValue) {
            if(snapValue == 0f) {
                return vector;
            }
            return Snap(vector / snapValue) * snapValue;
        }

        public static Vector3 Snap(this Vector3 vector, float snapValue, Transform relativeTo) {
            if(snapValue.Equals(0f, 0.001f))
                return vector;
            if(relativeTo == null)
                return vector.Snap(snapValue);
            return relativeTo.TransformPoint(relativeTo.InverseTransformPoint(vector).Snap(snapValue));
        }

        public static Vector3 Snap(this Vector3 vector, Transform relativeTo) {
            if(relativeTo == null)
                return vector.Snap();
            return relativeTo.TransformPoint(relativeTo.InverseTransformPoint(vector).Snap());
        }

        #endregion

        #region Mirror

        public static Vector3 Mirror(this Vector3 input, Pose mirror) {
            var norm = (mirror.rotation) * new Vector3(0, 0, 1);
            return mirror.position + Vector3.Reflect(input - mirror.position, norm);
        }

        #endregion

        #region Multiply

        public static Vector3 Multiply(this Vector3 vec, Vector3 other) {
            return new Vector3(vec.x * other.x, vec.y * other.y, vec.z * other.z);
        }

        public static Vector3 Multiply(this Vector3 vec, float x, float y, float z) {
            return new Vector3(vec.x * x, vec.y * y, vec.z * z);
        }

        #endregion

        #region InRange

        public static bool IsInRange(this Vector3 vec, Vector3 other, float range) => Vector3.Distance(vec, other) <= range;
        public static bool IsInRange(this Vector3 vec, Vector3 other, float maxRange, out float distance) => (distance = Vector3.Distance(vec, other)) <= maxRange;

        #endregion

        #region Equals

        public static bool Equals(this Vector3 value, Vector3 otherValue, float proximity) {
            return value.x + proximity >= otherValue.x && value.x - proximity <= otherValue.x &&
                value.y + proximity >= otherValue.y && value.y - proximity <= otherValue.y &&
                value.z + proximity >= otherValue.z && value.z - proximity <= otherValue.z;
        }

        #endregion

        #region Math

        public static Vector3 Min(this Vector3 vec, Vector3 otherVec) {
            vec.x = Mathf.Min(vec.x, otherVec.x);
            vec.y = Mathf.Min(vec.y, otherVec.y);
            vec.z = Mathf.Min(vec.z, otherVec.z);
            return vec;
        }

        public static Vector3 Min(this Vector3 vec, float x, float y, float z) {
            vec.x = Mathf.Min(vec.x, x);
            vec.y = Mathf.Min(vec.y, y);
            vec.z = Mathf.Min(vec.z, z);
            return vec;
        }

        public static Vector3 Max(this Vector3 vec, Vector3 otherVec) {
            vec.x = Mathf.Max(vec.x, otherVec.x);
            vec.y = Mathf.Max(vec.y, otherVec.y);
            vec.z = Mathf.Max(vec.z, otherVec.z);
            return vec;
        }

        public static Vector3 Max(this Vector3 vec, float x, float y, float z) {
            vec.x = Mathf.Max(vec.x, x);
            vec.y = Mathf.Max(vec.y, y);
            vec.z = Mathf.Max(vec.z, z);
            return vec;
        }

        public static Vector3 Clamp(this Vector3 vec, Vector3 min, Vector3 max) {
            vec.x = Mathf.Clamp(vec.x, min.x, max.x);
            vec.y = Mathf.Clamp(vec.y, min.y, max.y);
            vec.z = Mathf.Clamp(vec.z, min.z, max.z);
            return vec;
        }

        public static Vector3 Clamp(this Vector3 vec, float min, float max) {
            vec.x = Mathf.Clamp(vec.x, min, max);
            vec.y = Mathf.Clamp(vec.y, min, max);
            vec.z = Mathf.Clamp(vec.z, min, max);
            return vec;
        }

        public static Vector3 Clamp(this Vector3 vec, float minX, float maxX, float minY, float maxY, float minZ, float maxZ) {
            vec.x = Mathf.Clamp(vec.x, minX, maxX);
            vec.y = Mathf.Clamp(vec.y, minY, maxY);
            vec.z = Mathf.Clamp(vec.z, minZ, maxZ);
            return vec;
        }

        public static Vector3 Abs(this Vector3 vec) => new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));

        public static Vector3 Divide(this Vector3 vec, float value)
            => vec / value;

        public static Vector3 Divide(this Vector3 vec, Vector3 other) {
            return new Vector3(
                vec.x / other.x,
                vec.y / other.y,
                vec.z / other.z);
        }

        #endregion

        #region Highest / Lowest

        public static float Highest(this Vector3 vec) {
            float val = float.MinValue;
            for(int i = 0; i < 3; i++)
                if(vec[i] > val)
                    val = vec[i];
            return val;
        }

        public static float Highest(this Vector3 vec, out int index) {
            float val = float.MinValue;
            index = 0;
            for(int i = 0; i < 3; i++)
                if(vec[i] > val) {
                    val = vec[i];
                    index = i;
                }
            return val;
        }

        public static float Lowest(this Vector3 vec) {
            float val = float.MaxValue;
            for(int i = 0; i < 3; i++)
                if(vec[i] < val)
                    val = vec[i];
            return val;
        }

        public static float Lowest(this Vector3 vec, out int index) {
            float val = float.MaxValue;
            index = 0;
            for(int i = 0; i < 3; i++)
                if(vec[i] < val) {
                    val = vec[i];
                    index = i;
                }
            return val;
        }

        #endregion

        #region Rounding

        public static Vector3 Round(this Vector3 vec) => new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
        public static Vector3Int RoundToInt(this Vector3 vec) => new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));

        public static Vector3 Floor(this Vector3 vec) => new Vector3(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z));
        public static Vector3Int FloorToInt(this Vector3 vec) => new Vector3Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y), Mathf.FloorToInt(vec.z));

        public static Vector3 Ceil(this Vector3 vec) => new Vector3(Mathf.Ceil(vec.x), Mathf.Ceil(vec.y), Mathf.Ceil(vec.z));
        public static Vector3Int CeilToInt(this Vector3 vec) => new Vector3Int(Mathf.CeilToInt(vec.x), Mathf.CeilToInt(vec.y), Mathf.CeilToInt(vec.z));

        #endregion

        #region Average

        public static float Average(this Vector3 v) => (v.x + v.y + v.z) / 3f;

        public static Vector3 Average(this Vector3 v, Vector3 other) => new Vector3(
            (v.x + other.x) / 2f,
            (v.y + other.y) / 2f,
            (v.z + other.z) / 2f);

        #endregion
    }
}
