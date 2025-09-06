using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.UI {

    /// <summary>
    /// Don't use. Only used for caching.
    /// </summary>
    public interface IAssignable {

    }

    public interface IAssignable<T> : IAssignable {
        void Assign(T item);
    }

    public interface IAssignable<T0, T1> : IAssignable {
        void Assign(T0 item0, T1 item1);
    }

    public static class Assignable {

        public enum Search {
            /// <summary>
            /// Disables search
            /// </summary>
            Disable = -1,
            /// <summary>
            /// Uses GetComponents on same GameObject
            /// </summary>
            Default,
            /// <summary>
            /// Uses GetComponentsInChildren.
            /// </summary>
            IncludeChildren,
            /// <summary>
            /// Uses GetComponentsInChildren with include inactive.
            /// </summary>
            IncludeInactiveChildren
        }

        #region Variables

        private class Cache {
            public int handles;
            public GameObject target;
            public Search search;
            public List<IAssignable> assignables = new List<IAssignable>();
            public List<Type> types = new List<Type>();

            public void Update(GameObject target, Search search) {
                this.target = target;
                this.search = search;
                if(search != Search.Default)
                    target.GetComponentsInChildren(search == Search.IncludeInactiveChildren, assignables);
                else
                    target.GetComponents(assignables);
            }
        }

        private class CacheHandle : IDisposable {
            public Cache Cache { get; private set; }
            public List<IAssignable> Assignables => Cache.assignables;

            public CacheHandle(Cache cache) {
                Cache = cache;
                cache.handles++;
                sanityPreventRecursion++;
                if(sanityPreventRecursion > 100)
                    throw new Exception("Aarghh, too many assignable instances");
            }

            public void Dispose() {
                if(Cache == null)
                    return;
                Cache.handles--;
                Cache = null;
                sanityPreventRecursion--;
            }
        }

        private const string TAG = "[Toolkit.UI.Assignable] - ";
        private static List<Cache> caches = new List<Cache>();
        private static int sanityPreventRecursion = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Don't use! Debug purposes only!
        /// </summary>
        internal static IEnumerable<(int handles, GameObject target, Search search, IReadOnlyList<IAssignable> assignables)> Caches => caches.Select(x => (x.handles, x.target, x.search, x.assignables as IReadOnlyList<IAssignable>));

        #endregion

        #region Util

        private static CacheHandle GetCache(GameObject target, Search search) {
            for(int i = caches.Count - 1; i >= 0; i--) {
                if(caches[i].target == target && search == caches[i].search) {
                    return new CacheHandle(caches[i]);
                }
            }

            for(int i = caches.Count - 1; i >= 0; i--) {
                if(caches[i].handles == 0) {
                    caches[i].Update(target, search);
                    return new CacheHandle(caches[i]);
                }
            }
            var c = new Cache();
            c.Update(target, search);
            caches.Add(c);
            return new CacheHandle(c);
        }

        public static bool IsAssigning<T>(GameObject target) {
            for(int i = caches.Count - 1; i >= 0; i--) {
                if(caches[i].target == target) {
                    return caches[i].types.Contains(typeof(T));
                }
            }
            return false;
        }

        #endregion

        #region Assign

        public static void Assign<T>(GameObject targetGameObject, T item, Search search = Search.Default) {
            if(search == Search.Disable)
                return;
            if(targetGameObject == null) {
                Debug.LogError(TAG + "GameObject is null");
                return;
            }

            if(IsAssigning<T>(targetGameObject)) {
                Debug.LogWarning(TAG + "Skip assign to prevent recursive assign");
                return;
            }

            using(var handle = GetCache(targetGameObject, search)) {
                handle.Cache.types.Add(typeof(T));
                foreach(var c in handle.Assignables) {
                    try {
                        if(c is IAssignable<T> assign)
                            assign.Assign(item);
                    }
                    catch(Exception e) {
                        Debug.LogException(e);
                    }
                }
                handle.Cache.types.Remove(typeof(T));
            }
        }

        public static void Assign<T0, T1>(GameObject targetGameObject, T0 item0, T1 item1, Search search = Search.Default) {
            if(search == Search.Disable)
                return;
            if(targetGameObject == null) {
                Debug.LogError(TAG + "GameObject is null");
                return;
            }

            bool isAssigning0 = IsAssigning<T0>(targetGameObject);
            bool isAssigning1 = IsAssigning<T1>(targetGameObject);

            if(isAssigning0 && isAssigning1) {
                Debug.LogWarning(TAG + "Skip assign to prevent recursive assign");
                return;
            }

            using(var handle = GetCache(targetGameObject, search)) {
                handle.Cache.types.Add(typeof(T0));
                handle.Cache.types.Add(typeof(T1));
                foreach(var c in handle.Assignables) {
                    try {
                        switch(c) {
                            case IAssignable<T0, T1> i01:
                                i01.Assign(item0, item1);
                                break;
                            case IAssignable<T0> i0:
                                if(!isAssigning0)
                                    i0.Assign(item0);
                                break;
                            case IAssignable<T1> i1:
                                if(!isAssigning1)
                                    i1.Assign(item1);
                                break;
                        }
                    }
                    catch(Exception e) {
                        Debug.LogException(e);
                    }
                }
                handle.Cache.types.Remove(typeof(T0));
                handle.Cache.types.Remove(typeof(T1));
            }
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Toolkit/UI/Print Assignable Caches")]
#endif
        private static void PrintCaches() {
            Debug.Log(TAG + $"Caches ({caches.Count}):\n" + string.Join('\n', Caches.Select(x => $"{x.handles:00} - {x.search.ToStringFast()} - {x.target} -> {x.assignables.Count}")));
        }

        #endregion
    }
}
