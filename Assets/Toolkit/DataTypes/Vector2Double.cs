using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct Vector2Double
    {
        #region Variables

        public double x;
        public double y;

        #endregion

        #region Properties

        public double magnitude => Math.Sqrt(x * x + y * y);
        public double sqrMagnitude => x * x + y * y;

        public static Vector2Double zero => new Vector2Double(0, 0);
        public static Vector2Double one => new Vector2Double(1, 1);
        public static Vector2Double left => new Vector2Double(-1, 0);
        public static Vector2Double right => new Vector2Double(1, 0);
        public static Vector2Double up => new Vector2Double(0, 1);
        public static Vector2Double down => new Vector2Double(0, -1);

        public static Vector2Double MIN => new Vector2Double(double.MinValue, double.MinValue);
        public static Vector2Double MAX => new Vector2Double(double.MaxValue, double.MaxValue);

        #endregion

        #region Constructor

        public Vector2Double(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Vector2Double(int x, int y) {
            this.x = (double)x;
            this.y = (double)y;
        }

        public Vector2Double(float x, float y) {
            this.x = (double)x;
            this.y = (double)y;
        }

        public Vector2Double(Vector2 vec) {
            this.x = (double)vec.x;
            this.y = (double)vec.y;
        }

        public Vector2Double(Vector2Int vecInt) {
            this.x = (double)vecInt.x;
            this.y = (double)vecInt.y;
        }

        #endregion

        #region Methods

        public void Set(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y) {
            this.x = (double)x;
            this.y = (double)y;
        }

        public void Scale(Vector2Double other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Scale(Vector2Int other) {
            this.x *= other.x;
            this.y *= other.y;
        }

        public void Clamp(Vector2Double min, Vector2Double max) {
            this.x = this.x > max.x ? max.x : (this.x < min.x ? min.x : x);
            this.y = this.y > max.y ? max.y : (this.y < min.y ? min.y : y);
        }

        public void Clamp(Vector2Int min, Vector2Int max) {
            this.x = Mathf.Clamp((int)this.x, min.x, max.x);
            this.y = Mathf.Clamp((int)this.y, min.y, max.y);
        }

        public void Clamp(Vector2 min, Vector2 max) {
            this.x = this.x > max.x ? max.x : (this.x < min.x ? min.x : x);
            this.y = this.y > max.y ? max.y : (this.y < min.y ? min.y : y);
        }

        #endregion

        #region Static Methods

        public static double Distance(Vector2Double lhs, Vector2Double rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return Math.Sqrt(x * x + y * y);
        }

        public static float DistanceFloat(Vector2Double lhs, Vector2Double rhs) {
            var x = rhs.x - lhs.x;
            var y = rhs.y - lhs.y;
            return (float)Math.Sqrt(x * x + y * y);
        }

        public static Vector2Double Max(Vector2Double lhs, Vector2Double rhs) {
            return new Vector2Double(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2Double Min(Vector2Double lhs, Vector2Double rhs) {
            return new Vector2Double(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        #endregion

        #region Operations

        public static Vector2Double operator +(Vector2Double lhs, Vector2Double rhs) => new Vector2Double(lhs.x + rhs.x, lhs.x + rhs.x);
        public static Vector2Double operator -(Vector2Double lhs, Vector2Double rhs) => new Vector2Double(lhs.x - rhs.x, lhs.x - rhs.x);

        public static Vector2Double operator *(Vector2Double lhs, Vector2Double rhs) => new Vector2Double(lhs.x * rhs.x, lhs.x * rhs.x);
        public static Vector2Double operator /(Vector2Double lhs, Vector2Double rhs) => new Vector2Double(lhs.x / rhs.x, lhs.x / rhs.x);

        public static Vector2Double operator *(Vector2Double lhs, int rhs) => new Vector2Double(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Double operator /(Vector2Double lhs, int rhs) => new Vector2Double(lhs.x / rhs, lhs.x / rhs);

        public static Vector2Double operator *(Vector2Double lhs, float rhs) => new Vector2Double(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Double operator /(Vector2Double lhs, float rhs) => new Vector2Double(lhs.x / rhs, lhs.x / rhs);

        public static Vector2Double operator *(Vector2Double lhs, double rhs) => new Vector2Double(lhs.x * rhs, lhs.x * rhs);
        public static Vector2Double operator /(Vector2Double lhs, double rhs) => new Vector2Double(lhs.x / rhs, lhs.x / rhs);

        // Int
        public static implicit operator Vector2Int(Vector2Double value) => new Vector2Int((int)value.x, (int)value.y);
        public static implicit operator Vector2Double(Vector2Int value) => new Vector2Double(value.x, value.y);
        // Float
        public static implicit operator Vector2(Vector2Double value) => new Vector2((float)value.x, (float)value.y);
        public static implicit operator Vector2Double(Vector2 value) => new Vector2Double(value.x, value.y);
        // Custom
        public static implicit operator Vector2Byte(Vector2Double value) => new Vector2Byte((float)value.x, (float)value.y);
        public static implicit operator Vector2SByte(Vector2Double value) => new Vector2SByte((float)value.x, (float)value.y);
        public static implicit operator Vector2UShort(Vector2Double value) => new Vector2UShort((float)value.x, (float)value.y);
        public static implicit operator Vector2Short(Vector2Double value) => new Vector2Short((float)value.x, (float)value.y);
        public static implicit operator Vector2UInt(Vector2Double value) => new Vector2UInt((float)value.x, (float)value.y);
        public static implicit operator Vector2Long(Vector2Double value) => new Vector2Long((long)value.x, (long)value.y);
        public static implicit operator Vector2ULong(Vector2Double value) => new Vector2ULong((ulong)value.x, (ulong)value.y);

        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            if(obj is Vector2Double other) {
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
