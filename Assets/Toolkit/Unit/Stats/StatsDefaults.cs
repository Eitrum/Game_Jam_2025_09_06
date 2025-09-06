using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static class StatsDefaults {

        private static Dictionary<StatType, Stat> defaults = new Dictionary<StatType, Stat>();

        static StatsDefaults() {
            defaults.Add(StatType.Critical_Strike_Chance, new Stat(0.05f, 0f, 0f));
        }

        public static Stat Get(StatType type) => defaults.TryGetValue(type, out var stat) ? stat : default;
    }
}
