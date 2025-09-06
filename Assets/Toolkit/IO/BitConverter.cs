///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

using System.Collections.Generic;
using UnityEngine;
namespace Toolkit.IO {
    public static unsafe class BitConverter {
        
        public static readonly bool IsLittleEndian;
        private static IBitConverter converter;

        public static bool IsReversed => converter is ReversedConverter;
        public static IBitConverter Converter => converter;

        static BitConverter() {
            IsLittleEndian = System.BitConverter.IsLittleEndian;
            if(IsLittleEndian)
                converter = new ReversedConverter();
            else
                converter = new DefaultConverter();
        }

        public static byte[] GetBytes(byte value) => converter.GetBytes(value);
        public static void GetBytes(byte value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(byte value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static byte ToByte(byte* array) => converter.ToByte(array);
        public static void ToValue(byte* array, out byte oValue) => converter.ToValue(array, out oValue);
        public static byte ToByte(IList<byte> array, int index) => converter.ToByte(array, index);
        public static void ToValue(IList<byte> array, int index, out byte oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(sbyte value) => converter.GetBytes(value);
        public static void GetBytes(sbyte value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(sbyte value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static sbyte ToSbyte(byte* array) => converter.ToSbyte(array);
        public static void ToValue(byte* array, out sbyte oValue) => converter.ToValue(array, out oValue);
        public static sbyte ToSbyte(IList<byte> array, int index) => converter.ToSbyte(array, index);
        public static void ToValue(IList<byte> array, int index, out sbyte oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(bool value) => converter.GetBytes(value);
        public static void GetBytes(bool value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(bool value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static bool ToBool(byte* array) => converter.ToBool(array);
        public static void ToValue(byte* array, out bool oValue) => converter.ToValue(array, out oValue);
        public static bool ToBool(IList<byte> array, int index) => converter.ToBool(array, index);
        public static void ToValue(IList<byte> array, int index, out bool oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(short value) => converter.GetBytes(value);
        public static void GetBytes(short value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(short value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static short ToShort(byte* array) => converter.ToShort(array);
        public static void ToValue(byte* array, out short oValue) => converter.ToValue(array, out oValue);
        public static short ToShort(IList<byte> array, int index) => converter.ToShort(array, index);
        public static void ToValue(IList<byte> array, int index, out short oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(ushort value) => converter.GetBytes(value);
        public static void GetBytes(ushort value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(ushort value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static ushort ToUshort(byte* array) => converter.ToUshort(array);
        public static void ToValue(byte* array, out ushort oValue) => converter.ToValue(array, out oValue);
        public static ushort ToUshort(IList<byte> array, int index) => converter.ToUshort(array, index);
        public static void ToValue(IList<byte> array, int index, out ushort oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(uint value) => converter.GetBytes(value);
        public static void GetBytes(uint value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(uint value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static uint ToUint(byte* array) => converter.ToUint(array);
        public static void ToValue(byte* array, out uint oValue) => converter.ToValue(array, out oValue);
        public static uint ToUint(IList<byte> array, int index) => converter.ToUint(array, index);
        public static void ToValue(IList<byte> array, int index, out uint oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(int value) => converter.GetBytes(value);
        public static void GetBytes(int value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(int value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static int ToInt(byte* array) => converter.ToInt(array);
        public static void ToValue(byte* array, out int oValue) => converter.ToValue(array, out oValue);
        public static int ToInt(IList<byte> array, int index) => converter.ToInt(array, index);
        public static void ToValue(IList<byte> array, int index, out int oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(ulong value) => converter.GetBytes(value);
        public static void GetBytes(ulong value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(ulong value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static ulong ToUlong(byte* array) => converter.ToUlong(array);
        public static void ToValue(byte* array, out ulong oValue) => converter.ToValue(array, out oValue);
        public static ulong ToUlong(IList<byte> array, int index) => converter.ToUlong(array, index);
        public static void ToValue(IList<byte> array, int index, out ulong oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(long value) => converter.GetBytes(value);
        public static void GetBytes(long value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(long value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static long ToLong(byte* array) => converter.ToLong(array);
        public static void ToValue(byte* array, out long oValue) => converter.ToValue(array, out oValue);
        public static long ToLong(IList<byte> array, int index) => converter.ToLong(array, index);
        public static void ToValue(IList<byte> array, int index, out long oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(float value) => converter.GetBytes(value);
        public static void GetBytes(float value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(float value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static float ToFloat(byte* array) => converter.ToFloat(array);
        public static void ToValue(byte* array, out float oValue) => converter.ToValue(array, out oValue);
        public static float ToFloat(IList<byte> array, int index) => converter.ToFloat(array, index);
        public static void ToValue(IList<byte> array, int index, out float oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(double value) => converter.GetBytes(value);
        public static void GetBytes(double value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(double value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static double ToDouble(byte* array) => converter.ToDouble(array);
        public static void ToValue(byte* array, out double oValue) => converter.ToValue(array, out oValue);
        public static double ToDouble(IList<byte> array, int index) => converter.ToDouble(array, index);
        public static void ToValue(IList<byte> array, int index, out double oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(decimal value) => converter.GetBytes(value);
        public static void GetBytes(decimal value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(decimal value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static decimal ToDecimal(byte* array) => converter.ToDecimal(array);
        public static void ToValue(byte* array, out decimal oValue) => converter.ToValue(array, out oValue);
        public static decimal ToDecimal(IList<byte> array, int index) => converter.ToDecimal(array, index);
        public static void ToValue(IList<byte> array, int index, out decimal oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(char value) => converter.GetBytes(value);
        public static void GetBytes(char value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(char value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static char ToChar(byte* array) => converter.ToChar(array);
        public static void ToValue(byte* array, out char oValue) => converter.ToValue(array, out oValue);
        public static char ToChar(IList<byte> array, int index) => converter.ToChar(array, index);
        public static void ToValue(IList<byte> array, int index, out char oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(Vector2 value) => converter.GetBytes(value);
        public static void GetBytes(Vector2 value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(Vector2 value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static Vector2 ToVector2(byte* array) => converter.ToVector2(array);
        public static void ToValue(byte* array, out Vector2 oValue) => converter.ToValue(array, out oValue);
        public static Vector2 ToVector2(IList<byte> array, int index) => converter.ToVector2(array, index);
        public static void ToValue(IList<byte> array, int index, out Vector2 oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(Vector3 value) => converter.GetBytes(value);
        public static void GetBytes(Vector3 value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(Vector3 value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static Vector3 ToVector3(byte* array) => converter.ToVector3(array);
        public static void ToValue(byte* array, out Vector3 oValue) => converter.ToValue(array, out oValue);
        public static Vector3 ToVector3(IList<byte> array, int index) => converter.ToVector3(array, index);
        public static void ToValue(IList<byte> array, int index, out Vector3 oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(Vector4 value) => converter.GetBytes(value);
        public static void GetBytes(Vector4 value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(Vector4 value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static Vector4 ToVector4(byte* array) => converter.ToVector4(array);
        public static void ToValue(byte* array, out Vector4 oValue) => converter.ToValue(array, out oValue);
        public static Vector4 ToVector4(IList<byte> array, int index) => converter.ToVector4(array, index);
        public static void ToValue(IList<byte> array, int index, out Vector4 oValue) => converter.ToValue(array, index, out oValue);

        public static byte[] GetBytes(Quaternion value) => converter.GetBytes(value);
        public static void GetBytes(Quaternion value, byte* array) => converter.GetBytes(value, array);
        public static void GetBytes(Quaternion value, IList<byte> array, int index) => converter.GetBytes(value, array, index);
        public static Quaternion ToQuaternion(byte* array) => converter.ToQuaternion(array);
        public static void ToValue(byte* array, out Quaternion oValue) => converter.ToValue(array, out oValue);
        public static Quaternion ToQuaternion(IList<byte> array, int index) => converter.ToQuaternion(array, index);
        public static void ToValue(IList<byte> array, int index, out Quaternion oValue) => converter.ToValue(array, index, out oValue);
    }

    public unsafe interface IBitConverter {
        byte[] GetBytes(byte value);
        void GetBytes(byte value, byte* array);
        void GetBytes(byte value, IList<byte> array, int index);
        byte ToByte(byte* array);
        void ToValue(byte* array, out byte oValue);
        byte ToByte(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out byte oValue);

        byte[] GetBytes(sbyte value);
        void GetBytes(sbyte value, byte* array);
        void GetBytes(sbyte value, IList<byte> array, int index);
        sbyte ToSbyte(byte* array);
        void ToValue(byte* array, out sbyte oValue);
        sbyte ToSbyte(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out sbyte oValue);

        byte[] GetBytes(bool value);
        void GetBytes(bool value, byte* array);
        void GetBytes(bool value, IList<byte> array, int index);
        bool ToBool(byte* array);
        void ToValue(byte* array, out bool oValue);
        bool ToBool(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out bool oValue);

        byte[] GetBytes(short value);
        void GetBytes(short value, byte* array);
        void GetBytes(short value, IList<byte> array, int index);
        short ToShort(byte* array);
        void ToValue(byte* array, out short oValue);
        short ToShort(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out short oValue);

        byte[] GetBytes(ushort value);
        void GetBytes(ushort value, byte* array);
        void GetBytes(ushort value, IList<byte> array, int index);
        ushort ToUshort(byte* array);
        void ToValue(byte* array, out ushort oValue);
        ushort ToUshort(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out ushort oValue);

        byte[] GetBytes(uint value);
        void GetBytes(uint value, byte* array);
        void GetBytes(uint value, IList<byte> array, int index);
        uint ToUint(byte* array);
        void ToValue(byte* array, out uint oValue);
        uint ToUint(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out uint oValue);

        byte[] GetBytes(int value);
        void GetBytes(int value, byte* array);
        void GetBytes(int value, IList<byte> array, int index);
        int ToInt(byte* array);
        void ToValue(byte* array, out int oValue);
        int ToInt(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out int oValue);

        byte[] GetBytes(ulong value);
        void GetBytes(ulong value, byte* array);
        void GetBytes(ulong value, IList<byte> array, int index);
        ulong ToUlong(byte* array);
        void ToValue(byte* array, out ulong oValue);
        ulong ToUlong(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out ulong oValue);

        byte[] GetBytes(long value);
        void GetBytes(long value, byte* array);
        void GetBytes(long value, IList<byte> array, int index);
        long ToLong(byte* array);
        void ToValue(byte* array, out long oValue);
        long ToLong(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out long oValue);

        byte[] GetBytes(float value);
        void GetBytes(float value, byte* array);
        void GetBytes(float value, IList<byte> array, int index);
        float ToFloat(byte* array);
        void ToValue(byte* array, out float oValue);
        float ToFloat(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out float oValue);

        byte[] GetBytes(double value);
        void GetBytes(double value, byte* array);
        void GetBytes(double value, IList<byte> array, int index);
        double ToDouble(byte* array);
        void ToValue(byte* array, out double oValue);
        double ToDouble(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out double oValue);

        byte[] GetBytes(decimal value);
        void GetBytes(decimal value, byte* array);
        void GetBytes(decimal value, IList<byte> array, int index);
        decimal ToDecimal(byte* array);
        void ToValue(byte* array, out decimal oValue);
        decimal ToDecimal(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out decimal oValue);

        byte[] GetBytes(char value);
        void GetBytes(char value, byte* array);
        void GetBytes(char value, IList<byte> array, int index);
        char ToChar(byte* array);
        void ToValue(byte* array, out char oValue);
        char ToChar(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out char oValue);

        byte[] GetBytes(Vector2 value);
        void GetBytes(Vector2 value, byte* array);
        void GetBytes(Vector2 value, IList<byte> array, int index);
        Vector2 ToVector2(byte* array);
        void ToValue(byte* array, out Vector2 oValue);
        Vector2 ToVector2(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out Vector2 oValue);

        byte[] GetBytes(Vector3 value);
        void GetBytes(Vector3 value, byte* array);
        void GetBytes(Vector3 value, IList<byte> array, int index);
        Vector3 ToVector3(byte* array);
        void ToValue(byte* array, out Vector3 oValue);
        Vector3 ToVector3(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out Vector3 oValue);

        byte[] GetBytes(Vector4 value);
        void GetBytes(Vector4 value, byte* array);
        void GetBytes(Vector4 value, IList<byte> array, int index);
        Vector4 ToVector4(byte* array);
        void ToValue(byte* array, out Vector4 oValue);
        Vector4 ToVector4(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out Vector4 oValue);

        byte[] GetBytes(Quaternion value);
        void GetBytes(Quaternion value, byte* array);
        void GetBytes(Quaternion value, IList<byte> array, int index);
        Quaternion ToQuaternion(byte* array);
        void ToValue(byte* array, out Quaternion oValue);
        Quaternion ToQuaternion(IList<byte> array, int index);
        void ToValue(IList<byte> array, int index, out Quaternion oValue);
    }

    internal unsafe class DefaultConverter : IBitConverter {
        public System.Byte[] GetBytes(byte value) {
            var val = (byte*)&value;
            var bytes = new byte[1];
            bytes[0] = *(val + 0);
            return bytes;
        }

        public void GetBytes(byte value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
        }

        public void GetBytes(byte value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
        }

        public byte ToByte(byte* array) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out byte oValue) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            oValue = value;
        }

        public byte ToByte(IList<byte> array, int index) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out byte oValue) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(sbyte value) {
            var val = (byte*)&value;
            var bytes = new byte[1];
            bytes[0] = *(val + 0);
            return bytes;
        }

        public void GetBytes(sbyte value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
        }

        public void GetBytes(sbyte value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
        }

        public sbyte ToSbyte(byte* array) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out sbyte oValue) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            oValue = value;
        }

        public sbyte ToSbyte(IList<byte> array, int index) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out sbyte oValue) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(bool value) {
            var val = (byte*)&value;
            var bytes = new byte[1];
            bytes[0] = *(val + 0);
            return bytes;
        }

        public void GetBytes(bool value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
        }

        public void GetBytes(bool value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
        }

        public bool ToBool(byte* array) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out bool oValue) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            oValue = value;
        }

        public bool ToBool(IList<byte> array, int index) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out bool oValue) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(short value) {
            var val = (byte*)&value;
            var bytes = new byte[2];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            return bytes;
        }

        public void GetBytes(short value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
        }

        public void GetBytes(short value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
        }

        public short ToShort(byte* array) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            return value;
        }

        public void ToValue(byte* array, out short oValue) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            oValue = value;
        }

        public short ToShort(IList<byte> array, int index) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out short oValue) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            oValue = value;
        }

        public System.Byte[] GetBytes(ushort value) {
            var val = (byte*)&value;
            var bytes = new byte[2];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            return bytes;
        }

        public void GetBytes(ushort value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
        }

        public void GetBytes(ushort value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
        }

        public ushort ToUshort(byte* array) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            return value;
        }

        public void ToValue(byte* array, out ushort oValue) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            oValue = value;
        }

        public ushort ToUshort(IList<byte> array, int index) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out ushort oValue) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            oValue = value;
        }

        public System.Byte[] GetBytes(uint value) {
            var val = (byte*)&value;
            var bytes = new byte[4];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            return bytes;
        }

        public void GetBytes(uint value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
        }

        public void GetBytes(uint value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
        }

        public uint ToUint(byte* array) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            return value;
        }

        public void ToValue(byte* array, out uint oValue) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            oValue = value;
        }

        public uint ToUint(IList<byte> array, int index) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out uint oValue) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            oValue = value;
        }

        public System.Byte[] GetBytes(int value) {
            var val = (byte*)&value;
            var bytes = new byte[4];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            return bytes;
        }

        public void GetBytes(int value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
        }

        public void GetBytes(int value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
        }

        public int ToInt(byte* array) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            return value;
        }

        public void ToValue(byte* array, out int oValue) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            oValue = value;
        }

        public int ToInt(IList<byte> array, int index) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out int oValue) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            oValue = value;
        }

        public System.Byte[] GetBytes(ulong value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            return bytes;
        }

        public void GetBytes(ulong value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
        }

        public void GetBytes(ulong value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
        }

        public ulong ToUlong(byte* array) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            return value;
        }

        public void ToValue(byte* array, out ulong oValue) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            oValue = value;
        }

        public ulong ToUlong(IList<byte> array, int index) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out ulong oValue) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            oValue = value;
        }

        public System.Byte[] GetBytes(long value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            return bytes;
        }

        public void GetBytes(long value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
        }

        public void GetBytes(long value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
        }

        public long ToLong(byte* array) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            return value;
        }

        public void ToValue(byte* array, out long oValue) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            oValue = value;
        }

        public long ToLong(IList<byte> array, int index) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out long oValue) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            oValue = value;
        }

        public System.Byte[] GetBytes(float value) {
            var val = (byte*)&value;
            var bytes = new byte[4];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            return bytes;
        }

        public void GetBytes(float value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
        }

        public void GetBytes(float value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
        }

        public float ToFloat(byte* array) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            return value;
        }

        public void ToValue(byte* array, out float oValue) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            oValue = value;
        }

        public float ToFloat(IList<byte> array, int index) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out float oValue) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            oValue = value;
        }

        public System.Byte[] GetBytes(double value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            return bytes;
        }

        public void GetBytes(double value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
        }

        public void GetBytes(double value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
        }

        public double ToDouble(byte* array) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            return value;
        }

        public void ToValue(byte* array, out double oValue) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            oValue = value;
        }

        public double ToDouble(IList<byte> array, int index) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out double oValue) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            oValue = value;
        }

        public System.Byte[] GetBytes(decimal value) {
            var val = (byte*)&value;
            var bytes = new byte[16];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            bytes[8] = *(val + 8);
            bytes[9] = *(val + 9);
            bytes[10] = *(val + 10);
            bytes[11] = *(val + 11);
            bytes[12] = *(val + 12);
            bytes[13] = *(val + 13);
            bytes[14] = *(val + 14);
            bytes[15] = *(val + 15);
            return bytes;
        }

        public void GetBytes(decimal value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
            *(array + 8) = *(val + 8);
            *(array + 9) = *(val + 9);
            *(array + 10) = *(val + 10);
            *(array + 11) = *(val + 11);
            *(array + 12) = *(val + 12);
            *(array + 13) = *(val + 13);
            *(array + 14) = *(val + 14);
            *(array + 15) = *(val + 15);
        }

        public void GetBytes(decimal value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
            array[index + 8] = *(val + 8);
            array[index + 9] = *(val + 9);
            array[index + 10] = *(val + 10);
            array[index + 11] = *(val + 11);
            array[index + 12] = *(val + 12);
            array[index + 13] = *(val + 13);
            array[index + 14] = *(val + 14);
            array[index + 15] = *(val + 15);
        }

        public decimal ToDecimal(byte* array) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            *(val + 12) = *(array + 12);
            *(val + 13) = *(array + 13);
            *(val + 14) = *(array + 14);
            *(val + 15) = *(array + 15);
            return value;
        }

        public void ToValue(byte* array, out decimal oValue) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            *(val + 12) = *(array + 12);
            *(val + 13) = *(array + 13);
            *(val + 14) = *(array + 14);
            *(val + 15) = *(array + 15);
            oValue = value;
        }

        public decimal ToDecimal(IList<byte> array, int index) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            *(val + 12) = array[index + 12];
            *(val + 13) = array[index + 13];
            *(val + 14) = array[index + 14];
            *(val + 15) = array[index + 15];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out decimal oValue) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            *(val + 12) = array[index + 12];
            *(val + 13) = array[index + 13];
            *(val + 14) = array[index + 14];
            *(val + 15) = array[index + 15];
            oValue = value;
        }

        public System.Byte[] GetBytes(char value) {
            var val = (byte*)&value;
            var bytes = new byte[2];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            return bytes;
        }

        public void GetBytes(char value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
        }

        public void GetBytes(char value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
        }

        public char ToChar(byte* array) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            return value;
        }

        public void ToValue(byte* array, out char oValue) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            oValue = value;
        }

        public char ToChar(IList<byte> array, int index) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out char oValue) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            oValue = value;
        }

        public System.Byte[] GetBytes(Vector2 value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            return bytes;
        }

        public void GetBytes(Vector2 value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
        }

        public void GetBytes(Vector2 value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
        }

        public Vector2 ToVector2(byte* array) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            return value;
        }

        public void ToValue(byte* array, out Vector2 oValue) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            oValue = value;
        }

        public Vector2 ToVector2(IList<byte> array, int index) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Vector2 oValue) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            oValue = value;
        }

        public System.Byte[] GetBytes(Vector3 value) {
            var val = (byte*)&value;
            var bytes = new byte[12];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            bytes[8] = *(val + 8);
            bytes[9] = *(val + 9);
            bytes[10] = *(val + 10);
            bytes[11] = *(val + 11);
            return bytes;
        }

        public void GetBytes(Vector3 value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
            *(array + 8) = *(val + 8);
            *(array + 9) = *(val + 9);
            *(array + 10) = *(val + 10);
            *(array + 11) = *(val + 11);
        }

        public void GetBytes(Vector3 value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
            array[index + 8] = *(val + 8);
            array[index + 9] = *(val + 9);
            array[index + 10] = *(val + 10);
            array[index + 11] = *(val + 11);
        }

        public Vector3 ToVector3(byte* array) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            return value;
        }

        public void ToValue(byte* array, out Vector3 oValue) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            oValue = value;
        }

        public Vector3 ToVector3(IList<byte> array, int index) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Vector3 oValue) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            oValue = value;
        }

        public System.Byte[] GetBytes(Vector4 value) {
            var val = (byte*)&value;
            var bytes = new byte[16];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            bytes[8] = *(val + 8);
            bytes[9] = *(val + 9);
            bytes[10] = *(val + 10);
            bytes[11] = *(val + 11);
            bytes[12] = *(val + 12);
            bytes[13] = *(val + 13);
            bytes[14] = *(val + 14);
            bytes[15] = *(val + 15);
            return bytes;
        }

        public void GetBytes(Vector4 value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
            *(array + 8) = *(val + 8);
            *(array + 9) = *(val + 9);
            *(array + 10) = *(val + 10);
            *(array + 11) = *(val + 11);
            *(array + 12) = *(val + 12);
            *(array + 13) = *(val + 13);
            *(array + 14) = *(val + 14);
            *(array + 15) = *(val + 15);
        }

        public void GetBytes(Vector4 value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
            array[index + 8] = *(val + 8);
            array[index + 9] = *(val + 9);
            array[index + 10] = *(val + 10);
            array[index + 11] = *(val + 11);
            array[index + 12] = *(val + 12);
            array[index + 13] = *(val + 13);
            array[index + 14] = *(val + 14);
            array[index + 15] = *(val + 15);
        }

        public Vector4 ToVector4(byte* array) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            *(val + 12) = *(array + 12);
            *(val + 13) = *(array + 13);
            *(val + 14) = *(array + 14);
            *(val + 15) = *(array + 15);
            return value;
        }

        public void ToValue(byte* array, out Vector4 oValue) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            *(val + 12) = *(array + 12);
            *(val + 13) = *(array + 13);
            *(val + 14) = *(array + 14);
            *(val + 15) = *(array + 15);
            oValue = value;
        }

        public Vector4 ToVector4(IList<byte> array, int index) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            *(val + 12) = array[index + 12];
            *(val + 13) = array[index + 13];
            *(val + 14) = array[index + 14];
            *(val + 15) = array[index + 15];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Vector4 oValue) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            *(val + 12) = array[index + 12];
            *(val + 13) = array[index + 13];
            *(val + 14) = array[index + 14];
            *(val + 15) = array[index + 15];
            oValue = value;
        }

        public System.Byte[] GetBytes(Quaternion value) {
            var val = (byte*)&value;
            var bytes = new byte[16];
            bytes[0] = *(val + 0);
            bytes[1] = *(val + 1);
            bytes[2] = *(val + 2);
            bytes[3] = *(val + 3);
            bytes[4] = *(val + 4);
            bytes[5] = *(val + 5);
            bytes[6] = *(val + 6);
            bytes[7] = *(val + 7);
            bytes[8] = *(val + 8);
            bytes[9] = *(val + 9);
            bytes[10] = *(val + 10);
            bytes[11] = *(val + 11);
            bytes[12] = *(val + 12);
            bytes[13] = *(val + 13);
            bytes[14] = *(val + 14);
            bytes[15] = *(val + 15);
            return bytes;
        }

        public void GetBytes(Quaternion value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
            *(array + 1) = *(val + 1);
            *(array + 2) = *(val + 2);
            *(array + 3) = *(val + 3);
            *(array + 4) = *(val + 4);
            *(array + 5) = *(val + 5);
            *(array + 6) = *(val + 6);
            *(array + 7) = *(val + 7);
            *(array + 8) = *(val + 8);
            *(array + 9) = *(val + 9);
            *(array + 10) = *(val + 10);
            *(array + 11) = *(val + 11);
            *(array + 12) = *(val + 12);
            *(array + 13) = *(val + 13);
            *(array + 14) = *(val + 14);
            *(array + 15) = *(val + 15);
        }

        public void GetBytes(Quaternion value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
            array[index + 1] = *(val + 1);
            array[index + 2] = *(val + 2);
            array[index + 3] = *(val + 3);
            array[index + 4] = *(val + 4);
            array[index + 5] = *(val + 5);
            array[index + 6] = *(val + 6);
            array[index + 7] = *(val + 7);
            array[index + 8] = *(val + 8);
            array[index + 9] = *(val + 9);
            array[index + 10] = *(val + 10);
            array[index + 11] = *(val + 11);
            array[index + 12] = *(val + 12);
            array[index + 13] = *(val + 13);
            array[index + 14] = *(val + 14);
            array[index + 15] = *(val + 15);
        }

        public Quaternion ToQuaternion(byte* array) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            *(val + 12) = *(array + 12);
            *(val + 13) = *(array + 13);
            *(val + 14) = *(array + 14);
            *(val + 15) = *(array + 15);
            return value;
        }

        public void ToValue(byte* array, out Quaternion oValue) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            *(val + 1) = *(array + 1);
            *(val + 2) = *(array + 2);
            *(val + 3) = *(array + 3);
            *(val + 4) = *(array + 4);
            *(val + 5) = *(array + 5);
            *(val + 6) = *(array + 6);
            *(val + 7) = *(array + 7);
            *(val + 8) = *(array + 8);
            *(val + 9) = *(array + 9);
            *(val + 10) = *(array + 10);
            *(val + 11) = *(array + 11);
            *(val + 12) = *(array + 12);
            *(val + 13) = *(array + 13);
            *(val + 14) = *(array + 14);
            *(val + 15) = *(array + 15);
            oValue = value;
        }

        public Quaternion ToQuaternion(IList<byte> array, int index) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            *(val + 12) = array[index + 12];
            *(val + 13) = array[index + 13];
            *(val + 14) = array[index + 14];
            *(val + 15) = array[index + 15];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Quaternion oValue) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            *(val + 1) = array[index + 1];
            *(val + 2) = array[index + 2];
            *(val + 3) = array[index + 3];
            *(val + 4) = array[index + 4];
            *(val + 5) = array[index + 5];
            *(val + 6) = array[index + 6];
            *(val + 7) = array[index + 7];
            *(val + 8) = array[index + 8];
            *(val + 9) = array[index + 9];
            *(val + 10) = array[index + 10];
            *(val + 11) = array[index + 11];
            *(val + 12) = array[index + 12];
            *(val + 13) = array[index + 13];
            *(val + 14) = array[index + 14];
            *(val + 15) = array[index + 15];
            oValue = value;
        }
    }

    internal unsafe class ReversedConverter : IBitConverter {
        public System.Byte[] GetBytes(byte value) {
            var val = (byte*)&value;
            var bytes = new byte[1];
            bytes[0] = *(val + 0);
            return bytes;
        }

        public void GetBytes(byte value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
        }

        public void GetBytes(byte value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
        }

        public byte ToByte(byte* array) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out byte oValue) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            oValue = value;
        }

        public byte ToByte(IList<byte> array, int index) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out byte oValue) {
            byte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(sbyte value) {
            var val = (byte*)&value;
            var bytes = new byte[1];
            bytes[0] = *(val + 0);
            return bytes;
        }

        public void GetBytes(sbyte value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
        }

        public void GetBytes(sbyte value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
        }

        public sbyte ToSbyte(byte* array) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out sbyte oValue) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            oValue = value;
        }

        public sbyte ToSbyte(IList<byte> array, int index) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out sbyte oValue) {
            sbyte value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(bool value) {
            var val = (byte*)&value;
            var bytes = new byte[1];
            bytes[0] = *(val + 0);
            return bytes;
        }

        public void GetBytes(bool value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 0);
        }

        public void GetBytes(bool value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 0);
        }

        public bool ToBool(byte* array) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out bool oValue) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 0);
            oValue = value;
        }

        public bool ToBool(IList<byte> array, int index) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out bool oValue) {
            bool value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(short value) {
            var val = (byte*)&value;
            var bytes = new byte[2];
            bytes[0] = *(val + 1);
            bytes[1] = *(val + 0);
            return bytes;
        }

        public void GetBytes(short value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 1);
            *(array + 1) = *(val + 0);
        }

        public void GetBytes(short value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 1);
            array[index + 1] = *(val + 0);
        }

        public short ToShort(byte* array) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 1);
            *(val + 1) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out short oValue) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 1);
            *(val + 1) = *(array + 0);
            oValue = value;
        }

        public short ToShort(IList<byte> array, int index) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 1];
            *(val + 1) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out short oValue) {
            short value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 1];
            *(val + 1) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(ushort value) {
            var val = (byte*)&value;
            var bytes = new byte[2];
            bytes[0] = *(val + 1);
            bytes[1] = *(val + 0);
            return bytes;
        }

        public void GetBytes(ushort value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 1);
            *(array + 1) = *(val + 0);
        }

        public void GetBytes(ushort value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 1);
            array[index + 1] = *(val + 0);
        }

        public ushort ToUshort(byte* array) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 1);
            *(val + 1) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out ushort oValue) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 1);
            *(val + 1) = *(array + 0);
            oValue = value;
        }

        public ushort ToUshort(IList<byte> array, int index) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 1];
            *(val + 1) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out ushort oValue) {
            ushort value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 1];
            *(val + 1) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(uint value) {
            var val = (byte*)&value;
            var bytes = new byte[4];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            return bytes;
        }

        public void GetBytes(uint value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
        }

        public void GetBytes(uint value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
        }

        public uint ToUint(byte* array) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out uint oValue) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            oValue = value;
        }

        public uint ToUint(IList<byte> array, int index) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out uint oValue) {
            uint value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(int value) {
            var val = (byte*)&value;
            var bytes = new byte[4];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            return bytes;
        }

        public void GetBytes(int value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
        }

        public void GetBytes(int value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
        }

        public int ToInt(byte* array) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out int oValue) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            oValue = value;
        }

        public int ToInt(IList<byte> array, int index) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out int oValue) {
            int value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(ulong value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 7);
            bytes[1] = *(val + 6);
            bytes[2] = *(val + 5);
            bytes[3] = *(val + 4);
            bytes[4] = *(val + 3);
            bytes[5] = *(val + 2);
            bytes[6] = *(val + 1);
            bytes[7] = *(val + 0);
            return bytes;
        }

        public void GetBytes(ulong value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 7);
            *(array + 1) = *(val + 6);
            *(array + 2) = *(val + 5);
            *(array + 3) = *(val + 4);
            *(array + 4) = *(val + 3);
            *(array + 5) = *(val + 2);
            *(array + 6) = *(val + 1);
            *(array + 7) = *(val + 0);
        }

        public void GetBytes(ulong value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 7);
            array[index + 1] = *(val + 6);
            array[index + 2] = *(val + 5);
            array[index + 3] = *(val + 4);
            array[index + 4] = *(val + 3);
            array[index + 5] = *(val + 2);
            array[index + 6] = *(val + 1);
            array[index + 7] = *(val + 0);
        }

        public ulong ToUlong(byte* array) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 7);
            *(val + 1) = *(array + 6);
            *(val + 2) = *(array + 5);
            *(val + 3) = *(array + 4);
            *(val + 4) = *(array + 3);
            *(val + 5) = *(array + 2);
            *(val + 6) = *(array + 1);
            *(val + 7) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out ulong oValue) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 7);
            *(val + 1) = *(array + 6);
            *(val + 2) = *(array + 5);
            *(val + 3) = *(array + 4);
            *(val + 4) = *(array + 3);
            *(val + 5) = *(array + 2);
            *(val + 6) = *(array + 1);
            *(val + 7) = *(array + 0);
            oValue = value;
        }

        public ulong ToUlong(IList<byte> array, int index) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 7];
            *(val + 1) = array[index + 6];
            *(val + 2) = array[index + 5];
            *(val + 3) = array[index + 4];
            *(val + 4) = array[index + 3];
            *(val + 5) = array[index + 2];
            *(val + 6) = array[index + 1];
            *(val + 7) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out ulong oValue) {
            ulong value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 7];
            *(val + 1) = array[index + 6];
            *(val + 2) = array[index + 5];
            *(val + 3) = array[index + 4];
            *(val + 4) = array[index + 3];
            *(val + 5) = array[index + 2];
            *(val + 6) = array[index + 1];
            *(val + 7) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(long value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 7);
            bytes[1] = *(val + 6);
            bytes[2] = *(val + 5);
            bytes[3] = *(val + 4);
            bytes[4] = *(val + 3);
            bytes[5] = *(val + 2);
            bytes[6] = *(val + 1);
            bytes[7] = *(val + 0);
            return bytes;
        }

        public void GetBytes(long value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 7);
            *(array + 1) = *(val + 6);
            *(array + 2) = *(val + 5);
            *(array + 3) = *(val + 4);
            *(array + 4) = *(val + 3);
            *(array + 5) = *(val + 2);
            *(array + 6) = *(val + 1);
            *(array + 7) = *(val + 0);
        }

        public void GetBytes(long value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 7);
            array[index + 1] = *(val + 6);
            array[index + 2] = *(val + 5);
            array[index + 3] = *(val + 4);
            array[index + 4] = *(val + 3);
            array[index + 5] = *(val + 2);
            array[index + 6] = *(val + 1);
            array[index + 7] = *(val + 0);
        }

        public long ToLong(byte* array) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 7);
            *(val + 1) = *(array + 6);
            *(val + 2) = *(array + 5);
            *(val + 3) = *(array + 4);
            *(val + 4) = *(array + 3);
            *(val + 5) = *(array + 2);
            *(val + 6) = *(array + 1);
            *(val + 7) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out long oValue) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 7);
            *(val + 1) = *(array + 6);
            *(val + 2) = *(array + 5);
            *(val + 3) = *(array + 4);
            *(val + 4) = *(array + 3);
            *(val + 5) = *(array + 2);
            *(val + 6) = *(array + 1);
            *(val + 7) = *(array + 0);
            oValue = value;
        }

        public long ToLong(IList<byte> array, int index) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 7];
            *(val + 1) = array[index + 6];
            *(val + 2) = array[index + 5];
            *(val + 3) = array[index + 4];
            *(val + 4) = array[index + 3];
            *(val + 5) = array[index + 2];
            *(val + 6) = array[index + 1];
            *(val + 7) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out long oValue) {
            long value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 7];
            *(val + 1) = array[index + 6];
            *(val + 2) = array[index + 5];
            *(val + 3) = array[index + 4];
            *(val + 4) = array[index + 3];
            *(val + 5) = array[index + 2];
            *(val + 6) = array[index + 1];
            *(val + 7) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(float value) {
            var val = (byte*)&value;
            var bytes = new byte[4];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            return bytes;
        }

        public void GetBytes(float value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
        }

        public void GetBytes(float value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
        }

        public float ToFloat(byte* array) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out float oValue) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            oValue = value;
        }

        public float ToFloat(IList<byte> array, int index) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out float oValue) {
            float value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(double value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 7);
            bytes[1] = *(val + 6);
            bytes[2] = *(val + 5);
            bytes[3] = *(val + 4);
            bytes[4] = *(val + 3);
            bytes[5] = *(val + 2);
            bytes[6] = *(val + 1);
            bytes[7] = *(val + 0);
            return bytes;
        }

        public void GetBytes(double value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 7);
            *(array + 1) = *(val + 6);
            *(array + 2) = *(val + 5);
            *(array + 3) = *(val + 4);
            *(array + 4) = *(val + 3);
            *(array + 5) = *(val + 2);
            *(array + 6) = *(val + 1);
            *(array + 7) = *(val + 0);
        }

        public void GetBytes(double value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 7);
            array[index + 1] = *(val + 6);
            array[index + 2] = *(val + 5);
            array[index + 3] = *(val + 4);
            array[index + 4] = *(val + 3);
            array[index + 5] = *(val + 2);
            array[index + 6] = *(val + 1);
            array[index + 7] = *(val + 0);
        }

        public double ToDouble(byte* array) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 7);
            *(val + 1) = *(array + 6);
            *(val + 2) = *(array + 5);
            *(val + 3) = *(array + 4);
            *(val + 4) = *(array + 3);
            *(val + 5) = *(array + 2);
            *(val + 6) = *(array + 1);
            *(val + 7) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out double oValue) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 7);
            *(val + 1) = *(array + 6);
            *(val + 2) = *(array + 5);
            *(val + 3) = *(array + 4);
            *(val + 4) = *(array + 3);
            *(val + 5) = *(array + 2);
            *(val + 6) = *(array + 1);
            *(val + 7) = *(array + 0);
            oValue = value;
        }

        public double ToDouble(IList<byte> array, int index) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 7];
            *(val + 1) = array[index + 6];
            *(val + 2) = array[index + 5];
            *(val + 3) = array[index + 4];
            *(val + 4) = array[index + 3];
            *(val + 5) = array[index + 2];
            *(val + 6) = array[index + 1];
            *(val + 7) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out double oValue) {
            double value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 7];
            *(val + 1) = array[index + 6];
            *(val + 2) = array[index + 5];
            *(val + 3) = array[index + 4];
            *(val + 4) = array[index + 3];
            *(val + 5) = array[index + 2];
            *(val + 6) = array[index + 1];
            *(val + 7) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(decimal value) {
            var val = (byte*)&value;
            var bytes = new byte[16];
            bytes[0] = *(val + 15);
            bytes[1] = *(val + 14);
            bytes[2] = *(val + 13);
            bytes[3] = *(val + 12);
            bytes[4] = *(val + 11);
            bytes[5] = *(val + 10);
            bytes[6] = *(val + 9);
            bytes[7] = *(val + 8);
            bytes[8] = *(val + 7);
            bytes[9] = *(val + 6);
            bytes[10] = *(val + 5);
            bytes[11] = *(val + 4);
            bytes[12] = *(val + 3);
            bytes[13] = *(val + 2);
            bytes[14] = *(val + 1);
            bytes[15] = *(val + 0);
            return bytes;
        }

        public void GetBytes(decimal value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 15);
            *(array + 1) = *(val + 14);
            *(array + 2) = *(val + 13);
            *(array + 3) = *(val + 12);
            *(array + 4) = *(val + 11);
            *(array + 5) = *(val + 10);
            *(array + 6) = *(val + 9);
            *(array + 7) = *(val + 8);
            *(array + 8) = *(val + 7);
            *(array + 9) = *(val + 6);
            *(array + 10) = *(val + 5);
            *(array + 11) = *(val + 4);
            *(array + 12) = *(val + 3);
            *(array + 13) = *(val + 2);
            *(array + 14) = *(val + 1);
            *(array + 15) = *(val + 0);
        }

        public void GetBytes(decimal value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 15);
            array[index + 1] = *(val + 14);
            array[index + 2] = *(val + 13);
            array[index + 3] = *(val + 12);
            array[index + 4] = *(val + 11);
            array[index + 5] = *(val + 10);
            array[index + 6] = *(val + 9);
            array[index + 7] = *(val + 8);
            array[index + 8] = *(val + 7);
            array[index + 9] = *(val + 6);
            array[index + 10] = *(val + 5);
            array[index + 11] = *(val + 4);
            array[index + 12] = *(val + 3);
            array[index + 13] = *(val + 2);
            array[index + 14] = *(val + 1);
            array[index + 15] = *(val + 0);
        }

        public decimal ToDecimal(byte* array) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 15);
            *(val + 1) = *(array + 14);
            *(val + 2) = *(array + 13);
            *(val + 3) = *(array + 12);
            *(val + 4) = *(array + 11);
            *(val + 5) = *(array + 10);
            *(val + 6) = *(array + 9);
            *(val + 7) = *(array + 8);
            *(val + 8) = *(array + 7);
            *(val + 9) = *(array + 6);
            *(val + 10) = *(array + 5);
            *(val + 11) = *(array + 4);
            *(val + 12) = *(array + 3);
            *(val + 13) = *(array + 2);
            *(val + 14) = *(array + 1);
            *(val + 15) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out decimal oValue) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 15);
            *(val + 1) = *(array + 14);
            *(val + 2) = *(array + 13);
            *(val + 3) = *(array + 12);
            *(val + 4) = *(array + 11);
            *(val + 5) = *(array + 10);
            *(val + 6) = *(array + 9);
            *(val + 7) = *(array + 8);
            *(val + 8) = *(array + 7);
            *(val + 9) = *(array + 6);
            *(val + 10) = *(array + 5);
            *(val + 11) = *(array + 4);
            *(val + 12) = *(array + 3);
            *(val + 13) = *(array + 2);
            *(val + 14) = *(array + 1);
            *(val + 15) = *(array + 0);
            oValue = value;
        }

        public decimal ToDecimal(IList<byte> array, int index) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 15];
            *(val + 1) = array[index + 14];
            *(val + 2) = array[index + 13];
            *(val + 3) = array[index + 12];
            *(val + 4) = array[index + 11];
            *(val + 5) = array[index + 10];
            *(val + 6) = array[index + 9];
            *(val + 7) = array[index + 8];
            *(val + 8) = array[index + 7];
            *(val + 9) = array[index + 6];
            *(val + 10) = array[index + 5];
            *(val + 11) = array[index + 4];
            *(val + 12) = array[index + 3];
            *(val + 13) = array[index + 2];
            *(val + 14) = array[index + 1];
            *(val + 15) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out decimal oValue) {
            decimal value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 15];
            *(val + 1) = array[index + 14];
            *(val + 2) = array[index + 13];
            *(val + 3) = array[index + 12];
            *(val + 4) = array[index + 11];
            *(val + 5) = array[index + 10];
            *(val + 6) = array[index + 9];
            *(val + 7) = array[index + 8];
            *(val + 8) = array[index + 7];
            *(val + 9) = array[index + 6];
            *(val + 10) = array[index + 5];
            *(val + 11) = array[index + 4];
            *(val + 12) = array[index + 3];
            *(val + 13) = array[index + 2];
            *(val + 14) = array[index + 1];
            *(val + 15) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(char value) {
            var val = (byte*)&value;
            var bytes = new byte[2];
            bytes[0] = *(val + 1);
            bytes[1] = *(val + 0);
            return bytes;
        }

        public void GetBytes(char value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 1);
            *(array + 1) = *(val + 0);
        }

        public void GetBytes(char value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 1);
            array[index + 1] = *(val + 0);
        }

        public char ToChar(byte* array) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 1);
            *(val + 1) = *(array + 0);
            return value;
        }

        public void ToValue(byte* array, out char oValue) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 1);
            *(val + 1) = *(array + 0);
            oValue = value;
        }

        public char ToChar(IList<byte> array, int index) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 1];
            *(val + 1) = array[index + 0];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out char oValue) {
            char value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 1];
            *(val + 1) = array[index + 0];
            oValue = value;
        }

        public System.Byte[] GetBytes(Vector2 value) {
            var val = (byte*)&value;
            var bytes = new byte[8];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            bytes[4] = *(val + 7);
            bytes[5] = *(val + 6);
            bytes[6] = *(val + 5);
            bytes[7] = *(val + 4);
            return bytes;
        }

        public void GetBytes(Vector2 value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
            *(array + 4) = *(val + 7);
            *(array + 5) = *(val + 6);
            *(array + 6) = *(val + 5);
            *(array + 7) = *(val + 4);
        }

        public void GetBytes(Vector2 value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
            array[index + 4] = *(val + 7);
            array[index + 5] = *(val + 6);
            array[index + 6] = *(val + 5);
            array[index + 7] = *(val + 4);
        }

        public Vector2 ToVector2(byte* array) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            return value;
        }

        public void ToValue(byte* array, out Vector2 oValue) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            oValue = value;
        }

        public Vector2 ToVector2(IList<byte> array, int index) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Vector2 oValue) {
            Vector2 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            oValue = value;
        }

        public System.Byte[] GetBytes(Vector3 value) {
            var val = (byte*)&value;
            var bytes = new byte[12];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            bytes[4] = *(val + 7);
            bytes[5] = *(val + 6);
            bytes[6] = *(val + 5);
            bytes[7] = *(val + 4);
            bytes[8] = *(val + 11);
            bytes[9] = *(val + 10);
            bytes[10] = *(val + 9);
            bytes[11] = *(val + 8);
            return bytes;
        }

        public void GetBytes(Vector3 value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
            *(array + 4) = *(val + 7);
            *(array + 5) = *(val + 6);
            *(array + 6) = *(val + 5);
            *(array + 7) = *(val + 4);
            *(array + 8) = *(val + 11);
            *(array + 9) = *(val + 10);
            *(array + 10) = *(val + 9);
            *(array + 11) = *(val + 8);
        }

        public void GetBytes(Vector3 value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
            array[index + 4] = *(val + 7);
            array[index + 5] = *(val + 6);
            array[index + 6] = *(val + 5);
            array[index + 7] = *(val + 4);
            array[index + 8] = *(val + 11);
            array[index + 9] = *(val + 10);
            array[index + 10] = *(val + 9);
            array[index + 11] = *(val + 8);
        }

        public Vector3 ToVector3(byte* array) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            *(val + 8) = *(array + 11);
            *(val + 9) = *(array + 10);
            *(val + 10) = *(array + 9);
            *(val + 11) = *(array + 8);
            return value;
        }

        public void ToValue(byte* array, out Vector3 oValue) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            *(val + 8) = *(array + 11);
            *(val + 9) = *(array + 10);
            *(val + 10) = *(array + 9);
            *(val + 11) = *(array + 8);
            oValue = value;
        }

        public Vector3 ToVector3(IList<byte> array, int index) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            *(val + 8) = array[index + 11];
            *(val + 9) = array[index + 10];
            *(val + 10) = array[index + 9];
            *(val + 11) = array[index + 8];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Vector3 oValue) {
            Vector3 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            *(val + 8) = array[index + 11];
            *(val + 9) = array[index + 10];
            *(val + 10) = array[index + 9];
            *(val + 11) = array[index + 8];
            oValue = value;
        }

        public System.Byte[] GetBytes(Vector4 value) {
            var val = (byte*)&value;
            var bytes = new byte[16];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            bytes[4] = *(val + 7);
            bytes[5] = *(val + 6);
            bytes[6] = *(val + 5);
            bytes[7] = *(val + 4);
            bytes[8] = *(val + 11);
            bytes[9] = *(val + 10);
            bytes[10] = *(val + 9);
            bytes[11] = *(val + 8);
            bytes[12] = *(val + 15);
            bytes[13] = *(val + 14);
            bytes[14] = *(val + 13);
            bytes[15] = *(val + 12);
            return bytes;
        }

        public void GetBytes(Vector4 value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
            *(array + 4) = *(val + 7);
            *(array + 5) = *(val + 6);
            *(array + 6) = *(val + 5);
            *(array + 7) = *(val + 4);
            *(array + 8) = *(val + 11);
            *(array + 9) = *(val + 10);
            *(array + 10) = *(val + 9);
            *(array + 11) = *(val + 8);
            *(array + 12) = *(val + 15);
            *(array + 13) = *(val + 14);
            *(array + 14) = *(val + 13);
            *(array + 15) = *(val + 12);
        }

        public void GetBytes(Vector4 value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
            array[index + 4] = *(val + 7);
            array[index + 5] = *(val + 6);
            array[index + 6] = *(val + 5);
            array[index + 7] = *(val + 4);
            array[index + 8] = *(val + 11);
            array[index + 9] = *(val + 10);
            array[index + 10] = *(val + 9);
            array[index + 11] = *(val + 8);
            array[index + 12] = *(val + 15);
            array[index + 13] = *(val + 14);
            array[index + 14] = *(val + 13);
            array[index + 15] = *(val + 12);
        }

        public Vector4 ToVector4(byte* array) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            *(val + 8) = *(array + 11);
            *(val + 9) = *(array + 10);
            *(val + 10) = *(array + 9);
            *(val + 11) = *(array + 8);
            *(val + 12) = *(array + 15);
            *(val + 13) = *(array + 14);
            *(val + 14) = *(array + 13);
            *(val + 15) = *(array + 12);
            return value;
        }

        public void ToValue(byte* array, out Vector4 oValue) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            *(val + 8) = *(array + 11);
            *(val + 9) = *(array + 10);
            *(val + 10) = *(array + 9);
            *(val + 11) = *(array + 8);
            *(val + 12) = *(array + 15);
            *(val + 13) = *(array + 14);
            *(val + 14) = *(array + 13);
            *(val + 15) = *(array + 12);
            oValue = value;
        }

        public Vector4 ToVector4(IList<byte> array, int index) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            *(val + 8) = array[index + 11];
            *(val + 9) = array[index + 10];
            *(val + 10) = array[index + 9];
            *(val + 11) = array[index + 8];
            *(val + 12) = array[index + 15];
            *(val + 13) = array[index + 14];
            *(val + 14) = array[index + 13];
            *(val + 15) = array[index + 12];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Vector4 oValue) {
            Vector4 value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            *(val + 8) = array[index + 11];
            *(val + 9) = array[index + 10];
            *(val + 10) = array[index + 9];
            *(val + 11) = array[index + 8];
            *(val + 12) = array[index + 15];
            *(val + 13) = array[index + 14];
            *(val + 14) = array[index + 13];
            *(val + 15) = array[index + 12];
            oValue = value;
        }

        public System.Byte[] GetBytes(Quaternion value) {
            var val = (byte*)&value;
            var bytes = new byte[16];
            bytes[0] = *(val + 3);
            bytes[1] = *(val + 2);
            bytes[2] = *(val + 1);
            bytes[3] = *(val + 0);
            bytes[4] = *(val + 7);
            bytes[5] = *(val + 6);
            bytes[6] = *(val + 5);
            bytes[7] = *(val + 4);
            bytes[8] = *(val + 11);
            bytes[9] = *(val + 10);
            bytes[10] = *(val + 9);
            bytes[11] = *(val + 8);
            bytes[12] = *(val + 15);
            bytes[13] = *(val + 14);
            bytes[14] = *(val + 13);
            bytes[15] = *(val + 12);
            return bytes;
        }

        public void GetBytes(Quaternion value, byte* array) {
            var val = (byte*)&value;
            *(array + 0) = *(val + 3);
            *(array + 1) = *(val + 2);
            *(array + 2) = *(val + 1);
            *(array + 3) = *(val + 0);
            *(array + 4) = *(val + 7);
            *(array + 5) = *(val + 6);
            *(array + 6) = *(val + 5);
            *(array + 7) = *(val + 4);
            *(array + 8) = *(val + 11);
            *(array + 9) = *(val + 10);
            *(array + 10) = *(val + 9);
            *(array + 11) = *(val + 8);
            *(array + 12) = *(val + 15);
            *(array + 13) = *(val + 14);
            *(array + 14) = *(val + 13);
            *(array + 15) = *(val + 12);
        }

        public void GetBytes(Quaternion value, IList<byte> array, int index) {
            var val = (byte*)&value;
            array[index + 0] = *(val + 3);
            array[index + 1] = *(val + 2);
            array[index + 2] = *(val + 1);
            array[index + 3] = *(val + 0);
            array[index + 4] = *(val + 7);
            array[index + 5] = *(val + 6);
            array[index + 6] = *(val + 5);
            array[index + 7] = *(val + 4);
            array[index + 8] = *(val + 11);
            array[index + 9] = *(val + 10);
            array[index + 10] = *(val + 9);
            array[index + 11] = *(val + 8);
            array[index + 12] = *(val + 15);
            array[index + 13] = *(val + 14);
            array[index + 14] = *(val + 13);
            array[index + 15] = *(val + 12);
        }

        public Quaternion ToQuaternion(byte* array) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            *(val + 8) = *(array + 11);
            *(val + 9) = *(array + 10);
            *(val + 10) = *(array + 9);
            *(val + 11) = *(array + 8);
            *(val + 12) = *(array + 15);
            *(val + 13) = *(array + 14);
            *(val + 14) = *(array + 13);
            *(val + 15) = *(array + 12);
            return value;
        }

        public void ToValue(byte* array, out Quaternion oValue) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = *(array + 3);
            *(val + 1) = *(array + 2);
            *(val + 2) = *(array + 1);
            *(val + 3) = *(array + 0);
            *(val + 4) = *(array + 7);
            *(val + 5) = *(array + 6);
            *(val + 6) = *(array + 5);
            *(val + 7) = *(array + 4);
            *(val + 8) = *(array + 11);
            *(val + 9) = *(array + 10);
            *(val + 10) = *(array + 9);
            *(val + 11) = *(array + 8);
            *(val + 12) = *(array + 15);
            *(val + 13) = *(array + 14);
            *(val + 14) = *(array + 13);
            *(val + 15) = *(array + 12);
            oValue = value;
        }

        public Quaternion ToQuaternion(IList<byte> array, int index) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            *(val + 8) = array[index + 11];
            *(val + 9) = array[index + 10];
            *(val + 10) = array[index + 9];
            *(val + 11) = array[index + 8];
            *(val + 12) = array[index + 15];
            *(val + 13) = array[index + 14];
            *(val + 14) = array[index + 13];
            *(val + 15) = array[index + 12];
            return value;
        }

        public void ToValue(IList<byte> array, int index, out Quaternion oValue) {
            Quaternion value = default;
            var val = (byte*)&value;
            *(val + 0) = array[index + 3];
            *(val + 1) = array[index + 2];
            *(val + 2) = array[index + 1];
            *(val + 3) = array[index + 0];
            *(val + 4) = array[index + 7];
            *(val + 5) = array[index + 6];
            *(val + 6) = array[index + 5];
            *(val + 7) = array[index + 4];
            *(val + 8) = array[index + 11];
            *(val + 9) = array[index + 10];
            *(val + 10) = array[index + 9];
            *(val + 11) = array[index + 8];
            *(val + 12) = array[index + 15];
            *(val + 13) = array[index + 14];
            *(val + 14) = array[index + 13];
            *(val + 15) = array[index + 12];
            oValue = value;
        }
    }
}
