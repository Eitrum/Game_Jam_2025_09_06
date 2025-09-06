using System;
using System.Collections.Generic;
using System.Reflection;

namespace Toolkit
{
    public class DeterministicDelegates<T> where T : Delegate
    {
        #region Cache

        private struct Cache
        {
            public int order;
            public T callback;

            public Cache(T callback) {
                this.order = InvokeOrder.GetOrder(callback);
                this.callback = callback;
            }

            public Cache(T callback, int order) {
                this.order = order;
                this.callback = callback;
            }
        }

        #endregion

        #region Variables

        private List<Cache> callbacks = new List<Cache>(4);
        private T combined;
        private bool isDirty = false;

        #endregion

        #region Properties

        public T Delegate {
            get {
                if(isDirty) {
                    T temporary = null;
                    for(int i = 0, length = callbacks.Count; i < length; i++) {
                        temporary = (T)System.Delegate.Combine(temporary, callbacks[i].callback);
                    }
                    combined = temporary;
                    isDirty = false;
                }
                return combined;
            }
        }

        public bool IsEmpty => callbacks.Count == 0;
        public int Count => callbacks.Count;

        #endregion

        #region Add

        public void Add(T callback) {
            var cache = new Cache(callback);
            for(int i = callbacks.Count - 1; i >= 0; i--) {
                if(callbacks[i].order > cache.order) {
                    callbacks.Insert(i, cache);
                    isDirty = true;
                    return;
                }
            }
            callbacks.Add(cache);
            isDirty = true;
        }

        public void Add(T callback, int order) {
            var cache = new Cache(callback, order);
            for(int i = callbacks.Count - 1; i >= 0; i--) {
                if(callbacks[i].order > cache.order) {
                    callbacks.Insert(i, cache);
                    isDirty = true;
                    return;
                }
            }
            callbacks.Add(cache);
            isDirty = true;
        }

        #endregion

        #region Remove

        public void Remove(T callback) {
            for(int i = 0, length = callbacks.Count; i < length; i++) {
                if(callbacks[i].callback == callback) {
                    callbacks.RemoveAt(i);
                    isDirty = true;
                    return;
                }
            }
        }

        #endregion

        #region Clear

        public void Clear() {
            callbacks.Clear();
            isDirty = true;
        }

        #endregion

        #region Operators

        public static DeterministicDelegates<T> operator +(DeterministicDelegates<T> source, T callback) {
            source.Add(callback);
            return source;
        }

        public static DeterministicDelegates<T> operator -(DeterministicDelegates<T> source, T callback) {
            source.Remove(callback);
            return source;
        }

        #endregion
    }
}
