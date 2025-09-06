using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2Long
    {
        #region Variables

        public long x;
        public long y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2Long zero => new Vector2Long(0, 0);
        public static Vector2Long one => new Vector2Long(1, 1);
        public static Vector2Long left => new Vector2Long(-1, 0);
        public static Vector2Long right => new Vector2Long(1, 0);
        public static Vector2Long up => new Vector2Long(0, 1);
        public static Vector2Long down => new Vector2Long(0, -1);

        public static Vector2Long MIN => new Vector2Long(long.MinValue, long.MinValue);
        public static Vector2Long MAX => new Vector2Long(long.MaxValue, long.MaxValue);

        #endregion

        #region Constructor

        public Vector2Long(long x, long y) {
            this.x = x;
            this.y = y;
        }

        public Vector2Long(int x, int y) {
            this.x = (long)x;
            this.y = (long)y;
        }

        public Vector2Long(float x, float y) {
            this.x = (long)x;
            this.y = (long)y;
        }

        public Vector2Long(Vector2Int vecInt) {
            this.x = (long)vecInt.x;
            this.y = (long)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(long x, long y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y) {
            this.x = (long)x;
            this.y = (long)y;
        }

        public void Scale(Vector2UInt other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Clamp(Vector2Long min, Vector2Long max) {
            this.x = this.x > max.x ? max.x : (this.x < min.x ? min.x : x);
            this.y = this.y > max.y ? max.y : (this.y < min.y ? min.y : y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = Mathf.Clamp((int)this.x, min.x, max.x);
            this.y = Mathf.Clamp((int)this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static double Distance(Vector2Long lhs, Vector2Long rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Math.Sqrt(x * x + y * y);
        }

        public static float DistanceFloat(Vector2Long lhs, Vector2Long rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return (float)Math.Sqrt(x * x + y * y);
        }

        public static Vector2Long Max(Vector2Long lhs, Vector2Long rhs) {
            return new Vector2Long(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2Long Min(Vector2Long lhs, Vector2Long rhs) {
            return new Vector2Long(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2Long Round(Vector2 vector) {
            return new Vector2Long(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2Long Floor(Vector2 vector) {
            return new Vector2Long(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2Long Ceil(Vector2 vector) {
            return new Vector2Long(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        #endregion

        #region Operations

        public static Vector2Long operator +(Vector2Long lhs, Vector2Long rhs) => new Vector2Long(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2Long operator -(Vector2Long lhs, Vector2Long rhs) => new Vector2Long(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2Long operator *(Vector2Long lhs, Vector2Long rhs) => new Vector2Long(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2Long operator /(Vector2Long lhs, Vector2Long rhs) => new Vector2Long(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2Long operator *(Vector2Long lhs, int rhs) => new Vector2Long(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Long operator /(Vector2Long lhs, int rhs) => new Vector2Long(lhs.x / rhs, lhs.x / rhs);

        public static Vector2Long operator *(Vector2Long lhs, float rhs) => new Vector2Long(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Long operator /(Vector2Long lhs, float rhs) => new Vector2Long(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2Long value) => new Vector2Int((int)value.x, (int)value.y);
        public static implicit operator Vector2Long(Vector2Int value) => new Vector2Long(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2Long value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2Long(Vector2 value) => new Vector2Long(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2Long value) => new Vector2Byte(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2Long value) => new Vector2SByte(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2Long value) => new Vector2UShort(value.x, value.y);
        public static implicit operator Vector2Short(Vector2Long value) => new Vector2Short(value.x, value.y);
        public static implicit operator Vector2UInt(Vector2Long value) => new Vector2UInt(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2Long other) {
                return x == other.x && y == other.y;
            }
            return false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return $"({x}, {y})";
        }

        #endregion
    }
}
