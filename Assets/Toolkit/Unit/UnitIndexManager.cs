using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static class UnitIndexManager {

        private static Dictionary<int, IUnit> units = new Dictionary<int, IUnit>();
        private static int uniqueIndex = 0;

        public static int Add(IUnit unit) {
            units[uniqueIndex] = unit;
            return uniqueIndex++;
        }

        public static bool Remove(IUnit unit, int index) {
            if(units.TryGetValue(index, out IUnit other) && other == unit) {
                return units.Remove(index);
            }
            return false;
        }

        public static IUnit GetUnit(int index) {
            if(units.TryGetValue(index, out IUnit unit))
                return unit;
            return default;
        }

        public static bool HasUnit(int index)
            => units.ContainsKey(index);
    }
}
