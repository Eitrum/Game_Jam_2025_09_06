using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace Toolkit {
    public static class SingletonUtility {

        #region Tracking

        internal class SingletonRef {
            public string name;
            public object reference;
        }
        internal static List<SingletonRef> trackedSingletons = new List<SingletonRef>();
        public static void AddTracking(string name, object reference) {
            for(int i = 0, length = trackedSingletons.Count; i < length; i++) {
                if(trackedSingletons[i].name == name) {
                    trackedSingletons[i].reference = reference;
                    return;
                }
            }
            trackedSingletons.Add(new SingletonRef() { name = name, reference = reference });
        }

        public static bool RemoveTracking(string name) {
            for(int i = 0, length = trackedSingletons.Count; i < length; i++) {
                if(trackedSingletons[i].name == name) {
                    trackedSingletons[i].reference = null;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region properties

        public static bool IsRunning => Application.isPlaying;

        #endregion

        #region Resource Help Methods

        public static T GetMonoAsset<T>() where T : MonoSingleton<T> {
            var attr = typeof(T).GetCustomAttribute<ResourceSingletonAttribute>();
            GameObject go;
            if(attr != null && attr.HasResourcePath) {
                go = Resources.Load<GameObject>(attr.ResourcePath);
            }
            go = Resources.Load<GameObject>(typeof(T).Name);
            if(go && go.TryGetComponent(out T obj)) {
                return obj;
            }
            return default;
        }

        public static T GetScriptableAsset<T>() where T : ScriptableSingleton<T> {
            var attr = typeof(T).GetCustomAttribute<ResourceSingletonAttribute>();
            if(attr != null && attr.HasResourcePath) {
                return Resources.Load<T>(attr.ResourcePath);
            }
            return Resources.Load<T>(typeof(T).Name);
        }

        #endregion
    }
}
