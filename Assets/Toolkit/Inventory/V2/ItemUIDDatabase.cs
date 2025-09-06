using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [CreateAssetMenu]
    public class ItemUIDDatabase : ScriptableSingleton<ItemUIDDatabase> {

        #region Variables

        [SerializeField] private List<Entry> entries = new List<Entry>();

        private Dictionary<string, Entry> nameToEntry = new Dictionary<string, Entry>();
        private Dictionary<int, Entry> uidToEntry = new Dictionary<int, Entry>();

        #endregion

        #region Conversion

        public static int NameToId(string name) {
            return name.GetHash32();
        }

        #endregion

        #region Blueprint

        public static ItemBlueprint GetBlueprint(int uid)
            => TryGetBlueprint(uid, out var blueprint) ? blueprint : null;

        public static bool TryGetBlueprint(int uid, out ItemBlueprint blueprint) {
            if(!Instance.uidToEntry.TryGetValue(uid, out var entry)) {
                blueprint = null;
                return false;
            }
            blueprint = entry.blueprint;
            return blueprint != null;
        }

        #endregion

        [System.Serializable]
        public class Entry {
            public string name = "category/name";
            public int uid;

            public bool isLegacy;

            public ItemBlueprint blueprint;
        }
    }
}
