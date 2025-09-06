using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2Short
    {
        #region Variables

        public short x;
        public short y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2Short zero => new Vector2Short(0, 0);
        public static Vector2Short one => new Vector2Short(1, 1);
        public static Vector2Short left => new Vector2Short(-1, 0);
        public static Vector2Short right => new Vector2Short(1, 0);
        public static Vector2Short up => new Vector2Short(0, 1);
        public static Vector2Short down => new Vector2Short(0, -1);

        public static Vector2Short MIN => new Vector2Short(short.MinValue, short.MinValue);
        public static Vector2Short MAX => new Vector2Short(short.MaxValue, short.MaxValue);

        #endregion

        #region Constructor

        public Vector2Short(short x, short y) {
            this.x = x;
            this.y = y;
        }

        public Vector2Short(int x, int y) {
            this.x = (short)x;
            this.y = (short)y;
        }

        public Vector2Short(float x, float y) {
            this.x = (short)x;
            this.y = (short)y;
        }

        public Vector2Short(Vector2Int vecInt) {
            this.x = (short)vecInt.x;
            this.y = (short)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(short x, short y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = (short)x;
            this.y = (short)y;
        }

        public void Set(float x, float y) {
            this.x = (short)x;
            this.y = (short)y;
        }

        public void Scale(Vector2Short other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= (short)other.x;
            this.y *= (short)other.y;
        }

        public void Clamp(Vector2Short min, Vector2Short max) {
            this.x = (short)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (short)Mathf.Clamp(this.y, min.y, max.y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = (short)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (short)Mathf.Clamp(this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static float Distance(Vector2Short lhs, Vector2Short rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        public static Vector2Short Max(Vector2Short lhs, Vector2Short rhs) {
            return new Vector2Short(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2Short Min(Vector2Short lhs, Vector2Short rhs) {
            return new Vector2Short(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2Short Round(Vector2 vector) {
            return new Vector2Short(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2Short Floor(Vector2 vector) {
            return new Vector2Short(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2Short Ceil(Vector2 vector) {
            return new Vector2Short(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        public static Vector2Short FromHashCode(int value) {
            return new Vector2Short(value & 0xffff, (value >> 16) & 0xffff);
        }

        #endregion

        #region Operations

        public static Vector2Short operator +(Vector2Short lhs, Vector2Short rhs) => new Vector2Short(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2Short operator -(Vector2Short lhs, Vector2Short rhs) => new Vector2Short(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2Short operator *(Vector2Short lhs, Vector2Short rhs) => new Vector2Short(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2Short operator /(Vector2Short lhs, Vector2Short rhs) => new Vector2Short(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2Short operator *(Vector2Short lhs, int rhs) => new Vector2Short(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Short operator /(Vector2Short lhs, int rhs) => new Vector2Short(lhs.x / rhs, lhs.x / rhs);

        public static Vector2Short operator *(Vector2Short lhs, float rhs) => new Vector2Short(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Short operator /(Vector2Short lhs, float rhs) => new Vector2Short(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2Short value) => new Vector2Int(value.x, value.y);
        public static implicit operator Vector2Short(Vector2Int value) => new Vector2Short(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2Short value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2Short(Vector2 value) => new Vector2Short(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2Short value) => new Vector2Byte(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2Short value) => new Vector2SByte(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2Short value) => new Vector2UShort(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2Short other) {
                return x == other.x && y == other.y;
            }
            return false;
        }

        public override int GetHashCode() {
            return x + (y << 16);
        }

        public override string ToString() {
            return $"({x}, {y})";
        }

        #endregion
    }
}
