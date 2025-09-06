using System;

namespace Toolkit.Health
{
    /// <summary>
    /// Utility class for damage types
    /// </summary>
    public static class DamageTypeUtility {
        public static int UniqueDamageTypes(int damageTypeMask) {
            int uniqueTypes = 0;
            int mask = damageTypeMask;
            while(mask > 0) {
                uniqueTypes += mask & 1;
                mask = mask >> 1;
            }
            return uniqueTypes;
        }

        public static int UniqueDamageTypes(DamageType damageTypeMask) {
            int uniqueTypes = 0;
            int mask = (int)damageTypeMask;
            while(mask > 0) {
                uniqueTypes += mask & 1;
                mask = mask >> 1;
            }
            return uniqueTypes;
        }

        public static bool HasDamageType(int damageTypeMask, DamageType damageType) {
            return (damageTypeMask & (int)damageType) == (int)damageType;
        }

        public static bool HasDamageType(DamageType damageTypeMask, DamageType damageType) {
            return ((int)damageTypeMask & (int)damageType) == (int)damageType;
        }
    }
}
