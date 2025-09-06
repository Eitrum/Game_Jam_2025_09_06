using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Toolkit {
    public static class ComponentExtensions {
        #region Set Active

        public static void SetGameObjectActive<T>(this T component, bool active) where T : Component {
            component.gameObject.SetActive(active);
        }

        public static void SetGameObjectsActive<T>(this IReadOnlyList<T> componentArray, bool active) where T : Component {
            for(int i = 0, length = componentArray.Count; i < length; i++) {
                componentArray[i].gameObject.SetActive(active);
            }
        }

        public static void SetGameObjectsActive<T>(this IEnumerable<T> componentArray, bool active) where T : Component {
            foreach(var comp in componentArray)
                comp.gameObject.SetActive(active);
        }

        public static void SetActive(this Transform transform, bool active, bool recursive = false) {
            var childCount = transform.childCount;
            for(int i = 0; i < childCount; i++) {
                if(recursive) {
                    SetActive(transform.GetChild(i), active, recursive);
                }
            }
            transform.gameObject.SetActive(active);
        }

        #endregion

        #region Set Active Safe

        /// <summary>
        /// Set the gameObject active/inactive in a safe way.
        /// </summary>
        public static void SetGameObjectActiveSafe<T>(this T component, bool active) where T : Component {
            if(component)
                component.gameObject.SetActive(active);
        }

        /// <summary>
        /// Set the gameObject(s) active/inactive in a safe way.
        /// </summary>
        public static void SetGameObjectsActiveSafe<T>(this IReadOnlyList<T> componentArray, bool active) where T : Component {
            for(int i = 0, length = componentArray.Count; i < length; i++) {
                if(componentArray[i])
                    componentArray[i].gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Set the gameObject(s) active/inactive in a safe way.
        /// </summary>
        public static void SetGameObjectsActiveSafe<T>(this IEnumerable<T> componentArray, bool active) where T : Component {
            foreach(var comp in componentArray)
                if(comp)
                    comp.gameObject.SetActive(active);
        }

        #endregion

        #region Add Components

        public static T AddComponent<T>(this Component component) where T : Component {
            return component.gameObject.AddComponent<T>();
        }

        public static void AddComponents<T0, T1>(this Component component) where T0 : Component where T1 : Component {
            var go = component.gameObject;
            go.AddComponent<T0>();
            go.AddComponent<T1>();
        }

        public static void AddComponents<T0, T1>(this Component component, out T0 t0, out T1 t1) where T0 : Component where T1 : Component {
            var go = component.gameObject;
            t0 = go.AddComponent<T0>();
            t1 = go.AddComponent<T1>();
        }

        public static void AddComponents<T0, T1, T2>(this Component component) where T0 : Component where T1 : Component where T2 : Component {
            var go = component.gameObject;
            go.AddComponent<T0>();
            go.AddComponent<T1>();
            go.AddComponent<T2>();
        }

        public static void AddComponents<T0, T1, T2>(this Component component, out T0 t0, out T1 t1, out T2 t2) where T0 : Component where T1 : Component where T2 : Component {
            var go = component.gameObject;
            t0 = go.AddComponent<T0>();
            t1 = go.AddComponent<T1>();
            t2 = go.AddComponent<T2>();
        }

        public static void AddComponents<T0, T1, T2, T3>(this Component component) where T0 : Component where T1 : Component where T2 : Component where T3 : Component {
            var go = component.gameObject;
            go.AddComponent<T0>();
            go.AddComponent<T1>();
            go.AddComponent<T2>();
            go.AddComponent<T3>();
        }

        public static void AddComponents<T0, T1, T2, T3>(this Component component, out T0 t0, out T1 t1, out T2 t2, out T3 t3) where T0 : Component where T1 : Component where T2 : Component where T3 : Component {
            var go = component.gameObject;
            t0 = go.AddComponent<T0>();
            t1 = go.AddComponent<T1>();
            t2 = go.AddComponent<T2>();
            t3 = go.AddComponent<T3>();
        }

        #endregion

        #region GetOrAdd

        public static T GetOrAddComponent<T>(this Component component) where T : Component {
            var comp = component.GetComponent<T>();
            if(comp == null) {
                return component.AddComponent<T>();
            }
            return comp;
        }

        #endregion

        #region Copy

        // Check performance, with or without cloning caches?? Might be worth to preload in runtime??

        private static Dictionary<int, CloningCache> cloningCaches = new Dictionary<int, CloningCache>();

        public static void CloneComponentFromSource<T>(this T component, T source) where T : Component {
            var key = typeof(T).FullName.GetHash32();
            if(cloningCaches.TryGetValue(key, out CloningCache cache)) {
                cache.Clone(component, source);
            }
            else {
                var cc = new CloningCache(typeof(T));
                cloningCaches.Add(key, cc);
                cc.Clone(component, source);
            }
        }

        public static void CloneComponentToTarget<T>(this T component, T target) where T : Component {
            CloneComponentFromSource(target, component);
        }

        private class CloningCache {
            private FieldInfo[] fields;
            private PropertyInfo[] properties;

            public CloningCache(Type type) {
                fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x =>
                        x.IsPublic ?
                        (x.GetCustomAttribute(typeof(NonSerializedAttribute)) == null) :
                        (x.GetCustomAttribute(typeof(SerializeField)) != null))
                    .ToArray();
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanWrite && x.CanRead)
                    .Where(x => HandleUnityDontCopy(type, x))
                    .ToArray();
            }

            private static bool HandleUnityDontCopy(Type type, PropertyInfo info) {
                if(type.IsSubclassOf(typeof(UnityEngine.Object)) && info.Name == "name")
                    return false;
                if(type == typeof(MeshFilter)) {
                    return !(info.Name == "mesh");
                }
                else if(type == typeof(Renderer)) {
                    return !(info.Name == "materials" || info.Name == "material");
                }
                else if(type == typeof(MeshRenderer)) {
                    return !(info.Name == "materials" || info.Name == "material");
                }
                else if(type == typeof(SkinnedMeshRenderer)) {
                    return !(info.Name == "mesh" || info.Name == "materials" || info.Name == "material");
                }

                return true;
            }

            public void Clone(object target, object source) {
                for(int i = 0; i < fields.Length; i++) {
                    fields[i].SetValue(target, fields[i].GetValue(source));
                }
                for(int i = 0; i < properties.Length; i++) {
                    properties[i].SetValue(target, properties[i].GetValue(source));
                }
            }

            public void CopyTo<T>(object source, T target) {
                var t = typeof(T);
                for(int i = 0, length = fields.Length; i < length; i++) {
                    var sf = fields[i];
                    var tf = t.GetField(sf.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if(tf != null)
                        tf.SetValue(target, sf.GetValue(source));
                }
                for(int i = 0, length = properties.Length; i < length; i++) {
                    var sp = properties[i];
                    var tp = t.GetProperty(sp.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
                    if(tp != null)
                        tp.SetValue(target, sp.GetValue(source));
                }
            }
        }

        public static void CopyComponentValuesFromSource<TSource, TTarget>(this TTarget target, TSource source) where TSource : Component where TTarget : Component {
            var key = typeof(TSource).FullName.GetHash32();
            if(cloningCaches.TryGetValue(key, out CloningCache cache)) {
                cache.CopyTo(source, target);
            }
            else {
                var cc = new CloningCache(typeof(TSource));
                cloningCaches.Add(key, cc);
                cc.CopyTo(source, target);
            }
        }

        public static void CopyComponentValuesToSource<TSource, TTarget>(this TSource source, TTarget target) where TSource : Component where TTarget : Component {
            CopyComponentValuesFromSource(target, source);
        }

        #endregion

        #region Has Component

        public static bool HasComponent<T>(this Component component) where T : Component {
            return component.GetComponent<T>();
        }

        public static bool HasComponent<T>(this Component component, out T t) where T : Component {
            return (t = component.GetComponent<T>());
        }

        public static bool HasComponents<T0, T1>(this Component component) where T0 : Component where T1 : Component {
            return component.GetComponent<T0>() && component.GetComponent<T1>();
        }

        public static bool HasComponents<T0, T1>(this Component component, out T0 t0, out T1 t1) where T0 : Component where T1 : Component {
            t0 = component.GetComponent<T0>();
            t1 = component.GetComponent<T1>();
            return t0 && t1;
        }

        public static bool HasComponents<T0, T1, T2>(this Component component) where T0 : Component where T1 : Component where T2 : Component {
            return component.GetComponent<T0>() && component.GetComponent<T1>() && component.GetComponent<T2>();
        }

        public static bool HasComponents<T0, T1, T2>(this Component component, out T0 t0, out T1 t1, out T2 t2) where T0 : Component where T1 : Component where T2 : Component {
            t0 = component.GetComponent<T0>();
            t1 = component.GetComponent<T1>();
            t2 = component.GetComponent<T2>();
            return t0 && t1 && t2;
        }

        public static bool HasComponents<T0, T1, T2, T3>(this Component component) where T0 : Component where T1 : Component where T2 : Component where T3 : Component {
            return component.GetComponent<T0>() && component.GetComponent<T1>() && component.GetComponent<T2>() && component.GetComponent<T3>();
        }

        public static bool HasComponents<T0, T1, T2, T3>(this Component component, out T0 t0, out T1 t1, out T2 t2, out T3 t3) where T0 : Component where T1 : Component where T2 : Component where T3 : Component {
            t0 = component.GetComponent<T0>();
            t1 = component.GetComponent<T1>();
            t2 = component.GetComponent<T2>();
            t3 = component.GetComponent<T3>();
            return t0 && t1 && t2 && t3;
        }

        #endregion

        #region Nullable

#nullable enable
        public static T? ToNullable<T>(this T obj) {
            return obj == null ? default : obj;
        }
#nullable disable

        #endregion

        #region IsEnabled

        /// <summary>
        /// Wrapper for all types of components
        /// - Behaviour (MonoBehaviour)
        /// - Renderer
        /// - Collider
        /// </summary>
        public static bool IsEnabled(this Component component) {
            switch(component) {
                case Behaviour behaviour: return behaviour.enabled;
                case Renderer renderer: return renderer.enabled;
                case Collider collider: return collider.enabled;
            }
            return true;
        }


        /// <summary>
        /// Wrapper for all types of components
        /// - Behaviour (MonoBehaviour)
        /// - Renderer
        /// - Collider
        /// </summary>
        public static void SetEnabled(this Component component, bool active) {
            switch(component) {
                case Behaviour behaviour: behaviour.enabled = active; break;
                case Renderer renderer: renderer.enabled = active; break;
                case Collider collider: collider.enabled = active; break;
            }
        }


        /// <summary>
        /// Wrapper for all types of components
        /// - Behaviour (MonoBehaviour)
        /// - Renderer
        /// - Collider
        /// </summary>
        public static bool IsDisabled(this Component component) {
            switch(component) {
                case Behaviour behaviour: return !behaviour.enabled;
                case Renderer renderer: return !renderer.enabled;
                case Collider collider: return !collider.enabled;
            }
            return false;
        }

        #endregion
    }
}
