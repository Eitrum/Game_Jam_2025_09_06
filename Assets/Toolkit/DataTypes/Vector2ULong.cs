using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2ULong
    {
        #region Variables

        public ulong x;
        public ulong y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2ULong zero => new Vector2ULong(0, 0);
        public static Vector2ULong one => new Vector2ULong(1, 1);
        public static Vector2ULong right => new Vector2ULong(1, 0);
        public static Vector2ULong up => new Vector2ULong(0, 1);

        public static Vector2ULong MIN => new Vector2ULong(ulong.MinValue, ulong.MinValue);
        public static Vector2ULong MAX => new Vector2ULong(ulong.MaxValue, ulong.MaxValue);

        #endregion

        #region Constructor

        public Vector2ULong(ulong x, ulong y) {
            this.x = x;
            this.y = y;
        }

        public Vector2ULong(int x, int y) {
            this.x = (ulong)x;
            this.y = (ulong)y;
        }

        public Vector2ULong(float x, float y) {
            this.x = (ulong)x;
            this.y = (ulong)y;
        }

        public Vector2ULong(Vector2Int vecInt) {
            this.x = (ulong)vecInt.x;
            this.y = (ulong)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(ulong x, ulong y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = (ulong)x;
            this.y = (ulong)y;
        }

        public void Set(float x, float y) {
            this.x = (ulong)x;
            this.y = (ulong)y;
        }

        public void Scale(Vector2ULong other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= (ulong)other.x;
            this.y *= (ulong)other.y;
        }

        public void Clamp(Vector2ULong min, Vector2ULong max) {
            this.x = this.x > max.x ? max.x : (this.x < min.x ? min.x : x);
            this.y = this.y > max.y ? max.y : (this.y < min.y ? min.y : y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = (ulong)Mathf.Clamp((int)this.x, min.x, max.x);
            this.y = (ulong)Mathf.Clamp((int)this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static double Distance(Vector2ULong lhs, Vector2ULong rhs) {
            var x = rhs.x - (double)lhs.x;
            var y = rhs.y - (double)lhs.y;
            return Math.Sqrt(x * x + y * y);
        }

        public static float DistanceFloat(Vector2ULong lhs, Vector2ULong rhs) {
            var x = rhs.x - (double)lhs.x;
            var y = rhs.y - (double)lhs.y;
            return (float)Math.Sqrt(x * x + y * y);
        }

        public static Vector2ULong Max(Vector2ULong lhs, Vector2ULong rhs) {
            return new Vector2ULong(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2ULong Min(Vector2ULong lhs, Vector2ULong rhs) {
            return new Vector2ULong(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2ULong Round(Vector2 vector) {
            return new Vector2ULong(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2ULong Floor(Vector2 vector) {
            return new Vector2ULong(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2ULong Ceil(Vector2 vector) {
            return new Vector2ULong(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        #endregion

        #region Operations

        public static Vector2ULong operator +(Vector2ULong lhs, Vector2ULong rhs) => new Vector2ULong(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2ULong operator -(Vector2ULong lhs, Vector2ULong rhs) => new Vector2ULong(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2ULong operator *(Vector2ULong lhs, Vector2ULong rhs) => new Vector2ULong(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2ULong operator /(Vector2ULong lhs, Vector2ULong rhs) => new Vector2ULong(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2ULong operator *(Vector2ULong lhs, int rhs) => new Vector2ULong(lhs.x * (ulong)rhs, lhs.x * (ulong)rhs);
        public static Vector2ULong operator /(Vector2ULong lhs, int rhs) => new Vector2ULong(lhs.x / (ulong)rhs, lhs.x / (ulong)rhs);

        public static Vector2ULong operator *(Vector2ULong lhs, float rhs) => new Vector2ULong(lhs.x * rhs, lhs.x * rhs);
        public static Vector2ULong operator /(Vector2ULong lhs, float rhs) => new Vector2ULong(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2ULong value) => new Vector2Int((int)value.x, (int)value.y);
        public static implicit operator Vector2ULong(Vector2Int value) => new Vector2ULong(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2ULong value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2ULong(Vector2 value) => new Vector2ULong(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2ULong value) => new Vector2Byte(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2ULong value) => new Vector2SByte(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2ULong value) => new Vector2UShort(value.x, value.y);
        public static implicit operator Vector2Short(Vector2ULong value) => new Vector2Short(value.x, value.y);
        public static implicit operator Vector2UInt(Vector2ULong value) => new Vector2UInt(value.x, value.y);
        public static implicit operator Vector2Long(Vector2ULong value) => new Vector2Long(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2ULong other) {
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
