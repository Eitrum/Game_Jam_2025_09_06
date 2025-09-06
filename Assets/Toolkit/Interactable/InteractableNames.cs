using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Interactables {
    public class InteractableNames : ScriptableSingleton<InteractableNames>, ISerializationCallbackReceiver {
        protected override bool KeepInResources => true;

        #region Variables

        [SerializeField] private bool recordNewEntriesInEditor = true;
        [SerializeField] private List<Entry> entries = new List<Entry>();
        private Dictionary<string, Entry> entryLookup = new Dictionary<string, Entry>();

        #endregion

        #region TryGet

        public static bool TryGetEntry<T>(out Entry entry) {
            return TryGetEntry(typeof(T), out entry);
        }

        public static bool TryGetEntry(System.Type type, out Entry entry) {
            if(Instance.entryLookup.TryGetValue(type.FullName, out entry))
                return true;
            entry = new Entry(type.FullName);
#if UNITY_EDITOR
            if(Instance.recordNewEntriesInEditor) {
                Instance.entries.Add(entry);
                Instance.entryLookup.Add(type.FullName, entry);
                UnityEditor.EditorUtility.SetDirty(Instance);
            }
#endif
            return false;
        }

        #endregion

        #region Serialization Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            entryLookup.Clear();
            foreach(var e in entries)
                entryLookup.TryAdd(e.Type, e);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {

        }

        #endregion

        #region Editor

        [Button("Validate")]
        private void Validate() {
            foreach(var e in entries)
                if(!e.Validate())
                    Debug.LogError(this.FormatLog($"Failed to validate entry: '{e.Name}'"));
        }

        #endregion

        [System.Serializable]
        public class Entry {
            [SerializeField, Readonly] private string type;
            [SerializeField, Readonly] private string scriptGuid;
            [SerializeField] private string name;
            [SerializeField, TextArea(2, 8)] private string description;
            [SerializeField] private int order;

            public string Type => type;
            public string Name => name;
            public string Description => description;
            public int Order => order;

            public Entry(string type) {
                this.type = type;
#if UNITY_EDITOR
                var script = UnityEditor.MonoImporter.GetAllRuntimeMonoScripts().FirstOrDefault(x => x?.GetClass()?.FullName == type);
                if(script != null) {
                    scriptGuid = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(script));
                }
#endif
            }

            public bool Validate() {
                System.Type classType = System.Type.GetType(type);
                if(classType != null)
                    return true;
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(scriptGuid);
                if(string.IsNullOrEmpty(path)) {
                    Debug.LogError(this.FormatLog("Failed to find the script from guid", type, scriptGuid, name));
                    return false;
                }
                var ms = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(path);
                type = ms.GetType().FullName;
#endif
                return true;
            }
        }
    }
}
