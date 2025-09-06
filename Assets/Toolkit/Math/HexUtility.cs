using System;
using UnityEngine;

namespace Toolkit.Mathematics
{
    public enum HexDirection
    {
        None,
        Right,
        Left,
        UpRight,
        DownLeft,
        UpLeft,
        DownRight
    }

    public static class HexUtility
    {
        #region Constants

        public const float INNER_RADIUS = 0.8660254f;
        public const float OUTER_RADIUS = 1f;
        public const float INNER_RADIUS_2 = INNER_RADIUS * 2f;
        public const float OUTER_RADIUS_1_5 = OUTER_RADIUS * 1.5f;

        public const float HexOut2In = 0.8660254f;
        public const float HexIn2Out = 1.1547005f;

        #endregion

        #region Lerp

        public static HexCoord[] GetLerpArray(HexCoord hex0, HexCoord hex1) {
            var dist = HexCoord.Distance(hex0, hex1);
            var distSteps = Mathf.Ceil(dist) + 1;
            var n = distSteps - 1;
            HexCoord[] steps = new HexCoord[(int)distSteps];
            for(int i = 0; i < distSteps; i++) {
                var t = i / n;
                steps[i] = HexCoord.Lerp(hex0, hex1, t).RoundToInt();
            }
            return steps;
        }

        public static HexCoordInt[] GetLerpArrayInt(HexCoordInt hex0, HexCoordInt hex1) {
            var dist = HexCoordInt.Distance(hex0, hex1);
            var distSteps = Mathf.Ceil(dist) + 1;
            var n = distSteps - 1;
            HexCoordInt[] steps = new HexCoordInt[(int)distSteps];
            for(int i = 0; i < distSteps; i++) {
                var t = i / n;
                steps[i] = HexCoordInt.Lerp(hex0, hex1, t);
            }
            return steps;
        }

        #endregion

        #region Get from direction

        public static HexCoord GetHexCoord(HexDirection direction) {
            switch(direction) {
                case HexDirection.Right: return HexCoord.Right;
                case HexDirection.Left: return HexCoord.Left;
                case HexDirection.UpRight: return HexCoord.UpRight;
                case HexDirection.DownLeft: return HexCoord.DownLeft;
                case HexDirection.UpLeft: return HexCoord.UpLeft;
                case HexDirection.DownRight: return HexCoord.DownRight;
            }
            return HexCoord.Zero;
        }

        public static HexCoordInt GetHexCoordInt(HexDirection direction) {
            switch(direction) {
                case HexDirection.Right: return HexCoordInt.Right;
                case HexDirection.Left: return HexCoordInt.Left;
                case HexDirection.UpRight: return HexCoordInt.UpRight;
                case HexDirection.DownLeft: return HexCoordInt.DownLeft;
                case HexDirection.UpLeft: return HexCoordInt.UpLeft;
                case HexDirection.DownRight: return HexCoordInt.DownRight;
            }

            return HexCoordInt.Zero;
        }

        #endregion

        #region Grid

        public static HexCoord[,] CreateGrid(int width, int height)
            => CreateGrid(width, height, false, OUTER_RADIUS);

        public static HexCoord[,] CreateGrid(int width, int height, bool oddOffset)
            => CreateGrid(width, height, oddOffset, OUTER_RADIUS);

        public static HexCoord[,] CreateGrid(int width, int height, float outerRadius)
            => CreateGrid(width, height, false, outerRadius);

        public static HexCoord[,] CreateGrid(int width, int height, bool oddOffset, float outerRadius) {
            HexCoord[,] grid = new HexCoord[width, height];
            for(int y = 0; y < height; y++) {
                var xOffset = -y / 2;
                var xWidth = oddOffset ? width - y % 2 : width;
                for(int x = 0; x < xWidth; x++) {
                    grid[x, y] = new HexCoord((x + xOffset) * outerRadius, y * outerRadius);
                }
            }
            return grid;
        }

        #endregion
    }
}
