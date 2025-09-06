using System;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;
using BitConverter = Toolkit.IO.BitConverter;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.IO {
    public static partial class BufferExtensions {
        #region Read Cache<T>

        static class ReadCache<T> {

            public delegate TReturn ReadCacheDelegate<TReturn>(IBuffer buffer);
            public delegate TReturn[] ReadArrayCacheDelegate<TReturn>(IBuffer buffer);

            public static readonly ReadCacheDelegate<T> Read;
            public static readonly ReadArrayCacheDelegate<T> ReadArray;

            static ReadCache() {
                switch(Type<T>.Value) {
                    case byte: {
                            ReadCacheDelegate<byte> tread = ReadByte;
                            ReadArrayCacheDelegate<byte> treadarr = ReadByteArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case sbyte: {
                            ReadCacheDelegate<sbyte> tread = ReadSByte;
                            ReadArrayCacheDelegate<sbyte> treadarr = ReadSByteArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case short: {
                            ReadCacheDelegate<short> tread = ReadShort;
                            ReadArrayCacheDelegate<short> treadarr = ReadShortArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case ushort: {
                            ReadCacheDelegate<ushort> tread = ReadUShort;
                            ReadArrayCacheDelegate<ushort> treadarr = ReadUShortArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case int: {
                            ReadCacheDelegate<int> tread = ReadInt;
                            ReadArrayCacheDelegate<int> treadarr = ReadIntArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case uint: {
                            ReadCacheDelegate<uint> tread = ReadUInt;
                            ReadArrayCacheDelegate<uint> treadarr = ReadUIntArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case long: {
                            ReadCacheDelegate<long> tread = ReadLong;
                            ReadArrayCacheDelegate<long> treadarr = ReadLongArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case ulong: {
                            ReadCacheDelegate<ulong> tread = ReadULong;
                            ReadArrayCacheDelegate<ulong> treadarr = ReadULongArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;

                    case float: {
                            ReadCacheDelegate<float> tread = ReadFloat;
                            ReadArrayCacheDelegate<float> treadarr = ReadFloatArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case double: {
                            ReadCacheDelegate<double> tread = ReadDouble;
                            ReadArrayCacheDelegate<double> treadarr = ReadDoubleArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case decimal: {
                            ReadCacheDelegate<decimal> tread = ReadDecimal;
                            ReadArrayCacheDelegate<decimal> treadarr = ReadDecimalArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;

                    case bool: {
                            ReadCacheDelegate<bool> tread = ReadBool;
                            ReadArrayCacheDelegate<bool> treadarr = ReadBoolArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case char: {
                            ReadCacheDelegate<char> tread = ReadChar;
                            ReadArrayCacheDelegate<char> treadarr = ReadCharArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case DateTime: {
                            ReadCacheDelegate<DateTime> tread = ReadDateTime;
                            ReadArrayCacheDelegate<DateTime> treadarr = ReadDateTimeArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Guid: {
                            ReadCacheDelegate<Guid> tread = ReadGuid;
                            ReadArrayCacheDelegate<Guid> treadarr = ReadGuidArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case string: {
                            ReadCacheDelegate<string> tread = ReadString;
                            ReadArrayCacheDelegate<string> treadarr = ReadStringArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Vector2: {
                            ReadCacheDelegate<Vector2> tread = ReadVector2;
                            ReadArrayCacheDelegate<Vector2> treadarr = ReadVector2Array;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Vector3: {
                            ReadCacheDelegate<Vector3> tread = ReadVector3;
                            ReadArrayCacheDelegate<Vector3> treadarr = ReadVector3Array;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Vector4: {
                            ReadCacheDelegate<Vector4> tread = ReadVector4;
                            ReadArrayCacheDelegate<Vector4> treadarr = ReadVector4Array;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Quaternion: {
                            ReadCacheDelegate<Quaternion> tread = ReadQuaternion;
                            ReadArrayCacheDelegate<Quaternion> treadarr = ReadQuaternionArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Color: {
                            ReadCacheDelegate<Color> tread = ReadColor;
                            ReadArrayCacheDelegate<Color> treadarr = ReadColorArray;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Color32: {
                            ReadCacheDelegate<Color32> tread = ReadColor32;
                            ReadArrayCacheDelegate<Color32> treadarr = ReadColor32Array;
                            Read = (ReadCacheDelegate<T>)(object)tread;
                            ReadArray = (ReadArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                }

                if(typeof(T).IsEnum) {
                    var methodinfo = typeof(T).GetMethods().FirstOrDefault(x => x.Name == nameof(ReadEnum)).MakeGenericMethod(typeof(T));
                    Read = methodinfo.CreateDelegate(typeof(ReadCacheDelegate<T>)) as ReadCacheDelegate<T>;

                    var methodinfo2 = typeof(T).GetMethods().FirstOrDefault(x => x.Name == nameof(ReadEnumArray)).MakeGenericMethod(typeof(T));
                    ReadArray = methodinfo2.CreateDelegate(typeof(ReadArrayCacheDelegate<T>)) as ReadArrayCacheDelegate<T>;
                }
            }
        }

        #endregion

        #region Csharp

        public static unsafe T Read<T>(this IBuffer buffer)
            => ReadCache<T>.Read(buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ReadByte(this IBuffer buffer) {
            if(buffer.TryReserve(1, out var ptr))
                return BitConverter.ToByte(ptr);
            else
                throw new Exception(TAG + $"Failed to read byte value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe sbyte ReadSByte(this IBuffer buffer) {
            if(buffer.TryReserve(1, out var ptr))
                return BitConverter.ToSbyte(ptr);
            else
                throw new Exception(TAG + $"Failed to read sbyte value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe short ReadShort(this IBuffer buffer) {
            if(buffer.TryReserve(2, out var ptr))
                return BitConverter.ToShort(ptr);
            else
                throw new Exception(TAG + $"Failed to read short value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort ReadUShort(this IBuffer buffer) {
            if(buffer.TryReserve(2, out var ptr))
                return BitConverter.ToUshort(ptr);
            else
                throw new Exception(TAG + $"Failed to read ushort value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadInt(this IBuffer buffer) {
            if(buffer.TryReserve(4, out var ptr))
                return BitConverter.ToInt(ptr);
            else
                throw new Exception(TAG + $"Failed to read int value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint ReadUInt(this IBuffer buffer) {
            if(buffer.TryReserve(4, out var ptr))
                return BitConverter.ToUint(ptr);
            else
                throw new Exception(TAG + $"Failed to read uint value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long ReadLong(this IBuffer buffer) {
            if(buffer.TryReserve(8, out var ptr))
                return BitConverter.ToLong(ptr);
            else
                throw new Exception(TAG + $"Failed to read long value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong ReadULong(this IBuffer buffer) {
            if(buffer.TryReserve(8, out var ptr))
                return BitConverter.ToUlong(ptr);
            else
                throw new Exception(TAG + $"Failed to read ulong value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float ReadFloat(this IBuffer buffer) {
            if(buffer.TryReserve(4, out var ptr))
                return BitConverter.ToFloat(ptr);
            else
                throw new Exception(TAG + $"Failed to read float value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double ReadDouble(this IBuffer buffer) {
            if(buffer.TryReserve(8, out var ptr))
                return BitConverter.ToDouble(ptr);
            else
                throw new Exception(TAG + $"Failed to read double value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe decimal ReadDecimal(this IBuffer buffer) {
            if(buffer.TryReserve(16, out var ptr))
                return BitConverter.ToDecimal(ptr);
            else
                throw new Exception(TAG + $"Failed to read decimal value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool ReadBool(this IBuffer buffer) {
            if(buffer.TryReserve(1, out var ptr))
                return BitConverter.ToBool(ptr);
            else
                throw new Exception(TAG + $"Failed to read bool value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool ReadBoolean(this IBuffer buffer) {
            if(buffer.TryReserve(1, out var ptr))
                return BitConverter.ToBool(ptr);
            else
                throw new Exception(TAG + $"Failed to read bool value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe char ReadChar(this IBuffer buffer) {
            if(buffer.TryReserve(2, out var ptr))
                return BitConverter.ToChar(ptr);
            else
                throw new Exception(TAG + $"Failed to read char value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe System.DateTime ReadDateTime(this IBuffer buffer) {
            if(buffer.TryReserve(8, out var ptr))
                return new DateTime(BitConverter.ToLong(ptr));
            else
                throw new Exception(TAG + $"Failed to read DateTime value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T ReadEnum<T>(this IBuffer buffer) where T : System.Enum {
            switch(FastEnum<T>.TypeCode) {
                case TypeCode.Byte: return ReadByte(buffer).ToEnum<T>();
                case TypeCode.SByte: return ReadSByte(buffer).ToEnum<T>();
                case TypeCode.Int16: return ReadShort(buffer).ToEnum<T>();
                case TypeCode.UInt16: return ReadUShort(buffer).ToEnum<T>();
                case TypeCode.Int32: return ReadInt(buffer).ToEnum<T>();
                case TypeCode.UInt32: return ReadUInt(buffer).ToEnum<T>();
                case TypeCode.Int64: return ReadLong(buffer).ToEnum<T>();
                case TypeCode.UInt64: return ReadULong(buffer).ToEnum<T>();
                default:
                    throw new Exception(TAG + $"Unsupported enum type '{FastEnum<T>.TypeCode}'");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe System.Guid ReadGuid(this IBuffer buffer) {
            if(buffer.TryReserveAsSpan(GUID_LENGTH, out var span))
                return new Guid(span);
            else
                throw new Exception(TAG + $"Failed to read Guid value from buffer");
        }

        #endregion

        #region Array Variants

        public static T[] ReadArray<T>(this IBuffer buffer)
            => ReadCache<T>.ReadArray(buffer);

        #region Byte

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] ReadByteArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            byte[] bytes = new byte[len];
            if(buffer.TryReserve(len, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = ptr[i];
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read byte array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, byte[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = ptr[i];
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read byte array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<byte> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(ptr[i]);
            }
            else
                throw new Exception(TAG + $"Failed to read byte array value from buffer");
        }

        #endregion

        #region SByte

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe sbyte[] ReadSByteArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            sbyte[] bytes = new sbyte[len];
            if(buffer.TryReserve(len, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToSbyte(ptr + (i));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, sbyte[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToSbyte(ptr + (i));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<sbyte> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToSbyte(ptr + (i)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region Short

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe short[] ReadShortArray(IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            short[] bytes = new short[len];
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToShort(ptr + (i * 2));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, short[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToShort(ptr + (i * 2));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<short> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToShort(ptr + (i * 2)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region UShort

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort[] ReadUShortArray(IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            ushort[] bytes = new ushort[len];
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToUshort(ptr + (i * 2));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, ushort[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToUshort(ptr + (i * 2));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<ushort> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToUshort(ptr + (i * 2)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region Int

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int[] ReadIntArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            int[] bytes = new int[len];
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToInt(ptr + (i * 4));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, int[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToInt(ptr + (i * 4));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<int> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToInt(ptr + (i * 4)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region UInt

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint[] ReadUIntArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            uint[] bytes = new uint[len];
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToUint(ptr + (i * 4));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, uint[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToUint(ptr + (i * 4));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<uint> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToUint(ptr + (i * 4)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region Long

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long[] ReadLongArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            long[] bytes = new long[len];
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToLong(ptr + (i * 8));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, long[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToLong(ptr + (i * 8));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<long> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToLong(ptr + (i * 8)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region ULong

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong[] ReadULongArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            ulong[] bytes = new ulong[len];
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToUlong(ptr + (i * 8));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, ulong[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToUlong(ptr + (i * 8));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<ulong> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToUlong(ptr + (i * 8)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region Float

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float[] ReadFloatArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            float[] bytes = new float[len];
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToFloat(ptr + (i * 4));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, float[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToFloat(ptr + (i * 4));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<float> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToFloat(ptr + (i * 4)));
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        #endregion

        #region Double

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double[] ReadDoubleArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            double[] bytes = new double[len];
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToDouble(ptr + (i * 8));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, double[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToDouble(ptr + (i * 8));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<double> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToDouble(ptr + (i * 8)));
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        #endregion

        #region Decimal

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe decimal[] ReadDecimalArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            decimal[] bytes = new decimal[len];
            if(buffer.TryReserve(len * 16, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToDecimal(ptr + (i * 16));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, decimal[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToDecimal(ptr + (i * 16));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<decimal> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToDecimal(ptr + (i * 16)));
            }
            else
                throw new Exception(TAG + $"Failed to read float array value from buffer");
        }

        #endregion

        #region Bool

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool[] ReadBoolArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            bool[] bytes = new bool[len];
            if(buffer.TryReserve(len, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToBool(ptr + i);
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read byte array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, bool[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToBool(ptr + i);
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read byte array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<bool> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToBool(ptr + i));
            }
            else
                throw new Exception(TAG + $"Failed to read byte array value from buffer");
        }

        #endregion

        #region Char

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe char[] ReadCharArray(IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            char[] bytes = new char[len];
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToChar(ptr + (i * 2));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, char[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToChar(ptr + (i * 2));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<char> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToChar(ptr + (i * 2)));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region DateTime

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe DateTime[] ReadDateTimeArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            DateTime[] bytes = new DateTime[len];
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = new(BitConverter.ToLong(ptr + (i * 8)));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, DateTime[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = new(BitConverter.ToLong(ptr + (i * 8)));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<DateTime> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(new(BitConverter.ToLong(ptr + (i * 8))));
            }
            else
                throw new Exception(TAG + $"Failed to read int array value from buffer");
        }

        #endregion

        #region Enum

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T[] ReadEnumArray<T>(this IBuffer buffer) where T : System.Enum {
            var len = ReadCompressedInt(buffer);
            T[] bytes = new T[len];
            for(var i = 0; i < len; i++)
                bytes[i] = ReadEnum<T>(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray<T>(this IBuffer buffer, T[] values) where T : System.Enum {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadEnum<T>(buffer);
            for(int i = copyLength; i < len; i++)
                ReadEnum<T>(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray<T>(this IBuffer buffer, List<T> values) where T : System.Enum {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadEnum<T>(buffer));
        }

        #endregion

        #region Guid

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Guid[] ReadGuidArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Guid[] bytes = new Guid[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadGuid(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Guid[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);

            for(int i = 0; i < copyLength; i++)
                values[i] = ReadGuid(buffer);

            for(int i = copyLength; i < len; i++)
                ReadGuid(buffer);

            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Guid> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(int i = 0; i < len; i++)
                values.Add(ReadGuid(buffer));
        }

        #endregion

        #endregion

        #region Compressed

        public static unsafe int ReadCompressedInt(this IBuffer buffer) {
            var initialByte = buffer.ReadByte();
            bool isPositive = (initialByte & B.ByteMask.IOOO_OOOO) != B.ByteMask.IOOO_OOOO;
            int result;

            if((initialByte & B.ByteMask.OIOO_OOOO) != B.ByteMask.OIOO_OOOO) {
                result = (initialByte & B.OOII.IIII);
            }
            else if((initialByte & B.ByteMask.OOIO_OOOO) != B.ByteMask.OOIO_OOOO) {
                result = ((initialByte & B.OOOI.IIII) << 8) | buffer.ReadByte();
            }
            else if((initialByte & B.ByteMask.OOOI_OOOO) != B.ByteMask.OOOI_OOOO) {
                result = ((initialByte & B.OOOO.IIII) << 16) | (buffer.ReadByte() << 8) | buffer.ReadByte();
            }
            else if((initialByte & B.ByteMask.OOOO_IOOO) != B.ByteMask.OOOO_IOOO) {
                result = ((initialByte & B.OOOO.OIII) << 24) | (buffer.ReadByte() << 16) | (buffer.ReadByte() << 8) | buffer.ReadByte();
            }
            else {
                result = (buffer.ReadByte() << 24) | (buffer.ReadByte() << 16) | (buffer.ReadByte() << 8) | buffer.ReadByte();
            }

            return isPositive ? result : -result;
        }

        public static unsafe int[] ReadCompressedArrayInt(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            int[] bytes = new int[len];
            for(var i = 0; i < len; i++)
                bytes[i] = ReadCompressedInt(buffer);
            return bytes;
        }

        #endregion

        #region String

        /// <summary>
        /// Using by default, big endian unicode encoding for the strings.
        /// </summary>
        /// <param name="str"></param>
        public static unsafe string ReadString(this IBuffer buffer) {
            return ReadString(buffer, System.Text.Encoding.BigEndianUnicode);
        }

        public static unsafe string ReadString(this IBuffer buffer, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII: return ReadString(buffer, System.Text.Encoding.ASCII);
                case EncodingType.BigEndianUnicode: return ReadString(buffer, System.Text.Encoding.BigEndianUnicode);
                case EncodingType.Unicode: return ReadString(buffer, System.Text.Encoding.Unicode);
                case EncodingType.UTF7: return ReadString(buffer, System.Text.Encoding.UTF7);
                case EncodingType.UTF8: return ReadString(buffer, System.Text.Encoding.UTF8);
                case EncodingType.UTF32: return ReadString(buffer, System.Text.Encoding.UTF32);
            }
            throw new Exception(TAG + $"Unsupported EncodingType '{encoding}'");
        }

        public static unsafe string ReadString(this IBuffer buffer, System.Text.Encoding encoding) {
            var len = ReadCompressedInt(buffer);
            if(buffer.TryReserve(len, out var ptr))
                return encoding.GetString(ptr, len);
            else
                throw new Exception(TAG + $"Failed to read string value from buffer");
        }

        #region Array Variant

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static unsafe string[] ReadStringArray(this IBuffer buffer) {
            return ReadStringArray(buffer, System.Text.Encoding.BigEndianUnicode);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static unsafe string[] ReadStringArray(this IBuffer buffer, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII: return ReadStringArray(buffer, System.Text.Encoding.ASCII);
                case EncodingType.BigEndianUnicode: return ReadStringArray(buffer, System.Text.Encoding.BigEndianUnicode);
                case EncodingType.Unicode: return ReadStringArray(buffer, System.Text.Encoding.Unicode);
                case EncodingType.UTF7: return ReadStringArray(buffer, System.Text.Encoding.UTF7);
                case EncodingType.UTF8: return ReadStringArray(buffer, System.Text.Encoding.UTF8);
                case EncodingType.UTF32: return ReadStringArray(buffer, System.Text.Encoding.UTF32);
            }
            throw new Exception(TAG + $"Unsupported EncodingType '{encoding}'");
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static unsafe string[] ReadStringArray(this IBuffer buffer, System.Text.Encoding encoding) {
            var len = ReadCompressedInt(buffer);
            string[] strings = new string[len];
            for(int i = 0; i < len; i++)
                strings[i] = ReadString(buffer, encoding);
            return strings;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, string[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadString(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, string[] values, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII: return ReadArray(buffer, values, System.Text.Encoding.ASCII);
                case EncodingType.BigEndianUnicode: return ReadArray(buffer, values, System.Text.Encoding.BigEndianUnicode);
                case EncodingType.Unicode: return ReadArray(buffer, values, System.Text.Encoding.Unicode);
                case EncodingType.UTF7: return ReadArray(buffer, values, System.Text.Encoding.UTF7);
                case EncodingType.UTF8: return ReadArray(buffer, values, System.Text.Encoding.UTF8);
                case EncodingType.UTF32: return ReadArray(buffer, values, System.Text.Encoding.UTF32);
            }
            throw new Exception(TAG + $"Unsupported EncodingType '{encoding}'");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, string[] values, System.Text.Encoding encoding) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadString(buffer, encoding);
            for(int i = copyLength; i < len; i++)
                ReadString(buffer, encoding);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<string> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadString(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<string> values, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII: ReadArray(buffer, values, System.Text.Encoding.ASCII); return;
                case EncodingType.BigEndianUnicode: ReadArray(buffer, values, System.Text.Encoding.BigEndianUnicode); return;
                case EncodingType.Unicode: ReadArray(buffer, values, System.Text.Encoding.Unicode); return;
                case EncodingType.UTF7: ReadArray(buffer, values, System.Text.Encoding.UTF7); return;
                case EncodingType.UTF8: ReadArray(buffer, values, System.Text.Encoding.UTF8); return;
                case EncodingType.UTF32: ReadArray(buffer, values, System.Text.Encoding.UTF32); return;
                default: throw new Exception(TAG + $"Unsupported EncodingType '{encoding}'");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<string> values, System.Text.Encoding encoding) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(int i = 0; i < len; i++)
                values.Add(ReadString(buffer, encoding));
        }

        #endregion

        #endregion

        #region Toolkit Data Types

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe TKDateTime ReadTKDateTime(this IBuffer buffer) {
            if(buffer.TryReserve(8, out var ptr))
                return new TKDateTime(BitConverter.ToLong(ptr));
            else
                throw new Exception(TAG + $"Failed to read TKDateTime value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe HSV ReadHSV(this IBuffer buffer) {
            if(buffer.TryReserve(12, out var ptr)) {
                var h = BitConverter.ToFloat(ptr);
                var s = BitConverter.ToFloat(ptr + 4);
                var v = BitConverter.ToFloat(ptr + 8);
                return new HSV(h, s, v);
            }
            else
                throw new Exception(TAG + $"Failed to read HSV value from buffer");
        }

        #endregion

        #region Unity

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector2 ReadVector2(this IBuffer buffer) {
            if(buffer.TryReserve(8, out var ptr))
                return BitConverter.ToVector2(ptr);
            else
                throw new Exception(TAG + $"Failed to read Vector2 value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector3 ReadVector3(this IBuffer buffer) {
            if(buffer.TryReserve(12, out var ptr))
                return BitConverter.ToVector3(ptr);
            else
                throw new Exception(TAG + $"Failed to read Vector3 value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector4 ReadVector4(this IBuffer buffer) {
            if(buffer.TryReserve(16, out var ptr))
                return BitConverter.ToVector4(ptr);
            else
                throw new Exception(TAG + $"Failed to read Vector4 value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Quaternion ReadQuaternion(this IBuffer buffer) {
            if(buffer.TryReserve(16, out var ptr))
                return BitConverter.ToQuaternion(ptr);
            else
                throw new Exception(TAG + $"Failed to read Quaternion value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Color ReadColor(this IBuffer buffer) {
            if(buffer.TryReserve(16, out var ptr)) {
                var r = BitConverter.ToFloat(ptr + 0);
                var g = BitConverter.ToFloat(ptr + 4);
                var b = BitConverter.ToFloat(ptr + 8);
                var a = BitConverter.ToFloat(ptr + 12);
                return new Color(r, g, b, a);
            }
            else
                throw new Exception(TAG + $"Failed to read Color value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Color32 ReadColor32(this IBuffer buffer) {
            if(buffer.TryReserve(4, out var ptr)) {
                return new Color32(ptr[0], ptr[1], ptr[2], ptr[3]);
            }
            else
                throw new Exception(TAG + $"Failed to read Color32 value from buffer");
        }

        #endregion

        #region Unity Array Variants

        #region Vector2

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector2[] ReadVector2Array(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Vector2[] bytes = new Vector2[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadVector2(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Vector2[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadVector2(buffer);
            for(int i = copyLength; i < len; i++)
                ReadVector2(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Vector2> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadVector2(buffer));
        }

        #endregion

        #region Vector3

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector3[] ReadVector3Array(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Vector3[] bytes = new Vector3[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadVector3(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Vector3[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadVector3(buffer);
            for(int i = copyLength; i < len; i++)
                ReadVector3(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Vector3> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadVector3(buffer));
        }

        #endregion

        #region Vector4

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector4[] ReadVector4Array(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Vector4[] bytes = new Vector4[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadVector4(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Vector4[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadVector4(buffer);
            for(int i = copyLength; i < len; i++)
                ReadVector4(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Vector4> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadVector4(buffer));
        }

        #endregion

        #region Quaternion

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Quaternion[] ReadQuaternionArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Quaternion[] bytes = new Quaternion[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadQuaternion(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Quaternion[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadQuaternion(buffer);
            for(int i = copyLength; i < len; i++)
                ReadQuaternion(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Quaternion> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadQuaternion(buffer));
        }

        #endregion

        #region Color

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Color[] ReadColorArray(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Color[] bytes = new Color[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadColor(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Color[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadColor(buffer);
            for(int i = copyLength; i < len; i++)
                ReadColor(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Color> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadColor(buffer));
        }

        #endregion

        #region Color32

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Color32[] ReadColor32Array(this IBuffer buffer) {
            var len = ReadCompressedInt(buffer);
            Color32[] bytes = new Color32[len];
            for(int i = 0; i < len; i++)
                bytes[i] = ReadColor32(buffer);
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadArray(this IBuffer buffer, Color32[] values) {
            var len = ReadCompressedInt(buffer);
            var copyLength = Mathf.Min(len, values.Length);
            for(int i = 0; i < copyLength; i++)
                values[i] = ReadColor32(buffer);
            for(int i = copyLength; i < len; i++)
                ReadColor32(buffer);
            return copyLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReadArray(this IBuffer buffer, List<Color32> values) {
            values.Clear();
            var len = ReadCompressedInt(buffer);
            for(var i = 0; i < len; i++)
                values.Add(ReadColor32(buffer));
        }

        #endregion

        #endregion

        #region IBuffer Serializable

        public static T ReadAsSerializable<T>(this IBuffer buffer) where T : IBufferSerializable {
            var result = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
            result.Deserialize(buffer);
            return result;
        }

        #endregion
    }
}
