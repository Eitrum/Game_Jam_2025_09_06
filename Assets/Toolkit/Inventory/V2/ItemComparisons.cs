
namespace Toolkit.Inventory.V2 {
    public static class ItemComparisons {

        #region UID

        public static int UID(Item item0, Item item1)
            => item0.UID.CompareTo(item1.UID);

        public static int UID_Reverse(Item item0, Item item1)
            => UID(item1, item0);

        public static int UIDAsName(Item item0, Item item1)
            => item0.UIDAsName.CompareTo(item1.UIDAsName);

        public static int UIDAsName_Reverse(Item item0, Item item1)
            => UIDAsName(item1, item0);

        #endregion

        #region Type

        public static int Type(Item item0, Item item1) {
            var type0 = item0.TryGetData<IItemType>(out var itype0) ? itype0.Type : ItemType.None;
            var type1 = item1.TryGetData<IItemType>(out var itype1) ? itype1.Type : ItemType.None;
            return type0.CompareTo(type1);
        }

        public static int Type_Reverse(Item item0, Item item1)
            => Type(item1, item0);

        #endregion

        #region Name

        public static int Name(Item item0, Item item1) {
            var name0 = item0.TryGetData<IItemName>(out var iname0) ? iname0.Name : string.Empty;
            var name1 = item1.TryGetData<IItemName>(out var iname1) ? iname1.Name : string.Empty;
            return name0.CompareTo(name1);
        }

        public static int Name_Reverse(Item item0, Item item1)
            => Name(item1, item0);

        #endregion

        #region Weight

        public static int Weight(Item item0, Item item1) {
            var weight0 = ItemCalculations.CalculateWeight(item0);
            var weight1 = ItemCalculations.CalculateWeight(item1);
            return weight0.CompareTo(weight1);
        }

        public static int Weight_Reverse(Item item0, Item item1)
            => Weight(item1, item0);

        #endregion
    }
}
