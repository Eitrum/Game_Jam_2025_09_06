using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    /// <summary>
    /// Linear array converted to cubic. Priority order: z -> y -> x.
    /// for (x)
    ///     for (y)
    ///         for (z)
    /// </summary>
    /// <typeparam name="T">Any value type</typeparam>
    [System.Serializable]
    public class CubicArray<T> : IEnumerable<T>, IEnumerable, IReadOnlyList<T> {
        [SerializeField]
        protected T[] array = new T[0];

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public int Size => array.Length;
        public int Count => array.Length;

        public T this[int index] {
            get {
                return array[index];
            }
            set {
                array[index] = value;
            }
        }

        public T this[Vector3Int pos] {
            get => this[pos.x, pos.y, pos.z];
            set => this[pos.x, pos.y, pos.z] = value;
        }

        public T this[int x, int y, int z] {
            get {
                return array[(x * Height * Depth) + (y * Depth) + (z)];
            }
            set {
                array[(x * Height * Depth) + (y * Depth) + (z)] = value;
            }
        }

        public CubicArray(int size) : this(size, size, size) { }

        public CubicArray(int width, int height, int depth) {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            array = new T[width * height * depth];
        }

        public CubicArray(Vector3Int vector) : this(vector.x, vector.y, vector.z) { }
        public CubicArray(Vector3 vector) : this((int)vector.x, (int)vector.y, (int)vector.z) { }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return array.GetEnumerator();
        }
    }
}
