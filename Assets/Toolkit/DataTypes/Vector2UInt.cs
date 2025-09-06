using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2UInt
    {
        #region Variables

        public uint x;
        public uint y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2UInt zero => new Vector2UInt(0, 0);
        public static Vector2UInt one => new Vector2UInt(1, 1);
        public static Vector2UInt right => new Vector2UInt(1, 0);
        public static Vector2UInt up => new Vector2UInt(0, 1);

        public static Vector2UInt MIN => new Vector2UInt(uint.MinValue, uint.MinValue);
        public static Vector2UInt MAX => new Vector2UInt(uint.MaxValue, uint.MaxValue);

        #endregion

        #region Constructor

        public Vector2UInt(uint x, uint y) {
            this.x = x;
            this.y = y;
        }

        public Vector2UInt(int x, int y) {
            this.x = (uint)x;
            this.y = (uint)y;
        }

        public Vector2UInt(float x, float y) {
            this.x = (uint)x;
            this.y = (uint)y;
        }

        public Vector2UInt(Vector2Int vecInt) {
            this.x = (uint)vecInt.x;
            this.y = (uint)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(uint x, uint y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = (uint)x;
            this.y = (uint)y;
        }

        public void Set(float x, float y) {
            this.x = (uint)x;
            this.y = (uint)y;
        }

        public void Scale(Vector2UInt other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= (uint)other.x;
            this.y *= (uint)other.y;
        }

        public void Clamp(Vector2UInt min, Vector2UInt max) {
            this.x = (uint)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (uint)Mathf.Clamp(this.y, min.y, max.y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = (uint)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (uint)Mathf.Clamp(this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static float Distance(Vector2UInt lhs, Vector2UInt rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        public static Vector2UInt Max(Vector2UInt lhs, Vector2UInt rhs) {
            return new Vector2UInt(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2UInt Min(Vector2UInt lhs, Vector2UInt rhs) {
            return new Vector2UInt(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2UInt Round(Vector2 vector) {
            return new Vector2UInt(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2UInt Floor(Vector2 vector) {
            return new Vector2UInt(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2UInt Ceil(Vector2 vector) {
            return new Vector2UInt(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        #endregion

        #region Operations

        public static Vector2UInt operator +(Vector2UInt lhs, Vector2UInt rhs) => new Vector2UInt(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2UInt operator -(Vector2UInt lhs, Vector2UInt rhs) => new Vector2UInt(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2UInt operator *(Vector2UInt lhs, Vector2UInt rhs) => new Vector2UInt(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2UInt operator /(Vector2UInt lhs, Vector2UInt rhs) => new Vector2UInt(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2UInt operator *(Vector2UInt lhs, int rhs) => new Vector2UInt(lhs.x * rhs, lhs.x * rhs);
        public static Vector2UInt operator /(Vector2UInt lhs, int rhs) => new Vector2UInt(lhs.x / rhs, lhs.x / rhs);

        public static Vector2UInt operator *(Vector2UInt lhs, float rhs) => new Vector2UInt(lhs.x * rhs, lhs.x * rhs);
        public static Vector2UInt operator /(Vector2UInt lhs, float rhs) => new Vector2UInt(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2UInt value) => new Vector2Int((int)value.x, (int)value.y);
        public static implicit operator Vector2UInt(Vector2Int value) => new Vector2UInt(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2UInt value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2UInt(Vector2 value) => new Vector2UInt(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2UInt value) => new Vector2Byte(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2UInt value) => new Vector2SByte(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2UInt value) => new Vector2UShort(value.x, value.y);
        public static implicit operator Vector2Short(Vector2UInt value) => new Vector2Short(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2UInt other) {
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
