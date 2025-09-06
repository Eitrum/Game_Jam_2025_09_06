using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit {
    public static class FastPool // Testing simple pool system for non-game objects
    {
        #region Global Setup

        [RuntimeInitializeOnLoadMethod]
        private static void SetupGlobal() {
            FastPool<AnimationCurve>.Global.AssignResetMethod(ClearAnimationCurve);
        }

        public static void ClearAnimationCurve(AnimationCurve curve) {
            for(int i = curve.length - 1; i >= 0; i--) {
                curve.RemoveKey(i);
            }
        }

        #endregion

        #region Reset Methods

        public static void ClearOnReset<T>(this FastPool<List<T>> fastpool) {
            fastpool.AssignResetMethod(ListClear);
        }
        private static void ListClear<T>(List<T> list) => list.Clear();

        public static void ClearOnReset<TKey, TItem>(this FastPool<Dictionary<TKey, TItem>> fastpool) {
            fastpool.AssignResetMethod(DictionaryClear);
        }
        private static void DictionaryClear<TKey, TItem>(IDictionary<TKey, TItem> dict) => dict.Clear();

        #endregion
    }

    public class FastPool<T> where T : new() {
        #region Variables

        private static FastPool<T> global;
        private int maxSize = 128;
        private List<T> pooledObjects = new List<T>();
        private Action<T> onReset = null;
        public static readonly bool IsDisposable = false;

        #endregion

        #region Properties

        public static FastPool<T> Global => global;
        public int MaxSize {
            get => maxSize;
            set => maxSize = Mathf.Max(4, value);
        }

        public bool HasPooledObjects => pooledObjects.Count > 0;

        #endregion

        #region Constructor

        static FastPool() {
            global = new FastPool<T>();
            IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
        }

        public FastPool() { }
        public FastPool(int size) { this.maxSize = size; }
        public FastPool(Action<T> onReset) {
            this.onReset = onReset;
        }
        public FastPool(int size, Action<T> onReset) {
            this.maxSize = size;
            this.onReset = onReset;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns existing pooled objects, if no objects exist, create a new object.
        /// </summary>
        public T Get()
            => Pop();

        /// <summary>
        /// Returns existing pooled objects, if no objects exist, create a new object.
        /// </summary>
        public T Pop() {
            lock(pooledObjects) {
                var l = pooledObjects.Count;
                if(l == 0)
                    return new T();
                var t = pooledObjects[l - 1];
                pooledObjects.RemoveAt(l - 1);
                onReset?.Invoke(t);
                return t;
            }
        }

        /// <summary>
        /// Only returns objects if they exist. Does not create a new object.
        /// </summary>
        public bool TryGet(out T result)
            => TryPop(out result);

        /// <summary>
        /// Only returns objects if they exist. Does not create a new object.
        /// </summary>
        public bool TryPop(out T result) {
            lock(pooledObjects) {
                var l = pooledObjects.Count;
                if(l > 0) {
                    result = pooledObjects[l - 1];
                    pooledObjects.RemoveAt(l - 1);
                    onReset?.Invoke(result);
                    return true;
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Returns true if it successfully returned an object to the list.
        /// </summary>
        public bool Return(T obj)
            => Push(obj);

        /// <summary>
        /// Returns true if it successfully returned an object to the list.
        /// </summary>
        public bool Push(T obj) {
            if(obj == null)
                return false;
            lock(pooledObjects) {
                if(pooledObjects.Count < maxSize) {
                    pooledObjects.Add(obj);
                    return true;
                }
            }
            return false;
        }

        public Scope GetScope() => new Scope(this);

        public void AssignResetMethod(Action<T> onReset) {
            if(this.onReset != null)
                Debug.LogWarning("Overriding on-reset method on FastPool object");
            this.onReset = onReset;
        }

        #endregion

        #region Dispose

        public void Dispose() {
            if(IsDisposable) {
                for(int i = pooledObjects.Count - 1; i >= 0; i--) {
                    if(pooledObjects[i] is IDisposable disposable)
                        disposable.Dispose();
                }
            }
            pooledObjects.Clear();
        }

        public void Dispose(Action<T> disposeMethod) {
            for(int i = pooledObjects.Count - 1; i >= 0; i--)
                try {
                    disposeMethod(pooledObjects[i]);
                }
                catch { };
            pooledObjects.Clear();
        }

        #endregion

        public class Scope : IDisposable {
            private FastPool<T> Pool;
            public T Value { get; private set; }
            public Scope(FastPool<T> pool) {
                this.Pool = pool;
                this.Value = pool.Pop();
            }

            public void Dispose() {
                this.Pool?.Push(this.Value);
            }
        }
    }



    public class FastPoolArray<T> where T : new() {
        #region Variables

        private readonly int arraySize;
        private int maxSize = 128;
        private List<T[]> pooledObjects = new List<T[]>();
        private Action<T[]> onReset = null;
        public static readonly bool IsDisposable = false;

        #endregion

        #region Properties

        public int MaxSize {
            get => maxSize;
            set => maxSize = Mathf.Max(4, value);
        }

        public bool HasPooledObjects => pooledObjects.Count > 0;

        #endregion

        #region Constructor

        static FastPoolArray() {
            IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
        }

        public FastPoolArray(int arraySize) {
            this.arraySize = arraySize;
        }
        public FastPoolArray(int arraySize, int poolSize) {
            this.arraySize = arraySize;
            this.maxSize = poolSize;
        }
        public FastPoolArray(int arraySize, Action<T[]> onReset) {
            this.arraySize = arraySize;
            this.onReset = onReset;
        }
        public FastPoolArray(int arraySize, int poolSize, Action<T[]> onReset) {
            this.arraySize = arraySize;
            this.maxSize = poolSize;
            this.onReset = onReset;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns existing pooled objects, if no objects exist, create a new object.
        /// </summary>
        public T[] Get()
            => Pop();

        /// <summary>
        /// Returns existing pooled objects, if no objects exist, create a new object.
        /// </summary>
        public T[] Pop() {
            lock(pooledObjects) {
                var l = pooledObjects.Count;
                if(l == 0)
                    return new T[arraySize];
                var t = pooledObjects[l - 1];
                pooledObjects.RemoveAt(l - 1);
                onReset?.Invoke(t);
                return t;
            }
        }

        /// <summary>
        /// Only returns objects if they exist. Does not create a new object.
        /// </summary>
        public bool TryGet(out T[] result)
            => TryPop(out result);

        /// <summary>
        /// Only returns objects if they exist. Does not create a new object.
        /// </summary>
        public bool TryPop(out T[] result) {
            lock(pooledObjects) {
                var l = pooledObjects.Count;
                if(l > 0) {
                    result = pooledObjects[l - 1];
                    pooledObjects.RemoveAt(l - 1);
                    onReset?.Invoke(result);
                    return true;
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Returns true if it successfully returned an object to the list.
        /// </summary>
        public bool Return(T[] obj)
            => Push(obj);

        /// <summary>
        /// Returns true if it successfully returned an object to the list.
        /// </summary>
        public bool Push(T[] obj) {
            if(obj == null)
                return false;
            lock(pooledObjects) {
                if(pooledObjects.Count < maxSize) {
                    pooledObjects.Add(obj);
                    return true;
                }
            }
            return false;
        }

        public Scope GetScope() => new Scope(this);

        public void AssignResetMethod(Action<T[]> onReset) {
            if(this.onReset != null)
                Debug.LogWarning("Overriding on-reset method on FastPool object");
            this.onReset = onReset;
        }

        #endregion

        #region Dispose

        public void Dispose() {
            if(IsDisposable) {
                for(int i = pooledObjects.Count - 1; i >= 0; i--) {
                    if(pooledObjects[i] is IDisposable disposable)
                        disposable.Dispose();
                }
            }
            pooledObjects.Clear();
        }

        public void Dispose(Action<T[]> disposeMethod) {
            for(int i = pooledObjects.Count - 1; i >= 0; i--)
                try {
                    disposeMethod(pooledObjects[i]);
                }
                catch { };
            pooledObjects.Clear();
        }

        #endregion

        public class Scope : IDisposable {
            private FastPoolArray<T> Pool;
            public T[] Value { get; private set; }
            public Scope(FastPoolArray<T> pool) {
                this.Pool = pool;
                this.Value = pool.Pop();
            }

            public void Dispose() {
                this.Pool?.Push(this.Value);
            }
        }
    }
}
