using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [CreateAssetMenu(menuName = "Toolkit/Unit/Stats/Stats Defaults")]
    public class StatsDefaultsObject : ScriptableObject, ISerializationCallbackReceiver {
        #region Variables

        [SerializeField] private StatsEntry[] stats = { };
        private Dictionary<StatType, StatsEntry> statsLookup = new Dictionary<StatType, StatsEntry>();

        #endregion

        #region Properties

        public StatsEntry this[int index] => stats[index];
        public StatsEntry this[StatType type] => statsLookup.TryGetValue(type, out var modifier) ? modifier : new StatsEntry(type);

        #endregion

        #region Methods

        public StatsEntry Get(StatType type) {
            return statsLookup.TryGetValue(type, out StatsEntry entry) ? entry : new StatsEntry(type);
        }

        public Stat GetStat(StatType type) {
            return statsLookup.TryGetValue(type, out StatsEntry entry) ? entry.Stat : StatsDefaults.Get(type);
        }

        #endregion

        #region ISerialization Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            statsLookup.Clear();
            foreach(var s in stats)
                statsLookup[s.Type] = s;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion
    }
}
