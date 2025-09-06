using System;

namespace Toolkit.Inventory.V2 {
    public static class ItemCalculations {
        #region Weight

        public static float CalculateWeight(Item item) {
            float weight = item.TryGetData<IItemWeight>(out var weightcomp) ? weightcomp.Weight : 0f;
            if(item.TryGetData(out IStackable stackable))
                weight *= stackable.Count;
            return weight;
        }

        #endregion
    }
}
