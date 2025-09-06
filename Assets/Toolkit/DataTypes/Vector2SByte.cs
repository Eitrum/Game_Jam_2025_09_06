using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2SByte
    {
        #region Variables

        public sbyte x;
        public sbyte y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2SByte zero => new Vector2SByte(0, 0);
        public static Vector2SByte one => new Vector2SByte(1, 1);
        public static Vector2SByte left => new Vector2SByte(-1, 0);
        public static Vector2SByte right => new Vector2SByte(1, 0);
        public static Vector2SByte up => new Vector2SByte(1, 0);
        public static Vector2SByte down => new Vector2SByte(-1, 0);

        public static Vector2SByte MIN => new Vector2SByte(sbyte.MinValue, sbyte.MinValue);
        public static Vector2SByte MAX => new Vector2SByte(sbyte.MaxValue, sbyte.MaxValue);

        #endregion

        #region Constructor

        public Vector2SByte(sbyte x, sbyte y) {
            this.x = x;
            this.y = y;
        }

        public Vector2SByte(int x, int y) {
            this.x = (sbyte)x;
            this.y = (sbyte)y;
        }

        public Vector2SByte(float x, float y) {
            this.x = (sbyte)x;
            this.y = (sbyte)y;
        }

        public Vector2SByte(Vector2Int vecInt) {
            this.x = (sbyte)vecInt.x;
            this.y = (sbyte)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(sbyte x, sbyte y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = (sbyte)x;
            this.y = (sbyte)y;
        }

        public void Set(float x, float y) {
            this.x = (sbyte)x;
            this.y = (sbyte)y;
        }

        public void Scale(Vector2SByte other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= (sbyte)other.x;
            this.y *= (sbyte)other.y;
        }

        public void Clamp(Vector2SByte min, Vector2SByte max) {
            this.x = (sbyte)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (sbyte)Mathf.Clamp(this.y, min.y, max.y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = (sbyte)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (sbyte)Mathf.Clamp(this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static float Distance(Vector2SByte lhs, Vector2SByte rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        public static Vector2SByte Max(Vector2SByte lhs, Vector2SByte rhs) {
            return new Vector2SByte(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2SByte Min(Vector2SByte lhs, Vector2SByte rhs) {
            return new Vector2SByte(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2SByte Round(Vector2 vector) {
            return new Vector2SByte(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2SByte Floor(Vector2 vector) {
            return new Vector2SByte(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2SByte Ceil(Vector2 vector) {
            return new Vector2SByte(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        public static Vector2SByte FromHashCode(int value) {
            return new Vector2SByte(value & 0xff, (value >> 8) & 0xff);
        }

        #endregion

        #region Operations

        public static Vector2SByte operator +(Vector2SByte lhs, Vector2SByte rhs) => new Vector2SByte(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2SByte operator -(Vector2SByte lhs, Vector2SByte rhs) => new Vector2SByte(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2SByte operator *(Vector2SByte lhs, Vector2SByte rhs) => new Vector2SByte(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2SByte operator /(Vector2SByte lhs, Vector2SByte rhs) => new Vector2SByte(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2SByte operator *(Vector2SByte lhs, int rhs) => new Vector2SByte(lhs.x * rhs, lhs.x * rhs);
        public static Vector2SByte operator /(Vector2SByte lhs, int rhs) => new Vector2SByte(lhs.x / rhs, lhs.x / rhs);

        public static Vector2SByte operator *(Vector2SByte lhs, float rhs) => new Vector2SByte(lhs.x * rhs, lhs.x * rhs);
        public static Vector2SByte operator /(Vector2SByte lhs, float rhs) => new Vector2SByte(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2SByte value) => new Vector2Int(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2Int value) => new Vector2SByte(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2SByte value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2 value) => new Vector2SByte(value.x, value.y);
        // To custom
        public static implicit operator Vector2Short(Vector2SByte value) => new Vector2Short(value.x, value.y);
        public static implicit operator Vector2Byte(Vector2SByte value) => new Vector2Byte(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2SByte value) => new Vector2UShort(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2SByte other) {
                return x == other.x && y == other.y;
            }
            return false;
        }

        public override int GetHashCode() {
            return x + (y << 8);
        }

        public override string ToString() {
            return $"({x}, {y})";
        }

        #endregion
    }
}
