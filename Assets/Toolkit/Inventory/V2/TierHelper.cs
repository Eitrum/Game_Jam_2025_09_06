using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public static class TierHelper {

        public static bool TryPack(int tier, float value, out float tierValue) {
            value = Mathf.Clamp01(value);
            if(tier < 0) {
                tierValue = 0;
                return false;
            }
            tierValue = tier + value;
            return true;
        }

        public static bool TryExtract(float tierValue, out int tier, out float value) {
            if(tierValue < 0) {
                tier = 0;
                value = 0;
                return false;
            }

            tier = (int)tierValue;
            value = tierValue - tier;
            return true;
        }
    }
}
