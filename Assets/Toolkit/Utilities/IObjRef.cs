using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    namespace Internal
    {
        [System.Serializable]
        public abstract class IObjRef
        {
            [SerializeField] protected UnityEngine.Object reference;
        }
    }

    [System.Serializable]
    public class IObjRef<T> : Internal.IObjRef, ISerializationCallbackReceiver where T : class
    {
        #region Variables

        protected T actualReference;

        #endregion

        #region Properties

        // To mimic NSOReference
        public T Value {
            get => actualReference;
            set{
                actualReference = value;
                reference = value as UnityEngine.Object;
            }
        }

        public T Reference {
            get => actualReference;
            set {
                actualReference = value;
                reference = value as UnityEngine.Object;
            }
        }

        public UnityEngine.Object BaseObject {
            get => reference;
            set {
                if(value is T t) {
                    reference = value;
                    actualReference = t;
                }
                else if(value == null) {
                    reference = null;
                    actualReference = null;
                }
                else if(value is GameObject go) {
                    BaseObject = go.GetComponent<T>() as UnityEngine.Object;
                }
                else if(value is Component comp) {
                    BaseObject = comp.GetComponent<T>() as UnityEngine.Object;
                }
            }
        }

        public bool IsValid => actualReference != null;
        public bool IsSerializationValid => reference != null && IsValid;
        public static System.Type GetImplType() => typeof(T);

        #endregion

        #region Constructor

        public IObjRef() { }
        public IObjRef(T item) => Reference = item;
        public IObjRef(UnityEngine.Object obj) => BaseObject = obj;

        #endregion

        #region Operator

        public static implicit operator T(IObjRef<T> obj) => obj.Reference;

        #endregion

        #region ISerialization Callback

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            actualReference = reference as T;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion
    }

    public static class IObjRefExtensions 
    {
        public static void Add<TList, T>(this IList<TList> list, T item)
            where T : class
            where TList : IObjRef<T> 
            => list?.Add(new IObjRef<T>(item) as TList);

        public static bool Remove<TList, T>(this IList<TList> list, T item)
            where T : class
            where TList : IObjRef<T>
            {
            for(int i = list.Count - 1; i >= 0; i--) {
                if(list[i].Reference == item){
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
