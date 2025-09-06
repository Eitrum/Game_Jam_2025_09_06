using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static class TraitExtensions {
        #region Enum ToName

        public static string GetName(this TraitType type, int value)
            => TraitsUtility.GetName(type, value);

        public static string GetName(this TraitType type, bool positive)
            => TraitsUtility.GetName(type, positive);

        public static string GetPositiveName(this TraitType type)
            => TraitsUtility.GetPositiveName(type);

        public static string GetNegativeName(this TraitType type)
            => TraitsUtility.GetNegativeName(type);

        #endregion

        #region Add

        public static void Add(this ITrait trait, TraitEntry entry) {
            var value = trait.GetTrait(entry.Type);
            value += entry.Value;
            trait.SetTrait(entry.Type, value);
        }

        public static void Add(this ITrait trait, TraitType type, float value) {
            var current = trait.GetTrait(type);
            current += value;
            trait.SetTrait(type, current);
        }

        #endregion

        #region Remove

        public static void Remove(this ITrait trait, TraitEntry entry) {
            var value = trait.GetTrait(entry.Type);
            value -= entry.Value;
            trait.SetTrait(entry.Type, value);
        }

        public static void Remove(this ITrait trait, TraitType type, float value) {
            var current = trait.GetTrait(type);
            current -= value;
            trait.SetTrait(type, current);
        }

        #endregion

        #region Set

        public static void SetTrait(this ITrait trait, TraitEntry entry) {
            trait.SetTrait(entry.Type, entry.Value);
        }

        #endregion
    }
}
