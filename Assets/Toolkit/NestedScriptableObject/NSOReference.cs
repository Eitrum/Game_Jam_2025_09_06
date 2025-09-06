using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit {
    [System.Serializable]
    public abstract class NSOReference {
        #region Variables

        private readonly Type DEFAULT_TYPE = typeof(ScriptableObject);
        [SerializeField] protected ScriptableObject scriptable = null;

        #endregion

        #region Properties

        public virtual Type NestedScriptableObjectType => DEFAULT_TYPE;

        #endregion
    }

    [System.Serializable]
    public class NSOReference<T> : NSOReference where T : class {
        #region Properties

        public override Type NestedScriptableObjectType => typeof(T);

        public T Value => scriptable as T;

        // To mimic IObjRef
        public T Reference => scriptable as T;

        public bool IsValid => scriptable is T;

        #endregion

        #region TryGet

        public bool TryGet(out T obj) {
            if(scriptable is T gg) {
                obj = gg;
                return true;
            }
            obj = default;
            return false;
        }

        #endregion

        #region Operators

        public static implicit operator T(NSOReference<T> reference) => reference.Value;

        #endregion
    }

    [System.Serializable]
    public abstract class NSOReferenceArray {
        #region Variables

        private readonly Type DEFAULT_TYPE = typeof(ScriptableObject);
        [SerializeField] protected List<ScriptableObject> scriptables = new List<ScriptableObject>();

        #endregion

        #region Properties

        public virtual Type NestedScriptableObjectType => DEFAULT_TYPE;

        #endregion
    }

    [System.Serializable]
    public class NSOReferenceArray<T> : NSOReferenceArray, IReadOnlyList<T>, ISerializationCallbackReceiver, IEnumerable<T> where T : class {
        #region Properties

        private List<T> cached;

        public override Type NestedScriptableObjectType => typeof(T);

        public int Count => cached.Count;
        public List<T> Values => cached;

        public T this[int index] => cached[index];

        #endregion

        #region Operators

        public static implicit operator List<T>(NSOReferenceArray<T> reference) => reference.Values;

        #endregion

        #region Add

        public void Add(T item) {
            if(item is ScriptableObject so) {
                scriptables.Add(so);
                if(cached != null)
                    cached.Add(item);
            }
        }

        public bool Remove(T item) {
            if(item is ScriptableObject so) {
                for(int i = scriptables.Count - 1; i >= 0; i--) {
                    if(scriptables[i] == so) {
                        scriptables.RemoveAt(i);
                        if(cached != null)
                            cached.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Serialization Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if(scriptables == null)
                return;
            cached = scriptables
                .Select(x => x as T)
                .ToList();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion

        #region Enumerator Impl

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return cached.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return cached.GetEnumerator();
        }

        #endregion
    }
}
