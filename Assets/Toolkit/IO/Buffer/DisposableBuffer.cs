using System;
using UnityEngine;

namespace Toolkit.IO {
    /// <summary>
    /// A disposable buffer that rely on pooling for efficiency.
    /// Built for netcode serialization
    /// </summary>
    public class DisposableBuffer : IBuffer, IDisposable {

        private class ArrayCache {

            public byte[] bytes;

            public ArrayCache() { bytes = new byte[DEFAULT_SIZE]; }
            public ArrayCache(int size) { bytes = new byte[size]; }
        }

        public const int DEFAULT_SIZE = 512;

        #region Variables

        private byte[] bytes;
        private int index;
        private int length;
        private ArrayCache container;
        private static FastPool<ArrayCache> pool = new FastPool<ArrayCache>();

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

        private DisposableBuffer(ArrayCache container) {
            this.container = container;
            this.bytes = container.bytes;
            length = bytes.Length;
        }

        public DisposableBuffer() {
            container = pool.Pop();
            bytes = container.bytes;
            length = bytes.Length;
        }

        public DisposableBuffer(byte eventCode) {
            container = pool.Pop();
            bytes = container.bytes;
            length = bytes.Length;
            this.Write(eventCode);
        }

        public static DisposableBuffer Create(int size) {
            DisposableBuffer pbw;
            if(pool.HasPooledObjects) {
                pbw = new DisposableBuffer(pool.Pop());
                if(pbw.bytes.Length < size) {
                    pbw.Resize(size);
                }
            }
            else {
                pbw = new DisposableBuffer(new ArrayCache(Mathf.Max(size, DEFAULT_SIZE)));
            }
            return pbw;
        }

        public static DisposableBuffer Create(byte eventCode, int size) {
            size++;
            DisposableBuffer pbw;
            if(pool.HasPooledObjects) {
                pbw = new DisposableBuffer(pool.Pop());
                if(pbw.bytes.Length < size) {
                    pbw.Resize(size);
                }
            }
            else {
                pbw = new DisposableBuffer(new ArrayCache(Mathf.Max(size, DEFAULT_SIZE)));
            }
            pbw.Write(eventCode);
            return pbw;
        }

        public void Dispose() {
            if(container == null) {
                return;
            }
            pool.Push(container);
            container = null;
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
            if(index + length > this.length)
                Resize(Mathf.NextPowerOfTwo(index + length));
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

            Debug.Log(this.FormatLog("resize", length, newSize));
            var tbuf = bytes;
            this.bytes = new byte[newSize];
            this.container.bytes = this.bytes;
            this.length = newSize;
            tbuf.CopyTo(this.bytes, 0);
            return true;
        }

        #endregion
    }
}
