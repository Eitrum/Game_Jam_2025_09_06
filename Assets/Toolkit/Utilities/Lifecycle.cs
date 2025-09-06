using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit {
    public static class Lifecycle {
        #region Variables

        private const string TAG = "[Toolkit.Lifecycle] - ";
        public const bool DESTROY_RETURN_IF_NULL = true;
        public const bool UNTRACK_RETURN_IF_NULL = false;
        public const bool TRACK_RETURN_IF_NULL = false;

        private static List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
        private static List<IDisposable> disposables = new List<IDisposable>();

        // Threading fixes
        private static System.Threading.Thread mainThread;
        private static object lockObj = 42;

        // Handle marked to be removed
        private static List<UnityEngine.Object> flaggedForRemovalObjects = new List<UnityEngine.Object>();
        private static List<IDisposable> flaggedForRemovalDisposables = new List<IDisposable>();
        private static List<UnityEngine.Object> flaggedForRemovalObjectsDoubleBuffer = new List<UnityEngine.Object>();
        private static List<IDisposable> flaggedForRemovalDisposablesDoubleBuffer = new List<IDisposable>();

        #endregion

        #region Properties

        internal static IReadOnlyList<UnityEngine.Object> Objects => objects;
        internal static IReadOnlyList<IDisposable> Disposables => disposables;

        #endregion

        #region Init

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        private static void InitEditor() {
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += ForceDestroyAll;
            mainThread = System.Threading.Thread.CurrentThread;
            UnityEditor.EditorApplication.update += RemoveFlaggedObjects;
        }

#endif

        [RuntimeInitializeOnLoadMethod]
        private static void InitRuntime() {
            mainThread = System.Threading.Thread.CurrentThread;
            PlayerLoopUtilty.AddLast("PostLateUpdate", typeof(Lifecycle), RemoveFlaggedObjects);
        }

        private static void RemoveFlaggedObjects() {
            if(!IsMainThread) {
                Debug.LogError(TAG + "Attempting to remove flagged objects on a non-main thread!");
                return;
            }
            var tobjarray = flaggedForRemovalObjectsDoubleBuffer;
            var tdisarray = flaggedForRemovalDisposablesDoubleBuffer;
            flaggedForRemovalObjectsDoubleBuffer = flaggedForRemovalObjects;
            flaggedForRemovalDisposablesDoubleBuffer = flaggedForRemovalDisposables;
            flaggedForRemovalObjects = tobjarray;
            flaggedForRemovalDisposables = tdisarray;

            for(int i = flaggedForRemovalObjectsDoubleBuffer.Count - 1; i >= 0; i--)
                Destroy_Object(flaggedForRemovalObjectsDoubleBuffer[i]);
            flaggedForRemovalObjectsDoubleBuffer.Clear();

            for(int i = flaggedForRemovalDisposablesDoubleBuffer.Count - 1; i >= 0; i--)
                Destroy_Disposable(flaggedForRemovalDisposablesDoubleBuffer[i]);
            flaggedForRemovalDisposablesDoubleBuffer.Clear();
        }

        #endregion

        #region Help Methods

        private static bool IsMainThread => System.Threading.Thread.CurrentThread == mainThread;

        #endregion

        #region Force Cleanup

        public static void ForceDestroyAll() {
            if(Application.isPlaying) {
                for(int i = objects.Count - 1; i >= 0; i--)
                    UnityEngine.Object.Destroy(objects[i]);
                objects.Clear();
            }
            else {
                for(int i = objects.Count - 1; i >= 0; i--)
                    UnityEngine.Object.DestroyImmediate(objects[i], false);
                objects.Clear();
            }

            for(int i = disposables.Count - 1; i >= 0; i--)
                disposables[i].Dispose();
            disposables.Clear();
        }

        #endregion

        #region Track

        public static bool Track(CommandBuffer buffer)
            => Track_Disposable(buffer);

        public static bool Track(ComputeBuffer buffer)
            => Track_Disposable(buffer);

        public static bool Track(ComputeShader shader)
            => Track_Object(shader);

        public static bool Track(GameObject gameObject)
            => Track_Object(gameObject);

        public static bool Track(Material material)
            => Track_Object(material);

        public static bool Track(Texture2D texture)
            => Track_Object(texture);

        public static bool Track<T>(T item) {
            if(item == null)
                return TRACK_RETURN_IF_NULL;
            switch(item) {
                case UnityEngine.Object obj:
                    return Track_Object(obj);
                case IDisposable disposable:
                    return Track_Disposable(disposable);
                default:
                    Debug.LogError(TAG + "Unsupported type: " + typeof(T).FullName);
                    return false;
            }
        }

        private static bool Track_Object(UnityEngine.Object obj) {
            if(obj == null)
                return TRACK_RETURN_IF_NULL;
            objects.Add(obj);
            return true;
        }

        private static bool Track_Disposable(IDisposable disposable) {
            if(disposable == null)
                return TRACK_RETURN_IF_NULL;
            disposables.Add(disposable);
            return true;
        }

        #endregion

        #region Untrack

        public static bool Untrack(CommandBuffer buffer)
            => Untrack_Disposable(buffer);

        public static bool Untrack(ComputeBuffer buffer)
            => Untrack_Disposable(buffer);

        public static bool Untrack(ComputeShader shader)
            => Untrack_Object(shader);

        public static bool Untrack(GameObject gameObject)
            => Untrack_Object(gameObject);

        public static bool Untrack(Material material)
            => Untrack_Object(material);

        public static bool Untrack(Texture2D texture)
            => Untrack_Object(texture);

        public static bool Untrack<T>(T item) {
            if(item == null)
                return UNTRACK_RETURN_IF_NULL;
            switch(item) {
                case UnityEngine.Object obj:
                    return Untrack_Object(obj);
                case IDisposable disposable:
                    return Untrack_Disposable(disposable);
                default:
                    Debug.LogError(TAG + "Unsupported type: " + typeof(T).FullName);
                    return false;
            }
        }

        private static bool Untrack_Object(UnityEngine.Object obj) {
            if(obj == null)
                return UNTRACK_RETURN_IF_NULL;
            return objects.ReplaceWithLastAndRemove(obj);
        }

        private static bool Untrack_Disposable(IDisposable disposable) {
            if(disposable == null)
                return UNTRACK_RETURN_IF_NULL;
            return disposables.ReplaceWithLastAndRemove(disposable);
        }

        #endregion

        #region Destroy

        public static bool Destroy(CommandBuffer buffer)
            => Destroy_Disposable(buffer);

        public static bool Destroy(ComputeBuffer buffer)
            => Destroy_Disposable(buffer);

        public static bool Destroy(ComputeShader shader)
            => Destroy_Object(shader);

        public static bool Destroy(GameObject gameObject)
            => Destroy_Object(gameObject);

        public static bool Destroy(Material material)
            => Destroy_Object(material);

        public static bool Destroy(Texture2D texture)
            => Destroy_Object(texture);

        public static bool Destroy<T>(T item) {
            if(item == null)
                return DESTROY_RETURN_IF_NULL;
            switch(item) {
                case UnityEngine.Object obj:
                    return Destroy_Object(obj);
                case IDisposable disposable:
                    return Destroy_Disposable(disposable);
                default:
                    Debug.LogError(TAG + "Unsupported type: " + typeof(T).FullName);
                    return false;
            }
        }

        private static bool Destroy_Object(UnityEngine.Object obj) {
            try {
                if(!IsMainThread) {
                    lock(lockObj)
                        flaggedForRemovalObjects.Add(obj);
                    return false;
                }
                if(obj == null)
                    return DESTROY_RETURN_IF_NULL;
                objects.ReplaceWithLastAndRemove(obj);
#if UNITY_EDITOR
                if(Application.isPlaying)
                    UnityEngine.Object.Destroy(obj);
                else
                    UnityEngine.Object.DestroyImmediate(obj);
#else
                UnityEngine.Object.Destroy(obj);
#endif
                return true;
            }
            catch(Exception) {
                //Debug.LogException(e);
                return false;
            }
        }

        private static bool Destroy_Disposable(IDisposable disposable) {
            try {
                if(!IsMainThread) {
                    lock(lockObj)
                        flaggedForRemovalDisposables.Add(disposable);
                    return false;
                }
                if(disposable == null)
                    return DESTROY_RETURN_IF_NULL;
                disposables.ReplaceWithLastAndRemove(disposable);
                disposable.Dispose();
                return true;
            }
            catch(Exception) {
                //Debug.LogException(e);
                return false;
            }
        }

        #endregion
    }
}
