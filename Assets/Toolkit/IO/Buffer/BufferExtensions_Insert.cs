using System;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;
using BitConverter = Toolkit.IO.BitConverter;
using UnityEngine;
using System.Collections.Generic;

namespace Toolkit.IO {
    public static partial class BufferExtensions {

        #region Csharp

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, byte value) {
            if(buffer.Shift(index, 1) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write byte value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, sbyte value) {
            if(buffer.Shift(index, 1) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write sbyte value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, short value) {
            if(buffer.Shift(index, 2) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write short value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, ushort value) {
            if(buffer.Shift(index, 2) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write ushort value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, int value) {
            if(buffer.Shift(index, 4) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write int value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, uint value) {
            if(buffer.Shift(index, 4) && buffer.TryReserve(4, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write uint value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, long value) {
            if(buffer.Shift(index, 8) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write long value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, ulong value) {
            if(buffer.Shift(index, 8) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write ulong value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, float value) {
            if(buffer.Shift(index, 4) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write float value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, double value) {
            if(buffer.Shift(index, 8) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write double value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, decimal value) {
            if(buffer.Shift(index, 16) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write decimal value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, bool value) {
            if(buffer.Shift(index, 1) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write boolean value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, char value) {
            if(buffer.Shift(index, 2) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write char value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, System.DateTime value) {
            if(buffer.Shift(index, 8) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value.Ticks, ptr);
            else
                throw new Exception(TAG + $"Failed to write DateTime value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void InsertEnum<T>(this IBuffer buffer, int index, T value) where T : System.Enum {
            var converted = Convert.ChangeType(value, FastEnum<T>.TypeCode);
            switch(FastEnum<T>.TypeCode) {
                case TypeCode.Byte:
                    Insert(buffer, index, (byte)converted);
                    break;
                case TypeCode.SByte:
                    Insert(buffer, index, (sbyte)converted);
                    break;
                case TypeCode.Int16:
                    Insert(buffer, index, (short)converted);
                    break;
                case TypeCode.UInt16:
                    Insert(buffer, index, (ushort)converted);
                    break;
                case TypeCode.Int32:
                    Insert(buffer, index, (int)converted);
                    break;
                case TypeCode.UInt32:
                    Insert(buffer, index, (uint)converted);
                    break;
                case TypeCode.Int64:
                    Insert(buffer, index, (long)converted);
                    break;
                case TypeCode.UInt64:
                    Insert(buffer, index, (ulong)converted);
                    break;
                default:
                    throw new Exception(TAG + $"Unsupported enum type '{FastEnum<T>.TypeCode}'");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, System.Guid guid) {
            if(buffer.Shift(index, GUID_LENGTH) && buffer.TryGetSpanAt(index, GUID_LENGTH, out var span))
                guid.TryWriteBytes(span);
            else
                throw new Exception(TAG + $"Failed to write Guid value ({guid}) into buffer");
        }

        #endregion

        #region Array Variants

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, IReadOnlyList<byte> values) {
            var len = values.Count;
            var bytesUsed = InsertCompressed(buffer, index, len);
            if(buffer.Shift(index + bytesUsed, len) && buffer.TryGetPointerAt(index + bytesUsed, out var ptr)) {
                for(var i = 0; i < len; i++)
                    ptr[i] = values[i];
            }
            else
                throw new Exception(TAG + $"Failed to write byte array value into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, IReadOnlyList<int> values) {
            var len = values.Count;
            var bytesUsed = InsertCompressed(buffer, index, len);
            if(buffer.Shift(index + bytesUsed, len * 4) && buffer.TryGetPointerAt(index + bytesUsed, out var ptr)) {
                for(var i = 0; i < len; i++)
                    BitConverter.GetBytes(values[i], ptr + i * 4);
            }
            else
                throw new Exception(TAG + $"Failed to write int array value into buffer");
        }

        #endregion

        #region String

        /// <summary>
        /// Using by default, big endian unicode encoding for the strings.
        /// </summary>
        /// <param name="str"></param>
        public static unsafe void Insert(this IBuffer buffer, int index, string str) {
            Insert(buffer, str, index, System.Text.Encoding.BigEndianUnicode);
        }

        public static unsafe void Insert(this IBuffer buffer, string str, int index, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII:
                    Insert(buffer, str, index, System.Text.Encoding.ASCII);
                    break;
                case EncodingType.BigEndianUnicode:
                    Insert(buffer, str, index, System.Text.Encoding.BigEndianUnicode);
                    break;
                case EncodingType.Unicode:
                    Insert(buffer, str, index, System.Text.Encoding.Unicode);
                    break;
                case EncodingType.UTF7:
                    Insert(buffer, str, index, System.Text.Encoding.UTF7);
                    break;
                case EncodingType.UTF8:
                    Insert(buffer, str, index, System.Text.Encoding.UTF8);
                    break;
                case EncodingType.UTF32:
                    Insert(buffer, str, index, System.Text.Encoding.UTF32);
                    break;
            }
        }

        public static unsafe void Insert(this IBuffer buffer, string str, int index, System.Text.Encoding encoding) {
            var len = encoding.GetByteCount(str);
            var bytesUsed = InsertCompressed(buffer, index, len);
            var writeIndex = index + bytesUsed;
            if(buffer.Shift(index + bytesUsed, len))
                encoding.GetBytes(str, 0, str.Length, buffer.Raw, writeIndex);
            else
                throw new Exception(TAG + $"Failed to write string value ({str}) into buffer");
        }

        #endregion

        #region Compressed

        public static unsafe int InsertCompressed(this IBuffer buffer, int index, int value) {
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
                if(buffer.Shift(index, 5) && buffer.TryGetPointerAt(index, out var ptr)) {
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
                    return 5;
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else if(largestIndex > 19) {
                if(buffer.Shift(index, 4) && buffer.TryGetPointerAt(index, out var ptr)) {
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
                    return 4;
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else if(largestIndex > 12) {
                if(buffer.Shift(index, 3) && buffer.TryGetPointerAt(index, out var ptr)) {
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
                    return 3;
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else if(largestIndex > 5) {
                if(buffer.Shift(index, 2) && buffer.TryGetPointerAt(index, out var ptr)) {
                    var initialByte = isPositive ? B.OIOO.OOOO : B.IIOO.OOOO;
                    if(BitConverter.IsLittleEndian) {
                        ptr[0] = (byte)(initialByte | b[1]);
                        ptr[1] = b[0];
                    }
                    else {
                        ptr[0] = (byte)(initialByte | b[2]);
                        ptr[1] = b[3];
                    }
                    return 2;
                }
                else
                    throw new Exception(TAG + $"Failed to write compressed int value ({value}) into buffer");
            }
            else {
                if(buffer.Shift(index, 1) && buffer.TryGetPointerAt(index, out var ptr)) {
                    var initialByte = isPositive ? B.OOOO.OOOO : B.IOOO.OOOO;
                    ptr[0] = (byte)(initialByte | b[0]);
                    return 1;
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

        #endregion

        #region Toolkit Data Types

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, TKDateTime value) {
            if(buffer.Shift(index, 8) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value.Ticks, ptr);
            else
                throw new Exception(TAG + $"Failed to write TKDateTime value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, HSV value) {
            if(buffer.Shift(index, 12) && buffer.TryGetPointerAt(index, out var ptr)) {
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
        public static unsafe void Insert(this IBuffer buffer, int index, Vector2 value) {
            if(buffer.Shift(index, 8) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Vector2 value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, Vector3 value) {
            if(buffer.Shift(index, 12) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Vector3 value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, Vector4 value) {
            if(buffer.Shift(index, 16) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Vector4 value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, Quaternion value) {
            if(buffer.Shift(index, 16) && buffer.TryGetPointerAt(index, out var ptr))
                BitConverter.GetBytes(value, ptr);
            else
                throw new Exception(TAG + $"Failed to write Quaternion value ({value}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, Color color) {
            if(buffer.Shift(index, 16) && buffer.TryGetPointerAt(index, out var ptr)) {
                BitConverter.GetBytes(color.r, ptr + 0);
                BitConverter.GetBytes(color.g, ptr + 4);
                BitConverter.GetBytes(color.b, ptr + 8);
                BitConverter.GetBytes(color.a, ptr + 12);
            }
            else
                throw new Exception(TAG + $"Failed to write Color value ({color}) into buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Insert(this IBuffer buffer, int index, Color32 color) {
            if(buffer.Shift(index, 4) && buffer.TryGetPointerAt(index, out var ptr)) {
                ptr[0] = color.r;
                ptr[1] = color.g;
                ptr[2] = color.b;
                ptr[3] = color.a;
            }
            else
                throw new Exception(TAG + $"Failed to write Color32 value ({color}) into buffer");
        }

        #endregion
    }
}
