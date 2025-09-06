using System;
using UnityEngine;

namespace Toolkit {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {

        #region Const

        private const string TAG = "<color=blue>[MonoSingleton]</color> - ";

        #endregion

        #region Instance

        private static T instance;
        public static T Instance {
            get {
                if(instance == null) {
#if UNITY_EDITOR
                    if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                        return instance;
#endif
                    var existing = FindFirstObjectByType<T>(FindObjectsInactive.Exclude);

                    // check if asset exists in resources
                    var asset = SingletonUtility.GetMonoAsset<T>();
                    if(asset != null) {
                        if(asset.KeepInResources || SingletonUtility.IsRunning == false) {
                            instance = asset;
                        }
                        else {
                            var go = Instantiate(asset.gameObject);
                            instance = go.GetComponent<T>();
                            if(instance.KeepAlive) {
                                DontDestroyOnLoad(instance.gameObject);
                            }
                        }
                    }
                    else {
                        // if asset does not exist
                        instance = new GameObject(typeof(T).FullName).AddComponent<T>();
                        if(instance.KeepAlive) {
                            DontDestroyOnLoad(instance.gameObject);
                        }
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
        protected virtual bool KeepAlive {
            get => false;
            set {
                // if true, move to dont destroy on load scene
                if(value) DontDestroyOnLoad(gameObject);
                else {
                    // if object is in dont destroy on load scene, move to current active scene.
                    if((instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave) {
                        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(instance.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Note that this does not destroy the actual game object and only the mono behaviour component if the object exists in scene.
        /// </summary>
        public static void DestroyInstance() => DestroyInstance(false);
        public static void DestroyInstance(bool wholeGameObject) {
            if(HasInstance && (!instance.KeepInResources && SingletonUtility.IsRunning) && instance.gameObject.scene.isLoaded) {
                if(wholeGameObject)
                    Destroy(instance.gameObject);
                else
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
            if(_instance == instance) {
                UnityEngine.Debug.LogError(TAG + "Attempting to assign to existing instance");
                return false;
            }
            instance?.OnSingletonDestroyed();
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
