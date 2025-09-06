using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class CircularBuffer<T> : IEnumerable<T> {
        #region Variables

        protected T[] values;
        protected int index = 0;
        protected int writtenIndex = 0;

        #endregion

        #region Properties

        public T this[int index] {
            get {
                var i = (this.index - index + (Length - 1)) % values.Length;
                return values[i];
            }
        }

        public T MostRecent => this[index + Length - 1];
        public T Oldest => this[index];

        public int Length => values.Length;
        public int Count => values.Length;
        public int WrittenIndex => writtenIndex;
        public T[] Raw => values;

        #endregion

        #region Constructors

        public CircularBuffer(int size) {
            if(size < 1)
                throw new System.IndexOutOfRangeException("Circular buffer need to be of size 1 or larger!");
            values = new T[size];
        }

        #endregion

        #region Resize

        public void Resize(int newSize) {
            if(newSize < 1)
                throw new System.IndexOutOfRangeException("Circular buffer need to be of size 1 or larger!");
            if(newSize == Length)
                return;
            System.Array.Resize(ref values, newSize);
        }

        #endregion

        #region Write

        public void Write(T value) {
            if(this.index >= this.values.Length)
                this.index = 0;
            this.values[index++] = value;
            if(this.index > writtenIndex)
                writtenIndex = this.index;
        }

        public T Reverse(bool setDefaultValue = true) {
            var output = this[index];
            if(index == 0) {
                if(writtenIndex == 0)
                    return output;
                index = Length - 1;
                writtenIndex = index;
            }
            if(index == writtenIndex) {
                if(setDefaultValue)
                    values[index] = default;
                writtenIndex--;
            }
            else {
                index--;
            }
            return output;
        }

        #endregion

        #region Fill

        public void Fill()
            => Fill(default);

        public void Fill(T value) {
            while(writtenIndex < Length)
                Write(value);
        }

        #endregion

        #region Util

        public void Reset() {
            writtenIndex = 0;
            index = 0;
        }

        public void Clear() {
            writtenIndex = 0;
            index = 0;
            for(int i = values.Length - 1; i >= 0; i--)
                values[i] = default;
        }

        #endregion

        #region Checks

        public bool Contains(T item) {
            for(int i = 0; i < writtenIndex; i++)
                if(values[i].Equals(item)) return true;
            return false;
        }

        #endregion

        #region IEnumerable Impl

        public IEnumerator<T> GetEnumerator() {
            for(int i = 0; i < writtenIndex; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for(int i = 0; i < writtenIndex; i++)
                yield return this[i];
        }

        #endregion
    }

    public static class CircularBufferExtensions {
        #region Sum

        public static float Sum(this CircularBuffer<float> buffer) {
            float result = 0f;
            for(int i = 0, length = buffer.WrittenIndex; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result;
        }

        public static int Sum(this CircularBuffer<int> buffer) {
            int result = 0;
            for(int i = 0, length = buffer.WrittenIndex; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result;
        }

        public static long Sum(this CircularBuffer<long> buffer) {
            long result = 0;
            for(int i = 0, length = buffer.WrittenIndex; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result;
        }

        public static Vector2 Sum(this CircularBuffer<Vector2> buffer) {
            Vector2 result = default;
            for(int i = 0, length = buffer.WrittenIndex; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result;
        }

        public static Vector3 Sum(this CircularBuffer<Vector3> buffer) {
            Vector3 result = default;
            for(int i = 0, length = buffer.WrittenIndex; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result;
        }

        public static Vector4 Sum(this CircularBuffer<Vector4> buffer) {
            Vector4 result = default;
            for(int i = 0, length = buffer.WrittenIndex; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result;
        }

        #endregion

        #region Average

        public static float Average(this CircularBuffer<float> buffer) {
            float result = 0f;
            var length = buffer.WrittenIndex;
            if(length == 0)
                return default;
            for(int i = 0; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result / length;
        }

        public static int Average(this CircularBuffer<int> buffer) {
            int result = 0;
            var length = buffer.WrittenIndex;
            if(length == 0)
                return default;
            for(int i = 0; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result / length;
        }

        public static long Average(this CircularBuffer<long> buffer) {
            long result = 0;
            var length = buffer.WrittenIndex;
            if(length == 0)
                return default;
            for(int i = 0; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result / length;
        }

        public static Vector2 Average(this CircularBuffer<Vector2> buffer) {
            Vector2 result = default;
            var length = buffer.WrittenIndex;
            if(length == 0)
                return default;
            for(int i = 0; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result / length;
        }

        public static Vector3 Average(this CircularBuffer<Vector3> buffer) {
            Vector3 result = default;
            var length = buffer.WrittenIndex;
            if(length == 0)
                return default;
            for(int i = 0; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result / length;
        }

        public static Vector4 Average(this CircularBuffer<Vector4> buffer) {
            Vector4 result = default;
            var length = buffer.WrittenIndex;
            if(length == 0)
                return default;
            for(int i = 0; i < length; i++) {
                result += buffer.Raw[i];
            }
            return result / length;
        }

        #endregion

        #region Min

        public static float Min(this CircularBuffer<float> buffer) {
            float res = buffer.Raw[0];
            for(int i = buffer.WrittenIndex - 1; i >= 1; i--)
                res = Mathf.Min(res, buffer.Raw[i]);
            return res;
        }

        public static int Min(this CircularBuffer<int> buffer) {
            int res = buffer.Raw[0];
            for(int i = buffer.WrittenIndex - 1; i >= 1; i--)
                res = Mathf.Min(res, buffer.Raw[i]);
            return res;
        }

        public static long Min(this CircularBuffer<long> buffer) {
            long res = buffer.Raw[0];
            for(int i = buffer.WrittenIndex - 1; i >= 1; i--)
                res = System.Math.Min(res, buffer.Raw[i]);
            return res;
        }

        #endregion

        #region Max

        public static float Max(this CircularBuffer<float> buffer) {
            float res = buffer.Raw[0];
            for(int i = buffer.WrittenIndex - 1; i >= 1; i--)
                res = Mathf.Max(res, buffer.Raw[i]);
            return res;
        }

        public static int Max(this CircularBuffer<int> buffer) {
            int res = buffer.Raw[0];
            for(int i = buffer.WrittenIndex - 1; i >= 1; i--)
                res = Mathf.Max(res, buffer.Raw[i]);
            return res;
        }

        public static long Max(this CircularBuffer<long> buffer) {
            long res = buffer.Raw[0];
            for(int i = buffer.WrittenIndex - 1; i >= 1; i--)
                res = System.Math.Max(res, buffer.Raw[i]);
            return res;
        }

        #endregion

        #region Contains

        public static bool Contains(this CircularBuffer<string> buffer, string value) {
            var raw = buffer.Raw;
            var written = buffer.WrittenIndex;
            for(int i = 0; i < written; i++)
                if(raw[i].Equals(value))
                    return true;
            return false;
        }

        public static bool Contains(this CircularBuffer<string> buffer, string value, System.StringComparison comparison) {
            var raw = buffer.Raw;
            var written = buffer.WrittenIndex;
            for(int i = 0; i < written; i++)
                if(raw[i].Equals(value, comparison))
                    return true;
            return false;
        }

        #endregion
    }
}
