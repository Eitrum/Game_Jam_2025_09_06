using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [Toolkit.NSOFile("Metadata/Stackable", typeof(IItemDataBlueprint))]
    public class ItemStackableBlueprint : ScriptableObject, IItemDataStackableBlueprint {

        [SerializeField, Min(1)] private int maxStack = 1;

        public int MaxStack => maxStack;
        public System.Type GetItemType() => typeof(Stackable);

        public bool TryCreate(System.Random random, int level, out IItemData data) {
            data = new Stackable(1, maxStack);
            return true;
        }
    }
}
