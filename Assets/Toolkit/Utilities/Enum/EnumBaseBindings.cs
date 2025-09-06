using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit {
    public abstract class EnumBaseBindings : ScriptableObject {

        #region Variables

        [SerializeField] protected string key = "customData";
        [SerializeField] protected string enumType;

        private bool initialized = false;
        protected System.Type cachedType;

        #endregion

        #region Properties

        public string Key => key;

        public System.Type EnumType {
            get {
                if(cachedType == null) {
                    try {
                        cachedType = System.Type.GetType(enumType, false);
                        if(cachedType != null && !cachedType.IsEnum)
                            cachedType = null;
                    }
                    catch { }
                }
                return cachedType;
            }
        }

        public bool IsValid => EnumType?.IsEnum ?? false;
        public bool IsInitialized => initialized;

        #endregion

        public void ClearCache() {
            cachedType = null;
            initialized = false;
        }

        [ContextMenu("Initialize")]
        public void Initialize() {
            if(!IsValid) {
                Debug.LogError(this.FormatLog("Unable to initialize, type if invalid.", enumType, "key:" + key));
                return;
            }

            try {
                var t = EnumType;
                FastEnum.Add(t);
                if(!FastEnum.TryGetBindT(t, out var bindMethod)) {
                    Debug.LogError(this.FormatLog("Could not find bind method for type: " + enumType));
                    return;
                }
                FastEnum.TryGetFastT(t, out var fastEnumT);
                var parseMethod = fastEnumT.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .FirstOrDefault(x=>x.Name == "TryParseIgnoreCase" && x.GetParameters().Length == 2);
                Dictionary<string, object> data = new Dictionary<string, object>();
                Fill(data);
                foreach(var d in data) {
                    try {
                        if(!parseMethod.InvokeStaticTryGet<string, System.Enum, bool>(d.Key, out System.Enum enumValue))
                            continue;

                        bindMethod.InvokeStatic(enumValue, key, d.Value);
                    }
                    catch { }
                }
                initialized = true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        protected virtual void Fill(Dictionary<string, object> data) { }
    }

    public abstract class EnumBaseBindings<T> : EnumBaseBindings {

        [System.Serializable]
        private class KeyValuePair {
            public string Key;
            public T Value;
        }

        #region Variables

        [SerializeField] private KeyValuePair[] values = new KeyValuePair[0];

        #endregion

        #region Fill

        protected override void Fill(Dictionary<string, object> data) {
            foreach(var v in values) {
                if(!Validate(v.Value))
                    continue;
                data[v.Key] = v.Value;
            }
        }

        protected virtual bool Validate(T value) { return true; }

        #endregion
    }
}
