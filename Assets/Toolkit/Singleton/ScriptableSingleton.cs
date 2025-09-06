using System;
using UnityEngine;

namespace Toolkit {
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T> {

        #region Const

        private const string TAG = "<color=blue>[ScriptableSingleton]</color> - ";

        #endregion

        #region Instance

        private static T instance;

        public static T Instance {
            get {
                if(instance == null) {
                    // check if asset exists in resources
                    var asset = SingletonUtility.GetScriptableAsset<T>();
                    if(asset != null) {
                        if((asset.KeepInResources || SingletonUtility.IsRunning == false))
                            instance = asset;
                        else
                            instance = Instantiate(asset);
                    }
                    else {
                        // if asset does not exist
                        instance = CreateInstance<T>();
                    }
                    instance.OnSingletonCreated();
                    SingletonUtility.AddTracking(typeof(T).FullName, instance);
                }
                return instance;
            }
            protected set {
                AssignInstance(value);
            }
        }

        #endregion

        #region Properties

        public static bool HasInstance => instance != null;
        protected virtual bool KeepInResources => false;

        #endregion

        #region Methods

        /// <summary>
        /// Note that this does not destroy the actual game object and only the mono behaviour component if the object exists in scene.
        /// </summary>
        public static void DestroyInstance() {
            if(HasInstance && (!instance.KeepInResources && SingletonUtility.IsRunning)) {
                Destroy(instance);
            }
            instance = null;
            SingletonUtility.RemoveTracking(typeof(T).FullName);
        }

        public static bool AssignInstance(T _instance) {
            if(_instance == null) {
                UnityEngine.Debug.LogError(TAG + $"of type '{typeof(T).FullName}' is trying to Assign Instance with value of null, if you trying to remove instance, use 'DestroyInstance'");
                return false;
            }
            if(instance != null)
                instance.OnSingletonDestroyed();

            instance = _instance;
            _instance.OnSingletonCreated();
            SingletonUtility.AddTracking(typeof(T).FullName, instance);
            return true;
        }

        protected virtual void OnSingletonCreated() { }
        protected virtual void OnSingletonDestroyed() { }

        #endregion
    }
}
