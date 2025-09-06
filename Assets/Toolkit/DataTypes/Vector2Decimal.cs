using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2Decimal
    {
        #region Variables

        public decimal x;
        public decimal y;

        #endregion

        #region Properties

        public double magnitude => Math.Sqrt((double)(x * x + y * y));
        public double sqrMagnitude => (double)(x * x + y * y);

        public static Vector2Decimal zero => new Vector2Decimal(0, 0);
        public static Vector2Decimal one => new Vector2Decimal(1, 1);
        public static Vector2Decimal left => new Vector2Decimal(-1, 0);
        public static Vector2Decimal right => new Vector2Decimal(1, 0);
        public static Vector2Decimal up => new Vector2Decimal(0, 1);
        public static Vector2Decimal down => new Vector2Decimal(0, -1);

        public static Vector2Decimal MIN => new Vector2Decimal(decimal.MinValue, decimal.MinValue);
        public static Vector2Decimal MAX => new Vector2Decimal(decimal.MaxValue, decimal.MaxValue);

        #endregion

        #region Constructor

        public Vector2Decimal(decimal x, decimal y) {
            this.x = x;
            this.y = y;
        }

        public Vector2Decimal(int x, int y) {
            this.x = (decimal)x;
            this.y = (decimal)y;
        }

        public Vector2Decimal(float x, float y) {
            this.x = (decimal)x;
            this.y = (decimal)y;
        }

        public Vector2Decimal(Vector2 vec) {
            this.x = (decimal)vec.x;
            this.y = (decimal)vec.y;
        }

        public Vector2Decimal(Vector2Int vecInt) {
            this.x = (decimal)vecInt.x;
            this.y = (decimal)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(decimal x, decimal y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y) {
            this.x = (decimal)x;
            this.y = (decimal)y;
        }

        public void Scale(Vector2Decimal other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Clamp(Vector2Decimal min, Vector2Decimal max) {
            this.x = this.x > max.x ? max.x : (this.x < min.x ? min.x : x);
            this.y = this.y > max.y ? max.y : (this.y < min.y ? min.y : y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = Mathf.Clamp((int)this.x, min.x, max.x);
            this.y = Mathf.Clamp((int)this.y, min.y, max.y);
        }

        public void Clamp(Vector2 min, Vector2 max) {
            this.x = this.x > (decimal)max.x ? (decimal)max.x : (this.x < (decimal)min.x ? (decimal)min.x : x);
            this.y = this.y > (decimal)max.y ? (decimal)max.y : (this.y < (decimal)min.y ? (decimal)min.y : y);
        }

        #endregion

        #region Static Methods

        public static double Distance(Vector2Decimal lhs, Vector2Decimal rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Math.Sqrt((double)(x * x + y * y));
        }

        public static float DistanceFloat(Vector2Decimal lhs, Vector2Decimal rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return (float)Math.Sqrt((double)(x * x + y * y));
        }

        public static Vector2Decimal Max(Vector2Decimal lhs, Vector2Decimal rhs) {
            return new Vector2Decimal(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2Decimal Min(Vector2Decimal lhs, Vector2Decimal rhs) {
            return new Vector2Decimal(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        #endregion

        #region Operations

        public static Vector2Decimal operator +(Vector2Decimal lhs, Vector2Decimal rhs) => new Vector2Decimal(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2Decimal operator -(Vector2Decimal lhs, Vector2Decimal rhs) => new Vector2Decimal(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2Decimal operator *(Vector2Decimal lhs, Vector2Decimal rhs) => new Vector2Decimal(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2Decimal operator /(Vector2Decimal lhs, Vector2Decimal rhs) => new Vector2Decimal(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2Decimal operator *(Vector2Decimal lhs, int rhs) => new Vector2Decimal(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Decimal operator /(Vector2Decimal lhs, int rhs) => new Vector2Decimal(lhs.x / rhs, lhs.x / rhs);

        public static Vector2Decimal operator *(Vector2Decimal lhs, float rhs) => new Vector2Decimal(lhs.x * (decimal)rhs, lhs.x * (decimal)rhs);
        public static Vector2Decimal operator /(Vector2Decimal lhs, float rhs) => new Vector2Decimal(lhs.x / (decimal)rhs, lhs.x / (decimal)rhs);

        public static Vector2Decimal operator *(Vector2Decimal lhs, decimal rhs) => new Vector2Decimal(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Decimal operator /(Vector2Decimal lhs, decimal rhs) => new Vector2Decimal(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2Decimal value) => new Vector2Int((int)value.x, (int)value.y);
        public static implicit operator Vector2Decimal(Vector2Int value) => new Vector2Decimal(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2Decimal value) => new Vector2((float)value.x, (float)value.y);
        public static implicit operator Vector2Decimal(Vector2 value) => new Vector2Decimal(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2Decimal value) => new Vector2Byte((float)value.x, (float)value.y);
        public static implicit operator Vector2SByte(Vector2Decimal value) => new Vector2SByte((float)value.x, (float)value.y);
        public static implicit operator Vector2UShort(Vector2Decimal value) => new Vector2UShort((float)value.x, (float)value.y);
        public static implicit operator Vector2Short(Vector2Decimal value) => new Vector2Short((float)value.x, (float)value.y);
        public static implicit operator Vector2UInt(Vector2Decimal value) => new Vector2UInt((float)value.x, (float)value.y);
        public static implicit operator Vector2Long(Vector2Decimal value) => new Vector2Long((long)value.x, (long)value.y);
        public static implicit operator Vector2ULong(Vector2Decimal value) => new Vector2ULong((ulong)value.x, (ulong)value.y);
        public static implicit operator Vector2Double(Vector2Decimal value) => new Vector2Double((double)value.x, (double)value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2Decimal other) {
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
