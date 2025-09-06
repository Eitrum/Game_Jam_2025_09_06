using System;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;
using BitConverter = Toolkit.IO.BitConverter;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.IO {
    public static partial class BufferExtensions {

        private static unsafe readonly int GUID_LENGTH = sizeof(System.Guid);

        #region Write Cache<T>

        static class WriteCache<T> {

            public delegate void WriteCacheDelegate<TWrite>(IBuffer buffer, TWrite value);
            public delegate void WriteArrayCacheDelegate<TWrite>(IBuffer buffer, IReadOnlyList<TWrite> array);

            public static readonly WriteCacheDelegate<T> WriteCallBack;
            public static readonly WriteArrayCacheDelegate<T> WriteArray;

            static WriteCache() {
                switch(Type<T>.Value) {
                    case byte: {
                            WriteCacheDelegate<byte> tread = Write;
                            WriteArrayCacheDelegate<byte> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case sbyte: {
                            WriteCacheDelegate<sbyte> tread = Write;
                            WriteArrayCacheDelegate<sbyte> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case short: {
                            WriteCacheDelegate<short> tread = Write;
                            WriteArrayCacheDelegate<short> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case ushort: {
                            WriteCacheDelegate<ushort> tread = Write;
                            WriteArrayCacheDelegate<ushort> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case int: {
                            WriteCacheDelegate<int> tread = Write;
                            WriteArrayCacheDelegate<int> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case uint: {
                            WriteCacheDelegate<uint> tread = Write;
                            WriteArrayCacheDelegate<uint> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case long: {
                            WriteCacheDelegate<long> tread = Write;
                            WriteArrayCacheDelegate<long> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case ulong: {
                            WriteCacheDelegate<ulong> tread = Write;
                            WriteArrayCacheDelegate<ulong> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;

                    case float: {
                            WriteCacheDelegate<float> tread = Write;
                            WriteArrayCacheDelegate<float> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case double: {
                            WriteCacheDelegate<double> tread = Write;
                            WriteArrayCacheDelegate<double> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case decimal: {
                            WriteCacheDelegate<decimal> tread = Write;
                            WriteArrayCacheDelegate<decimal> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;

                    case bool: {
                            WriteCacheDelegate<bool> tread = Write;
                            WriteArrayCacheDelegate<bool> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case char: {
                            WriteCacheDelegate<char> tread = Write;
                            WriteArrayCacheDelegate<char> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case DateTime: {
                            WriteCacheDelegate<DateTime> tread = Write;
                            WriteArrayCacheDelegate<DateTime> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Guid: {
                            WriteCacheDelegate<Guid> tread = Write;
                            WriteArrayCacheDelegate<Guid> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case string: {
                            WriteCacheDelegate<string> tread = Write;
                            WriteArrayCacheDelegate<string> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Vector2: {
                            WriteCacheDelegate<Vector2> tread = Write;
                            WriteArrayCacheDelegate<Vector2> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Vector3: {
                            WriteCacheDelegate<Vector3> tread = Write;
                            WriteArrayCacheDelegate<Vector3> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Vector4: {
                            WriteCacheDelegate<Vector4> tread = Write;
                            WriteArrayCacheDelegate<Vector4> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Quaternion: {
                            WriteCacheDelegate<Quaternion> tread = Write;
                            WriteArrayCacheDelegate<Quaternion> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Color: {
                            WriteCacheDelegate<Color> tread = Write;
                            WriteArrayCacheDelegate<Color> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                    case Color32: {
                            WriteCacheDelegate<Color32> tread = Write;
                            WriteArrayCacheDelegate<Color32> treadarr = Write;
                            WriteCallBack = (WriteCacheDelegate<T>)(object)tread;
                            WriteArray = (WriteArrayCacheDelegate<T>)(object)treadarr;
                        }
                        break;
                }

                if(typeof(T).IsEnum) {
                    var methodinfo = typeof(T).GetMethods().FirstOrDefault(x => x.Name == nameof(WriteEnum) && x.GetParameters()[1].ParameterType == typeof(T)).MakeGenericMethod(typeof(T));
                    WriteCallBack = methodinfo.CreateDelegate(typeof(WriteCacheDelegate<T>)) as WriteCacheDelegate<T>;

                    var methodinfo2 = typeof(T).GetMethods().FirstOrDefault(x => x.Name == nameof(WriteEnum) && x.GetParameters()[1].ParameterType != typeof(T)).MakeGenericMethod(typeof(T));
                    WriteArray = methodinfo2.CreateDelegate(typeof(WriteArrayCacheDelegate<T>)) as WriteArrayCacheDelegate<T>;
                }
            }
        }

        #endregion

        #region Csharp

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, byte value) {
            if(buffer.TryReserve(1, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write byte value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, sbyte value) {
            if(buffer.TryReserve(1, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write sbyte value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, short value) {
            if(buffer.TryReserve(2, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write short value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, ushort value) {
            if(buffer.TryReserve(2, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write ushort value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, int value) {
            if(buffer.TryReserve(4, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write int value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, uint value) {
            if(buffer.TryReserve(4, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write uint value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, long value) {
            if(buffer.TryReserve(8, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write long value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, ulong value) {
            if(buffer.TryReserve(8, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write ulong value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, float value) {
            if(buffer.TryReserve(4, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write float value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, double value) {
            if(buffer.TryReserve(8, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write double value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, decimal value) {
            if(buffer.TryReserve(16, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write decimal value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, bool value) {
            if(buffer.TryReserve(1, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write boolean value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, char value) {
            if(buffer.TryReserve(2, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write char value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, System.DateTime value) {
            if(buffer.TryReserve(8, out var ptr))
                BitConverter.GetBytes(value.Ticks, ptr);
            else
                throw new Exception(TAG + $"Failed to write DateTime value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteEnum<T>(this IBuffer buffer, T value) where T : System.Enum {
            var converted = Convert.ChangeType(value, FastEnum<T>.TypeCode);
            switch(FastEnum<T>.TypeCode) {
                case TypeCode.Byte:
                    Write(buffer, (byte)converted);
                    break;
                case TypeCode.SByte:
                    Write(buffer, (sbyte)converted);
                    break;
                case TypeCode.Int16:
                    Write(buffer, (short)converted);
                    break;
                case TypeCode.UInt16:
                    Write(buffer, (ushort)converted);
                    break;
                case TypeCode.Int32:
                    Write(buffer, (int)converted);
                    break;
                case TypeCode.UInt32:
                    Write(buffer, (uint)converted);
                    break;
                case TypeCode.Int64:
                    Write(buffer, (long)converted);
                    break;
                case TypeCode.UInt64:
                    Write(buffer, (ulong)converted);
                    break;
                default:
                    throw new Exception(TAG + $"Unsupported enum type '{FastEnum<T>.TypeCode}'");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, System.Guid guid) {
            if(buffer.TryReserveAsSpan(GUID_LENGTH, out var span))
                guid.TryWriteBytes(span);
            else
                throw new Exception(TAG + $"Failed to write Guid value ({guid}) into buffer");
        }

        #endregion

        #region Array Variants

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<byte> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    ptr[i] = values[i];
            }
            else
                throw new Exception(TAG + $"Failed to write byte array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<sbyte> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i);
            }
            else
                throw new Exception(TAG + $"Failed to write byte array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<short> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 2);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<ushort> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 2);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<int> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 4);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<uint> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 4);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<long> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 8);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<ulong> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 8);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<float> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 4);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<double> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 8);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<decimal> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 16, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 16);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<bool> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<char> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 2, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 2);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<DateTime> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Guid> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<string> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<string> values, EncodingType encodingType) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i], encodingType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<string> values, System.Text.Encoding encodingType) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i], encodingType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteEnum<T>(this IBuffer buffer, IReadOnlyList<T> values) where T : Enum {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                WriteEnum(buffer, values[i]);
        }

        #endregion

        #region String

        /// <summary>
        /// Using by default, big endian unicode encoding for the strings.
        /// </summary>
        /// <param name="str"></param>
        public static unsafe void Write(this IBuffer buffer, string str) {
            Write(buffer, str, System.Text.Encoding.BigEndianUnicode);
        }

        public static unsafe void Write(this IBuffer buffer, string str, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII:
                    Write(buffer, str, System.Text.Encoding.ASCII);
                    break;
                case EncodingType.BigEndianUnicode:
                    Write(buffer, str, System.Text.Encoding.BigEndianUnicode);
                    break;
                case EncodingType.Unicode:
                    Write(buffer, str, System.Text.Encoding.Unicode);
                    break;
                case EncodingType.UTF7:
                    Write(buffer, str, System.Text.Encoding.UTF7);
                    break;
                case EncodingType.UTF8:
                    Write(buffer, str, System.Text.Encoding.UTF8);
                    break;
                case EncodingType.UTF32:
                    Write(buffer, str, System.Text.Encoding.UTF32);
                    break;
            }
        }

        public static unsafe void Write(this IBuffer buffer, string str, System.Text.Encoding encoding) {
            var len = encoding.GetByteCount(str);
            WriteCompressed(buffer, len);
            var index = buffer.Index;
            if(buffer.TryReserve(len, out var ptr))
                encoding.GetBytes(str, 0, str.Length, buffer.Raw, index);
            else
                throw new Exception(TAG + $"Failed to write string value ({str}) into buffer");
        }

        #endregion

        #region Compressed

        public static unsafe void WriteCompressed(this IBuffer buffer, int value) {
            bool isPositive = (value & 0x80_00_00_00) != 0x80_00_00_00;
            int val = isPositive ? value : (~value + 1);

            // Find largest relevant bit
            int largestIndex = -1;
            for(int i = 31; i >= 0; i--) {
                if(((val >> i) & 1) == 1) {
                    largestIndex = i;
                    break;
                }
            }

            byte* b = (byte*)&val;

            if(largestIndex > 26) {
                if(buffer.TryReserve(5, out var ptr)) {
                    ptr[0] = isPositive ? B.OIII.IOOO : B.IIII.IOOO;
                    if(BitConverter.IsLittleEndian) {
                        ptr[1] = b[3];
                        ptr[2] = b[2];
                        ptr[3] = b[1];
                        ptr[4] = b[0];
                    }
                    else {
                        ptr[1] = b[0];
                        ptr[2] = b[1];
                        ptr[3] = b[2];
                        ptr[4] = b[3];
                    }
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else if(largestIndex > 19) {
                if(buffer.TryReserve(4, out var ptr)) {
                    var initialByte = isPositive ? B.OIII.OOOO : B.IIII.OOOO;
                    if(BitConverter.IsLittleEndian) {
                        ptr[0] = (byte)(initialByte | b[3]);
                        ptr[1] = b[2];
                        ptr[2] = b[1];
                        ptr[3] = b[0];
                    }
                    else {
                        ptr[0] = (byte)(initialByte | b[0]);
                        ptr[1] = b[1];
                        ptr[2] = b[2];
                        ptr[3] = b[3];
                    }
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else if(largestIndex > 12) {
                if(buffer.TryReserve(3, out var ptr)) {
                    var initialByte = isPositive ? B.OIIO.OOOO : B.IIIO.OOOO;
                    if(BitConverter.IsLittleEndian) {
                        ptr[0] = (byte)(initialByte | b[2]);
                        ptr[1] = b[1];
                        ptr[2] = b[0];
                    }
                    else {
                        ptr[0] = (byte)(initialByte | b[1]);
                        ptr[1] = b[2];
                        ptr[2] = b[3];
                    }
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else if(largestIndex > 5) {
                if(buffer.TryReserve(2, out var ptr)) {
                    var initialByte = isPositive ? B.OIOO.OOOO : B.IIOO.OOOO;
                    if(BitConverter.IsLittleEndian) {
                        ptr[0] = (byte)(initialByte | b[1]);
                        ptr[1] = b[0];
                    }
                    else {
                        ptr[0] = (byte)(initialByte | b[2]);
                        ptr[1] = b[3];
                    }
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else {
                if(buffer.TryReserve(1, out var ptr)) {
                    var initialByte = isPositive ? B.OOOO.OOOO : B.IOOO.OOOO;
                    ptr[0] = (byte)(initialByte | b[0]);
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }

            // b(sO 00_0000) = 6 bits = 63
            // b(sIO 0_0000 0000_0000) = 13 bits =  8191
            // b(sIIO _0000 0000_0000 0000_0000) = 20 bits = 1_048_575
            // b(sIIIO _000 0000_0000 0000_0000 0000_0000) = 27 bits = 134_217_727
            // b(sIIIIO _00 0000_0000 0000_0000 0000_0000 0000_0000) = 34 bits = 17_179_869_183
        }

        public static void WriteCompressed(this IBuffer buffer, IReadOnlyList<int> values) {
            var count = values.Count;
            buffer.WriteCompressed(count);
            for(int i = 0; i < count; i++)
                WriteCompressed(buffer, values[i]);
        }

        #endregion

        #region Toolkit Data Types

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, TKDateTime value) {
            if(buffer.TryReserve(8, out var ptr))
                BitConverter.GetBytes(value.Ticks, ptr);
            else
                throw new Exception(TAG + $"Failed to write TKDateTime value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, HSV value) {
            if(buffer.TryReserve(12, out var ptr)) {
                BitConverter.GetBytes(value.hue, ptr);
                BitConverter.GetBytes(value.saturation, ptr + 4);
                BitConverter.GetBytes(value.value, ptr + 8);
            }
            else
                throw new Exception(TAG + $"Failed to write HSV value ({value}) into buffer");
        }

        #endregion

        #region Unity

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, Vector2 value) {
            if(buffer.TryReserve(8, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Vector2 value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, Vector3 value) {
            if(buffer.TryReserve(12, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Vector3 value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, Vector4 value) {
            if(buffer.TryReserve(16, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Vector4 value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, Quaternion value) {
            if(buffer.TryReserve(16, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Quaternion value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, Color color) {
            if(buffer.TryReserve(16, out var ptr)) {
                BitConverter.GetBytes(color.r, ptr + 0);
                BitConverter.GetBytes(color.g, ptr + 4);
                BitConverter.GetBytes(color.b, ptr + 8);
                BitConverter.GetBytes(color.a, ptr + 12);
            }
            else
                throw new Exception(TAG + $"Failed to write Color value ({color}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, Color32 color) {
            if(buffer.TryReserve(4, out var ptr)) {
                ptr[0] = color.r;
                ptr[1] = color.g;
                ptr[2] = color.b;
                ptr[3] = color.a;
            }
            else
                throw new Exception(TAG + $"Failed to write Color32 value ({color}) into buffer");
        }

        #endregion

        #region Unity Array Variants

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Vector2> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 8, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 8);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Vector3> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 12, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 12);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Vector4> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 16, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 16);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Quaternion> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            if(buffer.TryReserve(len * 16, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 16);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Color> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IBuffer buffer, IReadOnlyList<Color32> values) {
            var len = values.Count;
            WriteCompressed(buffer, len);
            for(var i = 0; i < len; i++)
                Write(buffer, values[i]);
        }

        #endregion

        #region IBufferSerializable

        public static void Write(this IBuffer buffer, IBufferSerializable bufferSerializable) {
            bufferSerializable.Serialize(buffer);
        }

        #endregion
    }
}
