using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2UShort
    {
        #region Variables

        public ushort x;
        public ushort y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2UShort zero => new Vector2UShort(0, 0);
        public static Vector2UShort one => new Vector2UShort(1, 1);
        public static Vector2UShort right => new Vector2UShort(1, 0);
        public static Vector2UShort up => new Vector2UShort(0, 1);

        public static Vector2UShort MIN => new Vector2UShort(ushort.MinValue, ushort.MinValue);
        public static Vector2UShort MAX => new Vector2UShort(ushort.MaxValue, ushort.MaxValue);

        #endregion

        #region Constructor

        public Vector2UShort(ushort x, ushort y) {
            this.x = x;
            this.y = y;
        }

        public Vector2UShort(int x, int y) {
            this.x = (ushort)x;
            this.y = (ushort)y;
        }

        public Vector2UShort(float x, float y) {
            this.x = (ushort)x;
            this.y = (ushort)y;
        }

        public Vector2UShort(Vector2Int vecInt) {
            this.x = (ushort)vecInt.x;
            this.y = (ushort)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(ushort x, ushort y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = (ushort)x;
            this.y = (ushort)y;
        }

        public void Set(float x, float y) {
            this.x = (ushort)x;
            this.y = (ushort)y;
        }

        public void Scale(Vector2UShort other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= (ushort)other.x;
            this.y *= (ushort)other.y;
        }

        public void Clamp(Vector2UShort min, Vector2UShort max) {
            this.x = (ushort)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (ushort)Mathf.Clamp(this.y, min.y, max.y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = (ushort)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (ushort)Mathf.Clamp(this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static float Distance(Vector2UShort lhs, Vector2UShort rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        public static Vector2UShort Max(Vector2UShort lhs, Vector2UShort rhs) {
            return new Vector2UShort(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2UShort Min(Vector2UShort lhs, Vector2UShort rhs) {
            return new Vector2UShort(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2UShort RoundToByte(Vector2 vector) {
            return new Vector2UShort(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2UShort FloorToByte(Vector2 vector) {
            return new Vector2UShort(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2UShort CeilToByte(Vector2 vector) {
            return new Vector2UShort(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        public static Vector2UShort FromHashCode(int value) {
            return new Vector2UShort(value & 0xffff, (value >> 16) & 0xffff);
        }

        #endregion

        #region Operations

        public static Vector2UShort operator +(Vector2UShort lhs, Vector2UShort rhs) => new Vector2UShort(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2UShort operator -(Vector2UShort lhs, Vector2UShort rhs) => new Vector2UShort(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2UShort operator *(Vector2UShort lhs, Vector2UShort rhs) => new Vector2UShort(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2UShort operator /(Vector2UShort lhs, Vector2UShort rhs) => new Vector2UShort(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2UShort operator *(Vector2UShort lhs, int rhs) => new Vector2UShort(lhs.x * rhs, lhs.x * rhs);
        public static Vector2UShort operator /(Vector2UShort lhs, int rhs) => new Vector2UShort(lhs.x / rhs, lhs.x / rhs);

        public static Vector2UShort operator *(Vector2UShort lhs, float rhs) => new Vector2UShort(lhs.x * rhs, lhs.x * rhs);
        public static Vector2UShort operator /(Vector2UShort lhs, float rhs) => new Vector2UShort(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2UShort value) => new Vector2Int(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2Int value) => new Vector2UShort(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2UShort value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2 value) => new Vector2UShort(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2UShort value) => new Vector2Byte(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2UShort value) => new Vector2SByte(value.x, value.y);
        public static implicit operator Vector2Short(Vector2UShort value) => new Vector2Short(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2UShort other) {
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
