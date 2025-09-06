using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Toolkit
{
    public static class GameObjectExtensions
    {

        #region Set Active

        public static void SetActive(IReadOnlyList<GameObject> objs, bool active) {
            for(int i = 0, length = objs.Count; i < length; i++) {
                objs[i].SetActive(active);
            }
        }

        #endregion

        #region Copy Component

        public static T AddComponent<T>(this GameObject destination, T source) where T : Component {
            var copy = destination.AddComponent<T>();
            source.CloneComponentToTarget(copy);
            return copy;
        }

        #endregion

        #region Scene

        public static void MoveToScene(this GameObject go, string sceneName) {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, SceneManagement.SceneUtility.GetScene(sceneName));
        }

        public static void MoveToScene(this GameObject go, SceneManagement.SceneObject sceneObject) {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, sceneObject);
        }

        public static void MoveToScene(this GameObject go, UnityEngine.SceneManagement.Scene scene) {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, scene);
        }

        #endregion

        #region Destroy

        public static void Destroy(this GameObject gameObject) {
            UnityEngine.Object.Destroy(gameObject);
        }

        public static void Destroy(this GameObject gameObject, float t) {
            UnityEngine.Object.Destroy(gameObject, t);
        }

        #endregion

        #region Bounds

        private static List<Collider> colliders = new List<Collider>(32);

        public static Bounds GetBounds(this GameObject gameObject)
            => GetBounds(gameObject, false, false, false, false);

        public static Bounds GetBounds(this GameObject gameObject, bool includeChildren)
            => GetBounds(gameObject, includeChildren, false, false, false);

        public static Bounds GetBounds(this GameObject gameObject, bool includeChildren, bool includeInactiveChildren, bool includeTriggers, bool includeDisabledColliders) {
            if(includeChildren) {
                gameObject.GetComponentsInChildren(includeInactiveChildren, colliders);
            }
            else {
                gameObject.GetComponents(colliders);
            }
            return colliders.GetBounds(includeTriggers, includeDisabledColliders);
        }

        public static Bounds GetBounds(this GameObject gameObject, Quaternion rotate)
            => GetBounds(gameObject, rotate, false, false, false, false);

        public static Bounds GetBounds(this GameObject gameObject, Quaternion rotate, bool includeChildren)
            => GetBounds(gameObject, rotate, includeChildren, false, false, false);

        public static Bounds GetBounds(this GameObject gameObject, Quaternion rotate, bool includeChildren, bool includeInactiveChildren, bool includeTriggers, bool includeDisabledColliders) {
            if(includeChildren) {
                gameObject.GetComponentsInChildren(includeInactiveChildren, colliders);
            }
            else {
                gameObject.GetComponents(colliders);
            }
            return colliders.GetBounds(rotate, includeTriggers, includeDisabledColliders);
        }

        #endregion

        #region GetOrAdd

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
            var t = go.GetComponent<T>();
            if(t == null)
                t = go.AddComponent<T>();
            return t;
        }

        #endregion
    }
}
