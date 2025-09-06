using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Callback")]
    public class Callback : MonoBehaviour, ICallback
    {
        #region Variables

        private Dictionary<Type, object> callbacks = new Dictionary<Type, object>();

        #endregion

        #region Properties

        public IEnumerable<Type> Types => callbacks.Keys;

        #endregion

        #region Utility

        public T Get<T>() where T : Delegate {
            var type = typeof(T);
            if(callbacks.TryGetValue(type, out object o) && o is DeterministicDelegates<T> dg) {
                return dg.Delegate;
            }
            return default;
        }

        public bool Has<T>() where T : Delegate {
            var type = typeof(T);
            return callbacks.ContainsKey(type);
        }

        public void Clear() {
            callbacks.Clear();
        }

        #endregion

        #region Add Remove

        public void Add<T>(T callback) where T : Delegate {
            var type = typeof(T);
            if(callbacks.TryGetValue(type, out object o)) {
                if(o is DeterministicDelegates<T> dg)
                    dg.Add(callback);
            }
            else {
                var newdg = new DeterministicDelegates<T>();
                newdg.Add(callback);
                callbacks.Add(type, newdg);
            }
        }

        public void Add<T>(T callback, int order) where T : Delegate {
            var type = typeof(T);
            if(callbacks.TryGetValue(type, out object o)) {
                if(o is DeterministicDelegates<T> dg)
                    dg.Add(callback, order);
            }
            else {
                var newdg = new DeterministicDelegates<T>();
                newdg.Add(callback, order);
                callbacks.Add(type, newdg);
            }
        }

        public void Remove<T>(T callback) where T : Delegate {
            var type = typeof(T);
            if(callbacks.TryGetValue(type, out object o) && o is DeterministicDelegates<T> dg) {
                dg.Remove(callback);
            }
        }

        #endregion
    }
}
