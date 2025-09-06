using System;
using UnityEngine;

namespace Toolkit.IO {
    /// <summary>
    /// Fixed sized buffer
    /// </summary>
    public class Buffer : IBuffer {

        public const int DEFAULT_SIZE = 128;

        #region Variables

        private byte[] bytes;
        private int index;
        private int length;

        #endregion

        #region Properties

        public byte this[int index] {
            get => bytes[index];
            set => bytes[index] = value;
        }

        public int Length => length;

        public int Index {
            get => index;
            set => SetIndex(value);
        }

        public byte[] Raw => bytes;

        #endregion

        #region Constructor

        public Buffer() : this(DEFAULT_SIZE) { }
        public Buffer(int size) {
            bytes = new byte[size];
            this.length = size;
        }

        public Buffer(byte[] bytes) {
            this.bytes = bytes;
            this.length = bytes.Length;
        }

        public Buffer(byte[] bytes, bool createNewArray) {
            if(createNewArray) {
                this.bytes = new byte[bytes.Length];
                bytes.CopyTo<byte>(this.bytes.AsSpan());
            }
            else
                this.bytes = bytes;
            this.length = this.bytes.Length;
        }

        /// <summary>
        /// Create a buffer with a specified size and copy the byte array into the buffer.
        /// </summary>
        public Buffer(byte[] bytes, int size) {
            this.bytes = new byte[size];
            this.length = size;
            bytes.CopyTo(this.bytes, 0);
        }

        #endregion

        #region IBuffer Methods

        public byte[] Copy(BufferCopyMode mode = BufferCopyMode.Written) {
            switch(mode) {
                case BufferCopyMode.Written: {
                        byte[] copy = new byte[index];
                        bytes.CopyTo(copy, 0);
                        return copy;
                    }
                case BufferCopyMode.Raw: {
                        byte[] copy = new byte[bytes.Length];
                        bytes.CopyTo(copy, 0);
                        return copy;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public void Reset(BufferResetMode mode = BufferResetMode.IndexOnly) {
            switch(mode) {
                case BufferResetMode.IndexOnly: {
                        this.index = 0;
                    }
                    break;
                case BufferResetMode.Clear: {
                        Clear();
                        this.index = 0;
                    }
                    break;
                case BufferResetMode.Full: {
                        Clear(true);
                        this.index = 0;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public unsafe bool TryReserve(int length, out byte* ptr) {
            if(index + length > this.length) {
                ptr = default;
                return false;
            }
            fixed(byte* currentPtr = &bytes[index])
                ptr = currentPtr;
            index += length;
            return true;
        }

        #endregion

        #region Methods

        public void SetIndex(int index) {
            this.index = Mathf.Clamp(index, 0, length);
        }

        /// <summary>
        /// Clear without resetting the writing index
        /// </summary>
        public unsafe void Clear(bool full = false) {
            var size = full ? length : index;
            fixed(byte* ptr = &bytes[0]) {
                for(int i = size - 1; i >= 0; i--) {
                    ptr[i] = 0;
                }
            }
        }

        public bool Resize(int newSize) {
            if(newSize == length)
                return false;
            if(newSize < 0)
                return false;

            var tbuf = bytes;
            this.bytes = new byte[newSize];
            this.length = newSize;
            tbuf.CopyTo(this.bytes, 0);
            return true;
        }

        #endregion

        #region File

        public static Buffer ReadFromFile(string path)
            => TryReadFromFile(path, out var buffer) ? buffer : null;

        public static bool TryReadFromFile(string path, out Buffer buffer) {
            try {
                if(!System.IO.File.Exists(path)) {
                    buffer = null;
                    return false;
                }

                buffer = new Buffer(System.IO.File.ReadAllBytes(path));
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                buffer = null;
                return false;
            }
        }

        public static Buffer ReadFromAsset(TextAsset asset)
            => TryReadFromAsset(asset, out var buffer) ? buffer : null;

        public static bool TryReadFromAsset(TextAsset asset, out Buffer buffer) {
            try {
                if(asset == null) {
                    buffer = null;
                    return false;
                }

                buffer = new Buffer(asset.bytes);
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                buffer = null;
                return false;
            }
        }

        #endregion
    }
}
