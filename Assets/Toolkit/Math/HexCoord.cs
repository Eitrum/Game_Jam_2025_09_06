using System;
using UnityEngine;

namespace Toolkit.Mathematics
{
    [System.Serializable]
    public struct HexCoord
    {
        #region Variables

        public float x;
        public float y;

        #endregion

        #region Properties

        public float z {
            get => -x - y;
        }

        public float Magnitude => (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z)) / 2f;

        public Vector2 ToVector2 => new Vector2((x + y * 0.5f) * HexUtility.INNER_RADIUS_2, y * HexUtility.OUTER_RADIUS_1_5);
        public Vector3 ToVector3 => new Vector3((x + y * 0.5f) * HexUtility.INNER_RADIUS_2, 0f, y * HexUtility.OUTER_RADIUS_1_5);

        public static HexCoord Zero => new HexCoord(0, 0);
        public static HexCoord One => new HexCoord(1, 1);
        public static HexCoord Right => new HexCoord(1, 0);
        public static HexCoord Left => new HexCoord(-1, 0);
        public static HexCoord UpRight => new HexCoord(0, 1);
        public static HexCoord DownLeft => new HexCoord(0, -1);
        public static HexCoord UpLeft => new HexCoord(-1, 1);
        public static HexCoord DownRight => new HexCoord(1, -1);

        #endregion

        #region Consutructor

        public HexCoord(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public HexCoord(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static HexCoord FromVector2(Vector2 point) {
            var x = (point.x / HexUtility.INNER_RADIUS_2) - ((point.y / HexUtility.OUTER_RADIUS_1_5) / 2f);
            var y = (point.y) / HexUtility.OUTER_RADIUS_1_5;
            return new HexCoord(x, y);
        }

        #endregion

        #region Rotate

        /// <summary>
        /// Rotates the hex coordinate by 60 degrees clockwise
        /// </summary>
        public HexCoord RotateClockwise() {
            return new HexCoord(-z, -x);
        }

        /// <summary>
        /// Rotates the hex coordinate by 60 degrees counter clockwise
        /// </summary>
        public HexCoord RotateCounterClockwise() {
            return new HexCoord(-y, -z);
        }

        #endregion

        #region Help/Utility Methods

        public static float Distance(HexCoord lhs, HexCoord rhs) {
            return (lhs - rhs).Magnitude;
        }

        public static HexCoord Lerp(HexCoord start, HexCoord end, float t) {
            return new HexCoord(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t));
        }

        public HexCoord Round() {
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

            return new HexCoord(rx, ry);
        }

        public HexCoordInt RoundToInt() {
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

        public static HexCoord Parse(string s) {
            if(TryParse(s, out HexCoord coordinates)) {
                return coordinates;
            }
            throw new Exception($"Could not parse provided string {s}");
        }

        public static bool TryParse(string s, out HexCoord hexCoord) {
            var split = s.Split(',');
            if(split.Length < 2) {
                hexCoord = default;
                return false;
            }
            HexCoord coordinates = new HexCoord();
            if(!float.TryParse(split[0].Trim('(', ' '), out coordinates.x)) {
                hexCoord = default;
                return false;
            }

            if(!float.TryParse(split[1].Trim(')', ' '), out coordinates.y)) {
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

        #region Operations

        public static HexCoord operator -(HexCoord lhs, HexCoord rhs) {
            return new HexCoord(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static HexCoord operator +(HexCoord lhs, HexCoord rhs) {
            return new HexCoord(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static HexCoord operator *(HexCoord lhs, HexCoord rhs) {
            return new HexCoord(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static HexCoord operator *(HexCoord lhs, float rhs) {
            return new HexCoord(lhs.x * rhs, lhs.y * rhs);
        }

        public static HexCoord operator /(HexCoord lhs, HexCoord rhs) {
            return new HexCoord(lhs.x / rhs.x, lhs.y / rhs.y);
        }

        public static HexCoord operator /(HexCoord lhs, float rhs) {
            return new HexCoord(lhs.x / rhs, lhs.y / rhs);
        }

        #endregion
    }
}
