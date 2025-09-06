using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Toolkit.IO {
    public static partial class BufferExtensions {

        #region Variables

        private const string TAG = "<color=#808000>[BufferExtensions]</color> - ";

        #endregion

        #region Base Extensions

        public static unsafe bool TryReserve(this IBuffer buffer, int len) {
            return buffer.TryReserve(len, out byte* ptr);
        }

        public static unsafe bool TryGetPointerAt(this IBuffer buffer, int index, out byte* ptr) {
            var raw = buffer.Raw;
            if(raw.Length <= index || index < 0) {
                ptr = default;
                return false;
            }
            fixed(byte* tptr = &raw[index])
                ptr = tptr;
            return true;
        }

        /// <summary>
        /// Resizes the array if index+len is larger than buffer size.
        /// Mainly used for WriteAt(index, data) extensions.
        /// </summary>
        public static unsafe bool TryGetPointerAt(this IBuffer buffer, int index, int len, out byte* ptr) {
            var raw = buffer.Raw;
            if(index < 0) {
                ptr = default;
                return false;
            }
            if(index + len >= raw.Length) {
                // Resize
                if(!buffer.Resize(Mathf.NextPowerOfTwo(index + len))) {
                    ptr = default;
                    return false;
                }

                raw = buffer.Raw;
                // Move the writing index
                if(index + len > buffer.Index)
                    buffer.Index = index + len;
            }

            fixed(byte* tptr = &raw[index])
                ptr = tptr;
            return true;
        }

        public static SizeBlockScope CreateSizeScope(this IBuffer buffer) => new SizeBlockScope(buffer);
        public static InsertScope CreateInsertScope(this IBuffer buffer, int index) => new InsertScope(buffer, index);

        #endregion

        #region Shift

        public static unsafe bool Shift(this IBuffer buffer, int index, int amount) {
            if(amount == 0)
                return true;
            if(amount > 0) { // Add Amount
                var bytesToShift = buffer.Index - index;
                if(!buffer.TryReserve(amount, out byte* ptrTarget))
                    return false;
                var ptrSource = ptrTarget - amount;
                for(int i = 0; i < bytesToShift; i++)
                    ptrTarget[-i] = ptrSource[-i];
            }
            else { // Remove Amount
                var bytesToShift = buffer.Index - index;
                if(!buffer.TryGetPointerAt(index - 1, out var ptrTarget))
                    return false;
                var ptrSource = ptrTarget - amount;
                for(int i = 0; i < bytesToShift; i++)
                    ptrTarget[i] = ptrSource[i];

                buffer.Index += amount;
            }
            return true;
        }

        #endregion

        #region CopyTo

        public static void CopyTo(this IBuffer buffer, byte[] target, int index) {
            buffer.Raw
                .AsSpan()
                .Slice(index, Mathf.Min(target.Length, buffer.Index - index))
                .CopyTo(target.AsSpan());
        }

        public static void CopyTo(this IBuffer buffer, byte[] target, int index, int length) {
            buffer.Raw
                .AsSpan()
                .Slice(index, length)
                .CopyTo(target.AsSpan());
        }

        #endregion

        #region AsSpan

        public static Span<byte> AsSpan(this IBuffer buffer) => buffer.Raw.AsSpan();
        public static unsafe bool TryReserveAsSpan(this IBuffer buffer, int length, out Span<byte> result) {
            var index = buffer.Index;
            if(!buffer.TryReserve(length, out var ptr)) {
                result = default;
                return false;
            }

            result = buffer.Raw.AsSpan().Slice(index, length);
            return true;
        }

        public static unsafe bool TryGetSpanAt(this IBuffer buffer, int index, out Span<byte> result) {
            result = buffer.Raw.AsSpan().Slice(index);
            return true;
        }

        public static unsafe bool TryGetSpanAt(this IBuffer buffer, int index, int length, out Span<byte> result) {
            result = buffer.Raw.AsSpan().Slice(index, length);
            return true;
        }

        #endregion

        #region Peak Ptr

        public static unsafe bool GetCurrentIndexAsSpan(this IBuffer buffer, out Span<byte> result) {
            try {
                var index = buffer.Index;
                result = buffer.Raw.AsSpan().Slice(index);
                return index < buffer.Length;
            }
            catch(Exception e) {
                Debug.LogException(e);
                result = default;
                return false;
            }
        }

        public static unsafe bool GetCurrentIndexAsSpan(this IBuffer buffer, int length, out Span<byte> result) {
            try {
                var index = buffer.Index;
                if(index + length > buffer.Length) {
                    result = default;
                    return false;
                }
                result = buffer.Raw.AsSpan().Slice(index, length);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                result = default;
                return false;
            }
        }

        public static unsafe bool GetCurrentIndexAsPointer(this IBuffer buffer, out byte* ptr) {
            try {
                fixed(byte* tptr = &buffer.Raw[buffer.Index])
                    ptr = tptr;
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                ptr = default;
                return false;
            }
        }

        public static unsafe bool GetCurrentIndexAsPointer(this IBuffer buffer, int len, out byte* ptr) {
            try {
                if(buffer.Index + len > buffer.Raw.Length) {
                    ptr = default;
                    return false;
                }
                fixed(byte* tptr = &buffer.Raw[buffer.Index])
                    ptr = tptr;
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                ptr = default;
                return false;
            }
        }

        public static unsafe bool GetCurrentIndexAsPointer(this IBuffer buffer, int indexOffset, int len, out byte* ptr) {
            try {
                if((buffer.Index + len + indexOffset) > buffer.Raw.Length) {
                    ptr = default;
                    return false;
                }
                fixed(byte* tptr = &buffer.Raw[buffer.Index + indexOffset])
                    ptr = tptr;
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                ptr = default;
                return false;
            }
        }

        #endregion

        #region GetWritten

        public static byte[] GetWrittenBuffer(this IBuffer buffer) {
            return buffer.Copy(BufferCopyMode.Written);
        }

        #endregion
    }
}
