using System;

namespace Toolkit
{
    public enum IntegerType
    {
        Int,
        Byte,
        SByte,
        Short,
        UShort,
        UInt,
        Long,
        ULong,
    }

    public static class IntegerTypeUtility
    {
        public static TypeCode ToTypeCode(this IntegerType type) {
            switch(type) {
                case IntegerType.Int: return TypeCode.Int32;
                case IntegerType.Byte: return TypeCode.Byte;
                case IntegerType.SByte: return TypeCode.SByte;
                case IntegerType.Short: return TypeCode.Int64;
                case IntegerType.UShort: return TypeCode.UInt16;
                case IntegerType.UInt: return TypeCode.UInt32;
                case IntegerType.Long: return TypeCode.Int64;
                case IntegerType.ULong: return TypeCode.UInt64;
            }
            return TypeCode.Int32;
        }
    }
}
