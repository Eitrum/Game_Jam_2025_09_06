using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [System.Serializable]
    public struct StatsEntry : IStatsModify {
        #region Variables

        [SerializeField] private StatType type;
        [SerializeField] private Stat stat;

        #endregion

        #region Properties

        public StatType Type {
            get => type;
            set => type = value;
        }

        public Stat Stat {
            get => stat;
            set => stat = value;
        }

        #endregion

        #region Constructor

        public StatsEntry(StatType type) {
            this.type = type;
            this.stat = StatsDefaults.Get(type);
        }

        public StatsEntry(StatType type, Stat stat) {
            this.type = type;
            this.stat = stat;
        }

        #endregion

        #region Methods

        public void GetStatsModifiersNonAlloc(List<StatsModifier> modifiers) {
            if(!stat.Value.Equals(0f, Mathf.Epsilon))
                modifiers.Add(new StatsModifier(type, Stat.ValueType.Base, stat.Value));
            if(!stat.Increased.Equals(0f, Mathf.Epsilon))
                modifiers.Add(new StatsModifier(type, Stat.ValueType.Increase, stat.Increased));
            if(!stat.More.Equals(0f, Mathf.Epsilon))
                modifiers.Add(new StatsModifier(type, Stat.ValueType.More, stat.More));
        }

        #endregion

        #region Operators

        public static implicit operator StatType(StatsEntry entry) => entry.type;
        public static implicit operator Stat(StatsEntry entry) => entry.stat;

        #endregion
    }
}
