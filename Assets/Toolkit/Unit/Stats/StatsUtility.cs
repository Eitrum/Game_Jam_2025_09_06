using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static partial class StatsUtility {
        #region Recalculate

        public static List<StatsModifier> GetStatsModifiers(IUnit unit) {
            List<StatsModifier> modifiers = new List<StatsModifier>();

            foreach(var p in unit.Perks.Active)
                if(p.Value.Effect is IStatsModify sm)
                    sm.GetStatsModifiersNonAlloc(modifiers);

            foreach(var s in unit.StatusEffects.Effects)
                if(s is IStatsModify sm)
                    sm.GetStatsModifiersNonAlloc(modifiers);

            for(int i = 0; i < unit.Equipment.EquipmentSlots; i++) {
                if(unit.Equipment.GetEquipment(i) is IStatsModify sm)
                    sm.GetStatsModifiersNonAlloc(modifiers);
            }

            return modifiers;
        }

        #endregion
    }
}
