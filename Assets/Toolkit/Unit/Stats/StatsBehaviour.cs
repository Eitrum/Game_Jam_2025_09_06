using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [AddComponentMenu("Toolkit/Unit/Stats")]
    public class StatsBehaviour : MonoBehaviour, IStats {

        #region Variables

        private const string TAG = "[StatsBehaviour] - ";
        private Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
        private OnStatsChangedDelegate onStatsChanged;

        #endregion

        #region Properties

        public int AssignedStats => stats.Count;
        public IReadOnlyDictionary<StatType, Stat> Stats => stats;
        public event OnStatsChangedDelegate OnStatsChanged{
            add => onStatsChanged += value;
            remove => onStatsChanged -= value;
        }

        #endregion

        #region Utility

        public void Reset() {
            stats.Clear();
        }

        public void Add(StatType type)
            => Add(type, GetDefault(type));

        public void Add(StatType type, Stat stat) {
            if(stats.ContainsKey(type)) {
                Debug.LogWarning(TAG + $"Attemping to add a stat that already exist '{type}' ({stat})");
            }
            else
                SetStat(type, stat);
        }

        public void Remove(StatType type) {
            if(stats.Remove(type))
                onStatsChanged?.Invoke(type);
        }

        public Stat GetDefault(StatType type) {
            return StatsDefaults.Get(type);
        }

        #endregion

        #region IStats Impl

        public Stat GetStat(StatType type)
            => stats.TryGetValue(type, out var stat) ? stat : GetDefault(type);

        public void SetStat(StatType type, Stat stat) {
            stats[type] = stat;
            onStatsChanged?.Invoke(type);
        }

        #endregion
    }
}
