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
        public static unsafe byte PeakByte(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(1, out var ptr))
                return BitConverter.ToByte(ptr);
            else
                throw new Exception(TAG + $"Failed to read byte value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte PeakByte(this IBuffer buffer, int bytesToSkip) {
            if(buffer.GetCurrentIndexAsPointer(1 + bytesToSkip, out var ptr))
                return BitConverter.ToByte(ptr + bytesToSkip);
            else
                throw new Exception(TAG + $"Failed to read byte value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe sbyte PeakSByte(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(1, out var ptr))
                return BitConverter.ToSbyte(ptr);
            else
                throw new Exception(TAG + $"Failed to read sbyte value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe short PeakShort(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(2, out var ptr))
                return BitConverter.ToShort(ptr);
            else
                throw new Exception(TAG + $"Failed to read short value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort PeakUShort(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(2, out var ptr))
                return BitConverter.ToUshort(ptr);
            else
                throw new Exception(TAG + $"Failed to read ushort value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int PeakInt(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(4, out var ptr))
                return BitConverter.ToInt(ptr);
            else
                throw new Exception(TAG + $"Failed to read int value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int PeakInt(this IBuffer buffer, int bytesToSkip) {
            if(buffer.GetCurrentIndexAsPointer(4 + bytesToSkip, out var ptr))
                return BitConverter.ToInt(ptr + bytesToSkip);
            else
                throw new Exception(TAG + $"Failed to read int value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint PeakUInt(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(4, out var ptr))
                return BitConverter.ToUint(ptr);
            else
                throw new Exception(TAG + $"Failed to read uint value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long PeakLong(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(8, out var ptr))
                return BitConverter.ToLong(ptr);
            else
                throw new Exception(TAG + $"Failed to read long value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong PeakULong(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(8, out var ptr))
                return BitConverter.ToUlong(ptr);
            else
                throw new Exception(TAG + $"Failed to read ulong value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float PeakFloat(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(4, out var ptr))
                return BitConverter.ToFloat(ptr);
            else
                throw new Exception(TAG + $"Failed to read float value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float PeakFloat(this IBuffer buffer, int bytesToSkip) {
            if(buffer.GetCurrentIndexAsPointer(4 + bytesToSkip, out var ptr))
                return BitConverter.ToFloat(ptr + bytesToSkip);
            else
                throw new Exception(TAG + $"Failed to read float value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double PeakDouble(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(8, out var ptr))
                return BitConverter.ToDouble(ptr);
            else
                throw new Exception(TAG + $"Failed to read double value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe decimal PeakDecimal(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(16, out var ptr))
                return BitConverter.ToDecimal(ptr);
            else
                throw new Exception(TAG + $"Failed to read decimal value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool PeakBool(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(1, out var ptr))
                return BitConverter.ToBool(ptr);
            else
                throw new Exception(TAG + $"Failed to read bool value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool PeakBoolean(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(1, out var ptr))
                return BitConverter.ToBool(ptr);
            else
                throw new Exception(TAG + $"Failed to read bool value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe char PeakChar(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(2, out var ptr))
                return BitConverter.ToChar(ptr);
            else
                throw new Exception(TAG + $"Failed to read char value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe System.DateTime PeakDateTime(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(8, out var ptr))
                return new DateTime(BitConverter.ToLong(ptr));
            else
                throw new Exception(TAG + $"Failed to Peak DateTime value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T PeakEnum<T>(this IBuffer buffer) where T : System.Enum {
            switch(FastEnum<T>.TypeCode) {
                case TypeCode.Byte: return PeakByte(buffer).ToEnum<T>();
                case TypeCode.SByte: return PeakSByte(buffer).ToEnum<T>();
                case TypeCode.Int16: return PeakShort(buffer).ToEnum<T>();
                case TypeCode.UInt16: return PeakUShort(buffer).ToEnum<T>();
                case TypeCode.Int32: return PeakInt(buffer).ToEnum<T>();
                case TypeCode.UInt32: return PeakUInt(buffer).ToEnum<T>();
                case TypeCode.Int64: return PeakLong(buffer).ToEnum<T>();
                case TypeCode.UInt64: return PeakULong(buffer).ToEnum<T>();
                default:
                    throw new Exception(TAG + $"Unsupported enum type '{FastEnum<T>.TypeCode}'");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe System.Guid PeakGuid(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsSpan(GUID_LENGTH, out var span))
                return new Guid(span);
            else
                throw new Exception(TAG + $"Failed to Peak Guid value from buffer");
        }

        #endregion

        #region Array Variants

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] PeakByteArray(this IBuffer buffer) {
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            byte[] bytes = new byte[len];
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = ptr[i];
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to Peak byte array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int PeakByteArray(this IBuffer buffer, byte[] values) {
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = ptr[i];
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to Peak byte array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PeakByteArray(this IBuffer buffer, List<byte> values) {
            values.Clear();
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(ptr[i]);
            }
            else
                throw new Exception(TAG + $"Failed to Peak byte array value from buffer");
        }

        /// ------------- Integer ------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int[] PeakIntArray(this IBuffer buffer) {
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            int[] bytes = new int[len];
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len * 4, out var ptr)) {
                for(int i = 0; i < len; i++)
                    bytes[i] = BitConverter.ToInt(ptr + (i * 4));
                return bytes;
            }
            else
                throw new Exception(TAG + $"Failed to Peak int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int PeakIntArray(this IBuffer buffer, int[] values) {
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            var copyLength = Mathf.Min(len, values.Length);
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len * 4, out var ptr)) {
                for(int i = 0; i < copyLength; i++)
                    values[i] = BitConverter.ToInt(ptr + (i * 4));
                return copyLength;
            }
            else
                throw new Exception(TAG + $"Failed to Peak int array value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PeakIntArray(this IBuffer buffer, List<int> values) {
            values.Clear();
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len * 4, out var ptr)) {
                for(var i = 0; i < len; i++)
                    values.Add(BitConverter.ToInt(ptr + (i * 4)));
            }
            else
                throw new Exception(TAG + $"Failed to Peak int array value from buffer");
        }

        #endregion

        #region Compressed

        public static unsafe int PeakCompressedInt(this IBuffer buffer) {
            var initialByte = buffer.PeakByte();
            bool isPositive = (initialByte & B.ByteMask.IOOO_OOOO) != B.ByteMask.IOOO_OOOO;
            int result;

            if((initialByte & B.ByteMask.OIOO_OOOO) != B.ByteMask.OIOO_OOOO) {
                result = (initialByte & B.OOII.IIII);
            }
            else if((initialByte & B.ByteMask.OOIO_OOOO) != B.ByteMask.OOIO_OOOO) {
                result = ((initialByte & B.OOOI.IIII) << 8) | buffer.PeakByte(1);
            }
            else if((initialByte & B.ByteMask.OOOI_OOOO) != B.ByteMask.OOOI_OOOO) {
                result = ((initialByte & B.OOOO.IIII) << 16) | (buffer.PeakByte(1) << 8) | buffer.PeakByte(2);
            }
            else if((initialByte & B.ByteMask.OOOO_IOOO) != B.ByteMask.OOOO_IOOO) {
                result = ((initialByte & B.OOOO.OIII) << 24) | (buffer.PeakByte(1) << 16) | (buffer.PeakByte(2) << 8) | buffer.PeakByte(3);
            }
            else {
                result = (buffer.PeakByte(1) << 24) | (buffer.PeakByte(2) << 16) | (buffer.PeakByte(3) << 8) | buffer.PeakByte(4);
            }

            return isPositive ? result : -result;
        }

        public static unsafe int PeakCompressedInt(this IBuffer buffer, out int bytesUsed) {
            var initialByte = buffer.PeakByte();
            bool isPositive = (initialByte & B.ByteMask.IOOO_OOOO) != B.ByteMask.IOOO_OOOO;
            int result;

            if((initialByte & B.ByteMask.OIOO_OOOO) != B.ByteMask.OIOO_OOOO) {
                result = (initialByte & B.OOII.IIII);
                bytesUsed = 1;
            }
            else if((initialByte & B.ByteMask.OOIO_OOOO) != B.ByteMask.OOIO_OOOO) {
                result = ((initialByte & B.OOOI.IIII) << 8) | buffer.PeakByte(1);
                bytesUsed = 2;
            }
            else if((initialByte & B.ByteMask.OOOI_OOOO) != B.ByteMask.OOOI_OOOO) {
                result = ((initialByte & B.OOOO.IIII) << 16) | (buffer.PeakByte(1) << 8) | buffer.PeakByte(2);
                bytesUsed = 3;
            }
            else if((initialByte & B.ByteMask.OOOO_IOOO) != B.ByteMask.OOOO_IOOO) {
                result = ((initialByte & B.OOOO.OIII) << 24) | (buffer.PeakByte(1) << 16) | (buffer.PeakByte(2) << 8) | buffer.PeakByte(3);
                bytesUsed = 4;
            }
            else {
                result = (buffer.PeakByte(1) << 24) | (buffer.PeakByte(2) << 16) | (buffer.PeakByte(3) << 8) | buffer.PeakByte(4);
                bytesUsed = 5;
            }

            return isPositive ? result : -result;
        }

        #endregion

        #region String

        /// <summary>
        /// Using by default, big endian unicode encoding for the strings.
        /// </summary>
        /// <param name="str"></param>
        public static unsafe string PeakString(this IBuffer buffer) {
            return PeakString(buffer, System.Text.Encoding.BigEndianUnicode);
        }

        public static unsafe string PeakString(this IBuffer buffer, EncodingType encoding) {
            switch(encoding) {
                case EncodingType.ASCII: return PeakString(buffer, System.Text.Encoding.ASCII);
                case EncodingType.BigEndianUnicode: return PeakString(buffer, System.Text.Encoding.BigEndianUnicode);
                case EncodingType.Unicode: return PeakString(buffer, System.Text.Encoding.Unicode);
                case EncodingType.UTF7: return PeakString(buffer, System.Text.Encoding.UTF7);
                case EncodingType.UTF8: return PeakString(buffer, System.Text.Encoding.UTF8);
                case EncodingType.UTF32: return PeakString(buffer, System.Text.Encoding.UTF32);
            }
            throw new Exception(TAG + $"Unsupported EncodingType '{encoding}'");
        }

        public static unsafe string PeakString(this IBuffer buffer, System.Text.Encoding encoding) {
            var len = PeakCompressedInt(buffer, out int bytesToSkip);
            if(buffer.GetCurrentIndexAsPointer(bytesToSkip, len, out var ptr))
                return encoding.GetString(ptr, len);
            else
                throw new Exception(TAG + $"Failed to Peak string value from buffer");
        }

        #endregion

        #region Toolkit Data Types

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe TKDateTime PeakTKDateTime(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(8, out var ptr))
                return new TKDateTime(BitConverter.ToLong(ptr));
            else
                throw new Exception(TAG + $"Failed to Peak TKDateTime value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe HSV PeakHSV(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(12, out var ptr)) {
                var h = BitConverter.ToFloat(ptr);
                var s = BitConverter.ToFloat(ptr + 4);
                var v = BitConverter.ToFloat(ptr + 8);
                return new HSV(h, s, v);
            }
            else
                throw new Exception(TAG + $"Failed to Peak HSV value from buffer");
        }

        #endregion

        #region Unity

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector2 PeakVector2(this IBuffer buffer, Vector2 value) {
            if(buffer.GetCurrentIndexAsPointer(8, out var ptr))
                return BitConverter.ToVector2(ptr);
            else
                throw new Exception(TAG + $"Failed to Peak Vector2 value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector3 PeakVector3(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(12, out var ptr))
                return BitConverter.ToVector3(ptr);
            else
                throw new Exception(TAG + $"Failed to Peak Vector3 value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Vector4 PeakVector4(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(16, out var ptr))
                return BitConverter.ToVector4(ptr);
            else
                throw new Exception(TAG + $"Failed to Peak Vector4 value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Quaternion PeakQuaternion(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(16, out var ptr))
                return BitConverter.ToQuaternion(ptr);
            else
                throw new Exception(TAG + $"Failed to Peak Quaternion value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Color PeakColor(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(16, out var ptr)) {
                var r = BitConverter.ToFloat(ptr + 0);
                var g = BitConverter.ToFloat(ptr + 4);
                var b = BitConverter.ToFloat(ptr + 8);
                var a = BitConverter.ToFloat(ptr + 12);
                return new Color(r, g, b, a);
            }
            else
                throw new Exception(TAG + $"Failed to Peak Color value from buffer");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Color32 PeakColor32(this IBuffer buffer) {
            if(buffer.GetCurrentIndexAsPointer(4, out var ptr)) {
                return new Color32(ptr[0], ptr[1], ptr[2], ptr[3]);
            }
            else
                throw new Exception(TAG + $"Failed to Peak Color32 value from buffer");
        }

        #endregion
    }
}
