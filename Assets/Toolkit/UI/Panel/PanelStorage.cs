using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public static class PanelStorage {
        #region Variables

        private const string TAG = "[Toolkit.UI.PanelSystem.PanelStorage] - ";
        private static Dictionary<string, Func<Promise<GameObject>>> storage = new Dictionary<string, Func<Promise<GameObject>>>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        public static IEnumerable<string> StorageKeys => storage.Keys;

        #endregion

        #region Init

        [RuntimeInitializeOnLoadMethod]
        private static void Init() {
            var instance = PanelStorageRegister.Instance; // Default load existing storage registers
        }

        #endregion

        #region Register Prefab

        public static void Register(string name, GameObject prefab, bool @override = false) {
            if(string.IsNullOrEmpty(name)) {
                Debug.LogError(TAG + $"Register name is null!");
                return;
            }
            if(storage.TryGetValue(name, out var func)) {
                if(@override) {
                    Debug.LogWarning(TAG + $"Register collision on '{name}'. Overriding it!");
                }
                else {
                    Debug.LogError(TAG + $"Register collision on '{name}'");
                    return;
                }
            }

            storage[name] = () => new Promise<GameObject>().Complete(prefab);
        }

        public static void RegisterResourceLoad(string name, bool @override = false) {
            if(string.IsNullOrEmpty(name)) {
                Debug.LogError(TAG + $"Register name is null!");
                return;
            }
            if(storage.TryGetValue(name, out var func)) {
                if(@override) {
                    Debug.LogWarning(TAG + $"Register collision on '{name}'. Overriding it!");
                }
                else {
                    Debug.LogError(TAG + $"Register collision on '{name}'");
                    return;
                }
            }

            storage[name] = () => ResourceLoadPromise(name);
        }

        public static void RegisterResourceLoad(string name, string resourcePath, bool @override = false) {
            if(string.IsNullOrEmpty(name)) {
                Debug.LogError(TAG + $"Register name is null!");
                return;
            }
            if(storage.TryGetValue(name, out var func)) {
                if(@override) {
                    Debug.LogWarning(TAG + $"Register collision on '{name}'. Overriding it!");
                }
                else {
                    Debug.LogError(TAG + $"Register collision on '{name}'");
                    return;
                }
            }

            storage[name] = () => ResourceLoadPromise(resourcePath);
        }

        public static void Register(string name, Func<Promise<GameObject>> generic, bool @override = false) {
            if(string.IsNullOrEmpty(name)) {
                Debug.LogError(TAG + $"Register name is null!");
                return;
            }
            if(storage.TryGetValue(name, out var func)) {
                if(@override) {
                    Debug.LogWarning(TAG + $"Register collision on '{name}'. Overriding it!");
                }
                else {
                    Debug.LogError(TAG + $"Register collision on '{name}'");
                    return;
                }
            }

            storage[name] = generic;
        }

        #endregion

        #region Unregister

        public static bool Unregister(string name) {
            if(string.IsNullOrEmpty(name))
                return false;
            return storage.Remove(name);
        }

        #endregion

        #region GetPanel

        public static bool TryGetPanelPrefab(string name, out Promise<GameObject> promise) {
            return TryGetPanelPrefab(name, out promise, true);
        }

        public static bool TryGetPanelPrefab(string name, out Promise<GameObject> promise, bool useResourcesFallback) {
            var result = storage.TryGetValue(name, out var func);
            if(result) {
                promise = func();
                return true;
            }
            if(useResourcesFallback) {
                promise = ResourceLoadPromise(name);
                return true;
            }
            else
                promise = new Promise<GameObject>().Error($"no panel named '{name}'");
            return false;
        }

        private static Promise<GameObject> ResourceLoadPromise(string resourcePath) {
            var p = new Promise<GameObject>();
            var request = Resources.LoadAsync<GameObject>(resourcePath);
            request.completed += (r) => {
                if(request.asset != null)
                    p.Complete(request.asset as GameObject);
                else
                    p.Error("asset not found");
            };
            return p;
        }

        #endregion
    }
}
