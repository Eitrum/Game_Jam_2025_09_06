using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit
{
    [CreateAssetMenu]
    public class CustomizableSettingDatabase : ScriptableSingleton<CustomizableSettingDatabase>
    {
        protected override bool KeepInResources => true;

        [SerializeField] private List<ScriptableObject> settings = new List<ScriptableObject>();

        internal static T Get<T>() where T : CustomizableSetting<T> {
            var settings = Instance.settings;
            for(int i = 0, length = settings.Count; i < length; i++) {
                if(settings[i] is T val)
                    return val;
            }
            Debug.LogError($"Setting '{typeof(T).Name}' not found!");
            var res = CreateInstance<T>();
            settings.Add(res);
            return res;
        }

        public static void Add<T>(T obj) where T : CustomizableSetting<T> {
            Instance.settings.Add(obj);
        }

        private void OnValidate() {
            for(int i = 0; i < settings.Count; i++) {
                if(settings[i] == null) {
                    settings.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void AddIfMissing<T>(T obj) where T : CustomizableSetting<T> {
            /* Disabled as causes some random issues
#if UNITY_EDITOR
            if(!Instance.settings.Any(x=>x is T)) {
                UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(Instance);
                Instance.settings.Add(obj);
                so.Update();
                so.ApplyModifiedProperties();
            }
#endif
*/
        }
    }
}
