using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Mathematics
{
    public enum ComparitorOperatorType
    {
        None,
        [InspectorName("Equal (==)")] Equal,
        [InspectorName("More or Equal (>=)")] MoreOrEqual,
        [InspectorName("Less or Equal (<=)")] LessOrEqual,
        [InspectorName("More (>)")] More,
        [InspectorName("Less (<)")] Less,
        [InspectorName("Not Equal (!=)")] NotEqual,
    }

    public static class ComparitorOperatorUtility
    {
        #region Float

        public static bool Is(float lhs, float rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(float lhs, ComparitorOperatorType type, float rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, float lhs, float rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Int

        public static bool Is(int lhs, int rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(int lhs, ComparitorOperatorType type, int rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, int lhs, int rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Double

        public static bool Is(double lhs, double rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(double lhs, ComparitorOperatorType type, double rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, double lhs, double rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Long

        public static bool Is(long lhs, long rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(long lhs, ComparitorOperatorType type, long rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, long lhs, long rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Byte

        public static bool Is(byte lhs, byte rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(byte lhs, ComparitorOperatorType type, byte rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, byte lhs, byte rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region SByte

        public static bool Is(sbyte lhs, sbyte rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(sbyte lhs, ComparitorOperatorType type, sbyte rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, sbyte lhs, sbyte rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Short

        public static bool Is(short lhs, short rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(short lhs, ComparitorOperatorType type, short rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, short lhs, short rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region UShort

        public static bool Is(ushort lhs, ushort rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(ushort lhs, ComparitorOperatorType type, ushort rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, ushort lhs, ushort rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region UInt

        public static bool Is(uint lhs, uint rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(uint lhs, ComparitorOperatorType type, uint rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, uint lhs, uint rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region ULong

        public static bool Is(ulong lhs, ulong rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(ulong lhs, ComparitorOperatorType type, ulong rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, ulong lhs, ulong rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Decimal

        public static bool Is(decimal lhs, decimal rhs, ComparitorOperatorType type) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(decimal lhs, ComparitorOperatorType type, decimal rhs) {
            return Is(type, lhs, rhs);
        }

        public static bool Is(this ComparitorOperatorType type, decimal lhs, decimal rhs) {
            switch(type) {
                case ComparitorOperatorType.Equal: return lhs == rhs;
                case ComparitorOperatorType.NotEqual: return lhs != rhs;
                case ComparitorOperatorType.More: return lhs > rhs;
                case ComparitorOperatorType.Less: return lhs < rhs;
                case ComparitorOperatorType.MoreOrEqual: return lhs >= rhs;
                case ComparitorOperatorType.LessOrEqual: return lhs <= rhs;
            }
            return false;
        }

        #endregion

        #region Generic

        public static bool Is<T>(T lhs, T rhs, ComparitorOperatorType type) where T : IComparable<T> {
            return Is(type, lhs, rhs);
        }

        public static bool Is<T>(T lhs, ComparitorOperatorType type, T rhs) where T : IComparable<T> {
            return Is(type, lhs, rhs);
        }

        public static bool Is<T>(this ComparitorOperatorType type, T lhs, T rhs) where T : IComparable<T> {
            var res = lhs.CompareTo(rhs);
            switch(type) {
                case ComparitorOperatorType.Equal: return res == 0;
                case ComparitorOperatorType.Less: return res < 0;
                case ComparitorOperatorType.More: return res > 0;
                case ComparitorOperatorType.MoreOrEqual: return res >= 0;
                case ComparitorOperatorType.LessOrEqual: return res <= 0;
                case ComparitorOperatorType.NotEqual: return res != 0;
            }
            return false;
        }

        #endregion
    }
}
