using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Unit {
    [CreateAssetMenu(menuName = "Toolkit/Unit/Stats/Stats Entry")]
    public class StatsEntryPayload : ScriptableObject, IStatsModify {
        #region Variables

        private const string TAG = "[StatsEntryPayload] - ";
        [SerializeField] private StatsEntry[] entries = { };

        #endregion

        #region Properties

        public IReadOnlyList<StatsEntry> Entries => entries;

        #endregion

        #region Methods


        public void GetStatsModifiersNonAlloc(List<StatsModifier> modifiers) {
            foreach(var e in entries)
                e.GetStatsModifiersNonAlloc(modifiers);
        }

        #endregion
    }
}
