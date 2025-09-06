using System;

namespace Toolkit.IO {
    public interface IBuffer {
        int Length { get; }
        int Index { get; set; }
        byte[] Raw { get; }

        byte this[int index] { get; set; }

        void Reset(BufferResetMode mode = BufferResetMode.IndexOnly);
        byte[] Copy(BufferCopyMode mode = BufferCopyMode.Written);
        bool Resize(int newSize);

        unsafe bool TryReserve(int length, out byte* ptr);
    }

    public enum BufferCopyMode {
        /// <summary>
        /// The written part of a buffer
        /// </summary>
        Written,
        /// <summary>
        /// The entire byte array in a buffer
        /// </summary>
        Raw
    }

    public enum BufferResetMode {
        /// <summary>
        /// Resets the byte array to 0
        /// </summary>
        Clear,
        /// <summary>
        /// Only set the written index to 0
        /// </summary>
        IndexOnly,

        /// <summary>
        /// Resets the entire byte array to 0 including unwritten part.
        /// </summary>
        Full
    }
}
