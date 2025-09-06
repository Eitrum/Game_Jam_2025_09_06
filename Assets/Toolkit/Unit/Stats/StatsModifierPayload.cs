using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Unit {
    [CreateAssetMenu(menuName = "Toolkit/Unit/Stats/Stats Modifiers")]
    public class StatsModifierPayload : ScriptableObject, IStatsModify {
        #region Variables

        private const string TAG = "[StatsModifierPayload] - ";
        [SerializeField] private StatsModifier[] modifiers = { };

        #endregion

        #region Properties

        public IReadOnlyList<StatsModifier> Modifiers => modifiers;

        #endregion

        #region Methods

        public void GetStatsModifiersNonAlloc(List<StatsModifier> modifiers) {
            modifiers.AddRange(this.modifiers);
        }

        [ContextMenu("Verify Duplicates")]
        private void Verify() {
            var isUnique = modifiers.Select(x=> (x.StatType, x.ValueType)).IsAllUnique();
            if(!isUnique) {
                Debug.LogError(TAG + "Not all entries are unique");
            }
        }

        #endregion
    }
}
