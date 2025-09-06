using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Mathematics
{
    [System.Serializable]
    public struct HexCoordInt
    {
        #region Variables

        public int x;
        public int y;

        #endregion

        #region Properties

        public int z {
            get => -x - y;
        }

        public float Magnitude => (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z)) / 2f;

        public Vector2 ToVector2 => new Vector2((x + y * 0.5f) * HexUtility.INNER_RADIUS_2, y * HexUtility.OUTER_RADIUS_1_5);
        public Vector3 ToVector3 => new Vector3((x + y * 0.5f) * HexUtility.INNER_RADIUS_2, 0f, y * HexUtility.OUTER_RADIUS_1_5);

        public static HexCoordInt Zero => new HexCoordInt(0, 0);
        public static HexCoordInt One => new HexCoordInt(1, 1);
        public static HexCoordInt Right => new HexCoordInt(1, 0);
        public static HexCoordInt Left => new HexCoordInt(-1, 0);
        public static HexCoordInt UpRight => new HexCoordInt(0, 1);
        public static HexCoordInt DownLeft => new HexCoordInt(0, -1);
        public static HexCoordInt UpLeft => new HexCoordInt(-1, 1);
        public static HexCoordInt DownRight => new HexCoordInt(1, -1);

        #endregion

        #region Consutructor

        public HexCoordInt(float x, float y) {
            this.x = Mathf.RoundToInt(x);
            this.y = Mathf.RoundToInt(y);
        }

        public HexCoordInt(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static HexCoordInt FromVector2(Vector2 point) {
            var x = (point.x / HexUtility.INNER_RADIUS_2) - ((point.y / HexUtility.OUTER_RADIUS_1_5) / 2f);
            var y = (point.y) / HexUtility.OUTER_RADIUS_1_5;
            var z = -x - y;

            var rx = Mathf.Round(x);
            var ry = Mathf.Round(y);
            var rz = Mathf.Round(z);

            var xd = Mathf.Abs(rx - x);
            var yd = Mathf.Abs(ry - y);
            var zd = Mathf.Abs(rz - z);

            if(xd > yd && xd > zd)
                rx = -ry - rz;
            else if(yd > zd)
                ry = -rx - rz;

            return new HexCoordInt(rx, ry);
        }

        #endregion

        #region Rotate

        /// <summary>
        /// Rotates the hex coordinate by 60 degrees clockwise
        /// </summary>
        public HexCoordInt RotateClockwise() {
            return new HexCoordInt(-z, -x);
        }

        /// <summary>
        /// Rotates the hex coordinate by 60 degrees counter clockwise
        /// </summary>
        public HexCoordInt RotateCounterClockwise() {
            return new HexCoordInt(-y, -z);
        }

        #endregion

        #region Static Methods

        public static float Distance(HexCoordInt lhs, HexCoordInt rhs) {
            return (lhs - rhs).Magnitude;
        }

        public static HexCoordInt Lerp(HexCoordInt start, HexCoordInt end, float t) {
            // Lerp Values
            var x = Mathf.Lerp(start.x, end.x, t);
            var y = Mathf.Lerp(start.y, end.y, t);
            var z = -x - y;

            // Apply rounding
            var rx = Mathf.Round(x);
            var ry = Mathf.Round(y);
            var rz = Mathf.Round(z);

            var xd = Mathf.Abs(rx - x);
            var yd = Mathf.Abs(ry - y);
            var zd = Mathf.Abs(rz - z);

            if(xd > yd && xd > zd)
                rx = -ry - rz;
            else if(yd > zd)
                ry = -rx - rz;

            return new HexCoordInt(rx, ry);
        }

        #endregion

        #region Parse

        public static HexCoordInt Parse(string s) {
            if(TryParse(s, out HexCoordInt coordinates)) {
                return coordinates;
            }
            throw new Exception($"Could not parse provided string {s}");
        }

        public static bool TryParse(string s, out HexCoordInt hexCoord) {
            var split = s.Split(',');
            if(split.Length < 2) {
                hexCoord = default;
                return false;
            }
            HexCoordInt coordinates = new HexCoordInt();
            if(!int.TryParse(split[0].Trim('(', ' '), out coordinates.x)) {
                hexCoord = default;
                return false;
            }

            if(!int.TryParse(split[1].Trim(')', ' '), out coordinates.y)) {
                hexCoord = default;
                return false;
            }
            hexCoord = coordinates;
            return true;
        }

        #endregion

        #region ToString

        public override string ToString() => $"({x}, {y})";
        public string ToStringFull() => $"({x}, {y}, {z})";

        #endregion

        #region Equals

        public override bool Equals(object obj) {
            if(obj is HexCoordInt other) {
                return x == other.x && y == other.y;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static bool operator ==(HexCoordInt lhs, HexCoordInt rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
        public static bool operator !=(HexCoordInt lhs, HexCoordInt rhs) => lhs.x != rhs.x || lhs.y != rhs.y;

        #endregion

        #region Operations

        public static implicit operator HexCoord(HexCoordInt hexCoord) {
            return new HexCoord(hexCoord.x, hexCoord.y);
        }

        public static HexCoordInt operator -(HexCoordInt lhs, HexCoordInt rhs) {
            return new HexCoordInt(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static HexCoordInt operator +(HexCoordInt lhs, HexCoordInt rhs) {
            return new HexCoordInt(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static HexCoordInt operator *(HexCoordInt lhs, HexCoordInt rhs) {
            return new HexCoordInt(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static HexCoordInt operator *(HexCoordInt lhs, float rhs) {
            return new HexCoordInt(lhs.x * rhs, lhs.y * rhs);
        }

        public static HexCoordInt operator /(HexCoord lhs, HexCoordInt rhs) {
            return new HexCoordInt(lhs.x / rhs.x, lhs.y / rhs.y);
        }

        public static HexCoordInt operator /(HexCoordInt lhs, float rhs) {
            return new HexCoordInt(lhs.x / rhs, lhs.y / rhs);
        }

        #endregion
    }
}
