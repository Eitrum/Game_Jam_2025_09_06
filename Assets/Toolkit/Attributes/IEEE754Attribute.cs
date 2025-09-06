using System;
using UnityEngine;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IEEE754Attribute : PropertyAttribute
    {
        public const int FLOAT_EXPONENT_COUNT = 8;
        public const int FLOAT_MANTISSA_COUNT = 23;
        public const uint FLOAT_EXPONENT_MASK = 0b1111_1111_0000_0000_0000_0000_0000_0000;
        public const uint FLOAT_MANTISSA_MASK = 0b0000_0000_1111_1111_1111_1111_1111_1110;

        public const int DOUBLE_EXPONENT_COUNT = 11;
        public const int DOUBLE_MANTISSA_COUNT = 52;
        public const ulong DOUBLE_EXPONENT_MASK = 0b1111_1111_1111_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000;
        public const ulong DOUBLE_MANTISSA_MASK = 0b0000_0000_0000_0111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1110;
    }
}
