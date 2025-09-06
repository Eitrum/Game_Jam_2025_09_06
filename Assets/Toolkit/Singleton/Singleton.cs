using System;

namespace Toolkit {
    public abstract class Singleton<T> where T : Singleton<T> {

        #region Const

        private const string TAG = "<color=blue>[Singleton]</color> - ";

        #endregion

        #region Instance

        private static T instance;
        public static T Instance {
            get {
                if(instance == null) {
                    instance = Activator.CreateInstance(typeof(T), true) as T;
#if UNITY_EDITOR
                    if(instance == null)
                        throw new Exception(TAG + $"of type {typeof(T).FullName} can't be instantiated due to no default constructor");
#endif
                    instance?.OnSingletonCreated();
                    SingletonUtility.AddTracking(typeof(T).FullName, instance);
                }
                return instance;
            }
            protected set {
                AssignInstance(value, false);
            }
        }

        #endregion

        #region Properties

        public static bool HasInstance => instance != null;
        protected virtual bool AssignEnabled => false;

        #endregion

        #region Methods

        public static bool AssignInstance(T _instance) => AssignInstance(_instance, false);
        public static bool AssignInstance(T _instance, bool overrideSettings) {
            if(_instance == null) {
                UnityEngine.Debug.LogError(TAG + $"of type '{typeof(T).FullName}' is trying to Assign Instance with value of null, if you trying to remove instance, use 'DestroyInstance'");
                return false;
            }
            if(instance == null) {
                instance = _instance;
                _instance.OnSingletonCreated();
                SingletonUtility.AddTracking(typeof(T).FullName, instance);
                return true;
            }
            if(instance.AssignEnabled && _instance.AssignEnabled) {
                instance.OnSingletonDestroyed();
                instance = _instance;
                _instance.OnSingletonCreated();
                SingletonUtility.AddTracking(typeof(T).FullName, instance);
                return true;
            }
            if(overrideSettings) {
                instance.OnSingletonDestroyed();
                instance = _instance;
                _instance.OnSingletonCreated();
                SingletonUtility.AddTracking(typeof(T).FullName, instance);
                return true;
            }
            UnityEngine.Debug.LogWarning(TAG + "Can't assign instance of singleton " + typeof(T).FullName);
            return false;
        }

        public static void DestroyInstance() {
            instance = null;
            SingletonUtility.RemoveTracking(typeof(T).FullName);
        }

        protected virtual void OnSingletonCreated() { }
        protected virtual void OnSingletonDestroyed() { }

        #endregion
    }
}
