using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2Byte
    {
        #region Variables

        public byte x;
        public byte y;

        #endregion

        #region Properties

        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public static Vector2Byte zero => new Vector2Byte(0, 0);
        public static Vector2Byte one => new Vector2Byte(1, 1);
        public static Vector2Byte right => new Vector2Byte(1, 0);
        public static Vector2Byte up => new Vector2Byte(0, 1);

        public static Vector2Byte MIN => new Vector2Byte(0, 0);
        public static Vector2Byte MAX => new Vector2Byte(255, 255);

        #endregion

        #region Constructor

        public Vector2Byte(byte x, byte y) {
            this.x = x;
            this.y = y;
        }

        public Vector2Byte(int x, int y) {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public Vector2Byte(float x, float y) {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public Vector2Byte(Vector2Int vecInt) {
            this.x = (byte)vecInt.x;
            this.y = (byte)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(byte x, byte y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public void Set(float x, float y) {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public void Scale(Vector2Byte other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= (byte)other.x;
            this.y *= (byte)other.y;
        }

        public void Clamp(Vector2Byte min, Vector2Byte max) {
            this.x = (byte)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (byte)Mathf.Clamp(this.y, min.y, max.y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = (byte)Mathf.Clamp(this.x, min.x, max.x);
            this.y = (byte)Mathf.Clamp(this.y, min.y, max.y);
        }

        #endregion

        #region Static Methods

        public static float Distance(Vector2Byte lhs, Vector2Byte rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        public static Vector2Byte Max(Vector2Byte lhs, Vector2Byte rhs) {
            return new Vector2Byte(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2Byte Min(Vector2Byte lhs, Vector2Byte rhs) {
            return new Vector2Byte(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2Byte Round(Vector2 vector) {
            return new Vector2Byte(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector2Byte Floor(Vector2 vector) {
            return new Vector2Byte(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2Byte Ceil(Vector2 vector) {
            return new Vector2Byte(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        }

        public static Vector2Byte FromHashCode(int value) {
            return new Vector2Byte(value & 0xff, (value >> 8) & 0xff);
        }

        #endregion

        #region Operations

        public static Vector2Byte operator +(Vector2Byte lhs, Vector2Byte rhs) => new Vector2Byte(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2Byte operator -(Vector2Byte lhs, Vector2Byte rhs) => new Vector2Byte(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2Byte operator *(Vector2Byte lhs, Vector2Byte rhs) => new Vector2Byte(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2Byte operator /(Vector2Byte lhs, Vector2Byte rhs) => new Vector2Byte(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2Byte operator *(Vector2Byte lhs, int rhs) => new Vector2Byte(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Byte operator /(Vector2Byte lhs, int rhs) => new Vector2Byte(lhs.x / rhs, lhs.x / rhs);

        public static Vector2Byte operator *(Vector2Byte lhs, float rhs) => new Vector2Byte(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Byte operator /(Vector2Byte lhs, float rhs) => new Vector2Byte(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2Byte value) => new Vector2Int(value.x, value.y);
        public static implicit operator Vector2Byte(Vector2Int value) => new Vector2Byte(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2Byte value) => new Vector2(value.x, value.y);
        public static implicit operator Vector2Byte(Vector2 value) => new Vector2Byte(value.x, value.y);
        // Custom
        public static implicit operator Vector2Short(Vector2Byte value) => new Vector2Short(value.x, value.y);
        public static implicit operator Vector2SByte(Vector2Byte value) => new Vector2SByte(value.x, value.y);
        public static implicit operator Vector2UShort(Vector2Byte value) => new Vector2UShort(value.x, value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2Byte other) {
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
