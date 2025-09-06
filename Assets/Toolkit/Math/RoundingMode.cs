using System;
using UnityEngine;

namespace Toolkit.Mathematics {
    public enum RoundingMode {
        /// <summary>
        /// Default rounding behaviour on floating numbers to integers.
        /// </summary>
        Floor = 0,
        Round = 1,
        Ceil = 2,
    }

    public static class RoundingModeUtility {

        #region Float -> Int

        public static bool Convert(this RoundingMode mode, float input, out byte output) {
            switch(mode) {
                case RoundingMode.Floor: output = (byte)Mathf.FloorToInt(input); return true;
                case RoundingMode.Ceil: output = (byte)Mathf.CeilToInt(input); return true;
                case RoundingMode.Round: output = (byte)Mathf.RoundToInt(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out sbyte output) {
            switch(mode) {
                case RoundingMode.Floor: output = (sbyte)Mathf.FloorToInt(input); return true;
                case RoundingMode.Ceil: output = (sbyte)Mathf.CeilToInt(input); return true;
                case RoundingMode.Round: output = (sbyte)Mathf.RoundToInt(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out short output) {
            switch(mode) {
                case RoundingMode.Floor: output = (short)Mathf.FloorToInt(input); return true;
                case RoundingMode.Ceil: output = (short)Mathf.CeilToInt(input); return true;
                case RoundingMode.Round: output = (short)Mathf.RoundToInt(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out ushort output) {
            switch(mode) {
                case RoundingMode.Floor: output = (ushort)Mathf.FloorToInt(input); return true;
                case RoundingMode.Ceil: output = (ushort)Mathf.CeilToInt(input); return true;
                case RoundingMode.Round: output = (ushort)Mathf.RoundToInt(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out int output) {
            switch(mode) {
                case RoundingMode.Floor: output = Mathf.FloorToInt(input); return true;
                case RoundingMode.Ceil: output = Mathf.CeilToInt(input); return true;
                case RoundingMode.Round: output = Mathf.RoundToInt(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out uint output) {
            switch(mode) {
                case RoundingMode.Floor: output = (uint)Mathf.FloorToInt(input); return true;
                case RoundingMode.Ceil: output = (uint)Mathf.CeilToInt(input); return true;
                case RoundingMode.Round: output = (uint)Mathf.RoundToInt(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out long output) {
            switch(mode) {
                case RoundingMode.Floor: output = (long)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (long)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (long)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, float input, out ulong output) {
            switch(mode) {
                case RoundingMode.Floor: output = (ulong)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (ulong)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (ulong)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static int Convert(this RoundingMode mode, float input) {
            Convert(mode, input, out int output);
            return output;
        }

        #endregion

        #region Double -> Int

        public static bool Convert(this RoundingMode mode, double input, out byte output) {
            switch(mode) {
                case RoundingMode.Floor: output = (byte)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (byte)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (byte)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out sbyte output) {
            switch(mode) {
                case RoundingMode.Floor: output = (sbyte)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (sbyte)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (sbyte)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out short output) {
            switch(mode) {
                case RoundingMode.Floor: output = (short)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (short)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (short)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out ushort output) {
            switch(mode) {
                case RoundingMode.Floor: output = (ushort)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (ushort)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (ushort)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out int output) {
            switch(mode) {
                case RoundingMode.Floor: output = (int)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (int)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (int)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out uint output) {
            switch(mode) {
                case RoundingMode.Floor: output = (uint)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (uint)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (uint)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out long output) {
            switch(mode) {
                case RoundingMode.Floor: output = (long)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (long)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (long)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, double input, out ulong output) {
            switch(mode) {
                case RoundingMode.Floor: output = (ulong)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (ulong)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (ulong)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static int Convert(this RoundingMode mode, double input) {
            Convert(mode, input, out int output);
            return output;
        }

        #endregion

        #region Decimal -> Int

        public static bool Convert(this RoundingMode mode, decimal input, out int output) {
            switch(mode) {
                case RoundingMode.Floor: output = (int)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (int)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (int)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static bool Convert(this RoundingMode mode, decimal input, out long output) {
            switch(mode) {
                case RoundingMode.Floor: output = (long)Math.Floor(input); return true;
                case RoundingMode.Ceil: output = (long)Math.Ceiling(input); return true;
                case RoundingMode.Round: output = (long)Math.Round(input); return true;
            }
            output = 0;
            return false;
        }

        public static int Convert(this RoundingMode mode, decimal input) {
            Convert(mode, input, out int output);
            return output;
        }

        #endregion
    }
}
