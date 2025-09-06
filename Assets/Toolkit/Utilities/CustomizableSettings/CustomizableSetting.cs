using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit
{
    public abstract class CustomizableSetting<T> : ScriptableObject where T : CustomizableSetting<T>
    {
        [SerializeField, HideInInspector] private UnityEngine.Object scriptReference = null;
        [SerializeField, HideInInspector] private UnityEngine.Object inspectorReference = null;

        private static T instance;

        protected static T Instance {
            get {
                if(!instance) {
                    Restore();
                }
                return instance;
            }
        }

        public static void Restore() {
            if(instance != null) {
                Destroy(instance);
            }
            instance = Instantiate<T>(CustomizableSettingDatabase.Get<T>());
        }

        public static void Save() {
            throw new System.Exception("Unsupported");
        }

        public static void Load() {
            throw new System.Exception("Unsupported");
        }

        private void OnValidate() {
            if(scriptReference == null) {
                Debug.LogWarning("Script Reference is null, attempting to find it again: " + typeof(T).FullName);
#if UNITY_EDITOR
                var assets = UnityEditor.AssetDatabase.FindAssets(typeof(T).Name);
                if(assets != null) {
                    scriptReference = assets
                        .Select(x => UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(UnityEditor.AssetDatabase.GUIDToAssetPath(x)))
                        .FirstOrDefault(x => x != null && (x.GetClass() == typeof(T)));
                }
#endif
            }
            if(inspectorReference == null) {
                Debug.LogWarning("Inspector References is null, attempting to find it again: " + typeof(T).FullName);
#if UNITY_EDITOR
                var assets = UnityEditor.AssetDatabase.FindAssets(typeof(T).Name + "Inspector");
                if(assets != null) {
                    inspectorReference = assets
                        .Select(x => UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(UnityEditor.AssetDatabase.GUIDToAssetPath(x)))
                        .FirstOrDefault();
                }
#endif
            }
            CustomizableSettingDatabase.AddIfMissing(this as T);
        }
    }
}
